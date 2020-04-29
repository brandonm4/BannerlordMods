namespace TournamentXPanded.Configurator
{
    partial class Form1
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
            this.txtConfigFilePath = new System.Windows.Forms.TextBox();
            this.pnlSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.ddlLanguage = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtConfigFilePath
            // 
            this.txtConfigFilePath.Location = new System.Drawing.Point(174, 13);
            this.txtConfigFilePath.Name = "txtConfigFilePath";
            this.txtConfigFilePath.Size = new System.Drawing.Size(614, 20);
            this.txtConfigFilePath.TabIndex = 0;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSettings.AutoScroll = true;
            this.pnlSettings.Location = new System.Drawing.Point(12, 39);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(863, 776);
            this.pnlSettings.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(800, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // ddlLanguage
            // 
            this.ddlLanguage.FormattingEnabled = true;
            this.ddlLanguage.Location = new System.Drawing.Point(13, 13);
            this.ddlLanguage.Name = "ddlLanguage";
            this.ddlLanguage.Size = new System.Drawing.Size(155, 21);
            this.ddlLanguage.TabIndex = 3;
            this.ddlLanguage.SelectedIndexChanged += new System.EventHandler(this.DdlLanguage_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 827);
            this.Controls.Add(this.ddlLanguage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.txtConfigFilePath);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TournamentXPanded.Configurator";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConfigFilePath;
        private System.Windows.Forms.FlowLayoutPanel pnlSettings;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox ddlLanguage;
    }
}

