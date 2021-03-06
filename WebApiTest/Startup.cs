﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using WebApiTest.Auth;
using Swashbuckle.Application;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using System.Configuration;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Ninject.Web.Common.OwinHost;
using Ninject;
using System.Reflection;
using Ninject.Web.WebApi.OwinHost;
using WebApiTest.Intefaces;
using WebApiTest.Services;
using Ninject.Web.Common;
using Ninject.Web.WebApi;
using System.Web;

[assembly: OwinStartup(typeof(WebApiTest.Startup))]

namespace WebApiTest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Run(context =>
            //{
            //    if (context.Request.Path.Value == "/fail")
            //    {
            //        throw new Exception("Random exception");
            //    }

            //    context.Response.ContentType = "text/plain";
            //    return context.Response.WriteAsync("Hello, world.");
            //});

            

            HttpConfiguration config = new HttpConfiguration();
            //Swagger Registration
            SwaggerConfig.Register(config);

            ConfigureOAuth(app);
            WebApiConfig.Register(config);

            app.UseErrorPage()
               .UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll) //Allow Cors Requests
               .UseNinjectMiddleware(CreateKernel) //Ninject
               .UseNinjectWebApi(config);
            
            //app.UseWebApi(config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureOAuth(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["issuer"];
            var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            OAuthAuthorizationServerOptions OAuthServerOptions = 
                new OAuthAuthorizationServerOptions()
                {
                    AllowInsecureHttp = true,
                    TokenEndpointPath = new PathString("/oauth2/token"),
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                    Provider = new CustomOAuthProvider(),
                    AccessTokenFormat = new CustomJwtFormat(issuer)
                };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            var jwtBearerAuthenticationOptions = new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { ConfigurationManager.AppSettings["server_audience"] },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
               {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
               }
            };
            app.UseJwtBearerAuthentication(jwtBearerAuthenticationOptions);

        }

        private static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                GlobalConfiguration.Configuration.DependencyResolver =
                    new NinjectDependencyResolver(kernel);

                //kernel.Load(Assembly.GetExecutingAssembly());
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
           
        }

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IDbProvider>().To<MongoDbProvider>();//.InRequestScope();
            kernel.Bind<ILogService>().To<NLogService>();
            //kernel.Bind(typeof(IDbProvider)).To(typeof(MongoDbProvider)).InRequestScope();  
        }
    }
}
