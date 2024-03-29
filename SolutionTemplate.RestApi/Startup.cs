﻿using IdentityServer3.AccessTokenValidation;
using Microsoft.Owin;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using SharpRepository.Ioc.Ninject;
using SharpRepository.Repository.Ioc;
using SolutionTemplate.Core.ServiceInterfaces;
using SolutionTemplate.DataAccess;
using SolutionTemplate.RestApi.Authorization;
using SolutionTemplate.Service;
using System.Configuration;
using System.Data.Entity;

[assembly: OwinStartup(typeof(SolutionTemplate.RestApi.Startup))]

namespace SolutionTemplate.RestApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .UseResourceAuthorization(new ApiAuthorizationManager())
                .UseIdentityServerBearerTokenAuthentication(
                    new IdentityServerBearerTokenAuthenticationOptions
                    {
                        Authority = "https://localhost:44375/identity",
                        //AuthenticationType = "Bearer",
                        //AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                        //ValidationMode = ValidationMode.Local,
                        RequiredScopes = new[] { "openid", "profile", "solution-template-api" }
                    });

            app
                .UseNinjectMiddleware(CreateKernel)
                .UseNinjectWebApi(WebApiConfig.Register());
        }

        private static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.BindSharpRepository();

            RepositoryDependencyResolver.SetDependencyResolver(new NinjectDependencyResolver(kernel));

            var connectionString = ConfigurationManager.ConnectionStrings["SolutionTemplate"].ConnectionString;

            kernel.Bind<DbContext>()
                .To<SolutionTemplateContext>()
                .InRequestScope()
                .WithConstructorArgument("connectionString", connectionString);

            kernel.Bind<IWidgetService>().To<WidgetService>();

            return kernel;
        }
    }
}