import json
from typing import re

import flask
import requests
from flask import Flask, request, jsonify

def makeTokens(f):
    tkns_BySlash = str(f.encode('utf-8')).split('/')	# make tokens after splitting by slash
    total_Tokens = []
    for i in tkns_BySlash:
        tokens = str(i).split('-')	# make tokens after splitting by dash
        tkns_ByDot = []
        for j in range(0,len(tokens)):
            temp_Tokens = str(tokens[j]).split('.')	# make tokens after splitting by dot
            tkns_ByDot = tkns_ByDot + temp_Tokens
        total_Tokens = total_Tokens + tokens + tkns_ByDot
    total_Tokens = list(set(total_Tokens))	#remove redundant tokens
    if 'com' in total_Tokens:
        total_Tokens.remove('com')	#removing .com since it occurs a lot of times and it should not be included in our features
    return total_Tokens



#initialize our server
app = flask.Flask(__name__)


import joblib
# Load the model from the file
model = joblib.load('url_model.joblib')

import pickle
# Load the vectorizer from the file
with open("vectorizer_url.pkl", "rb") as f:
    vectorizer = pickle.load(f)



@app.route("/predict", methods=['POST'])
def predict():

    #url = "www.silkroadmeds-onlinepharmacy.com"
    url = request.json['url']
    url = [url]  # to have 1 element list
    url_v = vectorizer.transform(url)
    predict = model.predict(url_v).tolist()
    #print(predict)

    #return predict
    #return jsonify(predict)
    if(predict[0]=="good"):
        #return jsonify("good/friendly")
        return "good/friendly"
    elif(predict[0]=="bad"):
        #return jsonify("malicious/non-friendly")
        return "malicious/non-friendly"

###############################################################################################################



model_tweets = joblib.load("tweetshateful_v2.pkl")

with open("vectorizer_tweets_v2.pkl", "rb") as f:
    vectorizer_tweets = pickle.load(f)

@app.route("/predict/tweets", methods=['POST'])
def predict_tweets():

    #text = "i am thankful for sunshine. #thankful #positive     "
    text = request.json['text']
    text = [text]  # to have 1 element list
    text_v = vectorizer_tweets.transform(text)
    predict_tweets = model_tweets.predict(text_v).tolist()
    #print(predict_tweets)

    #return predict_tweets
    if(predict_tweets[0]==0):
        #return jsonify("good/non-hateful")
        return "good/non-hateful"
    elif(predict_tweets[0]==1):
        #return jsonify("hateful/aggressive")
        return "hateful/aggressive"


##############################################################################################################
import numpy as np
from flask import Flask, request, jsonify
from PIL import Image
import io


model_memes = joblib.load("memes_model_extra_trees.pkl")
scaler = joblib.load("scaler_for_memes.pkl")

@app.route("/predict/memes", methods=['POST'])
def predict_memes():
    # Read the image file sent through the POST request
    file = request.files['image']
    img_bytes = file.read()
    image = Image.open(io.BytesIO(img_bytes))

    # Preprocess the image
    image = np.array(image.convert('L').resize((200, 200)))
    image = np.ravel(image) / 255.0
    image = scaler.transform([image])

    # Make a prediction
    prediction_memes = model_memes.predict(image)[0]

    #print (prediction_memes)
    if (prediction_memes==0): return "neutral" #return jsonify("neutral")
    elif(prediction_memes==1): return "positive" #return jsonify("positive")
    elif (prediction_memes == 2): return "negative" #return jsonify("negative")

###################################################################### tweepy
import tweepy

#authentication for Twitter API
consumer_key = "RwO8K5JK6I63KLQsg7aRlgL23"
consumer_secret = "MFtIit2pBDxBUwLhTHRqXEf0gsyoCOI2N0K5daiDEM6MOBiVKE"
access_token = "1648370028172001281-IVJjKMaI2XzzHexUKZufa7IeuRqoDE"
access_token_secret = "gKjLbEf7GmzgZaMSL2T5dF8dC2f6yviq2TlXP7OYX0XJz"

#instantiation of the API
auth = tweepy.OAuth1UserHandler(
  consumer_key,
  consumer_secret,
  access_token,
  access_token_secret
)

api = tweepy.API(auth) #calls the Twitter API by creating an API object

