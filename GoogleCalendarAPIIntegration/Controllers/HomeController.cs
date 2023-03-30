using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using GoogleCalendarAPIIntegration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using RestSharp;
using System.Net;

namespace GoogleCalendarAPIIntegration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GoogleApiOptions _googleApiOptions;
        private readonly string _googleCalendarApiBaseUrl;
        private readonly string _tokenFilePath;
        private readonly RestClient _restClient;
        private readonly RestRequest _restRequest;


        public HomeController(ILogger<HomeController> logger, IOptions<GoogleApiOptions> googleApiOptions)
        {
            _logger = logger;
            _googleApiOptions = googleApiOptions.Value;
            _tokenFilePath = googleApiOptions.Value.TokenFilePath;
            _restClient = new RestClient();
            _restRequest = new RestRequest();
            _googleCalendarApiBaseUrl = googleApiOptions.Value.GoogleCalendarApiBaseUrl;


        }

        public IActionResult Index()
        {
            var tokenJson = System.IO.File.ReadAllText(_tokenFilePath);
            var token = JObject.Parse(tokenJson);
            var accessToken = token.Value<string>("access_token");

            _restRequest.AddParameter("access_token", accessToken);
            _restClient.BaseUrl = new Uri(_googleCalendarApiBaseUrl);


            var response = _restClient.Execute(_restRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.IsSignedIn = true;
            }
            else
            {
                ViewBag.IsSignedIn = false;
            }

            return View();
        }


        public ActionResult OauthRedirect()
        {
            JObject credentials = JObject.Parse(System.IO.File.ReadAllText(_googleApiOptions.CredentialsFilePath));
            var client_id = credentials["client_id"];

            var redirectUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                               "scope=https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events&" +
                               "access_type=offline&" +
                               "include_granted_scopes=true&" +
                               "response_type=code&" +
                               "state=hellothere&" +
                               "redirect_uri=https://localhost:44379/oauth/callback&" +
                               "client_id=" + client_id;

            return Redirect(redirectUrl);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
