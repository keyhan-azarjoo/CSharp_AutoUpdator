namespace ParsicAutoUpdater
{
    partial class Frm_VersionList2
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
            try
            {
                this.Close();
            }
            catch
            {
            }
            
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_VersionList2));
            this.Dg_VersionList = new System.Windows.Forms.DataGridView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Chk_Centeral = new System.Windows.Forms.CheckBox();
            this.Chk_QC = new System.Windows.Forms.CheckBox();
            this.Chk_Storage = new System.Windows.Forms.CheckBox();
            this.Chk_Journal = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Lbl_CenteralVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Lbl_DbVersion = new System.Windows.Forms.Label();
            this.Lbl_ScriptVersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Chk_Temperature = new System.Windows.Forms.CheckBox();
            this.Chk_Exe = new System.Windows.Forms.CheckBox();
            this.Chk_Scripts = new System.Windows.Forms.CheckBox();
            this.Chk_ParsicLabAndroid = new System.Windows.Forms.CheckBox();
            this.Btn_RollBack = new System.Windows.Forms.Button();
            this.Btn_Exit = new System.Windows.Forms.Button();
            this.Btn_SaveEventLog = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.Btn_Retry = new System.Windows.Forms.Button();
            this.Chk_QMatic = new System.Windows.Forms.CheckBox();
            this.Chk_PrinterCacher = new System.Windows.Forms.CheckBox();
            this.Chk_Web = new System.Windows.Forms.CheckBox();
            this.Txt_ScriptError = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Dg_VersionList)).BeginInit();
            this.SuspendLayout();
            // 
            // Dg_VersionList
            // 
            this.Dg_VersionList.AllowUserToAddRows = false;
            this.Dg_VersionList.AllowUserToDeleteRows = false;
            this.Dg_VersionList.AllowUserToResizeColumns = false;
            this.Dg_VersionList.AllowUserToResizeRows = false;
            this.Dg_VersionList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Dg_VersionList.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.Dg_VersionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dg_VersionList.Enabled = false;
            this.Dg_VersionList.Location = new System.Drawing.Point(4, 62);
            this.Dg_VersionList.Name = "Dg_VersionList";
            this.Dg_VersionList.Size = new System.Drawing.Size(1074, 18);
            this.Dg_VersionList.TabIndex = 0;
            this.Dg_VersionList.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(4, 56);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar1.Maximum = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1074, 36);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 23;
            this.progressBar1.Tag = "";
            // 
            // Chk_Centeral
            // 
            this.Chk_Centeral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_Centeral.AutoSize = true;
            this.Chk_Centeral.Enabled = false;
            this.Chk_Centeral.Location = new System.Drawing.Point(842, 9);
            this.Chk_Centeral.Name = "Chk_Centeral";
            this.Chk_Centeral.Size = new System.Drawing.Size(67, 17);
            this.Chk_Centeral.TabIndex = 24;
            this.Chk_Centeral.Text = "Centeral";
            this.Chk_Centeral.UseVisualStyleBackColor = true;
            // 
            // Chk_QC
            // 
            this.Chk_QC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_QC.AutoSize = true;
            this.Chk_QC.Enabled = false;
            this.Chk_QC.Location = new System.Drawing.Point(842, 32);
            this.Chk_QC.Name = "Chk_QC";
            this.Chk_QC.Size = new System.Drawing.Size(41, 17);
            this.Chk_QC.TabIndex = 25;
            this.Chk_QC.Text = "QC";
            this.Chk_QC.UseVisualStyleBackColor = true;
            // 
            // Chk_Storage
            // 
            this.Chk_Storage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_Storage.AutoSize = true;
            this.Chk_Storage.Enabled = false;
            this.Chk_Storage.Location = new System.Drawing.Point(774, 9);
            this.Chk_Storage.Name = "Chk_Storage";
            this.Chk_Storage.Size = new System.Drawing.Size(64, 17);
            this.Chk_Storage.TabIndex = 26;
            this.Chk_Storage.Text = "Storage";
            this.Chk_Storage.UseVisualStyleBackColor = true;
            // 
            // Chk_Journal
            // 
            this.Chk_Journal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_Journal.AutoSize = true;
            this.Chk_Journal.Enabled = false;
            this.Chk_Journal.Location = new System.Drawing.Point(774, 32);
            this.Chk_Journal.Name = "Chk_Journal";
            this.Chk_Journal.Size = new System.Drawing.Size(61, 17);
            this.Chk_Journal.TabIndex = 27;
            this.Chk_Journal.Text = "Journal";
            this.Chk_Journal.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "EXE Version :";
            // 
            // Lbl_CenteralVersion
            // 
            this.Lbl_CenteralVersion.AutoSize = true;
            this.Lbl_CenteralVersion.Location = new System.Drawing.Point(105, 9);
            this.Lbl_CenteralVersion.Name = "Lbl_CenteralVersion";
            this.Lbl_CenteralVersion.Size = new System.Drawing.Size(22, 13);
            this.Lbl_CenteralVersion.TabIndex = 29;
            this.Lbl_CenteralVersion.Text = "     ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "DB Exe Version :";
            // 
            // Lbl_DbVersion
            // 
            this.Lbl_DbVersion.AutoSize = true;
            this.Lbl_DbVersion.Location = new System.Drawing.Point(105, 33);
            this.Lbl_DbVersion.Name = "Lbl_DbVersion";
            this.Lbl_DbVersion.Size = new System.Drawing.Size(22, 13);
            this.Lbl_DbVersion.TabIndex = 29;
            this.Lbl_DbVersion.Text = "     ";
            // 
            // Lbl_ScriptVersion
            // 
            this.Lbl_ScriptVersion.AutoSize = true;
            this.Lbl_ScriptVersion.Location = new System.Drawing.Point(267, 9);
            this.Lbl_ScriptVersion.Name = "Lbl_ScriptVersion";
            this.Lbl_ScriptVersion.Size = new System.Drawing.Size(22, 13);
            this.Lbl_ScriptVersion.TabIndex = 1;
            this.Lbl_ScriptVersion.Text = "     ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Script Version :";
            // 
            // Chk_Temperature
            // 
            this.Chk_Temperature.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_Temperature.AutoSize = true;
            this.Chk_Temperature.Enabled = false;
            this.Chk_Temperature.Location = new System.Drawing.Point(656, 9);
            this.Chk_Temperature.Name = "Chk_Temperature";
            this.Chk_Temperature.Size = new System.Drawing.Size(88, 17);
            this.Chk_Temperature.TabIndex = 44;
            this.Chk_Temperature.Text = "Temperature";
            this.Chk_Temperature.UseVisualStyleBackColor = true;
            // 
            // Chk_Exe
            // 
            this.Chk_Exe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_Exe.AutoSize = true;
            this.Chk_Exe.Enabled = false;
            this.Chk_Exe.Location = new System.Drawing.Point(913, 9);
            this.Chk_Exe.Name = "Chk_Exe";
            this.Chk_Exe.Size = new System.Drawing.Size(44, 17);
            this.Chk_Exe.TabIndex = 47;
            this.Chk_Exe.Text = "EXE";
            this.Chk_Exe.UseVisualStyleBackColor = true;
            // 
            // Chk_Scripts
            // 
            this.Chk_Scripts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_Scripts.AutoSize = true;
            this.Chk_Scripts.Enabled = false;
            this.Chk_Scripts.Location = new System.Drawing.Point(913, 32);
            this.Chk_Scripts.Name = "Chk_Scripts";
            this.Chk_Scripts.Size = new System.Drawing.Size(58, 17);
            this.Chk_Scripts.TabIndex = 48;
            this.Chk_Scripts.Text = "Scripts";
            this.Chk_Scripts.UseVisualStyleBackColor = true;
            // 
            // Chk_ParsicLabAndroid
            // 
            this.Chk_ParsicLabAndroid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_ParsicLabAndroid.AutoSize = true;
            this.Chk_ParsicLabAndroid.Enabled = false;
            this.Chk_ParsicLabAndroid.Location = new System.Drawing.Point(656, 32);
            this.Chk_ParsicLabAndroid.Name = "Chk_ParsicLabAndroid";
            this.Chk_ParsicLabAndroid.Size = new System.Drawing.Size(114, 17);
            this.Chk_ParsicLabAndroid.TabIndex = 49;
            this.Chk_ParsicLabAndroid.Text = "Parsic Lab Android";
            this.Chk_ParsicLabAndroid.UseVisualStyleBackColor = true;
            // 
            // Btn_RollBack
            // 
            this.Btn_RollBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_RollBack.Location = new System.Drawing.Point(977, 3);
            this.Btn_RollBack.Name = "Btn_RollBack";
            this.Btn_RollBack.Size = new System.Drawing.Size(92, 46);
            this.Btn_RollBack.TabIndex = 50;
            this.Btn_RollBack.Text = "Roll Back";
            this.Btn_RollBack.UseVisualStyleBackColor = true;
            this.Btn_RollBack.Click += new System.EventHandler(this.Btn_SQLRollBack_Click);
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Exit.Location = new System.Drawing.Point(1022, 3);
            this.Btn_Exit.Name = "Btn_Exit";
            this.Btn_Exit.Size = new System.Drawing.Size(46, 46);
            this.Btn_Exit.TabIndex = 51;
            this.Btn_Exit.Text = "EXIT";
            this.Btn_Exit.UseVisualStyleBackColor = true;
            this.Btn_Exit.Visible = false;
            this.Btn_Exit.Click += new System.EventHandler(this.Btn_Exit_Click);
            // 
            // Btn_SaveEventLog
            // 
            this.Btn_SaveEventLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_SaveEventLog.Location = new System.Drawing.Point(977, 3);
            this.Btn_SaveEventLog.Name = "Btn_SaveEventLog";
            this.Btn_SaveEventLog.Size = new System.Drawing.Size(46, 46);
            this.Btn_SaveEventLog.TabIndex = 52;
            this.Btn_SaveEventLog.Text = "Save Log";
            this.Btn_SaveEventLog.UseVisualStyleBackColor = true;
            this.Btn_SaveEventLog.Visible = false;
            this.Btn_SaveEventLog.Click += new System.EventHandler(this.Btn_SaveEventLog_Click);
            // 
            // Btn_Retry
            // 
            this.Btn_Retry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Retry.Location = new System.Drawing.Point(406, 5);
            this.Btn_Retry.Name = "Btn_Retry";
            this.Btn_Retry.Size = new System.Drawing.Size(127, 46);
            this.Btn_Retry.TabIndex = 53;
            this.Btn_Retry.Text = "Retry";
            this.Btn_Retry.UseVisualStyleBackColor = true;
            this.Btn_Retry.Visible = false;
            this.Btn_Retry.Click += new System.EventHandler(this.Btn_Retry_Click);
            // 
            // Chk_QMatic
            // 
            this.Chk_QMatic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_QMatic.AutoSize = true;
            this.Chk_QMatic.Enabled = false;
            this.Chk_QMatic.Location = new System.Drawing.Point(495, 62);
            this.Chk_QMatic.Name = "Chk_QMatic";
            this.Chk_QMatic.Size = new System.Drawing.Size(59, 17);
            this.Chk_QMatic.TabIndex = 54;
            this.Chk_QMatic.Text = "QMatic";
            this.Chk_QMatic.UseVisualStyleBackColor = true;
            this.Chk_QMatic.Visible = false;
            // 
            // Chk_PrinterCacher
            // 
            this.Chk_PrinterCacher.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_PrinterCacher.AutoSize = true;
            this.Chk_PrinterCacher.Enabled = false;
            this.Chk_PrinterCacher.Location = new System.Drawing.Point(555, 9);
            this.Chk_PrinterCacher.Name = "Chk_PrinterCacher";
            this.Chk_PrinterCacher.Size = new System.Drawing.Size(95, 17);
            this.Chk_PrinterCacher.TabIndex = 55;
            this.Chk_PrinterCacher.Text = "Printer Cacher";
            this.Chk_PrinterCacher.UseVisualStyleBackColor = true;
            // 
            // Chk_Web
            // 
            this.Chk_Web.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Chk_Web.AutoSize = true;
            this.Chk_Web.Enabled = false;
            this.Chk_Web.Location = new System.Drawing.Point(555, 32);
            this.Chk_Web.Name = "Chk_Web";
            this.Chk_Web.Size = new System.Drawing.Size(48, 17);
            this.Chk_Web.TabIndex = 54;
            this.Chk_Web.Text = "Web";
            this.Chk_Web.UseVisualStyleBackColor = true;
            // 
            // Txt_ScriptError
            // 
            this.Txt_ScriptError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ScriptError.BackColor = System.Drawing.SystemColors.Control;
            this.Txt_ScriptError.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_ScriptError.Location = new System.Drawing.Point(4, 99);
            this.Txt_ScriptError.Name = "Txt_ScriptError";
            this.Txt_ScriptError.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.Txt_ScriptError.Size = new System.Drawing.Size(1074, 323);
            this.Txt_ScriptError.TabIndex = 56;
            this.Txt_ScriptError.Text = "";
            this.Txt_ScriptError.TextChanged += new System.EventHandler(this.Txt_ScriptError_TextChanged_1);
            // 
            // Frm_VersionList2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Azure;
            this.ClientSize = new System.Drawing.Size(1081, 425);
            this.Controls.Add(this.Txt_ScriptError);
            this.Controls.Add(this.Chk_Web);
            this.Controls.Add(this.Chk_PrinterCacher);
            this.Controls.Add(this.Btn_Retry);
            this.Controls.Add(this.Btn_SaveEventLog);
            this.Controls.Add(this.Btn_Exit);
            this.Controls.Add(this.Btn_RollBack);
            this.Controls.Add(this.Chk_ParsicLabAndroid);
            this.Controls.Add(this.Chk_Scripts);
            this.Controls.Add(this.Chk_Exe);
            this.Controls.Add(this.Chk_Temperature);
            this.Controls.Add(this.Lbl_ScriptVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Lbl_DbVersion);
            this.Controls.Add(this.Lbl_CenteralVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Chk_Journal);
            this.Controls.Add(this.Chk_Storage);
            this.Controls.Add(this.Chk_QC);
            this.Controls.Add(this.Chk_Centeral);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Dg_VersionList);
            this.Controls.Add(this.Chk_QMatic);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(871, 341);
            this.Name = "Frm_VersionList2";
            this.Text = " ";
            this.Load += new System.EventHandler(this.Frm_VersionList2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Dg_VersionList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Dg_VersionList;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox Chk_Centeral;
        private System.Windows.Forms.CheckBox Chk_QC;
        private System.Windows.Forms.CheckBox Chk_Storage;
        private System.Windows.Forms.CheckBox Chk_Journal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Lbl_CenteralVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Lbl_DbVersion;
        private System.Windows.Forms.Label Lbl_ScriptVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox Chk_Temperature;
        private System.Windows.Forms.CheckBox Chk_Exe;
        private System.Windows.Forms.CheckBox Chk_Scripts;
        private System.Windows.Forms.CheckBox Chk_ParsicLabAndroid;
        private System.Windows.Forms.Button Btn_RollBack;
        private System.Windows.Forms.Button Btn_Exit;
        private System.Windows.Forms.Button Btn_SaveEventLog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button Btn_Retry;
        private System.Windows.Forms.CheckBox Chk_QMatic;
        private System.Windows.Forms.CheckBox Chk_PrinterCacher;
        private System.Windows.Forms.CheckBox Chk_Web;
        private System.Windows.Forms.RichTextBox Txt_ScriptError;
    }
}