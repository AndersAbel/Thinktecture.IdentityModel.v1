using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;

namespace Thinktecture.IdentityModel.Utility
{
    internal partial class ClaimsViewerForm : Form
    {
        IClaimsPrincipal _principal;

        public ClaimsViewerForm(IClaimsPrincipal principal)
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            _principal = principal;
        }

        private void ClaimsViewerForm_Load(object sender, EventArgs e)
        {
            this.Show();
            Application.DoEvents();
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                LoadIdentities();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void LoadIdentities()
        {
            var ids = new List<string>();
            int count = 0;

            foreach (IClaimsIdentity id in _principal.Identities)
            {
                if (!string.IsNullOrEmpty(id.Name))
                {
                    ids.Add(string.Format("{0} ({1})",
                        id.Name,
                        id.Claims.Where(c => c.ClaimType == WSIdentityConstants.ClaimTypes.Name).First().Issuer));
                }
                else
                {
                    if (id.Claims.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(id.Claims[0].Issuer))
                        {
                            ids.Add(string.Format("Claims from {0}", id.Claims[0].Issuer));
                        }
                    }
                    else
                    {
                        ids.Add(string.Format("{0}", ++count));
                    }
                }
            }

            _ddlIdentities.DataSource = ids;
            ParseIdentity();
        }

        private void ParseIdentity()
        {
            var identity = _principal.Identities[_ddlIdentities.SelectedIndex];

            var claims = (from claim in identity.Claims
                          select new GridClaimsView
                          {
                              ClaimType = claim.ClaimType,
                              Value = claim.Value,
                              Issuer = claim.Issuer,
                              OriginalIssuer = claim.OriginalIssuer
                          }).ToList();
            
            this.claimsListView.Items.Clear();
            foreach (var claim in claims)
            {
                var item = new ListViewItem(claim.ClaimType, 0);
                item.SubItems.Add(claim.Value);
                item.SubItems.Add(claim.Issuer);
                item.SubItems.Add(claim.OriginalIssuer);
                this.claimsListView.Items.Add(item);
            }

            this.claimsListView.Columns[0].Width = -2;
            this.claimsListView.Columns[1].Width = -2;
        }

        private void _ddlIdentities_SelectedIndexChanged(object sender, EventArgs e)
        {
            Application.DoEvents();
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                ParseIdentity();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