@app.route("/accountdetails", methods=['POST'])
def account_details():
    username = request.get_json()['username']
    user = api.get_user(screen_name=username)
    # Create dictionary with user information
    user_info = {
        "name": user.name,
        "username": user.screen_name,
        "description": user.description,
        "location": user.location,
        "created_at": user.created_at,
        "friends_count": user.friends_count,
        #"profile_image_url": user.profile_image_url_https,
        "profile_image_url": user.profile_image_url_https.replace("_normal", ""),
        "followers_count": user.followers_count,
        "tweets_count": user.statuses_count,
        "verified_account": user.verified
        #"language_used": user.lang,
        #"time_zone": user.time_zone,
        #"UTC_offset": user.utc_offset

    }
    # Return user information as JSON
    return jsonify(user_info)


@app.route("/account_tweets_predicted", methods=['POST'])
def account_tweets_predicted():
    username = request.get_json()['username']
    #tweets = api.user_timeline(screen_name=username, count=200)
    tweets = api.user_timeline(screen_name=username, count=100, include_rts=True, tweet_mode='extended')
    #print("20 recent tweets:\n")
    tweet_list = []
    for tweet in tweets[:100]:
        #print(tweet.text)
        #print(" => Prediction:  ")
        tweet_dict = {}
        #tweet_dict['text'] = tweet.text
        #tweet.text = [tweet.text]  # to have 1 element list
        tweet_dict['text'] = tweet.full_text
        tweet.text = [tweet.full_text]
        tweet.text_v = vectorizer_tweets.transform(tweet.text)
        predict_tweets = model_tweets.predict(tweet.text_v).tolist()

        # return predict_tweets
        if (predict_tweets[0] == 0):
            #print("good/non-hateful")
            tweet_dict['prediction'] = 'good/non-hateful'
        elif (predict_tweets[0] == 1):
            #print("hateful/aggressive")
            tweet_dict['prediction'] = 'hateful/aggressive'
        tweet_list.append(tweet_dict)

    return jsonify(tweet_list)


#///////////////////////////////////////////////////////////////////////  NEW FUNCTION
import base64
@app.route("/account_tweets_predicted2", methods=['POST'])
def account_tweets_predicted2():
    username = request.get_json()['username']
    #tweets = api.user_timeline(screen_name=username, count=200, include_rts=False)
    tweets = api.user_timeline(screen_name=username, count=100, include_rts=False, tweet_mode='extended')
    tweet_list = []
    count = 0

    for tweet in tweets[:100]:
        if not tweet.in_reply_to_status_id:
            count = count + 1
            tweet_dict = {}
            #tweet_dict['tweet'] = tweet.text
            tweet_dict['tweet'] = tweet.full_text

            # Predict text
            #text = [tweet.text]
            text = [tweet.full_text]
            text_v = vectorizer_tweets.transform(text)
            predict_tweets = model_tweets.predict(text_v).tolist()
            if predict_tweets[0] == 0:
                tweet_dict['prediction_tweet'] = 'good/non-hateful'
            elif predict_tweets[0] == 1:
                tweet_dict['prediction_tweet'] = 'hateful/aggressive'

            """# Extract URLs
            urls = extract_urls(tweet.text)

            if urls:
                # Predict URLs
                predictions_urls = predict_urls(urls)
            else:
                predictions_urls = []

            tweet_dict['urls'] = urls if urls else None
            tweet_dict['prediction_urls'] = predictions_urls"""

            # Check if tweet has images/memes
            if 'media' in tweet.entities and tweet.entities['media'][0]['type'] == 'photo':
                image_url = tweet.entities['media'][0]['media_url']

                # Predict image/meme
                prediction_image = predict_image(image_url)
                #tweet_dict['prediction_image/meme'] = prediction_image
                image_bytes = base64.b64encode(requests.get(image_url).content).decode('utf-8')
                tweet_dict['image/meme'] = image_bytes
                tweet_dict['prediction_label'] = prediction_image


            tweet_list.append(tweet_dict)

            if count == 50:
                break

    #return jsonify(tweet_list)
    return json.dumps(tweet_list)




