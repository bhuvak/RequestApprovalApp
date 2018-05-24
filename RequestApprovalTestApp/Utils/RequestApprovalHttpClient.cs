namespace RequestApprovalTestApp
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System;

    public class RequestApprovalHttpClient : IHttpClient
    {
        private readonly HttpClient client;

        public RequestApprovalHttpClient()
        {
            this.client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, string token)
        {
            this.AddToken(token);
            return this.client.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, string token, object content)
        {
            this.AddToken(token);
            return this.client.PostAsync(requestUri, ToStringContent(content));
        }

        private static StringContent ToStringContent(object content)
        {
            return new StringContent(JsonConvert.SerializeObject(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json");
        }

        private void AddToken(string token)
        {
            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}