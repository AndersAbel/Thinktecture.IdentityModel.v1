﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ServiceModel;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Web;

namespace Thinktecture.IdentityModel
{
    /// <summary>
    /// Provides direct access to identity model related configuration
    /// </summary>
    public static class IdentityModelConfiguration
    {
        /// <summary>
        /// Gets the current WIF service configuration.
        /// </summary>
        /// <value>The service configuration.</value>
        public static ServiceConfiguration ServiceConfiguration
        {
            get
            {
                if (OperationContext.Current == null)
                {
                    // no WCF
                    return FederatedAuthentication.ServiceConfiguration;
                }

                // search message property
                if (OperationContext.Current.IncomingMessageProperties.ContainsKey("ServiceConfiguration"))
                {
                    var configuration = OperationContext.Current.IncomingMessageProperties["ServiceConfiguration"] as ServiceConfiguration;
                    if (configuration != null)
                    {
                        return configuration;
                    }
                }

                // return configuration from configuration file
                return new ServiceConfiguration();
            }
        }   
    }
}
