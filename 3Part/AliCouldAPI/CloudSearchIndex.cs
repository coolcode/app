// -----------------------------------------------------------------------
// <copyright file="CloudSearchIndex.cs" company="Aliyun-inc">
// CloudSearchIndex
// </copyright>
// -----------------------------------------------------------------------

namespace AliCould.com.API
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using System.Collections.Specialized;
    using Newtonsoft.Json;

    /// <summary>
    /// 云搜索客户端API索引结构。
    /// 
    /// 此类用于管理和检索索引数据的接口，包含以下功能：
    /// 
    /// 1. 获取指定索引状态信息;
    /// 2. 针对指定的索引添加或删除文档;
    /// 3. 获取指定索引状态;
    /// 4. 根据关键词或某些特定的条件查询索引。
    ///
    /// @package CloudSearchAPI
    /// @version 0.1.0
    /// @filesource 
    /// 
    /// </summary>
    public class CloudSearchIndex
    {

        /// <summary>
        /// CloudSearchAPI 对象。
        /// </summary>
        private CloudSearchApi _api = null;

        /// <summary>
        /// 索引名称，在该类被实例化的时候引入。
        /// </summary>
        private string _indexName = null;

        /// <summary>
        /// 访问或操作指定索引的URL。
        /// </summary>
        private string _indexURL = null;

        /// <summary>
        /// 查询、添加或删除指定索引中文档的URL。
        /// </summary>
        private string _docURL = null;

        /// <summary>
        /// 查询top query 的文档URL。
        /// </summary>
        private string _topURL = null;

        /// <summary>
        /// 获取指定索引元信息的URL。
        /// </summary>
        private string _metaURL = null;

        /// <summary>
        /// 获取指定索引错误信息的URL。
        /// </summary>
        private string _errorURL = null;

        /// <summary>
        /// 执行指定索引查询的URL。
        /// </summary>
        private string _searchURL = null;

        /// <summary>
        /// 指定索引的元信息。
        /// </summary>
        private JObject _meta = null;

        /// <summary>
        /// facet查询支持的参数列表。
        /// </summary>
        public static string[] FACET_PARAMS = new string[] 
        { 
            "facet_key", "facet_range", "facet_fun", "facet_filter", "facet_sampler_threshold", "facet_sampler_step", "facet_max_group"
        };

        /// <summary>
        /// 
        /// 构造函数，由CloudSearchApi对象调用，请点击以下链接来获取CloudSearchApi的
        /// getIndex方法详情。
        /// 
        /// </summary>
        /// <param name="api">用于管理索引的API对象</param>
        /// <param name="indexName">索引名称。</param>
        public CloudSearchIndex(CloudSearchApi api, string indexName)
        {
            this._api = api;
            this._indexName = indexName;

            this._indexURL = api.indexURL(this._indexName);
            this._docURL = api.docURL(this._indexName);
            this._topURL = api.topURL(this._indexName);
            this._metaURL = api.statusURL(this._indexName);
            this._errorURL = api.errorURL(this._indexName);
            this._searchURL = api.searchURL(this._indexName);
        }

        /// <summary>
        /// 获取索引所属的索引模板名称。
        /// </summary>
        /// <returns>返回该指定索引所使用的模板类型。</returns>
        public string getTemplate()
        {
            this._refreshMeta();
            return this._meta["result"]["template"].ToString();
        }

        /// <summary>
        /// 获取索引的最后更新时间戳。
        /// </summary>
        /// <returns>返回该索引的最后更新时间戳，精确到秒；如果没有获取到更新时间，则返回为空。</returns>
        public int getUpdateTime()
        {
            this._refreshMeta();
            return (int)this._meta["result"]["doc_last_update_time"];
        }

        /// <summary>
        /// 获取索引中包含的文档的总数量。
        /// 
        /// </summary>
        /// <returns>返回文档总数量。</returns>
        public int getTotalDocCount()
        {
            this._refreshMeta();
            string totalDocNum = this._meta["result"]["total_doc_num"].ToString(Formatting.None);
            int count = Convert.ToInt32(totalDocNum.Trim('{', '}', '"'));
            return count;
        }

        /// <summary>
        /// 获取索引上一天的UV值。
        /// </summary>
        /// <returns>返回索引的UV值，为前一天UV的数量。</returns>
        public int getUV()
        {
            this._refreshMeta();
            return (int)this._meta["result"]["uv"];
        }

        /// <summary>
        /// 获取索引上一天的PV值。
        /// </summary>
        /// <returns>返回索引的PV值，为前一天的PV的数量。</returns>
        public int getPV()
        {
            this._refreshMeta();
            return (int)this._meta["result"]["pv"];
        }

        /// <summary>
        /// 
        ///  获取索引的最近错误列表。
        ///  
        /// </summary>
        /// <param name="page">指定获取第几页的错误信息。</param>
        /// <param name="pageSize">指定每页显示的错误条数。</param>
        /// <returns>返回指定页数的错误信息列表。</returns>
        /// <exception cref="Exception">如果参数不正确，则抛出此异常。</exception>
        public string getErrorMessage(string page, string pageSize)
        {
            this._checkPageClause(page);
            this._checkPageSizeClause(pageSize);

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("page", page);
            parameters.Add("page_size", pageSize);

            return this._api.apiCall(this._errorURL, parameters);
        }

        /// <summary>
        /// 通过文档ID获取文档的详细内容。
        /// </summary>
        /// <param name="docId">指定文档的唯一编码。</param>
        /// <returns>返回文档内容的数据</returns>
        /// <exception cref="Exception">如果参数不正确，则抛出此异常。</exception>
        public string getDocument(string docId)
        {
            if (string.IsNullOrEmpty(docId))
            {
                throw new ArgumentNullException("$docId is NULL");
            }

            string resultStr = this._api.apiCall(this._docURL + '/' + docId);
            if (JObject.Parse(resultStr)["result"].ToString().Length == 2 && JArray.FromObject(JObject.Parse(resultStr)["result"]).Count == 0)
            {
                return null;
            }

            return resultStr;
        }

        /// <summary>
        /// 
        /// 将一篇文档添加到指定索引中。
        /// 
        /// NOTE: 如果要添加的文档ID在索引中已经存在，则会覆盖掉此索引中的文档数据。
        /// @todo 目前文档内容只支持utf-8编码，其他编码持将在后续版本实现
        /// 
        /// 例子:
        /// <code>
        /// var api = new CloudSearchApi("http://api.css.aliyun.com", "1", "xxx");
        /// var index = api.getIndex("myIndex");
        /// NameValueCollection fields = new NameValueCollection();
        /// fields.Add("title", "the title content");
        /// fields.Add("body", "the body content");
        /// try{
        ///     index.addDocument(1, fields);
        /// } catch(ArgumentException ex)
        /// {
        ///     //echo "Invalid argument " + e.Message;
        /// } catch(CloudSearchAuthorizeFailed ex)
        /// {
        ///     //echo ex.Message;
        /// } catch(CloudSearchHttpException ex)
        /// {
        ///     //echo "HTTP Exception";
        /// }
        /// 
        /// // TODO: Add sample code here
        /// </code>
        /// </summary>
        /// <param name="docId">文档唯一编码，此编码不能重复，如果重复则会替换掉系统中现有
        ///     的数据信息。</param>
        /// <param name="fields">已Key-value形式表示的文档的字段内容；fields中的字段内容需要和当前索引模板指定的字段一致。</param>
        /// <exception cref="ArgumentException">如果参数不正确，则抛出此异常。</exception>
        /// <exception cref="CloudSearchIndexNotExists">如果索引不存在，则抛出此异常。</exception>
        /// <exception cref="CloudSearchAuthorizeFailed">如果用户安全验证失败，则抛出磁异常。</exception>
        /// <exception cref="CloudSearchHttpException">如果出现网络错误，则抛出此异常。</exception>
        public void addDocument(string docId, NameValueCollection fields)
        {
            var docs = this.makeDocument("add", docId, fields, null);
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("action", "push");
            parameters.Add("items", docs);

            this._api.apiCall(this._docURL, parameters, "POST");
        }

        /// <summary>
        /// 
        /// 将一批文档添加到索引中。
        /// 
        /// </summary>
        /// <param name="documents">多篇文档的文档内容。</param>
        public void addDocuments(params string[] documents)
        {
            foreach (string document in documents)
            {
                JObject documentInJObject = JObject.Parse(document);
                if (null == documentInJObject["fields"])
                {
                    throw new ArgumentException("document $docs misses 'fields' section.");
                }

                if (null == JObject.Parse(documentInJObject["fields"].ToString(Formatting.None))["id"])
                {
                    throw new ArgumentException("document $fields misses 'id' section.");
                }
            }

            JArray documentsInJArray = new JArray();
            foreach (var doc in  documents )
            {
                documentsInJArray.Add(JObject.Parse(doc));
            }

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("action", "push");

            parameters.Add("items", documentsInJArray.ToString(Formatting.None));

            this._api.apiCall(this._docURL, parameters, "POST");
        }

        /// <summary>
        /// 
        /// 同时删除当前索引下的多条文档。
        /// 
        /// </summary>
        /// <param name="docIds">文档ID列表。</param>
        /// <exception cref="Exception">如果参数不正确，则抛出此异常。</exception>
        public void deleteDocuments(params string[] docIds)
        {
            if (null == docIds || docIds.Length < 1)
            {
                throw new ArgumentNullException("docIds is empty");
            }

            List<string> madeDocs = new List<string>();
            foreach (var docId in docIds)
            {
                madeDocs.Add(this.makeDocument("delete", docId, null, null));
            }

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("action", "push");

            // Convert the json string to JObject to construct JArray.
            JArray items = new JArray();
            foreach (var s in madeDocs)
            {
                foreach (var doc in JArray.Parse(s).Values())
                {
                    items.Add(doc);
                }
            }

            string itemsValueString = items.ToString(Formatting.None);

            parameters.Add("items", itemsValueString);

            this._api.apiCall(this._docURL, parameters, "POST");
        }

        public void topQuery(int num = 0, int days = 0)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("num", num.ToString());
            parameters.Add("days", days.ToString());

            this._api.apiCall(this._topURL, parameters);
        }

        /// <summary>
        /// 
        /// 根据指定的关键词或条件查询索引。
        /// 
        /// 例子:
        /// <code>
        /// $api = new CloudSearchApi(APIROOT, CLIENT_ID, CLIENT_SECRET);
        /// $index = $api->getIndex('test_index');
        /// $fields = array('title' => 'the title content',
        ///                 'body' => 'the body content');
        /// $index->addDocument(1, $fields);
        ///
        /// try {
        ///     $index->search('q=test');
        /// } catch (InvalidArgumentException $e) {
        ///    echo 'Invalid argument ' . $e->getMessage();
        /// } catch (CloudSearchAuthorizeFailed $e) {
        ///    echo $e->getMessage();
        /// } catch (CloudSearchHttpException $e) {
        ///    echo 'HTTP exception: ' . $e->getMessage();
        /// }
        /// </code>
        /// 
        /// </summary>
        /// <param name="query">
        /// 查询子句，支持简单和复杂查询表达式，对于简单的关键词查询: 
        ///     q=query_expression, 例如: q=apple
        ///     对于语法丰富的查询（布尔或短语查询等）: 
        ///     cq=title:iphone AND uname:phpwind AND price=1000...6000
        /// </param>
        /// <param name="page">返回查询结果的第几页，默认返回第一页。</param>
        /// <param name="pageSize">每一页结果条数，默认是10条。</param>
        /// <param name="sort">排序子句。</param>
        /// <param name="filter">过滤子句。</param>
        /// <param name="facet">Facet子句。</param>
        /// <param name="fetchFields">查询结果中需要返回的字段，多个字段用";"分割。</param>
        /// <returns>返回查询结果。</returns>
        /// <exception cref="ArgumentException">如果参数不正确，则抛出此异常。</exception>
        /// <exception cref="CloudSearchIndexNotExists">如果索引不存在，则抛出此异常。</exception>
        /// <exception cref="CloudSearchAuthorizeFailed">如果用户安全验证失败，则抛出磁异常。</exception>
        /// <exception cref="CloudSearchHttpException">如果出现网络错误，则抛出此异常。</exception>
        public string search(string query, string page = null, string pageSize = null, string[] sort = null,
            string filter = null, NameValueCollection facet = null, string[] fetchFields = null)
        {
            NameValueCollection args = new NameValueCollection();
            if (null != page)
            {
                this._checkPageClause(page);
                args.Add("page", page);
            }

            if (null != pageSize)
            {
                this._checkPageSizeClause(pageSize);
                {
                    args.Add("page_size", pageSize);
                }
            }

            if (null != sort && sort.Length > 0)
            {
                args.Add("sort", Utilities.Implode(sort, ";"));
            }

            if (null != filter)
            {
                args.Add("filter", filter);
            }

            if (null != facet)
            {
                this._checkFacetClause(facet);
                List<string> mappedFacet = new List<string>();
                foreach (var key in facet.AllKeys)
                {
                    mappedFacet.Add(string.Format("{0}:{1}", key, facet[key]));
                }

                args.Add("facet", Utilities.Implode(mappedFacet.ToArray(),","));
            }

            if (null != fetchFields)
            {
                args.Add("fetch_fields", Utilities.Implode(fetchFields,";"));
            }

            if (query.StartsWith("q="))
            {
                args.Add("q", query.Substring(2));
            }
            else if (query.StartsWith("cq="))
            {
                args.Add("cq", query.Substring(3));
            }
            else
            {
                throw new ArgumentException(
                    "query is invalid, missing \"q\" or \"cq\" clause."
                    );
            }

            return this._api.apiCall(this._searchURL, args);
        }

        /// <summary>
        /// 组装单篇文档内容。
        /// </summary>
        /// <param name="action">文档处理方式，'add' 表示添加文档，'delete'表示删除文档。</param>
        /// <param name="docId">文档ID。</param>
        /// <param name="fields">key-value形式的字段内容。</param>
        /// <param name="charset">文档编码。</param>
        /// <returns></returns>
        public string makeDocument(string action, string docId, NameValueCollection fields, string charset)
        {
            if (string.IsNullOrEmpty(docId))
            {
                throw new ArgumentException("docId can not be null", "docId");
            }

            JObject resultDoc = new JObject();
            resultDoc.Add("cmd", action);

            if (null != charset)
            {
                resultDoc["charset"] = charset;
            }

            if (null != fields && fields.Count > 0)
            {
                JProperty idProp = new JProperty("id", docId);
                resultDoc["fields"] = new JObject(idProp, Utilities.ToJProperty(fields));
                //resultDoc.Add("fields", fields);
            }
            else
            {
                resultDoc["fields"] = new JObject();
                resultDoc["fields"]["id"] = docId;
            }

            return new JArray(resultDoc).ToString(Formatting.None);
        }

        /// <summary>
        /// 重新获取索引的元信息
        /// </summary>
        private void _refreshMeta()
        {
            this._meta = JObject.Parse(this._api.apiCall(this._metaURL));
        }

        /// <summary>
        /// 检查page参数是否合法。
        /// </summary>
        /// <param name="page">指定的页码。</param>
        /// <exception cref="ArgumentException">如果参数不正确，则抛出此异常。</exception>
        private void _checkPageClause(object page)
        {
            int result;
            if (null == page || !int.TryParse(page.ToString(), out result))
            {
                throw new ArgumentException("Page is not an integer", "page");
            }

            if (result <= 0)
            {
                throw new ArgumentException("Page is not greater than or equal to 0", "page");
            }
        }

        /// <summary>
        /// 检查pageSize参数是否合法。
        /// </summary>
        /// <param name="pageSize"></param>
        /// <exception cref="ArgumentException">参数不合法</exception>
        private void _checkPageSizeClause(object pageSize)
        {
            int result;
            if (null == pageSize || !int.TryParse(pageSize.ToString(), out result))
            {
                throw new ArgumentException("pageSize is not an integer", "pageSize");
            }

            if (result <= 0)
            {
                throw new ArgumentException("pageSize is not greater than or equal to 0", "pageSize");
            }
        }

        /// <summary>
        /// 检查facet子句是否合法。
        /// </summary>
        /// <param name="facet"></param>
        /// <exception cref="ArgumentException">参数不合法</exception>
        private void _checkFacetClause(NameValueCollection facet)
        {
            if (null == facet || facet.Count == 0)
            {
                throw new ArgumentException("Invalid parameter: facet clause", "facet");
            }

            foreach (var key in facet.Keys)
            {
                if (!Utilities.Contains(FACET_PARAMS, key.ToString()))
                {
                    throw new ArgumentException("Invalid parameter: key in facet clause", "facet");
                }
            }
        }
    }
}
