/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

namespace Thinktecture.IdentityModel.Utility
{
    /// <summary>
    /// Window forms to show claims
    /// </summary>
    partial class ClaimsViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimsViewerForm));
            this.label1 = new System.Windows.Forms.Label();
            this._ddlIdentities = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.claimsListView = new System.Windows.Forms.ListView();
            this.claimTypeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.valueColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.issuerColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.originalIssuerColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.claimsImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "I&dentities:";
            // 
            // _ddlIdentities
            // 
            this._ddlIdentities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._ddlIdentities.FormattingEnabled = true;
            this._ddlIdentities.Location = new System.Drawing.Point(12, 26);
            this._ddlIdentities.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._ddlIdentities.Name = "_ddlIdentities";
            this._ddlIdentities.Size = new System.Drawing.Size(297, 21);
            this._ddlIdentities.TabIndex = 1;
            this._ddlIdentities.SelectedIndexChanged += new System.EventHandler(this._ddlIdentities_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Claims:";
            // 
            // claimsListView
            // 
            this.claimsListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.claimsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.claimsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.claimTypeColumnHeader,
            this.valueColumnHeader,
            this.issuerColumnHeader,
            this.originalIssuerColumnHeader});
            this.claimsListView.FullRowSelect = true;
            this.claimsListView.GridLines = true;
            this.claimsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.claimsListView.Location = new System.Drawing.Point(12, 74);
            this.claimsListView.Name = "claimsListView";
            this.claimsListView.Size = new System.Drawing.Size(560, 231);
            this.claimsListView.SmallImageList = this.claimsImageList;
            this.claimsListView.TabIndex = 4;
            this.claimsListView.UseCompatibleStateImageBehavior = false;
            this.claimsListView.View = System.Windows.Forms.View.Details;
            // 
            // claimTypeColumnHeader
            // 
            this.claimTypeColumnHeader.Text = "Type";
            this.claimTypeColumnHeader.Width = 206;
            // 
            // valueColumnHeader
            // 
            this.valueColumnHeader.Text = "Value";
            this.valueColumnHeader.Width = 110;
            // 
            // issuerColumnHeader
            // 
            this.issuerColumnHeader.Text = "Issuer ";
            this.issuerColumnHeader.Width = 120;
            // 
            // originalIssuerColumnHeader
            // 
            this.originalIssuerColumnHeader.Text = "Original Issuer ";
            this.originalIssuerColumnHeader.Width = 120;
            // 
            // claimsImageList
            // 
            this.claimsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("claimsImageList.ImageStream")));
            this.claimsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.claimsImageList.Images.SetKeyName(0, "Column.ico");
            // 
            // ClaimsViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 316);
            this.Controls.Add(this.claimsListView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._ddlIdentities);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClaimsViewerForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thinktecture.IdentityModel Claims Viewer";
            this.Load += new System.EventHandler(this.ClaimsViewerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _ddlIdentities;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView claimsListView;
        private System.Windows.Forms.ImageList claimsImageList;
        private System.Windows.Forms.ColumnHeader claimTypeColumnHeader;
        private System.Windows.Forms.ColumnHeader valueColumnHeader;
        private System.Windows.Forms.ColumnHeader issuerColumnHeader;
        private System.Windows.Forms.ColumnHeader originalIssuerColumnHeader;

    }
}