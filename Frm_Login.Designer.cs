namespace ParsicAutoUpdater
{
    partial class Frm_Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Login));
            this.CB_Username = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.chbShowPassword = new System.Windows.Forms.CheckBox();
            this.Txt_Password = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.Chk_Offline = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CB_Username
            // 
            this.CB_Username.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Username.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Username.BackColor = System.Drawing.Color.White;
            this.CB_Username.FormattingEnabled = true;
            this.CB_Username.Location = new System.Drawing.Point(42, 35);
            this.CB_Username.Name = "CB_Username";
            this.CB_Username.Size = new System.Drawing.Size(189, 21);
            this.CB_Username.TabIndex = 9;
            this.CB_Username.SelectedIndexChanged += new System.EventHandler(this.CB_Username_SelectedIndexChanged);
            this.CB_Username.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CB_Username_KeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(40, 113);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 32);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "خروج";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(147, 113);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(101, 32);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "ورود";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(237, 65);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblPassword.Size = new System.Drawing.Size(59, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "کلمه عبور :";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(237, 38);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblUserName.Size = new System.Drawing.Size(60, 13);
            this.lblUserName.TabIndex = 7;
            this.lblUserName.Text = "نام کاربری :";
            // 
            // chbShowPassword
            // 
            this.chbShowPassword.AutoSize = true;
            this.chbShowPassword.Location = new System.Drawing.Point(156, 89);
            this.chbShowPassword.Name = "chbShowPassword";
            this.chbShowPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chbShowPassword.Size = new System.Drawing.Size(104, 17);
            this.chbShowPassword.TabIndex = 11;
            this.chbShowPassword.Text = "نمایش کلمه عبور";
            this.chbShowPassword.UseVisualStyleBackColor = true;
            this.chbShowPassword.CheckedChanged += new System.EventHandler(this.chbShowPassword_CheckedChanged);
            // 
            // Txt_Password
            // 
            this.Txt_Password.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.Txt_Password.BackColor = System.Drawing.Color.White;
            this.Txt_Password.Location = new System.Drawing.Point(42, 62);
            this.Txt_Password.Name = "Txt_Password";
            this.Txt_Password.PasswordChar = '*';
            this.Txt_Password.Size = new System.Drawing.Size(189, 21);
            this.Txt_Password.TabIndex = 10;
            this.Txt_Password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(107, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(211, 13);
            this.lblTitle.TabIndex = 8;
            this.lblTitle.Text = "لطفا نام کاربری و کلمه عبور خود را وارد نمایید";
            // 
            // Chk_Offline
            // 
            this.Chk_Offline.AutoSize = true;
            this.Chk_Offline.Location = new System.Drawing.Point(33, 89);
            this.Chk_Offline.Name = "Chk_Offline";
            this.Chk_Offline.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Chk_Offline.Size = new System.Drawing.Size(117, 17);
            this.Chk_Offline.TabIndex = 12;
            this.Chk_Offline.Text = "ورود به صورت آفلاین";
            this.Chk_Offline.UseVisualStyleBackColor = true;
            // 
            // Frm_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.ClientSize = new System.Drawing.Size(362, 154);
            this.Controls.Add(this.Chk_Offline);
            this.Controls.Add(this.CB_Username);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.chbShowPassword);
            this.Controls.Add(this.Txt_Password);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(378, 193);
            this.MinimumSize = new System.Drawing.Size(378, 193);
            this.Name = "Frm_Login";
            this.Text = "آپدیتور اتوماتیک  11.47.1  1400/08/09";
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CB_Username;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.CheckBox chbShowPassword;
        private System.Windows.Forms.TextBox Txt_Password;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.CheckBox Chk_Offline;
    }
}