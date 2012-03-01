using System.Runtime.Serialization;

namespace Service
{
    [DataContract(Namespace = "http://identitymodel.thinktecture.com/samples")]
    public class ViewClaim
    {
        [DataMember(Order = 1)]
        public string ClaimType { get; set; }

        [DataMember(Order = 2)]
        public string Value { get; set; }

        [DataMember(Order = 3)]
        public string Issuer { get; set; }
    }
}
