using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace SeaSharp.Utils.CSharp
{
    public static class HttpUtils
    {
        #region Get同步请求
        public static string Get(string url, string token)
        {
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                if (!string.IsNullOrEmpty(token))
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "bearer " + token);
                }
                return client.DownloadString(url);
            }
        }
        #endregion

        #region Get异步请求
        public static void GetAsync(string url, string token, Action<string> callback, Action<Exception> errorback)
        {
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                if (!string.IsNullOrEmpty(token))
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "bearer " + token);
                }
                client.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        callback?.Invoke(e.Result);
                    }
                    else
                    {
                        errorback?.Invoke(e.Error);
                    }
                };
                client.DownloadStringAsync(new Uri(url));
            }
        }
        #endregion

        #region Post同步请求
        public static string Post(string url, string token, params string[] nameValues)
        {
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                if (!string.IsNullOrEmpty(token))
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "bearer " + token);
                }

                var length = nameValues.Length;
                switch (length)
                {
                    case 0:
                        return client.UploadString(url, "");
                    case 1:
                        return client.UploadString(url, nameValues[0]);
                    default:
                        if (length % 2 != 0)
                        {
                            throw new Exception("参数错误");
                        }

                        var collection = new NameValueCollection();
                        for (int i = 0; i < length; i += 2)
                        {
                            collection.Add(nameValues[i], nameValues[i + 1]);
                        }
                        var bytes = client.UploadValues(url, collection);
                        return Encoding.UTF8.GetString(bytes);
                }
            }
        }
        #endregion

        #region Post异步方法
        public static void PostAsync(string url, string token, string data, Action<string> callback, Action<Exception> errorback)
        {
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                if (!string.IsNullOrEmpty(token))
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "bearer " + token);
                }

                client.UploadStringCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        callback?.Invoke(e.Result);
                    }
                    else
                    {
                        errorback?.Invoke(e.Error);
                    }
                };
                client.UploadStringAsync(new Uri(url), data);
            }
        }

        public static void PostAsync(string url, string token, NameValueCollection nameValues, Action<string> callback, Action<Exception> errorback)
        {
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                if (!string.IsNullOrEmpty(token))
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "bearer " + token);
                }

                client.UploadValuesCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        callback?.Invoke(Encoding.UTF8.GetString(e.Result));
                    }
                    else
                    {
                        errorback?.Invoke(e.Error);
                    }
                };
                client.UploadValuesAsync(new Uri(url), nameValues);
            }
        }
        #endregion

    }
}
