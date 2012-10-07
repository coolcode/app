using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;

public abstract class BaseHttpRequest : oAuthBase, IHttpRequestMethod
{
    private const string OAuthSignaturePattern = "OAuth oauth_consumer_key=\"{0}\", oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"{1}\", oauth_nonce=\"{2}\", oauth_version=\"1.0\", oauth_token=\"{3}\",oauth_signature=\"{4}\"";

    private const string XAuthSignaturePattern = "OAuth oauth_consumer_key=\"{0}\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"{1}\",oauth_nonce=\"{2}\",oauth_version=\"1.0\",oauth_signature=\"{3}\",source=\"{4}\",x_auth_mode=\"client_auth\",x_auth_password=\"{5}\",x_auth_username=\"{6}\"";

    #region Properties
    private string _appKey;
    private string _appSecret;
    private string _token;
    private string _tokenSecret;

    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    public string TokenSecret
    {
        get { return _tokenSecret; }
        set { _tokenSecret = value; }
    }

    public string AppKey
    {
        get
        {
            if (string.IsNullOrEmpty(_appKey))
            {
                _appKey = ConfigurationManager.AppSettings["Sina_ApiKey"];
            }
            return _appKey;
        }
    }

    public string AppSecret
    {
        get
        {
            if (string.IsNullOrEmpty(_appSecret))
            {
                _appSecret = ConfigurationManager.AppSettings["Sina_ApiSecret"];
            }
            return _appSecret;
        }
    }
    #endregion

    protected string AppendSignatureString(string method, string url, out string outUrl)
    {
        string querystring;
        string signature = GenerateSignature(url, method, out outUrl, out querystring);

        querystring += "&oauth_signature=" + signature;
        return querystring;
    }

    private string GenerateSignature(string url, string method, out string outUrl, out string querystring)
    {
        Uri uri = new Uri(url);

        string nonce = GenerateNonce();
        string timeStamp = GenerateTimeStamp();

        //Generate Signature
        string signature = GenerateSignature(uri,
                                      AppKey,
                                      AppSecret,
                                      Token,
                                      TokenSecret,
                                      method,
                                      timeStamp,
                                      nonce,
                                      out outUrl,
                                      out querystring);
        return HttpUtility.UrlEncode(signature);
    }

    protected static string GetHttpWebResponse(WebRequest webRequest)
    {
        StreamReader responseReader = null;
        string responseData;
        try
        {
            responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            responseData = responseReader.ReadToEnd();
        }
        finally
        {
            webRequest.GetResponse().GetResponseStream().Close();
            responseReader.Close();
        }

        return responseData;
    }

    protected string GetAuthorizationHeader(string url, string method)
    {
        string timestamp = GenerateTimeStamp();
        string nounce = GenerateNonce();
        string normalizedString;
        string normalizedParameters;
        string signature = GenerateSignature(
            new Uri(url),
            AppKey,
            AppSecret,
            Token,
            TokenSecret,
            method,
            timestamp,
            nounce,
            out normalizedString,
            out normalizedParameters);
        signature = HttpUtility.UrlEncode(signature);
        return string.Format(
            CultureInfo.InvariantCulture,
            OAuthSignaturePattern,
            AppKey,
            timestamp,
            nounce,
            Token,
            signature);

    }
    protected string GetXAuthorizationHeader(string url, string method, string username, string password)
    {
        string timestamp = GenerateTimeStamp();
        string nounce = GenerateNonce();
        string normalizedString;
        string normalizedParameters;
        string signature = GenerateXSignature(
            new Uri(url),
            AppKey,
            AppSecret,
            Token,
            TokenSecret,
            method,
            timestamp,
            nounce,
            username,
            password,
            out normalizedString,
            out normalizedParameters);
        signature = HttpUtility.UrlEncode(signature);
        return string.Format(
            CultureInfo.InvariantCulture,
            XAuthSignaturePattern,
            AppKey,
            timestamp,
            nounce,
            signature,
            AppKey,
            password,
            username);
        //    private const string XAuthSignaturePattern = "OAuth oauth_consumer_key=\"{0}\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"{1}\",oauth_nonce=\"{2}\",oauth_version=\"1.0\",oauth_signature=\"{3}\",source=\"{4}\",x_auth_mode=\"client_auth\",x_auth_password=\"{5}\",x_auth_username=\"{6}\"";
    }
    public abstract string Request(string uri, string postData);
}
