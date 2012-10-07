using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkknil.Services {
    public class ReadabilityService {
        private string restUrl = "https://www.readability.com/api/rest/v1";
        protected HttpPost httpPost = new HttpPost();
        private string token;
        private string token_secret;

        public ReadabilityService(string username, string password) {
            var authResult = httpPost.GetAccessToken(username, password);
            token = authResult["oauth_token"];
            token_secret = authResult["oauth_token_secret"];
        }

        public string Bookmark(string url) {
            //oauth_token_secret=4ahuz948bbeJLNtSNnw4pJ7ErYqdgzJa&oauth_token=29UG83NpeWFcArZRuv&oauth_callback_confirmed=true

            BaseHttpRequest httpRequest = HttpRequestFactory.CreateHttpRequest(Method.POST);
            httpRequest.Token = token;
            httpRequest.TokenSecret = token_secret;
            string result = httpRequest.Request(restUrl + "/bookmarks", "url=" + url);

            return result;
        }
    }
}
