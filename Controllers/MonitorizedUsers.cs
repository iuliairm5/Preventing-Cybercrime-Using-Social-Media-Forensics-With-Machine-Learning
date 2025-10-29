using DisertatieIRIMIA.Data;
using DisertatieIRIMIA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web;
using System.Collections.Generic;

using Microsoft.Extensions.Hosting.Internal;

namespace DisertatieIRIMIA.Controllers
{
    [Authorize]
    public class MonitorizedUsers : Controller
    {
        
        private IStoreRepository2 repository;
        private IStoreRepository repository0;
        //private string websites2; // Declare the variable at the class level

        public MonitorizedUsers(IStoreRepository2 repo, IStoreRepository repo0)
        {
            repository = repo;
            repository0 = repo0;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var monitorizedusers= repository.MonitorizationforUser(userId).ToList();
            return View(monitorizedusers);

            //return View(repository.MonitorizedUsers);
        }



        /////////////////////////////////////////////////
        ///
        [HttpGet]
        public IActionResult AddUsername()//getting the form
        {
            return View(); //return View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUsername([Bind("Username")] TwitterAccount username1)
        {


            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //add in DB
            MonitorizedUser monitorizedUser = new MonitorizedUser
            {

                Username = username1.Username,
                DateTime = DateTime.Now,
                AccountLatestTweets = null,
                Location = null,
                AccountDetails = null,
                UserId = userId
            };
            await repository.SavePredictionAsync(monitorizedUser);
            //return View("Index");
            return RedirectToAction("Index");


        }

        
        /// /////////////////////////////////////////////////////////
       
        
        [HttpGet]
        public IActionResult AccountDetails(int monitorizationID)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            return View(product);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> AccountDetails(int monitorizationID, string username, [FromServices] BackgroundTaskService backgroundTaskService)
        public async Task<IActionResult> AccountDetails(int monitorizationID, string username)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            ViewBag.response0 = product.Username;

            HttpClient httpClient = new HttpClient();
            var content = new StringContent("{\"username\": \"" + product.Username + "\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/accountdetails", content);
            var responseJSON = await response.Content.ReadAsStringAsync();

            //var details = JsonConvert.DeserializeObject(responseJSON);
            var details = JsonConvert.DeserializeObject<dynamic>(responseJSON);

            var accountDetails = details?.ToString();
            product.AccountDetails = accountDetails;
            product.DateTime = DateTime.Now;

            if (details != null && details.location != null)
            {
                string location = details.location.ToString();
                product.Location = location;
            }


            await repository.UpdateAsync(product);

            ViewBag.response1 = details;

            //NEW
            //await backgroundTaskService.StartAsync(CancellationToken.None, monitorizationID);
            //
            return RedirectToAction("Index");
        }

        

        public IActionResult ViewAccountDetails(int monitorizationID)
        {
       

            return View("ViewAccountDetails", repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID));

        }




        /////////////////////////////////////////////////

        [HttpGet]
        public IActionResult AccountLatestTweets(int monitorizationID)//getting the form
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountLatestTweets(int monitorizationID, string username)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            ViewBag.response0 = product.Username;

            HttpClient httpClient = new HttpClient();
            var content = new StringContent("{\"username\": \"" + product.Username + "\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/account_tweets_predicted2", content);
            var responseJSON = await response.Content.ReadAsStringAsync();

            var tweets = JsonConvert.DeserializeObject(responseJSON);

            var accounttweets = tweets?.ToString();
            product.AccountLatestTweets = accounttweets;
            product.DateTime = DateTime.Now;
            await repository.UpdateAsync(product);

