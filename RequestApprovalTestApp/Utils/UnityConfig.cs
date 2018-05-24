namespace RequestApprovalTestApp
{
    using System.Web.Mvc;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Mvc;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = ContainerBootstrapper.RegisterTypes(new UnityContainer());
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}