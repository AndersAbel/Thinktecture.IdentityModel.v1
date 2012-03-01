using System.ServiceModel;

namespace Thinktecture.Samples
{
    [ServiceContract(Name = "ClaimsContract", Namespace = "urn:tt", ConfigurationName = "ClaimsContract")]
    interface IClaimsService
    {
        [OperationContract]
        void ShowClaims();

        [OperationContract]
        void DoSecret();

        [OperationContract]
        void DoTopSecret();

    }
}
