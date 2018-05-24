namespace RequestApprovalTestApp
{
    using Microsoft.Azure.ActiveDirectory.ERM.Utils.Interfaces.Environment;

    public interface ISettings : IEvoStsEnvironmentSettings, IAadApplicationEnvironmentSettings
    {
        string PartnerId { get; }
        string RequestApprovalHost { get; }
        string RequestApprovalAppId { get; }
    }
}