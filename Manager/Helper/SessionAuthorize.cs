using Manager.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Manager.Helper
{
    public class SessionAuthorize : AuthorizeAttribute
    {
        //protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        //{
        //    if (String.IsNullOrEmpty(context.HttpContext.Session.GetString(Constants.ID_FACEBOOK)))
        //    {
        //        context.Result = new RedirectResult("/Account/Login");
        //    }
        //}
    }
}
