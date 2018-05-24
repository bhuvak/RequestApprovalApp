namespace RequestApprovalTestApp
{
    using Microsoft.Azure.ActiveDirectory.ERM.Utils.Authentication;
    using Microsoft.Azure.ActiveDirectory.ERM.Utils.Interfaces.Authentication;
    using Microsoft.Azure.ActiveDirectory.ERM.Utils.Interfaces.Environment;
    using Microsoft.Practices.Unity;

    public static class ContainerBootstrapper
    {
        public static IUnityContainer RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ISettings, Settings>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEvoStsEnvironmentSettings, Settings>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAadApplicationEnvironmentSettings, Settings>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAccessTokenGetter, AppAccessTokenGetter>(new PerResolveLifetimeManager());
            container.RegisterType<IHttpClient, RequestApprovalHttpClient>(new ContainerControlledLifetimeManager());
            //container.RegisterType<Microsoft.Azure.ActiveDirectory.ERM.Utils.Interfaces.Authentication.ICurrentTenantGetter, Microsoft.Azure.ActiveDirectory.ERM.Utils.Authentication.CurrentTenantFromCurrentIdentity>(new PerRequestLifetimeManager());
            container.RegisterType<ITenantGetter, TenantIdFromClaimsGetter>(new PerRequestLifetimeManager());
            return container;
        }
    }
}