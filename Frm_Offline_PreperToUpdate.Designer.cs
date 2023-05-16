namespace ParsicAutoUpdater
{
    partial class Frm_Offline_PreperToUpdate
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
            this.Txt_ExePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Btn_GetExePath = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Txt_ScriptPath = new System.Windows.Forms.TextBox();
            this.Btn_GetScriptPath = new System.Windows.Forms.Button();
            this.Btn_Run = new System.Windows.Forms.Button();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Txt_ScriptError = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Btn_RollBack = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Txt_DescriptionPath = new System.Windows.Forms.TextBox();
            this.Btn_GetDescriptionPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Txt_ExePath
            // 
            this.Txt_ExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ExePath.Location = new System.Drawing.Point(114, 10);
            this.Txt_ExePath.Name = "Txt_ExePath";
            this.Txt_ExePath.Size = new System.Drawing.Size(600, 20);
            this.Txt_ExePath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Exe Path :";
            // 
            // Btn_GetExePath
            // 
            this.Btn_GetExePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_GetExePath.Location = new System.Drawing.Point(720, 9);
            this.Btn_GetExePath.Name = "Btn_GetExePath";
            this.Btn_GetExePath.Size = new System.Drawing.Size(27, 20);
            this.Btn_GetExePath.TabIndex = 2;
            this.Btn_GetExePath.Text = "...";
            this.Btn_GetExePath.UseVisualStyleBackColor = true;
            this.Btn_GetExePath.Click += new System.EventHandler(this.Btn_GetExePath_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Scripts Path : ";
            // 
            // Txt_ScriptPath
            // 
            this.Txt_ScriptPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ScriptPath.Location = new System.Drawing.Point(114, 62);
            this.Txt_ScriptPath.Name = "Txt_ScriptPath";
            this.Txt_ScriptPath.Size = new System.Drawing.Size(600, 20);
            this.Txt_ScriptPath.TabIndex = 4;
            // 
            // Btn_GetScriptPath
            // 
            this.Btn_GetScriptPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_GetScriptPath.Location = new System.Drawing.Point(720, 61);
            this.Btn_GetScriptPath.Name = "Btn_GetScriptPath";
            this.Btn_GetScriptPath.Size = new System.Drawing.Size(27, 20);
            this.Btn_GetScriptPath.TabIndex = 5;
            this.Btn_GetScriptPath.Text = "...";
            this.Btn_GetScriptPath.UseVisualStyleBackColor = true;
            this.Btn_GetScriptPath.Click += new System.EventHandler(this.Btn_GetScriptExePath_Click);
            // 
            // Btn_Run
            // 
            this.Btn_Run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Run.Location = new System.Drawing.Point(826, 4);
            this.Btn_Run.Name = "Btn_Run";
            this.Btn_Run.Size = new System.Drawing.Size(146, 39);
            this.Btn_Run.TabIndex = 6;
            this.Btn_Run.Text = "Run";
            this.Btn_Run.UseVisualStyleBackColor = true;
            this.Btn_Run.Click += new System.EventHandler(this.Btn_Run_Click);
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_Cancel.Location = new System.Drawing.Point(826, 48);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(146, 39);
            this.Btn_Cancel.TabIndex = 7;
            this.Btn_Cancel.Text = "Cancel";
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Txt_ScriptError
            // 
            this.Txt_ScriptError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_ScriptError.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Txt_ScriptError.Location = new System.Drawing.Point(12, 133);
            this.Txt_ScriptError.Multiline = true;
            this.Txt_ScriptError.Name = "Txt_ScriptError";
            this.Txt_ScriptError.ReadOnly = true;
            this.Txt_ScriptError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Txt_ScriptError.Size = new System.Drawing.Size(964, 369);
            this.Txt_ScriptError.TabIndex = 47;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 91);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar1.Maximum = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(964, 36);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 46;
            this.progressBar1.Tag = "";
            // 
            // Btn_RollBack
            // 
            this.Btn_RollBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_RollBack.Location = new System.Drawing.Point(826, 4);
            this.Btn_RollBack.Name = "Btn_RollBack";
            this.Btn_RollBack.Size = new System.Drawing.Size(146, 39);
            this.Btn_RollBack.TabIndex = 48;
            this.Btn_RollBack.Text = "Roll Back";
            this.Btn_RollBack.UseVisualStyleBackColor = true;
            this.Btn_RollBack.Visible = false;
            this.Btn_RollBack.Click += new System.EventHandler(this.Btn_RollBack_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Descriptions Path : ";
            // 
            // Txt_DescriptionPath
            // 
            this.Txt_DescriptionPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_DescriptionPath.Location = new System.Drawing.Point(114, 36);
            this.Txt_DescriptionPath.Name = "Txt_DescriptionPath";
            this.Txt_DescriptionPath.Size = new System.Drawing.Size(600, 20);
            this.Txt_DescriptionPath.TabIndex = 4;
            // 
            // Btn_GetDescriptionPath
            // 
            this.Btn_GetDescriptionPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_GetDescriptionPath.Location = new System.Drawing.Point(720, 36);
            this.Btn_GetDescriptionPath.Name = "Btn_GetDescriptionPath";
            this.Btn_GetDescriptionPath.Size = new System.Drawing.Size(27, 20);
            this.Btn_GetDescriptionPath.TabIndex = 5;
            this.Btn_GetDescriptionPath.Text = "...";
            this.Btn_GetDescriptionPath.UseVisualStyleBackColor = true;
            this.Btn_GetDescriptionPath.Click += new System.EventHandler(this.Btn_GetDescriptionPath_Click);
            // 
            // Frm_Offline_PreperToUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 510);
            this.Controls.Add(this.Btn_RollBack);
            this.Controls.Add(this.Txt_ScriptError);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Btn_Cancel);
            this.Controls.Add(this.Btn_Run);
            this.Controls.Add(this.Btn_GetDescriptionPath);
            this.Controls.Add(this.Btn_GetScriptPath);
            this.Controls.Add(this.Txt_DescriptionPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Txt_ScriptPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Btn_GetExePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Txt_ExePath);
            this.Name = "Frm_Offline_PreperToUpdate";
            this.Text = "آفلاین";
            this.Load += new System.EventHandler(this.Frm_Offline_PreperToUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Txt_ExePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Btn_GetExePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Txt_ScriptPath;
        private System.Windows.Forms.Button Btn_GetScriptPath;
        private System.Windows.Forms.Button Btn_Run;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox Txt_ScriptError;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button Btn_RollBack;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Txt_DescriptionPath;
        private System.Windows.Forms.Button Btn_GetDescriptionPath;
    }
}