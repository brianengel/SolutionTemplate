﻿using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace SolutionTemplate.RestApi.Authorization
{
    public class ApiAuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            switch (context.Resource.First().Value)
            {
                case Resource.Widgets:
                    return AuthorizeWidgets(context);

                default:
                    return Nok();
            }
        }

        private Task<bool> AuthorizeWidgets(ResourceAuthorizationContext context)
        {
            switch (context.Action.First().Value)
            {
                case Action.Read:
                    return Eval(context.Principal.HasClaim("role", Role.ApiReadWidgets) || context.Principal.HasClaim("role", Role.ApiWriteWidgets));

                default:
                    return Nok();
            }
        }
    }
}