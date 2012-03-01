using System;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityModel.Utility;
using Thinktecture.IdentityModel.Web.Old;

namespace WrapClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var wrapClient = new WrapClient(new Uri("https://roadie/idsrv/issue/wrap"));
            var swt = wrapClient.Issue("dominick", "abc!123", new Uri("https://roadie/rp/"));

            Console.WriteLine(swt.ToString());

            var id = swt.ToClaimsIdentity();
            ClaimsViewer.ShowConsole(ClaimsPrincipal.CreateFromIdentity(id));
        }
    }
}
