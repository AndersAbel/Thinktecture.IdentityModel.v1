using System.Data.Services.Common;

namespace Thinktecture.Samples
{
    [DataServiceKey("Id")]
    public class ViewClaim
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string Value { get; set; }
        public string Issuer { get; set; }
    }
}
