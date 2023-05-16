namespace ParsicAutoUpdater
{
    partial class Frm_AutoUpdate
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
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_Pause = new System.Windows.Forms.Button();
            this.Btn_MoreInfo = new System.Windows.Forms.Button();
            this.Lbl_NewVersion = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Lbl_AppVersion = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Lbl_Count = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Lbl_Size = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Txt_ScriptError = new System.Windows.Forms.TextBox();
            this.Pbar_Download = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.Location = new System.Drawing.Point(626, 43);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(62, 36);
            this.Btn_Cancel.TabIndex = 71;
            this.Btn_Cancel.Text = "Cancel";
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
            // 
            // Btn_Pause
            // 
            this.Btn_Pause.Location = new System.Drawing.Point(689, 43);
            this.Btn_Pause.Name = "Btn_Pause";
            this.Btn_Pause.Size = new System.Drawing.Size(62, 36);
            this.Btn_Pause.TabIndex = 70;
            this.Btn_Pause.Text = "Pause";
            this.Btn_Pause.UseVisualStyleBackColor = true;
            this.Btn_Pause.Click += new System.EventHandler(this.Btn_Pause_Click);
            // 
            // Btn_MoreInfo
            // 
            this.Btn_MoreInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_MoreInfo.Location = new System.Drawing.Point(751, 43);
            this.Btn_MoreInfo.Name = "Btn_MoreInfo";
            this.Btn_MoreInfo.Size = new System.Drawing.Size(62, 36);
            this.Btn_MoreInfo.TabIndex = 69;
            this.Btn_MoreInfo.Text = "More Info";
            this.Btn_MoreInfo.UseVisualStyleBackColor = true;
            this.Btn_MoreInfo.Click += new System.EventHandler(this.Btn_MoreInfo_Click);
            // 
            // Lbl_NewVersion
            // 
            this.Lbl_NewVersion.AutoSize = true;
            this.Lbl_NewVersion.Location = new System.Drawing.Point(456, 13);
            this.Lbl_NewVersion.Name = "Lbl_NewVersion";
            this.Lbl_NewVersion.Size = new System.Drawing.Size(37, 13);
            this.Lbl_NewVersion.TabIndex = 68;
            this.Lbl_NewVersion.Text = "          ";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(382, 13);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(76, 13);
            this.Label7.TabIndex = 67;
            this.Label7.Text = "New Version : ";
            // 
            // Lbl_AppVersion
            // 
            this.Lbl_AppVersion.AutoSize = true;
            this.Lbl_AppVersion.Location = new System.Drawing.Point(307, 13);
            this.Lbl_AppVersion.Name = "Lbl_AppVersion";
            this.Lbl_AppVersion.Size = new System.Drawing.Size(37, 13);
            this.Lbl_AppVersion.TabIndex = 66;
            this.Lbl_AppVersion.Text = "          ";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(236, 13);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(76, 13);
            this.Label5.TabIndex = 65;
            this.Label5.Text = "Your Version : ";
            // 
            // Lbl_Count
            // 
            this.Lbl_Count.AutoSize = true;
            this.Lbl_Count.Location = new System.Drawing.Point(178, 13);
            this.Lbl_Count.Name = "Lbl_Count";
            this.Lbl_Count.Size = new System.Drawing.Size(0, 13);
            this.Lbl_Count.TabIndex = 64;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(131, 13);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(44, 13);
            this.Label3.TabIndex = 63;
            this.Label3.Text = "Count : ";
            // 
            // Lbl_Size
            // 
            this.Lbl_Size.AutoSize = true;
            this.Lbl_Size.Location = new System.Drawing.Point(49, 13);
            this.Lbl_Size.Name = "Lbl_Size";
            this.Lbl_Size.Size = new System.Drawing.Size(16, 13);
            this.Lbl_Size.TabIndex = 62;
            this.Lbl_Size.Text = "   ";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(14, 13);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(39, 13);
            this.Label1.TabIndex = 61;
            this.Label1.Text = "Size :  ";
            // 
            // Txt_ScriptError
            // 
            this.Txt_ScriptError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ScriptError.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Txt_ScriptError.Location = new System.Drawing.Point(7, 103);
            this.Txt_ScriptError.Multiline = true;
            this.Txt_ScriptError.Name = "Txt_ScriptError";
            this.Txt_ScriptError.ReadOnly = true;
            this.Txt_ScriptError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_ScriptError.Size = new System.Drawing.Size(802, 249);
            this.Txt_ScriptError.TabIndex = 60;
            // 
            // Pbar_Download
            // 
            this.Pbar_Download.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Pbar_Download.Location = new System.Drawing.Point(5, 43);
            this.Pbar_Download.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Pbar_Download.Maximum = 50;
            this.Pbar_Download.Name = "Pbar_Download";
            this.Pbar_Download.Size = new System.Drawing.Size(616, 36);
            this.Pbar_Download.Step = 1;
            this.Pbar_Download.TabIndex = 59;
            this.Pbar_Download.Tag = "";
            // 
            // Frm_AutoUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 87);
            this.Controls.Add(this.Btn_Cancel);
            this.Controls.Add(this.Btn_Pause);
            this.Controls.Add(this.Btn_MoreInfo);
            this.Controls.Add(this.Lbl_NewVersion);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Lbl_AppVersion);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Lbl_Count);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Lbl_Size);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Txt_ScriptError);
            this.Controls.Add(this.Pbar_Download);
            this.Name = "Frm_AutoUpdate";
            this.Text = "آپدیت خودکار برنامه";
            this.Load += new System.EventHandler(this.Frm_AutoUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button Btn_Cancel;
        internal System.Windows.Forms.Button Btn_Pause;
        internal System.Windows.Forms.Button Btn_MoreInfo;
        internal System.Windows.Forms.Label Lbl_NewVersion;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label Lbl_AppVersion;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Lbl_Count;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Lbl_Size;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.TextBox Txt_ScriptError;
        private System.Windows.Forms.ProgressBar Pbar_Download;
    }
}