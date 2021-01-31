using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    /// <summary>Basic Authentication HeaderValue</summary>
    /// <seealso cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" />
    public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        /// <inheritdoc />
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public BasicAuthenticationHeaderValue(string userName, string password) : base("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"))) { }
    }

    /// <summary>
    /// HTTP Basic Authentication authorization header for RFC6749 client authentication
    /// </summary>
    /// <seealso cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" />
    public class BasicAuthenticationOAuthHeaderValue : AuthenticationHeaderValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Net.Http.BasicAuthenticationOAuthHeaderValue" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public BasicAuthenticationOAuthHeaderValue(string userName, string password) : base("Basic", EncodeCredential(userName, password))
        {
        }

        private static string EncodeCredential(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            return Convert.ToBase64String($"{UrlEncode(userName)}:{UrlEncode(password)}".ToByteArray());
        }

        private static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return Uri.EscapeDataString(value).Replace("%20", "+");
        }
    }

    public static class HttpClientExtension
    {
        private const string JsonMediaType = "application/json";

        /// <summary>
        /// PostAsJsonAsync
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        /// <param name="requestUri">requestUri</param>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>([NotNull] this HttpClient httpClient, string requestUri, T parameter)
            => httpClient.PostAsync(requestUri, new StringContent(parameter?.ToJson() ?? "", Encoding.UTF8, JsonMediaType));

        /// <summary>
        /// PutAsJsonAsync
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        /// <param name="requestUri">requestUri</param>
        /// <param name="parameter">param</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PutAsJsonAsync<T>([NotNull] this HttpClient httpClient, string requestUri, T parameter)
            => httpClient.PutAsync(requestUri, new StringContent(parameter?.ToJson() ?? "", Encoding.UTF8, JsonMediaType));

        /// <summary>
        /// PostAsFormAsync
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        /// <param name="requestUri">requestUri</param>
        /// <param name="paramDic">paramDictionary</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsFormAsync([NotNull] this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> paramDic)
            => httpClient.PostAsync(requestUri, new FormUrlEncodedContent(paramDic));

        /// <summary>
        /// PutAsFormAsync
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        /// <param name="requestUri">requestUri</param>
        /// <param name="paramDic">paramDictionary</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PutAsFormAsync([NotNull] this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> paramDic)
            => httpClient.PutAsync(requestUri, new FormUrlEncodedContent(paramDic));

        /// <summary>
        /// HttpClient Post a file
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        /// <param name="requestUrl">requestUrl</param>
        /// <param name="filePath">filePath</param>
        /// <param name="fileKey">fileKey,default is "file"</param>
        /// <param name="formFields">formFields,default is null</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostFileAsync([NotNull] this HttpClient httpClient,
            string requestUrl,
            string filePath,
            string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>>? formFields = null)
        {
            var content = new MultipartFormDataContent($"form--{DateTime.Now.Ticks:X}");

            if (formFields != null)
            {
                foreach (var kv in formFields)
                {
                    content.Add(new StringContent(kv.Value), kv.Key);
                }
            }

            content.Add(new StreamContent(File.OpenRead(filePath)), fileKey, Path.GetFileName(filePath));

            return httpClient.PostAsync(requestUrl, content);
        }

        /// <summary>
        /// HttpClient Post a file
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        /// <param name="requestUrl">requestUrl</param>
        /// <param name="file">fileStream</param>
        /// <param name="fileName">fileName</param>
        /// <param name="fileKey">fileKey,default is "file"</param>
        /// <param name="formFields">formFields,default is null</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostFileAsync([NotNull] this HttpClient httpClient,
            string requestUrl,
            Stream? file,
            string fileName,
            string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>>? formFields = null)
        {
            if (file == null)
            {
                return await httpClient.PostAsFormAsync(requestUrl, formFields ?? Array.Empty<KeyValuePair<string, string>>());
            }

            var content = new MultipartFormDataContent($"form--{DateTime.Now.Ticks:X}");

            if (formFields != null)
            {
                foreach (var kv in formFields)
                {
                    content.Add(new StringContent(kv.Value), kv.Key);
                }
            }

            content.Add(new StreamContent(file), fileKey, fileName);

            return await httpClient.PostAsync(requestUrl, content);
        }

        public static Task<HttpResponseMessage> PostFileAsync([NotNull] this HttpClient
                client, string requestUrl, ICollection<string> filePaths,
            IEnumerable<KeyValuePair<string, string>>? formFields = null) => client.PostFileAsync(requestUrl, filePaths.Select(p =>
                    new KeyValuePair<string, Stream>(Path.GetFileName(p), File.OpenRead(p))), formFields);

        /// <summary>
        /// HttpClient Post files. <see href="https://stackoverflow.com/questions/18059588/httpclient-multipart-form-post-in-c-sharp">See here</see>.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="files">The files. Key: File name, value: file read stream.</param>
        /// <param name="formFields">The form.</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostFileAsync([NotNull] this HttpClient httpClient,
            string requestUri,
            IEnumerable<KeyValuePair<string, Stream>>? files,
            IEnumerable<KeyValuePair<string, string>>? formFields = null)
        {
            if (files == null)
            {
                return await httpClient.PostAsFormAsync(requestUri, formFields ?? Array.Empty<KeyValuePair<string, string>>());
            }

            var content = new MultipartFormDataContent($"form--{DateTime.Now.Ticks:X}");

            if (formFields != null)
            {
                foreach (var kv in formFields)
                {
                    content.Add(new StringContent(kv.Value), kv.Key);
                }
            }

            foreach (var file in files)
            {
                content.Add(new StreamContent(file.Value), Path.GetFileNameWithoutExtension(file.Key), Path.GetFileName(file.Key));
            }

            return await httpClient.PostAsync(requestUri, content);
        }

        /// <summary>
        /// Sets the basic authentication.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthentication([NotNull] this HttpClient client, string userName, string password) =>
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);

        /// <summary>
        /// Sets a basic authentication header.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthentication(this HttpRequestMessage request, string userName, string password) => request.Headers.Authorization = new BasicAuthenticationHeaderValue(userName, password);

        /// <summary>
        /// Sets a basic authentication header for RFC6749 client authentication.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthenticationOAuth(this HttpRequestMessage request, string userName, string password) => request.Headers.Authorization = new BasicAuthenticationOAuthHeaderValue(userName, password);

        /// <summary>
        /// Sets the token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="token">The token.</param>
        public static void SetToken([NotNull] this HttpClient client, string scheme, string token) =>
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

        /// <summary>
        /// Sets an authorization header with a given scheme and value.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="token">The token.</param>
        public static void SetToken(this HttpRequestMessage request, string scheme, string token) => request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);

        /// <summary>
        /// Sets the bearer token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The token.</param>
        public static void SetBearerToken([NotNull] this HttpClient client, string token) => client.SetToken("Bearer", token);

        /// <summary>
        /// Sets an authorization header with a bearer token.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="token">The token.</param>
        public static void SetBearerToken(this HttpRequestMessage request, string token) => request.SetToken("Bearer", token);
    }
}
