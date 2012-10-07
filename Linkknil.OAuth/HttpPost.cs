using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

public class HttpPost : BaseHttpRequest
{
    private const string POST = "POST";

    public override string Request(string uri, string postData)
    {
        string appendUrl = AppendPostDataToUrl(postData, uri);
        string outUrl;
        string querystring = AppendSignatureString(POST, appendUrl, out outUrl);
        return WebRequest(POST, outUrl, querystring);
    }

    private static string WebRequest(string method, string url, string postData)
    {
        HttpWebRequest httpWebRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
        httpWebRequest.Method = method;
        httpWebRequest.ServicePoint.Expect100Continue = false;
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        return GetHttpWebResponse(httpWebRequest, postData);
    }

    private static string GetHttpWebResponse(WebRequest httpWebRequest, string postData)
    {
        StreamWriter requestWriter = new StreamWriter(httpWebRequest.GetRequestStream());
        try
        {
            requestWriter.Write(postData);
        }
        finally
        {
            requestWriter.Close();
        }
        return GetHttpWebResponse(httpWebRequest);
    }

    public IDictionary<string ,string > GetAccessToken(string username, string password)
    {
        string uploadApiUrl = "https://www.readability.com/api/rest/v1/oauth/access_token";
        string appendUrl = AppendPostDataToUrl(string.Empty, uploadApiUrl);
        string authorizationHeader = GetXAuthorizationHeader(appendUrl, POST, username, password);

        HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(uploadApiUrl);
        request.Headers.Add("Authorization", authorizationHeader);

        request.Method = POST;
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = 0;

        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312")))
            {
                string result = sr.ReadToEnd();
                return ToDictionary(result);
            }
        }
    }

    private IDictionary<string,string> ToDictionary(string queryString)
    {
        NameValueCollection r = HttpUtility.ParseQueryString(queryString);
        Dictionary<string ,string > dictionary = new Dictionary<string, string>();
        foreach (var key in r.AllKeys)
        {
            dictionary.Add(key, r[key]);
        }

        return dictionary;
    }

    private string AppendPostDataToUrl(string postData, string url)
    {
        if (url.IndexOf("?") > 0)
        {
            url += "&";
        }
        else
        {
            url += "?";
        }
        url += ParsePostData(postData);
        return url;
    }

    private string ParsePostData(string postData)
    {
        string appendedPostData = string.IsNullOrEmpty(postData) ? "source=" + AppKey : postData + "&source=" + AppKey;
        NameValueCollection queryString = HttpUtility.ParseQueryString(appendedPostData);
        string resultUrl = "";
        foreach (string key in queryString.AllKeys)
        {
            if (resultUrl.Length > 0)
            {
                resultUrl += "&";
            }
            EncodeUrl(queryString, key);
            resultUrl += (key + "=" + queryString[key]);
        }
        return resultUrl;
    }

    private void EncodeUrl(NameValueCollection queryString, string key)
    {
        queryString[key] = HttpUtility.UrlPathEncode(queryString[key]);
        queryString[key] = UrlEncode(queryString[key]);
    }

    /*
    private const string ContentEncoding = "iso-8859-1";
    
    public string RequestWithPicture(string url, string postData, byte[] file)
    {
        string uploadApiUrl = url;
        string status = postData.Split('=').GetValue(1).ToString();
        string appendUrl = AppendPostDataToUrl(postData, url);
        string authorizationHeader = GetAuthorizationHeader(appendUrl, POST);

        HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(uploadApiUrl);
        request.Headers.Add("Authorization", authorizationHeader);

        request.PreAuthenticate = true;
        request.AllowWriteStreamBuffering = true;
        request.Method = POST;
        request.UserAgent = "Jakarta Commons-HttpClient/3.1";

        byte[] bytes = GetContentsBytes(request, status, file);

        return GetHttpWebResponse(request, bytes);
    }

    private static string GetHttpWebResponse(WebRequest httpWebRequest, byte[] bytes)
    {
        Stream requestStream = httpWebRequest.GetRequestStream();
        try
        {
            requestStream.Write(bytes, 0, bytes.Length);
        }
        finally
        {
            requestStream.Close();
        }
        return GetHttpWebResponse(httpWebRequest);
    }

    private byte[] GetContentsBytes(WebRequest request, string status, byte[] file)
    {
        string boundary = Guid.NewGuid().ToString();
        string header = string.Format("--{0}", boundary);
        string footer = string.Format("--{0}--", boundary);

        StringBuilder contents = new StringBuilder();
        request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
        contents.AppendLine(header);
        contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "status"));
        contents.AppendLine("Content-Type: text/plain; charset=US-ASCII");
        contents.AppendLine("Content-Transfer-Encoding: 8bit");
        contents.AppendLine();
        contents.AppendLine(status);

        contents.AppendLine(header);
        contents.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", "source"));
        contents.AppendLine("Content-Type: text/plain; charset=US-ASCII");
        contents.AppendLine("Content-Transfer-Encoding: 8bit");
        contents.AppendLine();
        contents.AppendLine(AppKey);


        contents.AppendLine(header);
        string fileHeader = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", "pic",
                                          "E:\\wiki.jpg");
        string fileData = Encoding.GetEncoding(ContentEncoding).GetString(file);

        contents.AppendLine(fileHeader);
        contents.AppendLine("Content-Type: application/octet-stream; charset=UTF-8");
        contents.AppendLine("Content-Transfer-Encoding: binary");
        contents.AppendLine();
        contents.AppendLine(fileData);
        contents.AppendLine(footer);

        byte[] bytes = Encoding.GetEncoding(ContentEncoding).GetBytes(contents.ToString());
        request.ContentLength = bytes.Length;
        return bytes;
    }
     */ 
}