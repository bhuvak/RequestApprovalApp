namespace RequestApprovalTestApp.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Request : Microsoft.Azure.ActiveDirectory.RequestApprovals.Api.Models.Request
    {
        [DataMember]
        public string MyCustomField { get; set; }
    }
}