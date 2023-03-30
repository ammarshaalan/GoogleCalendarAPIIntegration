using GoogleCalendarAPIIntegration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GoogleCalendarAPIIntegration.Controllers
{
    public class OAuthController : Controller
    {
        private readonly GoogleApiOptions _googleApiOptions;
        private readonly string _tokenFilePath;
        private readonly string _credentialsFilePath;

        private readonly RestClient _restClient;
        private readonly RestRequest _restRequest;


        public OAuthController(IOptions<GoogleApiOptions> googleApiOptions)
        {
            _googleApiOptions = googleApiOptions.Value;
            _tokenFilePath = googleApiOptions.Value.TokenFilePath;
            _credentialsFilePath = googleApiOptions.Value.CredentialsFilePath;
            _restClient = new RestClient();
            _restRequest = new RestRequest();
        }

        public ActionResult Callback(string code, string error, string state)
        {
            bool isSignedIn = false;
            if (string.IsNullOrWhiteSpace(error))
            {
                 isSignedIn =this.GetTokens(code);
            }
            if(isSignedIn)
                return RedirectToAction("Index", "Home");

            return View("Error");
        }

        public bool GetTokens(string code)
        {
            var credentials = JObject.Parse(System.IO.File.ReadAllText(_credentialsFilePath));

            _restRequest.AddQueryParameter("client_id", credentials["client_id"].ToString());
            _restRequest.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            _restRequest.AddQueryParameter("code", code);
            _restRequest.AddQueryParameter("grant_type", "authorization_code");
            _restRequest.AddQueryParameter("redirect_uri", "https://localhost:44379/oauth/callback");

            _restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/token");
            var response = _restClient.Post(_restRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(_tokenFilePath, response.Content);
                return true;
            }

            return false;
        }

        public ActionResult RefreshToken()
        {
            try
            {
                var credentials = JObject.Parse(System.IO.File.ReadAllText(_credentialsFilePath));
                var tokens = JObject.Parse(System.IO.File.ReadAllText(_tokenFilePath));

                _restRequest.AddQueryParameter("client_id", credentials["client_id"].ToString());
                _restRequest.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
                _restRequest.AddQueryParameter("grant_type", "refresh_token");
                _restRequest.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());

                _restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/token");
                var response = _restClient.Post(_restRequest);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject newTokens = JObject.Parse(response.Content);
                    newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                    System.IO.File.WriteAllText(_tokenFilePath, newTokens.ToString());
                }
            }
            catch (System.Exception)
            {
                return View("Error");
            }
            return RedirectToAction("Index", "Home", new { status = "success" });


        }

        public ActionResult RevokeToken()
        {
            var tokens = JObject.Parse(System.IO.File.ReadAllText(_tokenFilePath));

            _restRequest.AddQueryParameter("token", tokens["access_token"].ToString());

            _restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/revoke");
            var response = _restClient.Post(_restRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Home", new { status = "success" });
            }

            return View("Error");
        }
    }
}
