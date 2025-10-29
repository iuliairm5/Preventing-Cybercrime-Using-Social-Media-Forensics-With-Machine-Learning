using DisertatieIRIMIA.Data;
using DisertatieIRIMIA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace DisertatieIRIMIA.Controllers
{
    [Authorize]
    public class MyProfileController : Controller
    {
        private IStoreRepository repository;

		public MyProfileController(IStoreRepository repo)
		{
			repository = repo;
		}


        public IActionResult Index()
		{
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var predictions = repository.PredictionsForUser(userId).ToList();
            return View(predictions);

            //return View(repository.Predictions);
        }

        /// //////////////////////////////////////////////////
        

        //////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult PredictURL()//getting the form
        {
            return View(); //return View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictURL(URLs url)//sending the completed form to a local repository
        {

            //send to flask server
            HttpClient httpClient = new HttpClient();

            var content = new StringContent("{\"url\": \"" + url.Link + "\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/predict", content);
            var responseJSON = await response.Content.ReadAsStringAsync();


            ViewBag.response0 = responseJSON;

            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //add in DB
            PREDICTION prediction = new PREDICTION
            {
                //NamePersonSocialMedia = "Name1",
                NamePersonSocialMedia = url.NamePerson,
                InputType = "URL",
                Prediction = responseJSON,
                DateTime =  DateTime.Now,
                Input = url.Link,
                InputImage = null,
                UserId = userId
            };
            await repository.SavePredictionAsync(prediction);
            return View("Thanks");
            //ViewBag.myview = prediction.Input;
            //return View("ViewInput");

        }

        [HttpGet]
        public IActionResult PredictTweet()//getting the form
        {
            return View(); //return View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictTweet(TWEETs tweet)//sending the completed form to a local repository
        {
           
            //send to flask server
            HttpClient httpClient = new HttpClient();
            var content = new StringContent("{\"text\": \"" + tweet.Text + "\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/predict/tweets", content);
            var responseJSON = await response.Content.ReadAsStringAsync();


            ViewBag.response1 = responseJSON;

            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


            //add in DB
            PREDICTION prediction = new PREDICTION
            {
                NamePersonSocialMedia = tweet.NamePerson,
                InputType = "Tweet/Text",
                Prediction = responseJSON,
                DateTime = DateTime.Now,
                Input = tweet.Text,
                InputImage = null,
                UserId=userId
            };
            await repository.SavePredictionAsync(prediction);
            return View("Thanks");


            //return View("Thanks"); //return View-ul Thanks

        }


        [HttpGet]
        public IActionResult PredictMeme()//getting the form
        {
            return View(); //return View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PredictMeme(MEMEs image)//sending the completed form to a local repository
        {

            // Read the contents of the uploaded image file
            var stream = image.Meme.OpenReadStream();
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);


            //send to flask server
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(720);
            //var content = new StringContent("{\"text\": \"" + tweet.Text + "\"}", Encoding.UTF8, "application/json");
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(bytes), "image", image.Meme.FileName);


            var response = await httpClient.PostAsync("http://127.0.0.1:5000/predict/memes", content);
            var responseJSON = await response.Content.ReadAsStringAsync();


            ViewBag.response2 = responseJSON;


            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //add in DB

            // Convert the file to a byte array
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await image.Meme.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            PREDICTION prediction = new PREDICTION
            {
                //NamePersonSocialMedia = "Name1",
                NamePersonSocialMedia = image.NamePerson,
                InputType = "Image/Meme",
                Prediction = responseJSON,
                DateTime = DateTime.Now,
                Input = null,
                InputImage = fileBytes,
                UserId = userId
            };
            await repository.SavePredictionAsync(prediction);
            return View("Thanks");

        }

        ////////////////////////////////////////////////////////////////////
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int predictionId)
        {
            PREDICTION deletedProduct = await repository.DeletePredictionAsync(predictionId);
            //if (deletedProduct != null)
            //{
              //  TempData["message"] = $"{deletedProduct.PREDICTIONID} was deleted";
            //}
            return RedirectToAction("Index");
        }
       
       
         public IActionResult ViewInput(int predictionId)
         {

            //var product = repository.Predictions.FirstOrDefault(p => p.PREDICTIONID == predictionId);
            //ViewBag.myview = product.InputType;

            var product = repository.Predictions.FirstOrDefault(p => p.PREDICTIONID == predictionId);
            var imageData = product.InputImage; // This is the byte array of the image data
            if (imageData != null) 
            { 
                var base64String = Convert.ToBase64String(imageData);
                ViewBag.ImageBase64 = base64String;
            }
            else ViewBag.ImageBase64 = null;

            return View("ViewInput", repository.Predictions.FirstOrDefault(p => p.PREDICTIONID == predictionId));
            
        }

        ////////////////////////////////////////////////////////////////////
        [HttpGet]
        public IActionResult AccountDetails()//getting the form
        {
            return View(); //return View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountDetails(TwitterAccount account)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent("{\"username\": \"" + account.Username + "\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/accountdetails", content);
            var responseJSON = await response.Content.ReadAsStringAsync();



            var details = JsonConvert.DeserializeObject(responseJSON);

            //ViewBag.response1 = responseJSON;
            ViewBag.response1 = details;
            return View("Thanks2");

            
        }

        [HttpGet]
        public IActionResult AccountLatestTweets()//getting the form
        {
            return View(); //return View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountLatestTweets(TwitterAccount account)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent("{\"username\": \"" + account.Username + "\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/account_tweets_predicted", content);
            var responseJSON = await response.Content.ReadAsStringAsync();

            var tweets = JsonConvert.DeserializeObject<List<dynamic>>(responseJSON);


            //ViewBag.response2 = responseJSON;
            ViewBag.response2 = tweets;
            return View("Thanks2");


        }

        ////////////////////////////////////////////////////////

        
            public IActionResult MonitorizedUsers()
            {
              
                return RedirectToAction("Index", "MonitorizedUsers");
            }


        ////////////////////////// NEW

        public IActionResult Edit(int predictionId)
        {
            var product = repository.Predictions.FirstOrDefault(p => p.PREDICTIONID == predictionId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int predictionId, PREDICTION acc)
        {

            var product = repository.Predictions.FirstOrDefault(p => p.PREDICTIONID == predictionId);
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            product.DateTime = DateTime.Now;
            product.NamePersonSocialMedia = acc.NamePersonSocialMedia;



            await repository.SavePredictionAsync2(product);
            return RedirectToAction("Index");

        }





    }
}
