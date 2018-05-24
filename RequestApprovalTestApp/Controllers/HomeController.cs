namespace RequestApprovalTestApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.Azure.ActiveDirectory.ERM.Utils.Interfaces.Authentication;
    using Microsoft.Azure.ActiveDirectory.RequestApprovals.Api.Models;
    using Newtonsoft.Json;
    using Utils;
    using Request = RequestApprovalTestApp.Models.Request;

    [System.Web.Mvc.Authorize]
    public class HomeController : Controller
    {
        private readonly ISettings settings;
        private readonly IHttpClient httpClient;
        private readonly IAccessTokenGetter accessTokenGetter;
        private readonly ITenantGetter tenantGetter;

        private const string tenantId = "629f581e-6f48-4447-a75e-66789a96f33d"; // 72f988bf-86f1-41af-91ab-2d7cd011db47     629f581e-6f48-4447-a75e-66789a96f33d

        public HomeController(ISettings settings, IHttpClient httpClient, IAccessTokenGetter accessTokenGetter, ITenantGetter tenantGetter)
        {
            this.settings = settings;
            this.httpClient = httpClient;
            this.accessTokenGetter = accessTokenGetter;
            this.tenantGetter = tenantGetter;
        }

        public Task<ActionResult> Index()
        {
            return Task.FromResult<ActionResult>(View());
        }

        public async Task<ActionResult> Ping()
        {
            await PingAsync();
            return View("Index");
        }

        public async Task<ActionResult> Act()
        {
            var businessFlow = await this.CreateBusinessFlowAsync();
            //Trace.WriteLine("Business flow Id is " + businessFlow.id);
            var request = await this.CreateRequestAsync(businessFlow.id);
            //// var allRequests = await this.GetRequests();
            //Trace.WriteLine("Request Id is " + request.id);
            //var activeRequest = await this.GetRequest(request.id);
            //var approvals = await this.GetApprovals("bf1682d8-1f7e-4d4f-a783-84bb40050b89");
            //var approval =  await this.GetApproval("bf1682d8-1f7e-4d4f-a783-84bb40050b89", "d5cc6188-e549-4bdf-8707-98de68c2315e");
            return View("Index");

            //var businessFlow = new BusinessFlow { id = "5C4B9CFD-611F-4A23-9699-79FA9B218CCE" };
        }

        public async Task<ActionResult> GetRequests()
        {
            var token = this.accessTokenGetter.GetAccessToken(this.settings.RequestApprovalAppId, tenantId);
            var response = await this.httpClient.GetAsync($"https://{this.settings.RequestApprovalHost}/requestApprovals/v1.0/Requests/all(partnerId={settings.PartnerId})", token);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var odataResponse = JsonConvert.DeserializeObject<ODataResponse<Request>>(json);
            foreach (var request in odataResponse.Values)
            {
                Trace.WriteLine($"{request.id} => {request.status} => {request.reason}");
            }

            return View("Index");
        }

        private async Task<Request> GetRequest(string requestId)
        {
            var token = this.accessTokenGetter.GetAccessToken(this.settings.RequestApprovalAppId, tenantId);
            var response = await this.httpClient.GetAsync($"https://{this.settings.RequestApprovalHost}/requestApprovals/v1.0/Requests/all(partnerId={settings.PartnerId})?$filter=({nameof(Models.Request.id)} eq '{requestId}')", token);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var odataResponse = JsonConvert.DeserializeObject<ODataResponse<Request>>(json);
            Debug.Assert(odataResponse.Values.Count() == 1, $"Expected 1 request with id {requestId}");
            return odataResponse.Values.First();
        }

        private async Task<Approval> GetApproval(string requestId, string userId)
        {
            var token = this.accessTokenGetter.GetAccessToken(this.settings.RequestApprovalAppId, tenantId);
            var response = await this.httpClient.GetAsync($"https://{this.settings.RequestApprovalHost}/requestApprovals/v1.0/approvals(requestId='{requestId}',id='{userId}')", token);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var approval = JsonConvert.DeserializeObject<Approval>(json);
            return approval;
        }

        private async Task<IEnumerable<Approval>> GetApprovals(string requestId)
        {
            var token = this.accessTokenGetter.GetAccessToken(this.settings.RequestApprovalAppId, tenantId);
            var response = await this.httpClient.GetAsync($"https://{this.settings.RequestApprovalHost}/requestApprovals/v1.0/Requests('{requestId}')/approvals", token);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var odataResponse = JsonConvert.DeserializeObject<ODataResponse<Approval>>(json);
            return odataResponse.Values;
        }


        private async Task PingAsync()
        {
            var token = this.accessTokenGetter.GetAccessToken(this.settings.RequestApprovalAppId, tenantId);
            var response = await this.httpClient.GetAsync($"https://{this.settings.RequestApprovalHost}/v1.0/ping", token);
            response.EnsureSuccessStatusCode();
        }

        private async Task<BusinessFlow> CreateBusinessFlowAsync()
        {
            var businessFlow = new BusinessFlow
            {
                id = Guid.NewGuid().ToString(),
                partnerId = this.settings.PartnerId,
                description = $"Test {DateTime.Now}",
                displayName = $"Settings for role {Guid.NewGuid()}",
                settings = new BusinessFlowSettings { approverReasonRequired = true, requestExpiresInDays = 3, notificationSettings = new NotificationSettings { notificationsEnabled = true, remindersEnabled = true } },
                approvers = new List<Actor>
                {
                    new Actor
                    {
                        id = "6b845bfa-268a-4377-8187-77fc54662bff",
                        displayName = "Bhuvana",
                        userPrincipalName = "bhkrishn@microsoft.com"
                    }
                }
            };

            var token = this.accessTokenGetter.GetAccessToken(this.settings.RequestApprovalAppId, tenantId);
            var response = await this.httpClient.PostAsync($"https://{this.settings.RequestApprovalHost}/requestApprovals/v1.0/businessFlows", token, businessFlow);
           // response.EnsureSuccessStatusCode();
            var businessFlowAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BusinessFlow>(businessFlowAsJson);
        }

        private async Task<Request> CreateRequestAsync(string businessFlowId)
        {
            var request = new Request
            {
                id = Guid.NewGuid().ToString(),
                businessFlowId = businessFlowId,
                reason = $"Requesting access at {DateTime.Now}", // DO NOT DELETE : d5cc6188-e549-4bdf-8707-98de68c2315e
                targetId =
                    "61d92990-8078-4741-b1c9-94b436dee706", // Looks like Any target id is working fine here, validation is not happening. Ask Razvan
                targetDisplayName = "temp", // request beneficiery  
                targetUserPrincipalName = "temp@microsoft.com", //,
                MyCustomField = "Hello world"
            };

            var token = this.accessTokenGetter.GetAccessToken(this.settings.RequestApprovalAppId, tenantId);
            var response = await this.httpClient.PostAsync(
                $"https://{this.settings.RequestApprovalHost}/requestApprovals/v1.0/requests", token, request);
            response.EnsureSuccessStatusCode();
            var requestAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Request>(requestAsJson);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}