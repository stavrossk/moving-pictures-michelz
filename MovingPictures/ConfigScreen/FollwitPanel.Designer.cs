﻿namespace MediaPortal.Plugins.MovingPictures.ConfigScreen
{
    partial class FollwitPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.userLinkLabel = new System.Windows.Forms.LinkLabel();
            this.syncButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.accountButton = new System.Windows.Forms.Button();
            this.publicProfileCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.restrictMoviesButton = new System.Windows.Forms.Button();
            this.restrictSyncedMoviesCheckBox = new Cornerstone.GUI.Controls.SettingCheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logoPanel1 = new Follwit.API.UI.Panels.LogoPanel();
            this.retryLinkLabel = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.statusLabel.Location = new System.Drawing.Point(0, 0);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(71, 13);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Logged in as:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.retryLinkLabel);
            this.panel1.Controls.Add(this.userLinkLabel);
            this.panel1.Controls.Add(this.statusLabel);
            this.panel1.Location = new System.Drawing.Point(127, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(236, 18);
            this.panel1.TabIndex = 3;
            // 
            // userLinkLabel
            // 
            this.userLinkLabel.AutoSize = true;
            this.userLinkLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.userLinkLabel.Location = new System.Drawing.Point(71, 0);
            this.userLinkLabel.Margin = new System.Windows.Forms.Padding(0);
            this.userLinkLabel.Name = "userLinkLabel";
            this.userLinkLabel.Size = new System.Drawing.Size(34, 13);
            this.userLinkLabel.TabIndex = 1;
            this.userLinkLabel.TabStop = true;
            this.userLinkLabel.Text = "fforde";
            this.userLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.userLinkLabel_LinkClicked);
            // 
            // syncButton
            // 
            this.syncButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.syncButton.Location = new System.Drawing.Point(126, 0);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(108, 23);
            this.syncButton.TabIndex = 4;
            this.syncButton.Text = "Synchronize Now";
            this.syncButton.UseVisualStyleBackColor = true;
            this.syncButton.Click += new System.EventHandler(this.syncButton_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.syncButton);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Location = new System.Drawing.Point(127, 106);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(236, 23);
            this.panel2.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.accountButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.panel3.Size = new System.Drawing.Size(126, 23);
            this.panel3.TabIndex = 7;
            // 
            // accountButton
            // 
            this.accountButton.AutoSize = true;
            this.accountButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.accountButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accountButton.Location = new System.Drawing.Point(0, 0);
            this.accountButton.Name = "accountButton";
            this.accountButton.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.accountButton.Size = new System.Drawing.Size(120, 23);
            this.accountButton.TabIndex = 1;
            this.accountButton.Text = "Disconnect Account";
            this.accountButton.UseVisualStyleBackColor = true;
            this.accountButton.Click += new System.EventHandler(this.accountButton_Click);
            // 
            // publicProfileCheckBox
            // 
            this.publicProfileCheckBox.AutoSize = true;
            this.publicProfileCheckBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.publicProfileCheckBox.Location = new System.Drawing.Point(127, 146);
            this.publicProfileCheckBox.Name = "publicProfileCheckBox";
            this.publicProfileCheckBox.Size = new System.Drawing.Size(208, 17);
            this.publicProfileCheckBox.TabIndex = 18;
            this.publicProfileCheckBox.Text = "Allow others to view my profile online.";
            this.publicProfileCheckBox.UseVisualStyleBackColor = true;
            this.publicProfileCheckBox.CheckedChanged += new System.EventHandler(this.publicProfileCheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Privacy:";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Location = new System.Drawing.Point(6, 66);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(541, 3);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox4";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(123, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(424, 3);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox4";
            // 
            // restrictMoviesButton
            // 
            this.restrictMoviesButton.Enabled = false;
            this.restrictMoviesButton.Location = new System.Drawing.Point(127, 202);
            this.restrictMoviesButton.Name = "restrictMoviesButton";
            this.restrictMoviesButton.Size = new System.Drawing.Size(168, 23);
            this.restrictMoviesButton.TabIndex = 22;
            this.restrictMoviesButton.Text = "Define Synchronized Movies";
            this.restrictMoviesButton.UseVisualStyleBackColor = true;
            this.restrictMoviesButton.Click += new System.EventHandler(this.restrictMoviesButton_Click);
            // 
            // restrictSyncedMoviesCheckBox
            // 
            this.restrictSyncedMoviesCheckBox.AutoSize = true;
            this.restrictSyncedMoviesCheckBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.restrictSyncedMoviesCheckBox.IgnoreSettingName = true;
            this.restrictSyncedMoviesCheckBox.Location = new System.Drawing.Point(127, 178);
            this.restrictSyncedMoviesCheckBox.Name = "restrictSyncedMoviesCheckBox";
            this.restrictSyncedMoviesCheckBox.Setting = null;
            this.restrictSyncedMoviesCheckBox.Size = new System.Drawing.Size(266, 17);
            this.restrictSyncedMoviesCheckBox.TabIndex = 21;
            this.restrictSyncedMoviesCheckBox.Text = "Restrict which movies are synchronized to follw.it.";
            this.restrictSyncedMoviesCheckBox.UseVisualStyleBackColor = true;
            this.restrictSyncedMoviesCheckBox.CheckedChanged += new System.EventHandler(this.restrictSyncedMoviesCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Account:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(6, 135);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(541, 3);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox4";
            // 
            // logoPanel1
            // 
            this.logoPanel1.Location = new System.Drawing.Point(6, 3);
            this.logoPanel1.Name = "logoPanel1";
            this.logoPanel1.Size = new System.Drawing.Size(205, 57);
            this.logoPanel1.TabIndex = 23;
            this.logoPanel1.Click += new System.EventHandler(this.logoPanel1_Click);
            this.logoPanel1.MouseEnter += new System.EventHandler(this.logoPanel1_MouseEnter);
            this.logoPanel1.MouseLeave += new System.EventHandler(this.logoPanel1_MouseLeave);
            // 
            // retryLinkLabel
            // 
            this.retryLinkLabel.AutoSize = true;
            this.retryLinkLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.retryLinkLabel.Location = new System.Drawing.Point(105, 0);
            this.retryLinkLabel.Name = "retryLinkLabel";
            this.retryLinkLabel.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.retryLinkLabel.Size = new System.Drawing.Size(37, 13);
            this.retryLinkLabel.TabIndex = 3;
            this.retryLinkLabel.TabStop = true;
            this.retryLinkLabel.Text = "Retry";
            this.retryLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.retryLinkLabel_LinkClicked);
            // 
            // FollwitPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logoPanel1);
            this.Controls.Add(this.restrictMoviesButton);
            this.Controls.Add(this.restrictSyncedMoviesCheckBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.publicProfileCheckBox);
            this.Controls.Add(this.label2);
            this.MinimumSize = new System.Drawing.Size(555, 87);
            this.Name = "FollwitPanel";
            this.Size = new System.Drawing.Size(555, 235);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel userLinkLabel;
        private System.Windows.Forms.Button syncButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button accountButton;
        private System.Windows.Forms.CheckBox publicProfileCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private Cornerstone.GUI.Controls.SettingCheckBox restrictSyncedMoviesCheckBox;
        private System.Windows.Forms.Button restrictMoviesButton;
        private Follwit.API.UI.Panels.LogoPanel logoPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel retryLinkLabel;
    }
}
