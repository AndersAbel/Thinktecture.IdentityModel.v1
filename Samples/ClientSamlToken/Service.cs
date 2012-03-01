using System.ServiceModel;
using System.Threading;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.Utility;

namespace Thinktecture.Samples
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Ping(string data);
    }

    public interface IServiceClientChannel : IService, IClientChannel
    { }

    class Service : IService
    {
        public string Ping(string data)
        {
            var principal = Thread.CurrentPrincipal.AsClaimsPrincipal();
            ClaimsViewer.ShowConsole(principal);

            return "success";
        }
    }
}
