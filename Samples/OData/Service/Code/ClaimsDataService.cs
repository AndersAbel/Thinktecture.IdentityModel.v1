using System.Data.Services;

namespace Thinktecture.Samples
{
    public class ClaimsDataService : DataService<ClaimsData>
    {
        public static void InitializeService(IDataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
        }
    }
}