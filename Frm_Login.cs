using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdaterClasses;

namespace ParsicAutoUpdater
{
    public partial class Frm_Login : Form
    {
        #region Variables

        public DialogResult Result = new DialogResult();
        public string UserName = string.Empty;
        public int Parsic_user_id = 0;
        public string ComputerName = string.Empty;
        GetAndInsertVersionInDB UpdaterFunctions;
        Parsic.Business.Security.Cls_Encryption MySecurity = new Parsic.Business.Security.Cls_Encryption();
        #endregion
        //get users
        List<Parsicwebservice.Cls_ParsicUser> users = new List<Parsicwebservice.Cls_ParsicUser>();
        //get users\
        public Frm_Login()
        {
            UpdaterFunctions = new GetAndInsertVersionInDB(100000);
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            ChangeRegionalLanguage();
            try
            {

                #region GetUsers
                Parsicwebservice.Service1SoapClient Myservice = new Parsicwebservice.Service1SoapClient();
                #endregion


                try
                {
                    UpdaterFunctions = new GetAndInsertVersionInDB(0);
                    var LastVersion = new DataTable();
                    LastVersion = Myservice.Get_LastVersionOfAutoUpdater(Get_UserName(), Get_Password());

                    string Name = LastVersion.Rows[0]["Str_FileName"].ToString();
                    string DBVerS = Name.Split('/')[1];
                    string AppVerS = System.Windows.Forms.Application.ProductVersion.ToString().Substring(0, 5);
                    double DBVerD = Convert.ToDouble(DBVerS);
                    double AppVerD = Convert.ToDouble(AppVerS);
                    try
                    {
                        if (DBVerD > AppVerD)
                        {
                            if (MessageBox.Show("ورژن جدید ( " + DBVerS + " ) برای نرم افزار وجود دارد" + "\r\n" + " آیا میخواهید ورژن جدید را دانلود و بروز رسانی کنید؟", "بروز رسانی", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                if (Convert.ToInt16( LastVersion.Rows[0]["Count"]) == 0)
                                {
                                    MessageBox.Show(" ورژن جدید وجود دارد ولی اطلاعات ابری کامل نمیباشد یا خطایی در ارتباط رخ داده است" + "\r\n" + " ورژن برنامه " + AppVerS + "\r\n" + " ورژن جدید " + DBVerS, "خطا");
                                }
                                else
                                {
                                    try
                                    {
                                        this.WindowState = FormWindowState.Minimized;
                                        var frm = new Frm_AutoUpdate(LastVersion, AppVerS, DBVerS);
                                        frm.Show();
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show("در ارتباط با سرور ابری مشکلی بوجود آمده است")
                }

                users.AddRange(Myservice.GetParsicUser(Get_UserName(), Get_Password()));

                for (int i = 0; i < users.Count(); i++)
                {
                    CB_Username.Items.Add(users[i].Str_UserName);
                }
            }
            catch (Exception EX)
            {
                
                MessageBox.Show("در دریافت اطلاعات از بانک ابری برای اطلاعات کاربران مشکلی بوجود آمده است\r\n لطفا از ارتباط با اینترنت اطمینان ،وی پی ان خود را خاموش و دوباره امتحان نمایید\r\nدر این شرایط فقط در حالت آفلاین میتوانید اقدام به ورژن زدن نمایید");
            }
        }

        private void chbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chbShowPassword.Checked)
            {
                Txt_Password.PasswordChar = '\0';
            }
            else
            {
                Txt_Password.PasswordChar = '*';
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            if (!IsAdministrator())
            {
                if(MessageBox.Show("برنامه بايد با دسترسي ادمين اجرا شود\r\n آيا با اين حال ادامه ميدهيد؟","هشدار",MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    Result = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                }
            }


            if (Chk_Offline.Checked)
            {
                if (CB_Username.Text == "*****" & Txt_Password.Text == "*****")
                {
                    Frm_Offline_PreperToUpdate Offline = new Frm_Offline_PreperToUpdate(-1);
                    Offline.Show();
                    this.Hide();
                    return;
                }
            }


         
            string myIP = "";
            try
            {
                string strHostName = System.Net.Dns.GetHostName();
                System.Net.IPHostEntry iphe = System.Net.Dns.GetHostEntry(strHostName);

                foreach (System.Net.IPAddress ipheal in iphe.AddressList)
                {
                    if (ipheal.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        myIP = ipheal.ToString();
                        break;
                    }
                }
            }
            catch
            {
            }


            if ((Environment.MachineName == "KEYHAN") & (myIP == "192.168.1.103"))
            {
                CB_Username.Text = "k.azarjoo";
                Txt_Password.Text = "*****";
            }



            if (CB_Username.Text== string.Empty)
            {
                MessageBox.Show("لطفا نام کاربری را وارد نمایید");
                CB_Username.Focus();
                return;
            }

            if (Txt_Password.Text == string.Empty)
            {
                MessageBox.Show("لطفا کلمه عبور خود را وارد نمایید");
                Txt_Password.Focus();
                return;
            }

            if (!IsUserValid(CB_Username.Text, Txt_Password.Text))
            {

                CB_Username.Focus();
                UpdaterFunctions.SaveTextExeption("UserPass Is Not Valid");
                return;
            }

            try
            {
                if (Chk_Offline.Checked)
                {
                    Frm_Offline_PreperToUpdate Offline = new Frm_Offline_PreperToUpdate(Parsic_user_id);
                    Offline.Show();
                    this.Hide();
                }
                else
                {
                    Result = System.Windows.Forms.DialogResult.OK;
                    Frm_MackeUpdateInfo GoForReady = new Frm_MackeUpdateInfo(Parsic_user_id,true,false);
                    GoForReady.Show();
                    this.Hide();
                }

            }
            catch(Exception EX)
            {
                MessageBox.Show("Error Frm_MackeUpdateInfo : " + EX.Message.ToString());
            }


            //this.Close();
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }





        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void CB_Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Txt_Password.Focus();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnCancel_Click(this, new EventArgs());
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(this, new EventArgs());
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnCancel_Click(this, new EventArgs());
            }
        }
        private bool IsUserValid(string strUserName, string strPassword)//find authentication of user who update DB
        {

            try
            {


                for (int i = 0; i < users.Count(); i++)
                {
                    if (users[i].Str_UserName == strUserName & users[i].Str_Password == strPassword)
                    {
                        Parsic_user_id = users[i].Prk_ParsicUser_AutoID;
                        return true;
                    }
                }
                MessageBox.Show("نام کاربری یا کلمه عبور اشتباه میباشد");
                return false;


            }
            catch (Exception ex)
            {
                MessageBox.Show("CONNECTION ERROR\r\n" + ex.Message);
            }

            return false;
        }


        public string Get_UserName()
        {
            // Return "hbb" & Now.Year & "parsic" & Now.Month
            return "updater";
            //return "Ticketing";
        }
        public string Get_Password()
        {
            try
            {
                string MyKey = "*************************************************************;
                DateTime MyTime = DateTime.Now;
                string MyDate = "*****" + "*****";
                string MyPass = "*****" + "*****";

                MyPass = MySecurity.EncryptData(MyPass, MyKey);

                return MyPass;
            }
            catch (Exception ex)
            {
                return "";
            }
        }







        //╔═════════════ Change RegionLanguage ══════════════╗
        //║                                                  ║
        //║                                                  ║
#region "Change Region and Language"


        private List<RegionalSettings> UKSettings = new List<RegionalSettings>();
        public Boolean ChangeRegionalLanguage()
        {
            string sCountry = "";


            sCountry =  Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\International", "sCountry", "kkk").ToString();


            if (sCountry == "United Kingdom")
            {
                return true;
            }


            UKSettings.Add(new RegionalSettings("iCountry", "44"));
            UKSettings.Add(new RegionalSettings("iCurrDigits", "2"));
            UKSettings.Add(new RegionalSettings("iCurrency", "0"));
            UKSettings.Add(new RegionalSettings("iDate", "1"));
            UKSettings.Add(new RegionalSettings("iDigits", "2"));
            UKSettings.Add(new RegionalSettings("iLZero", "1"));
            UKSettings.Add(new RegionalSettings("iMeasure", "0"));
            UKSettings.Add(new RegionalSettings("iNegCurr", "1"));
            UKSettings.Add(new RegionalSettings("iTime", "1"));
            UKSettings.Add(new RegionalSettings("iTLZero", "1"));
            UKSettings.Add(new RegionalSettings("Locale", "00000809"));
            UKSettings.Add(new RegionalSettings("s1159", "AM"));
            UKSettings.Add(new RegionalSettings("s2359", "PM"));
            UKSettings.Add(new RegionalSettings("sCountry", "United Kingdom"));
            UKSettings.Add(new RegionalSettings("sCurrency", "£"));
            UKSettings.Add(new RegionalSettings("sDate", "/"));
            UKSettings.Add(new RegionalSettings("sDecimal", "."));
            UKSettings.Add(new RegionalSettings("sLanguage", "ENG"));
            UKSettings.Add(new RegionalSettings("sList", ","));
            UKSettings.Add(new RegionalSettings("sLongDate", "dd MMMM yyyy"));
            UKSettings.Add(new RegionalSettings("sShortDate", "dd/MM/yyyy"));
            UKSettings.Add(new RegionalSettings("sThousand", ","));
            UKSettings.Add(new RegionalSettings("sTime", ":"));
            UKSettings.Add(new RegionalSettings("sTimeFormat", "HH:mm:ss"));
            UKSettings.Add(new RegionalSettings("iTimePrefix", "0"));
            UKSettings.Add(new RegionalSettings("sMonDecimalSep", "."));
            UKSettings.Add(new RegionalSettings("sMonThousandSep", ","));
            UKSettings.Add(new RegionalSettings("iNegNumber", "1"));
            UKSettings.Add(new RegionalSettings("sNativeDigits", "0123456789"));
            UKSettings.Add(new RegionalSettings("NumShape", "1"));
            UKSettings.Add(new RegionalSettings("iCalendarType", "1"));
            UKSettings.Add(new RegionalSettings("iFirstDayOfWeek", "0"));
            UKSettings.Add(new RegionalSettings("iFirstWeekOfYear", "0"));
            UKSettings.Add(new RegionalSettings("sGrouping", "3;0"));
            UKSettings.Add(new RegionalSettings("sMonGrouping", "3;0"));
            UKSettings.Add(new RegionalSettings("sPositiveSign", ""));
            UKSettings.Add(new RegionalSettings("sNegativeSign", "-"));


            foreach (RegionalSettings reg in UKSettings)
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\International", reg.entry, reg.value);
            NotifyInternationalChanges();

            return true;
        }
        private void NotifyInternationalChanges()
        {
            var HWND_BROADCAST = new IntPtr(0xFFFF); // broadcast to entire system
            var Lparam = Marshal.StringToBSTR("intl");
            SendNotifyMessage(HWND_BROADCAST, 0x1A, UIntPtr.Zero, Lparam);
            Marshal.FreeBSTR(Lparam);
        }
        public static bool SendNotifyMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            return true;
        }
        private class RegionalSettings
        {
            public string entry;
            public string value;
            public RegionalSettings(string key, string value)
            {
                this.entry = key; this.value = value;
            }
        }

        private void CB_Username_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        #endregion
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝







    }
}
