using System.Net;
using System.Web;
using System.Collections.Specialized;

public class HttpGet : BaseHttpRequest
{
    private const string GET = "GET";
    private const string AUTHORIZE = "•https://www.readability.com/api/rest/v1/oauth/authorize";
    private const string RequestToken = "•https://www.readability.com/api/rest/v1/oauth/request_token";
    private const string OauthToken = "oauth_token";
    private const string OauthTokenSecret = "oauth_token_secret";


    public override string Request(string uri, string postData)
    {
        string outUrl;
        string queryString = AppendSignatureString(GET, uri, out outUrl);
        if (queryString.Length > 0)
        {
            outUrl += "?";
        }
        return WebRequest(GET, outUrl + queryString);
    }   

    private void SetTokenAndTokenSecret(string url)
    {
        string response = Request(url, string.Empty);

        if (response.Length <= 0) return;
        NameValueCollection queryString = HttpUtility.ParseQueryString(response);
        if (queryString[OauthToken] != null)
        {
            Token = queryString[OauthToken];
        }
        if (queryString[OauthTokenSecret] != null)
        {
            TokenSecret = queryString[OauthTokenSecret];
        }
    }

    public void GetRequestToken()
    {
        SetTokenAndTokenSecret(RequestToken);
    }

    public string GetAuthorizationUrl()
    {
        string ret = string.Format("{0}?oauth_token={1}", AUTHORIZE, Token);
        return ret;
    }


    private static string WebRequest(string method, string url)
    {
        HttpWebRequest httpWebRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
        httpWebRequest.Method = method;
        httpWebRequest.ServicePoint.Expect100Continue = false;
        return GetHttpWebResponse(httpWebRequest);
    }
}