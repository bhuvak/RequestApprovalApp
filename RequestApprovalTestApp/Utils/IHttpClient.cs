namespace RequestApprovalTestApp
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, string token);

        Task<HttpResponseMessage> PostAsync(string requestUri, string token, object content);
    }
}