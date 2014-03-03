﻿using Autofac;
using Autofac.Integration.WebApi;
using Thinktecture.IdentityServer.Core.Connect;
using Thinktecture.IdentityServer.Core.Connect.Services;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.Configuration
{
    public class OurModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
        }
    }

    public static class AutoFacConfig
    {
        public static IContainer Configure(IdentityServerServiceFactory fact)
        {
            fact.Validate();

            var builder = new ContainerBuilder();

            // mandatory from factory
            builder.Register(ctx => fact.AuthorizationCodeStore()).As<IAuthorizationCodeStore>();
            builder.Register(ctx => fact.CoreSettings()).As<ICoreSettings>();
            builder.Register(ctx => fact.Logger()).As<ILogger>();
            builder.Register(ctx => fact.TokenHandleStore()).As<ITokenHandleStore>();
            builder.Register(ctx => fact.UserService()).As<IUserService>();

            // optional from factory
            if (fact.ClaimsProvider != null)
            {
                builder.Register(ctx => fact.ClaimsProvider()).As<IClaimsProvider>();
            }
            else
            {
                builder.RegisterType<DefaultClaimsProvider>().As<IClaimsProvider>();
            }

            if (fact.AssertionGrantValidator != null)
            {
                builder.Register(ctx => fact.AssertionGrantValidator()).As<IAssertionGrantValidator>();
            }
            else
            {
                builder.RegisterType<DefaultAssertionGrantValidator>().As<IAssertionGrantValidator>();
            }

            // validators
            builder.RegisterType<TokenRequestValidator>();
            builder.RegisterType<AuthorizeRequestValidator>();
            builder.RegisterType<UserInfoRequestValidator>();
            builder.RegisterType<ClientValidator>();

            // processors
            builder.RegisterType<TokenResponseGenerator>();
            builder.RegisterType<AuthorizeResponseGenerator>();
            builder.RegisterType<AuthorizeInteractionResponseGenerator>();
            builder.RegisterType<UserInfoResponseGenerator>();

            // services
            builder.RegisterType<DefaultTokenService>().As<ITokenService>();

            // controller
            builder.RegisterApiControllers(typeof(AuthorizeEndpointController).Assembly);

            return builder.Build();
        }
    }
}