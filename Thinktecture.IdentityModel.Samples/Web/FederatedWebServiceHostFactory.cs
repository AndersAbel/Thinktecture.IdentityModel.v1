/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

namespace Thinktecture.IdentityModel.Web.Old
{
    /// <summary>
    /// A Service Host Factory to integrate REST based WCF Services with Token-based Authentication
    /// </summary>
    [Obsolete]
    public class FederatedWebServiceHostFactory : WebServiceHostFactory
    {
        /// <summary>
        /// Creates an instance of the specified <see cref="T:System.ServiceModel.Web.WebServiceHost"/> derived class with the specified base addresses.
        /// </summary>
        /// <param name="serviceType">The type of service host to create.</param>
        /// <param name="baseAddresses">An array of base addresses for the service.</param>
        /// <returns>
        /// An instance of a <see cref="T:System.ServiceModel.ServiceHost"/> derived class.
        /// </returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var host = base.CreateServiceHost(serviceType, baseAddresses);

            host.Authorization.ServiceAuthorizationManager = new FederatedWebServiceAuthorizationManager();
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            return host;
        }
    }
}
