/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityModel.Utility
{
    /// <summary>
    /// This class contains helpers to dump an IClaimsPrincipal to various outputs
    /// </summary>
    public static class ClaimsViewer
    {
        /// <summary>
        /// Dumps an IClaimsPrincipal to the console.
        /// </summary>
        /// <param name="principal">The principal.</param>
        public static void ShowConsole(IClaimsPrincipal principal)
        {
            Contract.Requires(principal != null);


            int count = 1;
            foreach (IClaimsIdentity identity in principal.Identities)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Identity #{0}\n", count++);
                Console.ResetColor();

                Console.WriteLine("Principal Type:  {0}", principal.GetType().FullName);
                Console.WriteLine("Identity Type:   {0}", identity.GetType().FullName);
                Console.WriteLine();
                Console.WriteLine("User Name:       {0}", identity.Name);
                Console.WriteLine("Name Claim Type: {0}", identity.NameClaimType);
                Console.WriteLine("Role Claim Type: {0}", identity.RoleClaimType);

                //if (identity.Delegate != null)
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.WriteLine("\nDelegated via: {0}", identity.Delegate.Name);
                //    Console.ResetColor();
                //}

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nClaims:");
                Console.ResetColor();

                foreach (Claim claim in identity.Claims)
                {
                    Console.WriteLine(String.Format("\n{0}", claim.ClaimType));
                    Console.WriteLine(claim.Value);

                    if (claim.Properties.Count > 0)
                    {
                        Console.WriteLine("\nProperties:");
                        foreach (var prop in claim.Properties)
                        {
                            Console.WriteLine("  {0}\n    {1}", prop.Key, prop.Value);
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nIssuer:");
                Console.ResetColor();
                Console.WriteLine(identity.Claims[0].Issuer);
                
                //if (identity.Delegate != null)
                //{
                //    Console.ForegroundColor = ConsoleColor.Green;
                //    Console.WriteLine("\nDelegation Claims:");
                //    Console.ResetColor();

                //    foreach (Claim claim in identity.Delegate.Claims)
                //    {
                //        Console.WriteLine(String.Format("\n{0}", claim.ClaimType));
                //        Console.WriteLine(claim.Value);
                //    }
                //}
            }
        }

        /// <summary>
        /// Dumps an IClaimsPrincipal to a Windows form.
        /// </summary>
        /// <param name="principal">The principal.</param>
        public static void ShowForm(IClaimsPrincipal principal)
        {
            Contract.Requires(principal != null);
            

            Application.EnableVisualStyles();
            new ClaimsViewerForm(principal).ShowDialog();
        }

        /// <summary>
        /// Dumps an IClaimsPrincipal to an ASP.NET page.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="page">The page.</param>
        public static void ShowAspNet(IClaimsPrincipal principal, Page page)
        {
            Contract.Requires(principal != null);
            Contract.Requires(page != null);


            foreach (var identity in principal.Identities)
            {
                AddIdentityToPage(identity, page);
            }

            page.DataBind();
        }

        private static void AddIdentityToPage(IClaimsIdentity identity, Page page)
        {
            Contract.Requires(identity != null);
            Contract.Requires(identity.Claims != null);
            Contract.Requires(page != null);


            GridView view = new GridView();

            var claims = from claim in identity.Claims
                         select new
                         {
                             ClaimType = claim.ClaimType,
                             Value = claim.Value
                         };

            view.DataSource = claims;

            page.Form.Controls.Add(view);
        }
    }
}