            ViewBag.response2 = tweets;
            //return View("Thanks2");
            return RedirectToAction("Index");


        }


        public IActionResult ViewAccountLatestTweets(int monitorizationID)
        {


            return View("ViewAccountLatestTweets", repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID));

        }
        ///////////////////////////////////////////////////////////////////

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int monitorizationID)
        {
            MonitorizedUser deletedUser = await repository.DeleteUserAsync(monitorizationID);
           
            return RedirectToAction("Index");
        }






        [HttpGet]
        public IActionResult Edit(int monitorizationID)//getting the form
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            return View(product);
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsername(int monitorizationID, string newUsername)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);

            product.Username = newUsername; // Set the new username
            await repository.UpdateAsync(product);

            return RedirectToAction("Index");
        }
        */

        ///////////////////////////////////////////////////////////////////////////
        /*
        [HttpGet]
        public IActionResult PDF(int monitorizationID, string username)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            var product2 = repository0.Predictions.FirstOrDefault(p => p.NamePersonSocialMedia == username);

            int usercountpredictiontable = 0;
            int usercountmonitorizedtable = 0;

            int totalusercounts = 0;

            usercountmonitorizedtable = repository.MonitorizedUsers.Count(p => p.Username == product.Username);
            usercountpredictiontable = repository0.Predictions.Count(p => p.NamePersonSocialMedia == product2.NamePersonSocialMedia);

            totalusercounts = usercountmonitorizedtable + usercountpredictiontable;


            ViewBag.Username = product?.Username ?? product2?.NamePersonSocialMedia;
            return View(totalusercounts);

        }*/

        [HttpGet]
        public IActionResult PDF(int monitorizationID)//getting the form
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            if (product == null)
            {
                return NotFound(); // Return a 404 Not Found response if the product is not found
            }
            var accdetails = product.AccountDetails;
            var jsonObject = JObject.Parse(accdetails);
            // Extracting the values
            ViewBag.username = jsonObject["username"].ToString();
            ViewBag.name = jsonObject["name"].ToString();
            ViewBag.followersCount = (int)jsonObject["followers_count"];

            //NEW
            ViewBag.tweetsCount = (int)jsonObject["tweets_count"];
            //

            var product2 = repository0.Predictions.FirstOrDefault(p => p.NamePersonSocialMedia == product.Username && product.Username != null);
            

            int usercountpredictiontable = 0;
            int usercountmonitorizedtable = 0;
            int totalusercounts = 0;
            usercountmonitorizedtable = repository.MonitorizedUsers.Count(p => p.Username == product.Username);

            if (product2 != null)
            {

                usercountpredictiontable = repository0.Predictions.Count(p => p.NamePersonSocialMedia == product2.NamePersonSocialMedia);
            }
            totalusercounts = usercountmonitorizedtable + usercountpredictiontable;
            ViewBag.TotalCounts = totalusercounts;

            /////////////////////////////////////
            //a) number of all predictions

            int monitorizeduserpredictions = 0;
            var acclatesttweets = product.AccountLatestTweets;
            var location = product?.Location;
            ViewBag.location = location;
            var jsonArray = JArray.Parse(acclatesttweets);
            int predictionImageMemeCount = 0;
            int predictionTextCount = 0;

            foreach (var item in jsonArray)
            {
                if (item["prediction_label"] != null)
                {
                    predictionImageMemeCount++;
                }
              

                if (item["prediction_tweet"] != null)
                {
                    predictionTextCount++;
                }
            }

            monitorizeduserpredictions = predictionImageMemeCount + predictionTextCount;
            int allpredictions = usercountpredictiontable + monitorizeduserpredictions;
            ViewBag.allpredictions = allpredictions;


            //b) all memes, urls and text predictions

            var predictionsForUser = repository0.Predictions.Where(p => p.NamePersonSocialMedia == product.Username);
            int tweetTextCount = predictionsForUser.Count(p => p.InputType == "Tweet/Text");
            int urlCount = predictionsForUser.Count(p => p.InputType == "URL");
            int imageMemeCount = predictionsForUser.Count(p => p.InputType == "Image/Meme");

            ViewBag.URLallpredictions = urlCount;
            ViewBag.MemeAllPredictions = imageMemeCount + predictionImageMemeCount;
            ViewBag.TextAllPredictions = predictionTextCount + tweetTextCount;

            //c) malicious predictions for url, text, memes
            int maliciousmemeCount = 0;
            int malicioustextCount = 0;

            foreach (var item in jsonArray)
            {
                var memePrediction = item["prediction_label"]?.ToString();
                var textPrediction = item["prediction_tweet"]?.ToString();

                if (memePrediction == "negative")
                {
                    maliciousmemeCount++;
                }

                if (textPrediction == "hateful/aggressive")
                {
                    malicioustextCount++;
                }
            }

            /*
            */


            int predmalurl2 = predictionsForUser.Count(p => p.Prediction == "malicious/non-friendly");
            int predmaltxt2 = predictionsForUser.Count(p => p.Prediction == "hateful/aggressive");
            int predmalmeme2 = predictionsForUser.Count(p => p.Prediction == "negative");
            //!!!!!!!!!!!!!!!!!!!!!!! 

            ViewBag.predmalurl2 = predmalurl2;
            ViewBag.premaltxt2 = predmaltxt2;
            ViewBag.predmalmeme2 = predmalmeme2;
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ViewBag.maliciousTXT = malicioustextCount + predmaltxt2;
            ViewBag.maliciousMEME = maliciousmemeCount + predmalmeme2;
            ViewBag.maliciousURL = predmalurl2;

            //d) % stuff
            int malTOTAL = malicioustextCount + predmaltxt2 + maliciousmemeCount + predmalmeme2 + predmalurl2;
            double procentageMalicious = 0;
            double procentageMALmemes = 0;
            double procentageMALurl = 0;
            double procentageMALtext = 0;
            

            if (urlCount + imageMemeCount + predictionImageMemeCount + predictionTextCount + tweetTextCount != 0)
            {
                procentageMalicious = (100.0 * malTOTAL / (urlCount + imageMemeCount + predictionImageMemeCount + predictionTextCount + tweetTextCount));
                
            }

            if (malTOTAL != 0)
            {
                procentageMALmemes = (100.0 * (maliciousmemeCount + predmalmeme2) / malTOTAL);
                procentageMALurl = (100.0 * predmalurl2 / malTOTAL);
                procentageMALtext = (100.0 * (malicioustextCount + predmaltxt2) / malTOTAL);
            }


            string formattedProcentageMalicious = procentageMalicious.ToString("0.00");
            string formattedProcentageMALmemes = procentageMALmemes.ToString("0.00");
            string formattedProcentageMALurl = procentageMALurl.ToString("0.00");
            string formattedProcentageMALtext = procentageMALtext.ToString("0.00");
            ViewBag.procentageMalicious = formattedProcentageMalicious;
            ViewBag.procentageMALmemes = formattedProcentageMALmemes;
            ViewBag.procentageMALurl = formattedProcentageMALurl;
            ViewBag.procentageMALtext = formattedProcentageMALtext;

            //NEW
            // Access the web variable if it is not null
            //var websiteString = websites2 ;
            //ViewBag.websites = websiteString;

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PDF(int monitorizationID, string username)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            if (product == null)
            {
                return NotFound(); // Return a 404 Not Found response if the product is not found
            }

            string name = Request.Form["name"];
            string username1 = Request.Form["username"];
            int followersCount = int.Parse(Request.Form["followersCount"]);
            int tweetsCount = int.Parse(Request.Form["tweetsCount"]);
            double procentageMalicious = double.Parse(Request.Form["procentageMalicious"]);
            double procentageMALmemes = double.Parse(Request.Form["procentageMALmemes"]);
            double procentageMALurl = double.Parse(Request.Form["procentageMALurl"]);
            double procentageMALtext = double.Parse(Request.Form["procentageMALtext"]);
            int allpredictions = int.Parse(Request.Form["allpredictions"]);

            // Create the PDF document
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add content to the PDF document
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 26f);
                Paragraph title = new Paragraph("Social Media Forensics Analyzer - Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                Paragraph dateTime = new Paragraph("Date and time of the report: " + DateTime.Now);
                dateTime.SpacingAfter = 5f;
                document.Add(dateTime);

                Paragraph nameParagraph = new Paragraph("Name: " + name);
                nameParagraph.SpacingAfter = 5f;
                document.Add(nameParagraph);

                Paragraph usernameParagraph = new Paragraph("Username: " + username1);
                usernameParagraph.SpacingAfter = 5f;
                document.Add(usernameParagraph);

                Paragraph followersCountParagraph = new Paragraph("Number of followers: " + followersCount);
                followersCountParagraph.SpacingAfter = 5f;
                document.Add(followersCountParagraph);

                Paragraph tweetsCountParagraph = new Paragraph("Number of tweets posted: " + tweetsCount);
                tweetsCountParagraph.SpacingAfter = 5f;
                document.Add(tweetsCountParagraph);

                Paragraph allpredictionsParagraph = new Paragraph("Number of predictions made: " + allpredictions);
                allpredictionsParagraph.SpacingAfter = 5f;
                document.Add(allpredictionsParagraph);


                Paragraph maliciousContentParagraph = new Paragraph("Percentage of malicious content found: " + procentageMalicious + "%");
                maliciousContentParagraph.SpacingAfter = 5f;
                document.Add(maliciousContentParagraph);


                Paragraph hatefulMemesParagraph = new Paragraph(procentageMALmemes + "% is for hateful memes/images");
                hatefulMemesParagraph.SpacingAfter = 5f;
                document.Add(hatefulMemesParagraph);

                Paragraph maliciousURLsParagraph = new Paragraph(procentageMALurl + "% is for malicious URLs");
                maliciousURLsParagraph.SpacingAfter = 5f;
                document.Add(maliciousURLsParagraph);

                Paragraph hatefulTweetsParagraph = new Paragraph(procentageMALtext + "% is for hateful tweets/comments");
                hatefulTweetsParagraph.SpacingAfter = 20f;
                document.Add(hatefulTweetsParagraph);


                //NEW
                product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);

                HttpClient httpClient = new HttpClient();
                var content = new StringContent("{\"username\": \"" + product.Username + "\"}", Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://127.0.0.1:5000/search_websites2", content);
                var responseJSON = await response.Content.ReadAsStringAsync();
                var website = JsonConvert.DeserializeObject<dynamic>(responseJSON);
                var websites = website.results.ToObject<List<string>>();
                
                //var web = website?.ToString();
                Paragraph WebsitesParagraph = new Paragraph("Additional information: ", FontFactory.GetFont(FontFactory.HELVETICA_BOLD));
                WebsitesParagraph.SpacingAfter = 5f;
                document.Add(WebsitesParagraph);
                foreach (var site in websites)
                {
                    Paragraph websiteParagraph = new Paragraph(site);
                    websiteParagraph.SpacingAfter = 5f;
                    document.Add(websiteParagraph);
                }

                //




                //if (procentageMalicious > 45)
                if (procentageMalicious > 1.50)
                {
                    // Add a big red text at the end
                    Font bigRedFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16f, BaseColor.RED);
                    Paragraph potentialText = new Paragraph("POTENTIAL CYBER BULLY / CRIMINAL", bigRedFont);
                    potentialText.Alignment = Element.ALIGN_CENTER;
                    potentialText.SpacingBefore = 15f;
                    document.Add(potentialText);
                }
                // Add the stamp image
                string imagePath = "Content/Images/stamp.png"; // Replace with the actual path to your image
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);
                image.ScaleToFit(220f, 220f); // Adjust the width and height as needed
                document.Add(image);

                // Add more content to the PDF as needed

                document.Close();

                // Set the response headers
                byte[] byteArray = memoryStream.ToArray();

                // Return the PDF file as a response
                return File(byteArray, "application/pdf", "report.pdf");
            }


           // return RedirectToAction("Index");


        }

        
        //////////
        
        public IActionResult MyProfile()
        {

            return RedirectToAction("Index", "MyProfile");
        }



        /////////////////////////////////////////////////////////////// NEW

        [HttpGet]
        public IActionResult ViewInternetData(int monitorizationID)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewInternetData(int monitorizationID,string username)
        {
            var product = repository.MonitorizedUsers.FirstOrDefault(p => p.MonitorizationID == monitorizationID);
            
            HttpClient httpClient = new HttpClient();
            var content = new StringContent("{\"username\": \"" + product.Username + "\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:5000/search_websites", content);
            var responseJSON = await response.Content.ReadAsStringAsync();
            var website = JsonConvert.DeserializeObject<dynamic>(responseJSON);
           // dynamic websites = JsonConvert.DeserializeObject(responseJSON);
            var web = website?.ToString();
            
            ViewBag.websites = web;


           // var web2 = websites?.ToString();
           // websites2 = Convert.ToString(web2);

            return View("ViewInternetDataThanks");
        }
    }

}
