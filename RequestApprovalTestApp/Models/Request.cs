namespace RequestApprovalTestApp.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Request : Microsoft.Azure.ActiveDirectory.RequestApprovals.Api.Models.Request
    {
        [DataMember]
        public string SupportRequestId { get; set; }

        [DataMember]
        public string ApprovalDuration { get; set; }

        [DataMember]
        public string CorrelationId { get; set; }

        [DataMember]
        public string ResourceId { get; set; }
    }
}