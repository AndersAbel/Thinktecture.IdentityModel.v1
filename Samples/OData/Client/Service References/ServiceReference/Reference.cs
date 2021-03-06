//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Original file name:
// Generation date: 07.05.2010 17:04:49
namespace Client.ServiceReference
{
    
    /// <summary>
    /// There are no comments for ClaimsData in the schema.
    /// </summary>
    public partial class ClaimsData : global::System.Data.Services.Client.DataServiceContext
    {
        /// <summary>
        /// Initialize a new ClaimsData object.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public ClaimsData(global::System.Uri serviceRoot) : 
                base(serviceRoot)
        {
            this.ResolveName = new global::System.Func<global::System.Type, string>(this.ResolveNameFromType);
            this.ResolveType = new global::System.Func<string, global::System.Type>(this.ResolveTypeFromName);
            this.OnContextCreated();
        }
        partial void OnContextCreated();
        /// <summary>
        /// Since the namespace configured for this service reference
        /// in Visual Studio is different from the one indicated in the
        /// server schema, use type-mappers to map between the two.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        protected global::System.Type ResolveTypeFromName(string typeName)
        {
            if (typeName.StartsWith("Thinktecture.Samples", global::System.StringComparison.Ordinal))
            {
                return this.GetType().Assembly.GetType(string.Concat("Client.ServiceReference", typeName.Substring(20)), false);
            }
            return null;
        }
        /// <summary>
        /// Since the namespace configured for this service reference
        /// in Visual Studio is different from the one indicated in the
        /// server schema, use type-mappers to map between the two.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        protected string ResolveNameFromType(global::System.Type clientType)
        {
            if (clientType.Namespace.Equals("Client.ServiceReference", global::System.StringComparison.Ordinal))
            {
                return string.Concat("Thinktecture.Samples.", clientType.Name);
            }
            return null;
        }
        /// <summary>
        /// There are no comments for Claims in the schema.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public global::System.Data.Services.Client.DataServiceQuery<ViewClaim> Claims
        {
            get
            {
                if ((this._Claims == null))
                {
                    this._Claims = base.CreateQuery<ViewClaim>("Claims");
                }
                return this._Claims;
            }
        }
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        private global::System.Data.Services.Client.DataServiceQuery<ViewClaim> _Claims;
        /// <summary>
        /// There are no comments for Claims in the schema.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public void AddToClaims(ViewClaim viewClaim)
        {
            base.AddObject("Claims", viewClaim);
        }
    }
    /// <summary>
    /// There are no comments for Thinktecture.Samples.ViewClaim in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Id
    /// </KeyProperties>
    [global::System.Data.Services.Common.DataServiceKeyAttribute("Id")]
    public partial class ViewClaim
    {
        /// <summary>
        /// Create a new ViewClaim object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public static ViewClaim CreateViewClaim(int ID)
        {
            ViewClaim viewClaim = new ViewClaim();
            viewClaim.Id = ID;
            return viewClaim;
        }
        /// <summary>
        /// There are no comments for Property Id in the schema.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this.OnIdChanging(value);
                this._Id = value;
                this.OnIdChanged();
            }
        }
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        private int _Id;
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        /// <summary>
        /// There are no comments for Property ClaimType in the schema.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public string ClaimType
        {
            get
            {
                return this._ClaimType;
            }
            set
            {
                this.OnClaimTypeChanging(value);
                this._ClaimType = value;
                this.OnClaimTypeChanged();
            }
        }
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        private string _ClaimType;
        partial void OnClaimTypeChanging(string value);
        partial void OnClaimTypeChanged();
        /// <summary>
        /// There are no comments for Property Value in the schema.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this.OnValueChanging(value);
                this._Value = value;
                this.OnValueChanged();
            }
        }
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        private string _Value;
        partial void OnValueChanging(string value);
        partial void OnValueChanged();
        /// <summary>
        /// There are no comments for Property Issuer in the schema.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        public string Issuer
        {
            get
            {
                return this._Issuer;
            }
            set
            {
                this.OnIssuerChanging(value);
                this._Issuer = value;
                this.OnIssuerChanged();
            }
        }
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Services.Design", "1.0.0")]
        private string _Issuer;
        partial void OnIssuerChanging(string value);
        partial void OnIssuerChanged();
    }
}
