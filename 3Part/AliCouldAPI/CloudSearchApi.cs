// -----------------------------------------------------------------------
// <copyright file="CloudSearchApi.cs" company="Aliyun-inc">
// CloudSearchApi
// </copyright>
// -----------------------------------------------------------------------

namespace AliCould.com.API
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using System.Collections.Specialized;
    using System.Web;
    using System.IO;
    using System.Net.Sockets;

    /// <summary>
    /// 
    /// 云搜索客户端SDK。
    /// 
    /// 用于管理索引和获取指定索引的操作接口。
    /// 
    /// 管理索引：
    /// 1. 列出所有索引
    /// 2. 创建索引
    /// 3. 删除索引
    /// 4. 更改索引名称
    /// 
    /// </summary>
    public class CloudSearchApi
    {
        /// <summary>
        /// var string 定义API版本
        /// </summary>
        const string API_VERSION = "v1";

        /// <summary>
        /// API接入点 (Endpoint) URL
        /// </summary>
        private string _apiURL = null;

        /// <summary>
        /// 
        /// 用户唯一编号（client_id）。
        /// 
        /// 此编号为创建用户时自动创建，请访问以下链接来查看您的用户编号：
        /// 
        /// @link http://css.aliyun.com/manager/config/
        /// 
        /// </summary>
        private string _clientId = null;

        /// <summary>
        /// 
        /// 客户的密钥。
        /// 
        /// 此密钥为创建用户时自动创建，您可以访问以下链接来查看或重置您的密钥：
        /// 
        /// @link http://css.aliyun.com/manager/config/
        /// 
        /// </summary>
        private string _clientSecret = null;

        // 定义请求方式的常量，GET / POST两种
        const string METHOD_GET = "GET";
        const string METHOD_POST = "POST";

        /// <summary>
        /// 
        /// 构造函数。
        /// 和用户密钥，用户密钥可以在网站中重置，如果您在网站中重置了密钥，请务必确认传递给此 
        /// NOTE: 用户唯一编号和用户密钥在创建用户时自动创建，请访问一下链接来获取您的用户编号
        /// 参数的密钥为最新的密钥。
        /// 
        /// </summary>
        /// <seealso cref="http://css.aliyun.com/manager/config/"/>
        /// <param name="apiURL">API 接入点URL，非空字符串。</param>
        /// <param name="clientId">客户唯一ID，注册用户时获得。</param>
        /// <param name="clientSecret">客户的密钥，用于验证客户的操作合法性。</param>
        /// <exception cref="ArgumentException">参数不合法</exception>
        public CloudSearchApi(string apiURL, string clientId, string clientSecret)
        {
            string tempURL = apiURL.Trim();
            if (string.IsNullOrEmpty(tempURL))
            {
                throw new ArgumentException("apiURL is empty", "apiURL");
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId is empty", "clientId");
            }

            string tmpSecret = clientSecret.Trim();
            if (string.IsNullOrEmpty(tmpSecret))
            {
                throw new ArgumentException("clientSecret is empty", "clientSecret");
            }

            tempURL = tempURL.TrimEnd(new char[] { '/' });

            this._apiURL = Utilities.RTrim(tempURL, @"/v1/api");
            this._clientId = clientId;
            this._clientSecret = tmpSecret;
        }

        /// <summary>
        /// 
        /// 获取指定索引的CloudSearchIndex对象。
        /// 
        /// 该对象可以向索引中添加、删除文档和查询索引，请访问以下链接来访问该对象的方法和属性：
        /// 
        /// </summary>
        /// <param name="indexName">索引名称, 为非空字符串，必须是已经创建成功的索引。</param>
        /// <returns>返回CloudSearchIndex对象。</returns>
        /// <exception cref="ArgumentNullException">如果参数不合法，则抛出此异常。</exception>
        public CloudSearchIndex getIndex(string indexName)
        {
            string tmpIndexName = indexName.Trim();
            if (string.IsNullOrEmpty(tmpIndexName))
            {
                throw new ArgumentNullException("indexName", "indexName is empty or NULL");
            }

            return new CloudSearchIndex(this, tmpIndexName);
        }

        /// <summary>
        /// 
        /// 列出用户的所有索引。
        /// 
        /// </summary>
        /// <returns>返回所有索引列表。</returns>
        public string listIndexes()
        {
            return this.apiCall(this.indexRootURL());
        }

        /// <summary>
        /// 根据指定的索引名称和模板来创建一个索引。
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="template">
        ///     索引模板名称，目前支持社区、应用、小说和资讯四个模板，
        ///     对应名称为："bbs", "download", "novel", "news"。
        /// </param>
        public void createIndex(string indexName, string template)
        {
            indexName = indexName == null ? indexName : indexName.Trim();
            this.checkIndexName(indexName);

            template = template == null ? template : template.Trim();
            if (string.IsNullOrEmpty(template))
            {
                throw new ArgumentException("template is null.");
            }

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("action", "create");
            parameters.Add("index_name", indexName);
            parameters.Add("template", template);

            this.apiCall(this.indexRootURL(), parameters, METHOD_POST);
        }

        /// <summary>
        /// 
        /// 根据索引名称删除指定索引。
        /// 
        /// </summary>
        /// <param name="indexName">需要删除的索引名称。</param>
        /// <returns>返回调用api删除的response，通常可忽略</returns>
        /// <exception cref="ArgumentException">如果参数错误，则抛出此异常。</exception>
        public string deleteIndex(string indexName)
        {
            indexName = indexName == null ? indexName : indexName.Trim();
            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentException("indexName is null.");
            }

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("action", "delete");
            return this.apiCall(this.indexURL(indexName), parameters, METHOD_POST);
        }

        /// <summary>
        /// 
        ///  更新索引名称。
        ///
        /// </summary>
        /// <param name="indexName"> 需要更新的索引名称。</param>
        /// <param name="newIndexName"> 新的索引名称。</param>
        /// <returns>调用API的返回值，可忽略</returns>
        /// <exception cref="ArgumentException">如果参数错误，则抛出此异常。</exception>
        public string renameIndex(string indexName, string newIndexName)
        {
            if (string.IsNullOrEmpty(indexName.Trim()))
            {
                throw new ArgumentException("indexName is null");
            }

            if (string.IsNullOrEmpty(newIndexName.Trim()))
            {
                throw new ArgumentException("newIndexName is null");
            }

            indexName = indexName.Trim();
            newIndexName = newIndexName.Trim();

            if (string.Compare(indexName, newIndexName, true) == 0)
            {
                throw new ArgumentException("indexName is equal to newIndexName.");
            }

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("action", "update");
            parameters.Add("new_index_name", newIndexName);

            return this.apiCall(this.indexURL(indexName), parameters, METHOD_POST);
        }

        /// <summary>
        /// 检测指定索引是否存在。
        /// </summary>
        /// <param name="indexName">指定的索引名称。</param>
        /// <returns>如果索引存在则返回true，否则返回false。</returns>
        /// <exception cref="CloudSearchIndexNotExists">如果参数错误，测抛出此异常。</exception>
        /// <exception cref="CloudSearchInvalidIndexNameException">如果其他网络错误，则抛出此异常。</exception>
        public bool exists(string indexName)
        {
            indexName = indexName.Trim();
            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentException("indexName is null.");
            }

            try
            {
                var result = this.apiCall(this.statusURL(indexName));
                return true;
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("2001"))
                {
                    throw;
                }
                
                return false;
            }
        }

        /// <summary>
        /// 获取访问索引的根URL。
        /// </summary>
        /// <returns>返回URL。</returns>
        public string indexRootURL()
        {
            return this._apiURL + '/' + API_VERSION + "/api/index";
        }

        /// <summary>
        /// 
        /// 获取指定索引的根URL。
        /// 
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <returns>返回根URL。</returns>
        public string indexURL(string indexName)
        {
            return this._apiURL + '/' + API_VERSION + "/api/index/" + indexName;
        }

        /// <summary>
        /// 获取指定索引的doc访问URL。
        /// </summary>
        /// <param name="indexName">索引名称。</param>
        /// <returns>返回访问doc的URL。</returns>
        public string docURL(string indexName)
        {
            return this._apiURL + '/' + API_VERSION + "/api/index/doc/" + indexName;
        }

        /// <summary>
        /// 获取指定索引的top query访问URL。
        /// </summary>
        /// <param name="indexName">索引名称。</param>
        /// <returns>返回访问doc的URL</returns>
        public string topURL(string indexName)
        {
            return this._apiURL + '/' + API_VERSION + "/api/top/query/" + indexName;
        }

        /// <summary>
        /// 获取指定索引的状态访问URL。
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <returns>返回获取索引状态的URL。</returns>
        public string statusURL(string indexName)
        {
            return this._apiURL + '/' + API_VERSION + "/api/index/status/" + indexName;
        }

        /// <summary>
        /// 获取指定索引的错误访问URL。
        /// </summary>
        /// <param name="indexName">索引名称。</param>
        /// <returns>返回获取错误信息的URL。</returns>
        public string errorURL(string indexName)
        {
            return this._apiURL + '/' + API_VERSION + "/api/index/error/" + indexName;
        }

        /// <summary>
        /// 获取指定索引的搜索URL。
        /// </summary>
        /// <param name="indexName">索引名称。</param>
        /// <returns>返回指定索引检索的URL。</returns>
        public string searchURL(string indexName)
        {
            return this._apiURL + '/' + API_VERSION + "/api/search/" + indexName;
        }

        /// <summary>
        /// 获取API的根URL。
        /// </summary>
        /// <returns>返回api的根URL。</returns>
        public string rootURL()
        {
            return this._apiURL + '/' + API_VERSION + "/api";
        }

        /// <summary>
        /// 获取API URL。
        /// </summary>
        /// <returns>返回API URL。</returns>
        public string apiURL()
        {
            return this._apiURL;
        }

        /// <summary>
        /// 检查索引名称是否合法。
        /// </summary>
        /// <param name="indexName">索引名称。</param>
        /// <exception cref="ArgumentException">如果非法索引名称，则抛出此异常</exception>
        private void checkIndexName(string indexName)
        {
            if(string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentException("indexName", "indexName is null or empty.");
            }

            if(indexName.Contains("/"))
            {
                throw new ArgumentException("indexName", "indexName include \"/\"");
            }
        }

        /// <summary>
        /// 
        /// 根据参数创建签名信息。
        /// 
        /// @param array 参数数组。
        /// @return string 签名字符串。
        /// 
        /// </summary>
        /// <param name="parameter">参数数组</param>
        /// <returns>签名字符串</returns>
        public string _makeSign(NameValueCollection parameter)
        {
            string q = string.Empty;
            if (parameter != null)
            {
                parameter = Utilities.KeySort(parameter);
                q = Utilities.http_build_query(parameter);
            }

            return Utilities.CalcMd5(q + this._clientSecret);
        }

        /// <summary>
        /// 
        /// 创建Nonce信息。
        /// 
        /// </summary>
        /// <returns>返回Nonce信息。</returns>
        private string _makeNonce()
        {
            var time = Utilities.time();
            string nonce = Utilities.CalcMd5(this._clientId + this._clientSecret + time) + "." + time;
            return nonce;
        }

        /// <summary>
        /// 将参数数组转换成http query字符串
        /// 
        /// @param array $params 参数数组
        /// @return string query 字符串
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns>字符串</returns>
        public string buildParams(NameValueCollection parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return string.Empty;
            }

            string args = Utilities.http_build_query(parameters);

            return Utilities.PregReplace(new string[] { "/%5B(?:[0-9]|[1-9][0-9]+)%5D=/" }, new string[] { "=" }, args);
        }

        /// <summary>
        /// 调用Web API。
        /// </summary>
        /// <param name="method">HTTP请求类型，'GET' 或 'POST'。</param>
        /// <param name="url">WEB API的请求URL</param>
        /// <param name="parameters">参数数组。</param>
        /// <param name="httpOptions">http option参数数组。</param>
        /// <param name="webRequest">默认采用socket方式请求，请根据您的空间或服务器类型进行选择 。</param>
        /// <returns>返回decode后的HTTP response。</returns>
        public string apiCall(string url, NameValueCollection parameters = null, string method = METHOD_POST, NameValueCollection httpOptions = null, bool webRequest = false)
        {
            parameters = parameters ?? new NameValueCollection();
            httpOptions = httpOptions ?? new NameValueCollection();
            parameters.Add("nonce", this._makeNonce());
            parameters.Add("client_id", this._clientId);
            parameters.Add("sign", this._makeSign(parameters));

            JObject data;
            if (webRequest)
            {
                data = this.requestByWebrequest(method, url, parameters, httpOptions);
            }
            else
            {
                data = this.requestBySocket(method, url, parameters, httpOptions);
            }

            int httpCode = Convert.ToInt32(data["httpCode"].ToString());
            JObject rawResponse = data["rawResponse"] as JObject;

            if (Math.Floor((double)httpCode / 100) == 2)
            {
                uint statusCode = 0;

                if (rawResponse == null)
                {
                    throw new Exception("response is empty");
                }

                JToken errorsToke;
                if (rawResponse.TryGetValue("errors", out errorsToke))
                {
                    if (errorsToke.HasValues)
                    {
                        statusCode = Convert.ToUInt32(errorsToke[0]["code"].ToString());
                        this.responseToException(statusCode, rawResponse.ToString(Newtonsoft.Json.Formatting.None));
                    }
                    else
                    {
                        throw new Exception("Missing status code in response");
                    }
                }
                else
                {
                    // ok
                    return rawResponse.ToString(Newtonsoft.Json.Formatting.None);
                }
            }
            else
            {
                throw new Exception(rawResponse.ToString(Newtonsoft.Json.Formatting.None));
            }
            
            return null;
        }

        /// <summary>
        /// HttpWebRequest 版本
        /// 使用方法：
        /// var post_string = new NameValueCollection();;  
        /// post_string.Add(...);
        /// requestByWebrequest("POST", "http://alicloud.com/",post_string);  
        /// </summary>
        /// <param name="method">发送方式，POST/GET</param>
        /// <param name="url">WEB API的请求URL</param>
        /// <param name="parameters">参数数组。</param>
        /// <param name="httpOptions">http option参数数组。</param>
        /// <returns>返回decode后的HTTP response。</returns>
        private JObject requestByWebrequest(string method, string url, NameValueCollection parameters, NameValueCollection httpOptions = null)
        {

            string args = this.buildParams(parameters);
            Console.WriteLine(args);
            if (method == "GET")
            {
                url += "?" + args;
                args = string.Empty;
                Console.WriteLine(url);
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Clear();

            // set Expect100Continue to false, so it will send only one http package.
            request.ServicePoint.Expect100Continue = false;

            // Set request method: 'GET' / 'POST'
            request.Method = method;
            //request.Headers.Add("Expect:");
            request.ContentType = "application/x-www-form-urlencoded";

            var argsInBytes = Encoding.UTF8.GetBytes(args);
            request.ContentLength = argsInBytes.Length;

            //request.Accept = "*/*";
            // Write the post data for the POST request
            if (method == "POST")
            {
                //using (var sw = request.GetRequestStream())
                {

                    var sw = request.GetRequestStream();

                    sw.Write(argsInBytes, 0, argsInBytes.Length);
                    sw.Close();
                }
            }

            string responseString = string.Empty;
            HttpWebResponse webResonse = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(webResonse.GetResponseStream()))
            {
                responseString = sr.ReadToEnd();
            }

            webResonse.Close();
            JObject resultJObject = new JObject();
            resultJObject["httpCode"] = (int)webResonse.StatusCode;
            resultJObject["rawResponse"] = JObject.Parse(responseString);

            return resultJObject;
        }

        /// <summary>
        /// 
        /// Socket版本 
        /// 
        /// 使用方法： 
        /// 
        /// var post_string = new NameValueCollection();;  
        /// post_string.Add(...);
        /// requestBySocket("POST", "http://alicloud.com/",post_string);  
        /// 
        /// </summary>
        /// <param name="method">HTTP请求类型，'GET' 或 'POST'。</param>
        /// <param name="url">WEB API的请求URL</param>
        /// <param name="parameters">参数数组。</param>
        /// <param name="httpOptions">http option参数数组。</param>
        /// <returns>返回decode后的HTTP response。</returns>
        private JObject requestBySocket(string method, string url, NameValueCollection parameters, NameValueCollection httpOptions)
        {
            UriBuilder ub = new UriBuilder(url);
            string remote_server = ub.Host;
            string remote_path = ub.Path;
            method = method.ToUpper();

            UriBuilder parse = parseHost(url);
            string data = Utilities.http_build_query(parameters);
            string content = buildRequestContent(parse, method, data);

            string receivceStr = string.Empty;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            // Set default receive timeout to 10 seconds
            //socket.ReceiveTimeout = 10000;
            try
            {
                socket.Connect(ub.Host, ub.Port);
                byte[] contentInByte = Encoding.UTF8.GetBytes(content);
                socket.Send(contentInByte);

                receivceStr = string.Empty;
                byte[] recvBytes = new byte[1024];

                int bytes = 0;
                while (true)
                {
                    bytes = socket.Receive(recvBytes, recvBytes.Length, SocketFlags.None);

                    if (bytes <= 0)
                    {
                        break;
                    }

                    receivceStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            JObject ret = new JObject();
            var parsedRespon = parseResponse(receivceStr);
            ret["httpCode"] = Convert.ToInt32(parsedRespon["info"]["http_code"].ToString());
            ret["rawResponse"] = parsedRespon["result"];

            return ret;
        }

        /// <summary>
        /// 分析Web API的请求Uri
        /// </summary>
        /// <param name="host">Web API的请求Uri</param>
        /// <returns>封装后的UriBuilder</returns>
        UriBuilder parseHost(string host)
        {
            UriBuilder ub = new UriBuilder(host);
            if(ub.Scheme == "http")
            {
                ub.Scheme = "";
            }
            else if ( ub.Scheme == "https")
            {
                ub.Scheme = @"ssl://";
            }

            ub.Host = ub.Scheme + ub.Host;
            ub.Path = string.IsNullOrEmpty(ub.Path) ? "/" : ub.Path;

            string query = string.IsNullOrEmpty(ub.Query) ? "" : ub.Query;
            string path = ub.Path.Replace(@"\\", "/").Replace(@"//", "/");
            ub.Path = string.IsNullOrEmpty(query) ? path + "?" + query : path;

            return ub;
        }
        
        /// <summary>
        /// 构造发送请求的内容
        /// </summary>
        /// <param name="parse">封装后的UriBuilder</param>
        /// <param name="method">HTTP请求类型，'GET' 或 'POST'。</param>
        /// <param name="data">拼接好的Uri参数</param>
        /// <returns>构造的请求内容</returns>
        private static string buildRequestContent(UriBuilder parse, string method, string data)
        {
            string contentLengthStr = string.Empty;
            string postContent = string.Empty;
            string destinationUrl = Utilities.RTrim(parse.Path, "%3F");

            string query = null;
            if (method == METHOD_GET)
            {
                if (data[0] == '&')
                {
                    data = data.Substring(1);
                }

                query = string.IsNullOrEmpty(parse.Query) ? "" : parse.Query;
                destinationUrl += (string.IsNullOrEmpty(query) ? "?" : "&") + data;
            }
            else if (method == METHOD_POST)
            {
                contentLengthStr = "Content-length: " + data.Length + "\r\n";
                postContent = data;
            }

            string write = method + " " + destinationUrl + " HTTP/1.1\r\n";
            write += "Host: " + parse.Host + "\r\n";
            write += "Content-type: application/x-www-form-urlencoded\r\n";
            write += contentLengthStr;
            write += "Connection: close\r\n\r\n";
            write += postContent;

            return write;
        }

        /// <summary>
        /// 分析返回的Response内容并封装到JObject实例中。
        /// </summary>
        /// <param name="responseText">response内容</param>
        /// <returns>用response返回内容封装的JObject 实例</returns>
        static JObject parseResponse(string responseText)
        {
            JObject result = new JObject();
            string http_header_str = responseText.Substring(0, responseText.IndexOf("\r\n\r\n"));
            JObject http_headers = parseHttpSocketHeader(http_header_str);

            responseText = responseText.Substring(responseText.IndexOf("\r\n\r\n") + 4);
            result["result"] = JObject.Parse(responseText);
            result["info"] = new JObject();
            result["info"]["http_code"] = http_headers["Http_Code"] != null ? http_headers["Http_Code"] : 0;
            result["info"]["headers"] = http_headers;

            return result;
        }

        /// <summary>
        /// 
        /// 从socket header获取http 返回的status code
        /// 
        /// </summary>
        /// <param name="str">response内容的header，在'/r/n/r/n'之前。</param>
        /// <returns>将header封装到JObject 实例。</returns>
        static JObject parseHttpSocketHeader(string str)
        {
            var slice = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            JObject headers = new JObject();
            foreach (var sli in slice)
            {
                if (sli.Contains("HTTP"))
                {
                    headers["Http_Code"] = sli.Split(' ')[1];
                    headers["Status"] = sli;
                }
                else
                {
                    int indexOfSplit = sli.IndexOf(':');
                    headers[sli.Substring(0, indexOfSplit)] = sli.Substring(indexOfSplit + 1);
                }
            }

            return headers;
        }

        /// <summary>
        /// 将response返回的错误信息转换成Exception。
        /// 
        /// </summary>
        /// <param name="errorCode">错误编码</param>
        /// <param name="response">API返回的代码</param>
        /// <returns></returns>
        public Exception responseToException(uint errorCode, string response)
        {
            string errorMsg = "unknown error";
            JObject jobject = JObject.Parse(response);
            JToken errorsToke = null;

            if (jobject.TryGetValue("errors", out errorsToke))
            {
                errorMsg = jobject["errors"][0]["message"].ToString();
            }

            throw new Exception(errorCode + ": "+ response);
        }
    }
}
