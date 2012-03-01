/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;

namespace Thinktecture.IdentityModel.SecurityTokenService
{
    /// <summary>
    /// Service contract definition for a simple security token service
    /// </summary>
    [ServiceContract(Name = "SimpleTokenServiceContract", Namespace = "http://www.thinktecture.com/services/SimpleTokenService")]
    public interface ISimpleRestfulTokenServiceContract
    {
        /// <summary>
        /// Issues a token for the specified realm.
        /// </summary>
        /// <param name="realm">The realm name.</param>
        /// <returns>A SecurityToken as XElement</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/?realm={realm}")]
        XElement Issue(string realm);
    }
}