"""import re
import urllib.request
import requests
from urllib.parse import urlparse

def extract_urls(tweet_text):
    urls = re.findall(r'(https?://\S+)', tweet_text)
    expanded_urls = []
    for url in urls:
        parsed_url = urllib.parse.urlparse(url)
        if parsed_url.netloc == 't.co':
            # Skip t.co URLs
            continue
        expanded_urls.append(url)
    return expanded_urls

def extract_redirected_urls(url):
    response = requests.head(url, allow_redirects=True)
    redirected_url = response.url
    return redirected_url
def predict_urls(urls):
    predictions = []
    for url in urls:
        if "t.co" in url:
            # Expand t.co URL to the original URL
            expanded_url = requests.get(url).url
            parsed_url = urlparse(expanded_url)
            if parsed_url.netloc == "t.co":
                # Skip if the expanded URL is still a shortened URL
                continue
            redirected_url = extract_redirected_urls(expanded_url)
        else:
            redirected_url = extract_redirected_urls(url)

        url_v = vectorizer.transform([redirected_url])  # Pass the redirected URL as a list
        predict = model.predict(url_v).tolist()

        if predict[0] == "good":
            predictions.append("non-malicious/friendly")
        elif predict[0] == "bad":
            predictions.append("malicious/suspect")

    return predictions

"""


def predict_image(image_url):
    response = requests.get(image_url)
    image = Image.open(io.BytesIO(response.content))

    # Preprocess the image
    image = np.array(image.convert('L').resize((200, 200)))
    image = np.ravel(image) / 255.0
    image = scaler.transform([image])

    # Make a prediction
    prediction_memes = model_memes.predict(image)[0]

    if prediction_memes == 0:
        return "neutral"
    elif prediction_memes == 1:
        return "positive"
    elif prediction_memes == 2:
        return "negative"


#/////////////////////////////////////////////////////////////////////////// NEW GOOGLE SEARCH API
import requests
import urllib.parse
from bs4 import BeautifulSoup
from googlesearch import search
@app.route("/search_websites", methods=['POST'])
def search_websites():
    username = request.get_json()['username']

    # Create the search query using the username
    query = f'{username} website'

    # Perform the Google search
    search_results = search(query, num_results=30)

    # Process the search results
    count = 0
    website_results = []
    visited_urls = set()  # Track visited URLs to avoid duplicates
    for url in search_results:
        if url not in visited_urls:  # Skip already visited URLs
            visited_urls.add(url)

            # Extract relevant information from the URL
            title = ''
            description = ''

            # Determine the type of website based on the URL or title
            website_type = 'other'
            if 'wikipedia.org' in url:
                website_type = 'wikipedia'
            elif 'instagram.com' in url or 'facebook.com' in url or 'twitter.com' in url:
                website_type = 'social_media'

            # Extract title and description for non-social media websites
            if website_type == 'other':
                try:
                    response = requests.get(url)
                    soup = BeautifulSoup(response.content, 'html.parser')
                    title_tag = soup.find('title')
                    if title_tag:
                        title = title_tag.text.strip()
                    description_tag = soup.find('meta', attrs={'name': 'description'})
                    if description_tag:
                        description = description_tag['content'].strip()
                except:
                    # Error occurred while scraping, handle it accordingly
                    pass

            # Add relevant information based on the website type
            if website_type == 'wikipedia':
                website_results.append({
                    'url': url,
                    'description': description
                })
            elif website_type == 'social_media':
                website_results.append({
                    'url': url
                })
            else:
                website_results.append({
                    'url': url,
                    'title': title,
                    'description': description
                })
            count += 1
            if count == 20:  # Display only 20 websites
                break

    return jsonify({'results': website_results})



import random
@app.route("/search_websites2", methods=['POST'])
def search_websites2():
    username = request.get_json()['username']

    # Create the search query using the username
    query = f'{username} website'

    # Perform the Google search
    search_results = search(query, num_results=5)

    # Shuffle the search results randomly
    #random.shuffle(search_results)

    # Return the first website URL as a string
    if search_results:
        #return search_results[0]
        website_results = []
        visited_urls = set()  # Track visited URLs to avoid duplicates
        for url in search_results:
            if url not in visited_urls:  # Skip already visited URLs
                visited_urls.add(url)
                website_results.append(url)

        return jsonify({'results': website_results})
    else:
        return 'No websites found'


#/////////////////////////////////////////////////////////////////////////////////// NEW (RETRYING URL EXTRACTION)






#///////////////////////////////////////////////////////////////////////////////////
if __name__ == '__main__':
    app.run(port=5000)