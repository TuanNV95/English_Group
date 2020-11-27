using Microsoft.AspNetCore.Mvc;

namespace Manager.Helper
{
    public static class SesionHelper
    {
        public static RedirectResult CheckLogin()
        {
            return new RedirectResult("/Login");
        }
    }
}
