namespace RequestApprovalTestApp
{
    using System.Configuration;

    public class Settings : ISettings
    {
        public string LoginHost { get; }
        public string ApplicationId { get; }
        public string AppKey { get; }
        public string AppTenant { get; }
        public string PartnerId { get; }
        public string RequestApprovalHost { get; }
        public string RequestApprovalAppId { get; }

        public Settings()
        {
            this.PartnerId = ConfigurationManager.AppSettings["partnerId"];
            this.RequestApprovalHost = ConfigurationManager.AppSettings["requestApprovalHost"];
            this.RequestApprovalAppId = ConfigurationManager.AppSettings["requestApprovalAppId"];
            this.ApplicationId = ConfigurationManager.AppSettings["appId"];
            this.LoginHost = ConfigurationManager.AppSettings["evoStsLoginHost"];
            this.AppKey = ConfigurationManager.AppSettings["appKey"];
            this.AppTenant = ConfigurationManager.AppSettings["appTenant"];
        }
    }
}
