using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UpdaterClasses;

namespace ParsicAutoUpdater
{
    public partial class Frm_MackeUpdateInfo : Form
    {
        Int32 ParsicUserID;
        string connection = "";
        DataTable Dt;
        DataTable Db_Parsic;
        GetAndInsertVersionInDB UpdaterFunctions;
        double DBversion = 10.01;
        DataTable DbParsicMaster = new DataTable("Ver");
        string DB_ServerName = "";
        string DB_Name = "";
        string DB_Username = "";
        string DB_Password = "";
        string LabID = "";
        Boolean WithBackup = true;
        DataSet DS;
        DataTable DTV;
        Boolean IsFarFromHear = false;
        Boolean IsFromIIS = false;
        string Confirmer = " ";

        Parsicwebservice.Service1SoapClient PWS = new Parsicwebservice.Service1SoapClient();
        Parsic.Business.Security.Cls_Encryption MySecurity = new Parsic.Business.Security.Cls_Encryption();
        DataTable FirstToLastVer = new DataTable("DT");
        DataTable DTCloudVerID = new DataTable("Versions");
        public delegate void SetFrmHiden(Boolean IsHiden);
        String LastVersion = "";

        ToolTip toolTip1 = new ToolTip();
        string LabverC = "0";
        string LabverQ = "0";
        string LabverS = "0";
        string LabverJ = "0";
        string LabverT = "0";
        string LabverP = "0";
        string LabverPr = "0";
        string LabverQM = "0";
        string LabverW = "0";
        List<double> LabVersion = new List<double>();
        string LastCloudVersion;



        public Frm_MackeUpdateInfo(Int32 _ParsicUserID, Boolean _WithBackup = true, Boolean _IsFarFromHear = false, string _Confirmer = " ", string _LastVersion = "")//_WithBackup must be true
        {
            try
            {
                UpdaterFunctions = new GetAndInsertVersionInDB(_ParsicUserID);
                IsFarFromHear = _IsFarFromHear;
                if (IsFarFromHear)
                {
                    this.Hide();
                    this.HidenThis(true);
                }
                Confirmer = _Confirmer;
                WithBackup = _WithBackup;
                DS = new DataSet();
                ParsicUserID = _ParsicUserID;
                Boolean DoIT = false;
                Dt = new DataTable("Dt");
                double DB_version = 10.01;
                DataTable __DTParsicMaster = new DataTable();
                LastVersion = _LastVersion;
                try
                {
                    InitializeComponent();
                }
                catch (Exception EX) 
                {
                    if (!IsFarFromHear)
                    {
                        MessageBox.Show("Error : in Frm_MackeUpdateInfo InitializeComponent , Error : " + EX.Message.ToString());
                    }
                }
                List<string> GetODBCInfoList = new List<string>();
                try
                {
                    GetODBCInfoList = UpdaterFunctions.GetODBCInformation();
                    //if (GetODBCInfoList.Count == 1)
                    //    //return GetODBCInfoList[0];
                    try
                    {
                        Text = GetODBCInfoList[0];
                    }catch
                    {

                    }
                    if (GetODBCInfoList.Count == 1)
                    {
                        UpdaterFunctions.SaveTextExeption("ERROR : " + GetODBCInfoList[0]);
                        if (!IsFarFromHear)
                        {
                            MessageBox.Show(GetODBCInfoList[0], "هشدار");
                        }
                        Text = "";
                    }

                    __DTParsicMaster = UpdaterFunctions.UpdateGridView(GetODBCInfoList);

                    DbParsicMaster = __DTParsicMaster;
                    LabID = UpdaterFunctions.GetLabID(__DTParsicMaster);

                    if (LabID == "")
                    {
                        if (!IsFarFromHear)
                        {
                            MessageBox.Show("آی دی آزمایشگاه یافت نشد");
                        }

                    }
                    Boolean DBTrueExist = false;
                    string inererrormsg = "";
                    try
                    {
                        foreach (DataRow dr in __DTParsicMaster.Rows)
                        {
                            if ((dr["Is_Present"].ToString() == "True"))
                            {
                                UpdaterFunctions.SaveTextExeption("DB Name   :          " + dr["DBList_Name"].ToString());

                                if ((dr["DBList_IsActive"].ToString() == "True"))
                                {
                                    DBTrueExist = true;
                                    DB_ServerName = dr["DBList_Server"].ToString();
                                    DB_Name = dr["DBList_Name"].ToString();
                                    DB_Username = dr["DBList_Username"].ToString();
                                    DB_Password = dr["DBList_Password"].ToString();
                                    inererrormsg = "DB_ServerName = " + DB_ServerName + "\r\nDB_Name = " + DB_Name + "\r\n";
                                    inererrormsg = inererrormsg + "version = " + DB_version;
                                    DoIT = true;
                                    DB_version = Convert.ToDouble(dr["DBCurruntVersionC"]);
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        UpdaterFunctions.SaveTextExeption("Error : " + ex.Message.ToString());

                    }

                    if (DoIT == true)
                    {
                        Db_Parsic = __DTParsicMaster;

                    }
                    else
                    {

                        if (DBTrueExist)
                        {

                            if (!IsFarFromHear)
                            {
                                MessageBox.Show("در دسترسی به اطلاعات ورژن بانک پایین مشکلی بوجود آمده است" + "\r\n" + inererrormsg, "هشدار");
                            }
                        }
                        else
                        {
                            if (!IsFarFromHear)
                            {
                                MessageBox.Show("برای این کار حتما باید یک بانک فعال وجود داشته باشد", "هشدار");
                            }
                        }
                        System.Windows.Forms.Application.Exit();
                        return;
                    }

                }
                catch (Exception EX)
                {

                    if (!IsFarFromHear)
                    {

                        MessageBox.Show("Error in find DBParsicMaster");
                    }
                    return;
                }

                if (DoIT == true)
                {

                    DB_ServerName = GetODBCInfoList[0];
                    connection = UpdaterFunctions.MakeConnectionString(DB_ServerName, DB_Name, DB_Username, DB_Password);
                }

                DBversion = DB_version;
                UpdaterFunctions.SaveTextExeption("DBversion : " + DBversion.ToString());

            }
            catch (Exception EX)
            {
                UpdaterFunctions.SaveTextExeption("Error : " + EX.Message.ToString());
            }

        }


        public Frm_MackeUpdateInfo()
        {
            InitializeComponent();
        }


        private void Frm_MackeUpdateInfo_Load(object sender, EventArgs e)
        {
            if (!IsFarFromHear)
            {
                string p = UpdaterFunctions.FindSpecialPathInAllDrives("Web");
                if(p == "")
                {
                    MessageBox.Show("در هیچ یک از درایو ها پیدا نشد web فولدر\r\nلطفا برای سرویس های وب این پوشه را ایجاد نمایید");
                }

            }



            ShowWhichKindOfVersion();
            string VerCloud = "99.99";
            if (LastVersion != "")
            {
                VerCloud = LastVersion;
            }
            toolTip1.SetToolTip(this.Chk_Lock, "این آپشن در صورتی که ورژن سنترال در سرور ابری پایین تر باقی نرم افزار ها باشد اجازه ورژن زدن به نرم افزار های پایین تر را نمیدهد");
            toolTip1.SetToolTip(this.Chk_Backup, "این آپشن برای گرفتن بک آپ به صورت اتوماتیک میباشد\r\n میباشد AutoBackupBeforVersion بک آپ در یکی از درایو های سرور اسکیوال در فولدر");
            toolTip1.SetToolTip(this.Chk_Force, "اطلاعات مربوط به لب اندروید و لب آنلاین را بررسی میکند، و در صورت وجود ارور اجازه ورژن زدن داده نمیشود");

            ////DTV = PWS.Get_LastVersions(Get_UserName(), Get_Password(), VerCloud);


            DTV = GetCloudVersion(VerCloud);






            List<double> MyVersion = new List<double>();


            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["Centeral"])); } catch { DTV.Rows[0]["Centeral"] = 0;  MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["QC"])); } catch { DTV.Rows[0]["QC"] = 0; MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["Storage"])); } catch { DTV.Rows[0]["Storage"] = 0; MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["Journal"])); } catch { DTV.Rows[0]["Journal"] = 0; MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["Temperature"])); } catch { DTV.Rows[0]["Temperature"] = 0; MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"])); } catch { DTV.Rows[0]["ParsicLabAndroid"] = 0; MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["PrinterCacher"])); } catch { DTV.Rows[0]["PrinterCacher"] = 0; MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["QMatic"])); } catch { DTV.Rows[0]["QMatic"] = 0; MyVersion.Add(0); }
            try { MyVersion.Add(Convert.ToDouble(DTV.Rows[0]["Web"])); } catch { DTV.Rows[0]["Web"] = 0; MyVersion.Add(0); }

            MyVersion.Sort();
            LastCloudVersion = MyVersion[MyVersion.Count - 1].ToString();

            List<double> FirstVersions = new List<double>();
            for (int i = 0; i < MyVersion.Count - 1; i++)
            {
                if (MyVersion[i].ToString() == "0")
                {

                }
                else
                {
                    FirstVersions.Add(MyVersion[i]);
                }
            }

            double FLastver = Convert.ToDouble(LastCloudVersion);
            Cbox_LastVersions.Items.Clear();
            for (int i = 0; i < 30; i++)
            {
                string s = "0.01";
                if (FLastver.ToString().Length == 4)
                {
                    s = FLastver + "0";
                }
                else
                {
                    s = FLastver.ToString();
                }
                Cbox_LastVersions.Items.Add(s);
                FLastver = FLastver - 0.01;
            }
            try
            {

                Cbox_LastVersions.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                //UpdaterFunctions.SaveTextExeption("1120100 : " + ex.Message.ToString());
            }

            //FindLabUsers
            FindLabUsersAndDBs();
            //FindLabUsers\

            Docheck();
            if (IsFarFromHear)
            {
                Btn_Go.PerformClick();
            }
            if (IsFarFromHear)
            {
                this.Hide();
                this.HidenThis(true);
            }
        }
        public void ShowWhichKindOfVersion()
        {
            try
            {



                string CentralPath = UpdaterFunctions.FindFolder("Centeral");
                //Lbl_CenteralVersion.Text = UpdaterFunctions.GetVersion(CentralPath + "\\CenteralApp.exe");
                //Lbl_DbVersion.Text = UpdaterFunctions.GetDBVersion("Centeral",connection);
                //Lbl_ScriptVersion.Text = UpdaterFunctions.GetDBScriptVersion(connection);
                Lbl_CenteralVersion.Text = UpdaterFunctions.GetVersion(CentralPath + "\\CenteralApp.exe");
                Lbl_DbVersion.Text = UpdaterFunctions.GetDBVersion("Centeral", connection);
                Lbl_ScriptVersion.Text = UpdaterFunctions.GetDBScriptVersion(connection, 1);

                if (UpdaterFunctions.IsUserFromParsic())
                {
                    GBax_InLab.Visible = true;
                    this.Height = 840;
                }
                else
                {
                    GBax_InLab.Visible = false;
                    this.Height = 547;
                }

            }
            catch(Exception EX)
            {

            }
            //GBax_InLab.Visible = true;
            //this.Height = 442;
        }

        private void Dg_VersionList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if(UpdaterFunctions.GetServiceState("ParsicSMS") == "Running")
                {
                    if (!IsFarFromHear)
                    {
                        MessageBox.Show("سرویس پارسیک اس ام اس در حال اجرا میباشد" + "\r\n" + "در صورت اجرا بودن سرویس اس ام اس در زمان بروز رسانی برنامه اطلاعات به درستی جابجا نمیشوند" + "\r\n" + "در زمان بروزرسانی برنامه سنترال ابتدا سرویس را متوقف کنید و بعد از چند لحظه اقدام به ورژن زدن نمایید", "هشدار", MessageBoxButtons.OK);
                    }
                }


                bool DoIt = false;
                if (Dt.Rows[Dg_VersionList.CurrentRow.Index]["State"].ToString() == "Unstable")
                {
                    if (!IsFarFromHear)
                    {
                        MessageBox.Show("ورژن انتخاب شده ناپایدار میباشد");
                    }
                }
                else if (Dt.Rows[Dg_VersionList.CurrentRow.Index]["State"].ToString() == "Test")
                {
                    if (!IsFarFromHear)
                    {
                        if (MessageBox.Show("ورژن انتخاب شده برای تست میباشد، آیا با این حال ادامه میدهید؟", "هشدار", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            DoIt = true;
                    }
                    else
                    {
                            DoIt = true;
                    }
                }
                else
                    DoIt = true;
                string Warning = "";

                bool DoIt2 = false;

                if(Chk_ExeDownload.Checked == false && Chk_DoScripts.Checked == false)
                {
                    DoIt = false;
                    if (!IsFarFromHear)
                    {
                        MessageBox.Show("لطفا یکی از دانلود فایل یا اعمال اسکریپت را انتخاب نمایید");
                    }
                }


                if (DoIt)
                {
                    var loopTo = Dt.Rows.Count - 1;
                    for (var i = 0; i <= loopTo; i++)
                    {
                        if (Dt.Rows[i]["Description"].ToString() != "")
                            Warning = Warning + "\r\n" + Dt.Rows[i]["Description"];
                    }
                    if (Warning == "")
                        DoIt2 = true;
                    //else if (MessageBox.Show(Warning.Replace("!@#$%^&**&^%$#@!","\r\n").Replace( "$#@!!@#$","'").Replace( "*&^%%^&*","-"), "هشدار", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1,MessageBoxOptions.RtlReading) == DialogResult.OK)
                    else
                    {
                        if (!IsFarFromHear)
                        {
                            Frm_MessageBox MyMessagebox = new Frm_MessageBox(Warning.Replace("!@#$%^&**&^%$#@!", Environment.NewLine).Replace("$#@!!@#$", "'").Replace("*&^%%^&*", "-"), "هشدار");
                            if (MyMessagebox.ShowDialog() == DialogResult.OK)
                            {
                                DoIt2 = true;
                            }
                        }
                        else
                        {
                                DoIt2 = true;
                        }
                            
                    }

                }
                if (DoIt2)
                {
                    if (Cbox_ConfirmBy.SelectedIndex == -1)
                    {
                        DoIt2 = false;
                        if (!IsFarFromHear)
                        {
                            MessageBox.Show("تایید کننده وارد نشده است", "هشدار");
                        }
                    }
                }
                

                if (DoIt2)
                {
                    string Command = "";
                    string ParsicUserIDStr = ParsicUserID.ToString();
                    string VersionReleaseIdStr = Dt.Rows[Dg_VersionList.CurrentRow.Index]["Prk_VersionRelease_AutoID"].ToString();
                    string VersionNumberStr = Dt.Rows[Dg_VersionList.CurrentRow.Index]["VersionNumber"].ToString();
                    string SqlServerNameStr = DB_ServerName;
                    string Db_ParsicMasterStr = "Db_ParsicMaster";
                    string TBL_DBListStr = "TBL_DBList";
                    string UserNameStr = "sa";
                    //string PassWordStr = "who";
                    string PassWordStr = DB_Password;
                    string Exe; if (Chk_ExeDownload.Checked) { Exe = "1"; } else { Exe = "0"; }
                    string Scripts; if (Chk_DoScripts.Checked) { Scripts = "1"; } else { Scripts = "0"; }
                    string Chk_CenteralStr; if (Chk_Centeral.Checked) { Chk_CenteralStr = "1"; } else { Chk_CenteralStr = "0"; }
                    string Chk_QCStr; if (Chk_QC.Checked) { Chk_QCStr = "1"; } else { Chk_QCStr = "0"; }
                    string Chk_StorageStr; if (Chk_Storage.Checked ) { Chk_StorageStr = "1"; } else { Chk_StorageStr = "0"; }
                    string Chk_JournalStr; if (Chk_Journal.Checked) { Chk_JournalStr = "1"; } else { Chk_JournalStr = "0"; }
                    string Chk_TemperatureStr; if (Chk_Temperature.Checked) { Chk_TemperatureStr = "1"; } else { Chk_TemperatureStr = "0"; }
                    string Str_withBackUp; if (WithBackup) { Str_withBackUp = "1"; } else { Str_withBackUp = "0"; }
                    string Chk_ParsicLabAndroidStr; if (Chk_ParsicLabAndroid.Checked) { Chk_ParsicLabAndroidStr = "1"; } else { Chk_ParsicLabAndroidStr = "0"; }
                    string Chk_PrinterCacherStr; if (Chk_PrinterCacher.Checked) { Chk_PrinterCacherStr = "1"; } else { Chk_PrinterCacherStr = "0"; }
                    string Chk_QMaticStr; if (Chk_QMatic.Checked) { Chk_QMaticStr = "1"; } else { Chk_QMaticStr = "0"; }
                    string Chk_WebStr; if (Chk_Web.Checked || Chk_WebChk.Checked) { Chk_WebStr = "1"; } else { Chk_WebStr = "0"; }

                    //string VersN = DTV.Rows[0]["Centeral"].ToString() + "," + DTV.Rows[0]["QC"].ToString() + "," + DTV.Rows[0]["Storage"].ToString() + "," + DTV.Rows[0]["Journal"].ToString() + "," + DTV.Rows[0]["Temperature"].ToString() + "," + DTV.Rows[0]["ParsicLabAndroid"].ToString() + "," + DTV.Rows[0]["PrinterCacher"].ToString() + "," + DTV.Rows[0]["QMatic"].ToString();




                    Command = "UserId#" + ParsicUserIDStr + "!@#VerID#" + VersionReleaseIdStr + "!@#VerNum#" + VersionNumberStr + ", " + "!@#Server#" + SqlServerNameStr + "!@#DbName#" + Db_ParsicMasterStr + "!@#TblName#" + TBL_DBListStr + "!@#User#" + UserNameStr + "!@#Pass#" + PassWordStr + "!@#Exe#" + Exe + "!@#Scripts#" + Scripts + "!@#Central#" + Chk_CenteralStr + "!@#QC#" + Chk_QCStr + "!@#Storage#" + Chk_StorageStr + "!@#Jurnul#" + Chk_JournalStr + "!@#Temperature#" + Chk_TemperatureStr + "!@#ParsicLabAndroid#" + Chk_ParsicLabAndroidStr + "!@#PrinterCacher#" + Chk_PrinterCacherStr + "!@#QMatic#" + Chk_QMaticStr + "!@#Web#" + Chk_WebStr + "!@#BackUp#-1!@#CentralVer#-1!@#QCVer#-1!@#StorageVer#-1!@#JournalVer#-1!@#TemperatureVer#-1!@#ParsicLabAndroidVer#-1!@#PrinterCacherVer#-1!@#QMaticVer#-1!@#WebVer#-1" ;
                    string _Command = Command.Replace(" ", "#@!");
                    try
                    {
                        Boolean user_is_from_Parsic = UpdaterFunctions.IsUserFromParsic();
                        if (UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, Lbl_ScriptVersion.Text, VersionNumberStr, "", Convert.ToInt32( LabID), ParsicUserID,0, 5, Cbox_ConfirmBy.SelectedItem.ToString()))
                        {
                            if (!IsFarFromHear)
                            {
                                Frm_VersionList2 GoForReady = new Frm_VersionList2(_Command, Cbox_ConfirmBy.SelectedValue.ToString(), true);
                                GoForReady.Show();
                                this.Hide();
                            }
                            else
                            {
                                Frm_VersionList2 GoForReady = new Frm_VersionList2(_Command, Confirmer, true);
                                GoForReady.Show();
                                this.Hide();
                            }

                        }
                        else
                        {
                            if (!IsFarFromHear)
                            {
                                MessageBox.Show("در ارتباط با سرور ابری برای ارسال لاگ مشکلی وجود دارد", "خطا");
                            }
                        }
                    }
                    catch(Exception EX)
                    {
                        UpdaterFunctions.SaveTextExeption("Error : " + EX.Message.ToString());

                        if (!IsFarFromHear)
                        {
                            MessageBox.Show(EX.Message.ToString(), "خطا");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UpdaterFunctions.SaveTextExeption("Error 2233: " + ex.Message.ToString());

            }


        }

        private void Gbox_Cheks_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Gbox_Versions_Enter(object sender, EventArgs e)
        {

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
                string MyKey = "rPGhgO2urgm1k09RTU4cIIIbISbFKUXI2UqVGsYo91RXIZOHH3YsMmBmFWQbN7rhl8ds5GaMR/PXCUGOfaiDIFFIY1T49/uXg0frUHIJ1cArqch1zCJW6L78oqrtEEtcJk1arEy8F1Q=";
                //string MyKey = "/kTzjbqV5oKjaDjmpOdO/raZxL29fGKA7Dwat5h1+Ob9aXgA/mzgSICcITO+Mj/GtNllfSpaz5XDprdRpRQvPvA2IUSRU6zfl8ds5GaMR/Mdvgyb4+L9oICcITO+Mj/GtNllfSpaz5WEd9bSTa14qA==";
                DateTime MyTime = DateTime.Now;
                string MyDate = MyTime.Year + MyTime.Month.ToString().PadLeft(2, '0') + MyTime.Day.ToString().PadLeft(2, '0');
                string MyPass = "Parsic;" + MyTime.Second.ToString().PadLeft(2, '0') + ";Hbb;" + MyTime.Millisecond.ToString().PadLeft(3, '0') + ";!@#;" + MyDate + ";Raz;" + MyDate + MyTime.Second.ToString().PadLeft(2, '0') + MyTime.Millisecond.ToString().PadLeft(3, '0');

                MyPass = MySecurity.EncryptData(MyPass, MyKey);

                return MyPass;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private void Btn_Go_Click(object sender, EventArgs e)
        {



            if(!Chk_CentralChk.Checked  && !Chk_QCChk.Checked && !Chk_StorageChk.Checked && !Chk_JournalChk.Checked && !Chk_TemperatureChk.Checked && !Chk_ParsicLabAndroidChk.Checked && !Chk_PrinterCacherChk.Checked && !Chk_QMaticChk.Checked && !Chk_WebChk.Checked)
            {
                MessageBox.Show("هیچ ورژنی انتخاب نشده است", "هشدار");
                return;
            }


            if (!IsFromIIS )
            {
                if (!IsFarFromHear)
                {
                    string Path = "";
                    Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                    if (!Directory.Exists(Path))
                    {
                        if (MessageBox.Show("آپدیتور حتما باید بر روی سیستم آی آی اس ران شود\r\nدر صورد ادامه ورژن های مرتبط به آی آی اس بروز نمیشوند\r\n با این حال آیا ادامه میدهید؟","هشدار",MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }

                }
            }
            else
            {
                if (Chk_ParsicLabAndroidChk.Checked == true || Chk_WebChk.Checked == true)
                {
                    string Path = "";
                    Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                    if (Path != "")
                    {
                        string drive = Path.Substring(0, 3);
                        string[] drives = UpdaterFunctions.ListOfDrives();
                        try
                        {
                            DriveInfo dDrive = new DriveInfo(drive);
                            if (dDrive.IsReady)
                            {
                                // Calculate the percentage free space
                                Int64 FreeSpaceMB = (Int64)(((Int64)dDrive.TotalFreeSpace / 1024) / 1024);
                                Int64 FreeSpaceGB = (Int64)((((Int64)dDrive.TotalFreeSpace / 1024) / 1024) / 1024);

                                if (FreeSpaceGB <= 2)
                                {
                                    MessageBox.Show("فضای ذخیره سازی درایوری که فولدر وب در آن قرار دارد کمتر از 2 گیگابایت میباشد، لطفا بررسی نمایید", "هشدار");
                                    return;
                                }

                                //Space += "Name : " + dDrive.Name + "     Format : " + dDrive.DriveFormat + "     Total : " + (((((float)dDrive.TotalSize) / 1024) / 1024) / 1024) + "     Available : " + (((((float)dDrive.AvailableFreeSpace) / 1024) / 1024) / 1024) + "     Free Space percentage : " + freeSpacePerc + "%" + "\r\n";
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    else
                    {

                    }

                }
            }



            if(Chk_Force.Checked)
            {
                if(Chk_ParsicLabAndroid.Checked)
                {
                    try
                    {
                        string IISLabAndroidURL = "";
                        string curentPath = "";
                        DataTable LabInfo = new DataTable("DT");

                        LabInfo = UpdaterFunctions.GetCloudLabInfo(Convert.ToInt32(LabID));
                        if (LabInfo.Rows.Count == 0)
                        {
                            MessageBox.Show("اطلاعات پارسیک لب اندروید در تیکتینگ ثبت نشده است، لطفا اطلاعات را در تیکتینگ به درستی ثبت کنید");
                            return;
                        }

                        IISLabAndroidURL = LabInfo.Rows[0]["Str_ServiceURL"].ToString();
                        string MyPort = UpdaterFunctions.FindPort(IISLabAndroidURL);
                        if(MyPort == "")
                        {
                            MessageBox.Show("پورت لب اندروید سرویس در آدرس لب اندروید در تیکتینگ پیدا نشد\r\nلطفا آدرس لب اندروید آزمایشگاه را در تیکتینگ چک نمایید ");
                            return;
                        }
                        string MySiteName = UpdaterFunctions.FindIISSiteName(MyPort);
                        if (MySiteName == "")
                        {
                            MessageBox.Show("پرت ثبت شده برای لب اندروید سرویس در تیکتینگ با هیچ یک از سرور های آی آی اس برابری ندارد\r\n لطفا پرت آزمایشگاه در تیکتبنگ و نام و پرت سرور در آی آی اس آزمایشگاه را چک نمایید");
                            return;
                        }
                        curentPath = UpdaterFunctions.GetIISSitePath(MySiteName);
                        if(curentPath.Contains("Error"))
                        {
                            MessageBox.Show("مسیر پارسیک لب اندرویدی وجود ندارد\r\nلطفا مسیر ثبت شده برای لب اندروید در آی آی اس را چک نمایید\r\n"+ curentPath);
                            return;
                        }

                        if(!File.Exists(curentPath+"\\web.config"))
                        {
                            MessageBox.Show("فایل وب کانفیگ در آدرس ذخیره شده در آی آی اس وجود خارجی ندارد\r\n مطمئن شوید web.config لطفا از وجود فایل");
                            return;
                        }

                        DataTable ConfigDt = new DataTable("DT");
                        //Get Web.config
                        string mypath = curentPath + "\\web.config";
                        ConfigDt = UpdaterFunctions.GetWebConfigInfo(mypath);
                        if (ConfigDt.Rows.Count == 0)
                        {
                            MessageBox.Show(" قابل خواندن نمیباشد web.config فایل"+"\r\n"+"لطفا دسترسی های لازم را به فایل بدهید"+ "\r\npath:"+ mypath);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (Chk_Web.Checked)
                {
                    try
                    {
                        DataTable LabInfo = new DataTable("DT");
                        string IISLabOnlineURL = "";
                        string curentPath = "";

                        LabInfo = UpdaterFunctions.GetCloudLabInfo(Convert.ToInt32(LabID));
                        if (LabInfo.Rows.Count == 0)
                        {
                            MessageBox.Show("اطلاعات پارسیک لب آنلاین در تیکتینگ ثبت نشده است، لطفا اطلاعات را در تیکتینگ به درستی ثبت کنید");
                            return;
                        }

                        IISLabOnlineURL = LabInfo.Rows[0]["Str_WebSiteAddress"].ToString();
                        string MyPort = UpdaterFunctions.FindPort(IISLabOnlineURL);
                        if (MyPort == "")
                        {
                            MessageBox.Show("پورت لب آنلاین سرویس در آدرس لب آنلاین در تیکتینگ پیدا نشد\r\nلطفا آدرس لب آنلاین آزمایشگاه را در تیکتینگ چک نمایید ");
                            return;
                        }
                        string MySiteName = UpdaterFunctions.FindIISSiteName(MyPort);
                        if (MySiteName == "")
                        {
                            MessageBox.Show("پرت ثبت شده برای لب اندروید سرویس در تیکتینگ با هیچ یک از سرور های آی آی اس برابری ندارد\r\n لطفا پرت آزمایشگاه در تیکتبنگ و نام و پرت سرور در آی آی اس آزمایشگاه را چک نمایید");
                            return;
                        }


                        curentPath = UpdaterFunctions.GetIISSitePath(MySiteName);
                        if (curentPath.Contains("Error"))
                        {
                            MessageBox.Show("مسیر پارسیک لب آنلاین وجود ندارد\r\nلطفا مسیر ثبت شده برای لب آنلاین در آی آی اس را چک نمایید\r\n" + curentPath);
                            return;
                        }


                        if (!File.Exists(curentPath + "\\web.config"))
                        {
                            MessageBox.Show("فایل وب کانفیگ در آدرس ذخیره شده در آی آی اس وجود خارجی ندارد\r\n مطمئن شوید web.config لطفا از وجود فایل");
                            return;
                        }
                        if (!Directory.Exists(curentPath + "\\Content\\Images"))
                        {
                            MessageBox.Show("فایل تصاویر در آدرس زیر وجود خارجی ندارد\r\nلطفا از وجود فایل مطمئن شوید \r\n" + curentPath + "\\Content\\Images");
                            return;
                        }
                        if (!Directory.Exists(curentPath + "\\Content\\Qmatic\\Tabligh"))
                        {
                            MessageBox.Show("فایل تصاویر در آدرس زیر وجود خارجی ندارد\r\nلطفا از وجود فایل مطمئن شوید \r\n" + curentPath + "\\Content\\Qmatic\\Tabligh");
                            return;
                        }


                    }
                    catch(Exception ex)
                    {

                    }
                }
            }

            if (Cbox_LastVersions.SelectedIndex == 0)
            {

            }

            try
            {
                //if (UpdaterFunctions.GetServiceState("ParsicSMS") == "Running")
                //{
                //    if (!IsFarFromHear)
                //    {
                //        MessageBox.Show("سرویس پارسیک اس ام اس در حال اجرا میباشد" + "\r\n" + "در صورت اجرا بودن سرویس اس ام اس در زمان بروز رسانی برنامه اطلاعات به درستی جابجا نمیشوند" + "\r\n" + "در زمان بروزرسانی برنامه سنترال ابتدا سرویس را متوقف کنید و بعد از چند لحظه اقدام به ورژن زدن نمایید", "هشدار", MessageBoxButtons.OK);
                //    }
                //}


                if (!Chk_Backup.Checked)
                {
                    WithBackup = false;
                   if( MessageBox.Show("بک آپ به صورت اتوماتیک گرفته نمیشود\r\n حتما قبل از ورژن زدن از بانک اصلی بک آپ بگیرید\r\nبا این حال آیا ادامه میدهید؟", "هشدار", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    WithBackup = true;
                }

                if (!Chk_Lock.Checked)
                {
                   if( MessageBox.Show("قفل کنترل ورژن ها برداشته شده است\r\nاین عمل ورژن هایی که از سنترال بالا تر میباشد را اعمال میکند\r\n با این حال ادامه میدهید؟", "هشدار",MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                }

                bool DoIt = true;
                
                string Warning = "";

                bool DoIt2 = false;

                if (Chk_ExeDownload.Checked == false && Chk_DoScripts.Checked == false)
                {
                    DoIt = false;
                    if (!IsFarFromHear)
                    {
                        MessageBox.Show("لطفا یکی از دانلود فایل یا اعمال اسکریپت را انتخاب نمایید");
                    }
                }


                if (DoIt)
                {
                    DataTable VersionForWarning = new DataTable("VersionForWarning");
                    VersionForWarning = UpdaterFunctions.GetListOfExeVersion(Convert.ToDouble(LabverC) + 0.01);
                    var loopTo = VersionForWarning.Rows.Count - 1;
                    for (var i = 0; i <= loopTo; i++)
                    {
                        if (VersionForWarning.Rows[i]["Description"].ToString() != "")
                            Warning = Warning + "\r\n" + VersionForWarning.Rows[i]["Description"];
                    }
                    if (Warning == "")
                        DoIt2 = true;
                    //else if (MessageBox.Show(Warning.Replace("!@#$%^&**&^%$#@!","\r\n").Replace( "$#@!!@#$","'").Replace( "*&^%%^&*","-"), "هشدار", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1,MessageBoxOptions.RtlReading) == DialogResult.OK)
                    else
                    {
                        if (IsFarFromHear)
                        {
                            DoIt2 = true;
                        }
                        else
                        {
                            Frm_MessageBox MyMessagebox = new Frm_MessageBox(Warning.Replace("!@#$%^&**&^%$#@!", Environment.NewLine).Replace("$#@!!@#$", "'").Replace("*&^%%^&*", "-"), "هشدار");
                            if (MyMessagebox.ShowDialog() == DialogResult.OK)
                            {
                                DoIt2 = true;
                            }
                        }

                    }

                }
                if (DoIt2)
                {
                    if (Cbox_ConfirmBy.SelectedIndex == -1)
                    {
                        if (!IsFarFromHear)
                        {
                            DoIt2 = false;

                            MessageBox.Show("تایید کننده وارد نشده است", "هشدار");
                        }
                    }
                }


                if (DoIt2)
                {
                    string Command = "";
                    string ParsicUserIDStr = ParsicUserID.ToString();
                    string VersionReleaseIdStr = "";
                    string VersionNumberStr = "";
                    try
                    {
                        VersionReleaseIdStr = Dt.Rows[Dg_VersionList.CurrentRow.Index]["Prk_VersionRelease_AutoID"].ToString();
                        VersionNumberStr = Dt.Rows[Dg_VersionList.CurrentRow.Index]["VersionNumber"].ToString();
                    }
                    catch
                    {

                    }



                    string SqlServerNameStr = DB_ServerName;
                    string Db_ParsicMasterStr = "Db_ParsicMaster";
                    string TBL_DBListStr = "TBL_DBList";
                    string UserNameStr = "sa";
                    //string PassWordStr = "who";
                    string PassWordStr = DB_Password;
                    string Exe; if (Chk_ExeDownload.Checked) { Exe = "1"; } else { Exe = "0"; }
                    string Scripts; if (Chk_DoScripts.Checked) { Scripts = "1"; } else { Scripts = "0"; }
                    string Chk_CenteralStr; if (Chk_CentralChk.Checked) { Chk_CenteralStr = "1"; } else { Chk_CenteralStr = "0"; }
                    string Chk_QCStr; if (Chk_QCChk.Checked) { Chk_QCStr = "1"; } else { Chk_QCStr = "0"; }
                    string Chk_StorageStr; if (Chk_StorageChk.Checked) { Chk_StorageStr = "1"; } else { Chk_StorageStr = "0"; }
                    string Chk_JournalStr; if (Chk_JournalChk.Checked) { Chk_JournalStr = "1"; } else { Chk_JournalStr = "0"; }
                    string Chk_TemperatureStr; if (Chk_TemperatureChk.Checked) { Chk_TemperatureStr = "1"; } else { Chk_TemperatureStr = "0"; }
                    string Str_withBackUp; if (WithBackup) { Str_withBackUp = "1"; } else { Str_withBackUp = "0"; }
                    string Chk_ParsicLabAndroidStr; if (Chk_ParsicLabAndroid.Checked || Chk_ParsicLabAndroidChk.Checked) { Chk_ParsicLabAndroidStr = "1"; } else { Chk_ParsicLabAndroidStr = "0"; }
                    string Chk_PrinterCacherStr; if (Chk_PrinterCacher.Checked || Chk_PrinterCacherChk.Checked) { Chk_PrinterCacherStr = "1"; } else { Chk_PrinterCacherStr = "0"; }
                    string Chk_QMaticStr; if (Chk_QMatic.Checked || Chk_QMaticChk.Checked) { Chk_QMaticStr = "1"; } else { Chk_QMaticStr = "0"; }
                    string Chk_WebStr; if (Chk_Web.Checked || Chk_WebChk.Checked) { Chk_WebStr = "1"; } else { Chk_WebStr = "0"; }

                    string VersN = DTV.Rows[0]["Centeral"].ToString() + "," + DTV.Rows[0]["QC"].ToString() + "," + DTV.Rows[0]["Storage"].ToString() + "," + DTV.Rows[0]["Journal"].ToString() + "," + DTV.Rows[0]["Temperature"].ToString() + "," + DTV.Rows[0]["ParsicLabAndroid"].ToString() + "," + DTV.Rows[0]["PrinterCacher"].ToString() + "," + DTV.Rows[0]["QMatic"].ToString() + "," + DTV.Rows[0]["Web"].ToString();

                    Command = "UserId#" + ParsicUserIDStr + "!@#VerID#-1!@#VerNum#" + VersN + "!@#Server#" + SqlServerNameStr + "!@#DbName#" + Db_ParsicMasterStr + "!@#TblName#" + TBL_DBListStr + "!@#User#" + UserNameStr + "!@#Pass#" + PassWordStr + "!@#Exe#" + Exe + "!@#Scripts#" + Scripts + "!@#Central#" + Chk_CenteralStr  + "!@#QC#" + Chk_QCStr + "!@#Storage#" + Chk_StorageStr  + "!@#Jurnul#" + Chk_JournalStr + "!@#Temperature#" + Chk_TemperatureStr  + "!@#ParsicLabAndroid#" + Chk_ParsicLabAndroidStr + "!@#PrinterCacher#" + Chk_PrinterCacherStr + "!@#QMatic#" + Chk_QMaticStr + "!@#Web#" + Chk_WebStr + "!@#BackUp#" + Str_withBackUp + "!@#CentralVer#" + DTCloudVerID.Rows[0]["VerID"].ToString() + "!@#QCVer#" + DTCloudVerID.Rows[1]["VerID"].ToString() + "!@#StorageVer#" + DTCloudVerID.Rows[2]["VerID"].ToString() + "!@#JournalVer#" + DTCloudVerID.Rows[3]["VerID"].ToString() + "!@#TemperatureVer#" + DTCloudVerID.Rows[4]["VerID"].ToString() + "!@#ParsicLabAndroidVer#" + DTCloudVerID.Rows[5]["VerID"].ToString() + "!@#PrinterCacherVer#" + DTCloudVerID.Rows[6]["VerID"].ToString() + "!@#QMaticVer#" + DTCloudVerID.Rows[7]["VerID"].ToString() + "!@#WebVer#" + DTCloudVerID.Rows[8]["VerID"].ToString();
                    string _Command = Command.Replace(" ", "#@!");
                    try
                    {
                        Boolean user_is_from_Parsic = UpdaterFunctions.IsUserFromParsic();
                        string ConfirmBy = " ";
                        if (!IsFarFromHear)
                        {
                            ConfirmBy = Cbox_ConfirmBy.SelectedItem.ToString();
                        }
                        else
                        {
                            ConfirmBy = Confirmer;
                        }
                        if (UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, Lbl_ScriptVersion.Text, DTV.Rows[0]["Centeral"].ToString() , " ", Convert.ToInt32(LabID), ParsicUserID,0, 5, ConfirmBy))
                        {
                            if (IsFarFromHear)
                            {
                                Frm_VersionList2 GoForReady = new Frm_VersionList2(_Command, ConfirmBy, false);
                                GoForReady.Show();
                                this.Hide();
                            }
                            else
                            {
                                Frm_VersionList2 GoForReady = new Frm_VersionList2(_Command, ConfirmBy, true);
                                GoForReady.Show();
                                this.Hide();
                            }

                        }
                        else
                        {
                            if (!IsFarFromHear)
                            {
                                MessageBox.Show("در ارتباط با سرور ابری برای ارسال لاگ مشکلی وجود دارد", "خطا");
                            }
                        }
                    }
                    catch (Exception EX)
                    {
                        UpdaterFunctions.SaveTextExeption("Error 22 : " + EX.Message.ToString());


                        if (!IsFarFromHear)
                        {
                            MessageBox.Show(EX.Message.ToString(), "خطا");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }









        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        public void HidenThis(Boolean Hiden)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetFrmHiden(HidenThis), Hiden);
            else
            {
                try
                {
                    if (Hiden == true)
                    {
                        this.Hide();
                    }
                    else
                    {
                        this.Show();
                    }
                }
                catch (Exception EX)
                {
                    UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                }

            }
        }



        public void FindLabUsersAndDBs()
        {
            //Find Lab Users
            try
            {
                List<string> _ListOfLabUsers = UpdaterFunctions.GetAllLabUsers(connection);
                List<string> ListOfLabUsers = new List<string>();
                for (int i = 0; i < _ListOfLabUsers.Count; i++)
                {
                    if (_ListOfLabUsers[i].Contains("پرداخت آنلاین") || _ListOfLabUsers[i].Contains("monitor TV") || _ListOfLabUsers[i].Contains("صدور نوبت") || _ListOfLabUsers[i].Contains("*"))
                    {
                    }
                    else
                    {
                        ListOfLabUsers.Add(_ListOfLabUsers[i]);
                    }
                }

                Cbox_ConfirmBy.DataSource = ListOfLabUsers;

                    //Cbox_ConfirmBy.Text = ListOfLabUsers[0];

                Cbox_ConfirmBy.SelectedIndex = -1;
                Cbox_ConfirmBy.Text = "";

            }
            catch
            {
                UpdaterFunctions.SaveTextExeption("اررور در پیدا کردن کاربران آزمایشگاه");
            }
            //Find Lab Users\


            //List Of DB
            try
            {

                List<string> ListOfDB = UpdaterFunctions.GetAllLabDBInParsicMaster(Db_Parsic);

                CBox_DBParsicMasterName.DataSource = ListOfDB;

                //Cbox_ConfirmBy.Text = ListOfLabUsers[0];
                Cbox_ConfirmBy.SelectedIndex = -1;
                Cbox_ConfirmBy.Text = "";

            }
            catch
            {
                UpdaterFunctions.SaveTextExeption("اررور در پیدا کردن بانک های آزمایشگاه");
            }
            //List Of DB\
        }
        public void ListOfExeFile()
        {
            //List Of Exe File
            try
            {
                DataTable Pdt = new DataTable();

                double Ver = 99.99;
                for(Int16 i = 0; i < Db_Parsic.Rows.Count; i++)
                {
                    if(Ver > Convert.ToDouble(Db_Parsic.Rows[i]["DBCurruntVersionC"]))
                    {
                        Ver = Convert.ToDouble(Db_Parsic.Rows[i]["DBCurruntVersionC"]);
                    }
                }


                Pdt = UpdaterFunctions.GetListOfExeVersion(Ver);//.Tables["VerInfo"]
                Dt.Columns.Add("Prk_VersionRelease_AutoID");

                Dt.Columns.Add("VersionNumber");
                Dt.Columns.Add("Int_Status");
                Dt.Columns.Add("State");
                Dt.Columns.Add("Description");
                Dt.Columns.Add("Date");
                Dt.Columns.Add("Time");
                for (int i = 0; i < Pdt.Rows.Count; i++)
                {
                    DataRow dr = Dt.NewRow();
                    dr[0] = Pdt.Rows[i]["Prk_VersionRelease_AutoID"];
                    dr[1] = Pdt.Rows[i]["VersionNumber"];
                    dr[2] = Pdt.Rows[i]["Int_Status"];
                    dr[3] = Pdt.Rows[i]["State"];
                    dr[4] = Pdt.Rows[i]["Description"].ToString().Replace("!@#$%^&**&^%$#@!", Environment.NewLine).Replace("$#@!!@#$", "'").Replace("*&^%%^&*", "-");
                    dr[5] = Pdt.Rows[i]["Date"];
                    dr[6] = Pdt.Rows[i]["Time"];
                    Dt.Rows.Add(dr);
                }
                Dg_VersionList.DataSource = Dt;
                //Dg_VersionList.Columns.Remove("Description");
                Dg_VersionList.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Dg_VersionList.Columns.Remove("Prk_VersionRelease_AutoID");
                Dg_VersionList.Columns.Remove("Int_Status");
                // Color
                try
                {
                    for (int l = 0; l < Dg_VersionList.Rows.Count; l++)
                    {
                        if (l % 2 == 0)
                        {
                            Dg_VersionList.Rows[l].DefaultCellStyle.BackColor = Color.LightCyan;
                        }
                        else
                        {
                            Dg_VersionList.Rows[l].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }
                }
                catch
                {

                }
                try
                {
                    for (int l = 0; l < Dg_VersionList.Rows.Count; l++)
                    {
                        if (Dt.Rows[l]["State"].ToString() == "Unstable")
                        {
                            Dg_VersionList.Rows[l].DefaultCellStyle.BackColor = Color.Red;
                        }
                        if (Dt.Rows[l]["State"].ToString() == "Test")
                        {
                            Dg_VersionList.Rows[l].DefaultCellStyle.BackColor = Color.Yellow;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception EX)
            {

            }
            //List Of Exe File\
        }

        public void FindLabVersions()
        {
            // Set Label For Version Number

            try
            {
                //LabverC = UpdaterFunctions.GetDBVersion("Centeral", connection);
                //LabverQ = UpdaterFunctions.GetDBVersion("QC", connection);
                //LabverS = UpdaterFunctions.GetDBVersion("Storage", connection);
                //LabverJ = UpdaterFunctions.GetDBVersion("Journal", connection);
                //LabverT = UpdaterFunctions.GetDBVersion("Temperature", connection);
                //LabverP = UpdaterFunctions.GetDBVersion("ParsicLabAndroid", connection);



                DataTable ParsicMaster = new DataTable();


                ParsicMaster = DbParsicMaster;

                LabverC = "99.99";
                LabverQ = "99.99";
                LabverS = "99.99";
                LabverJ = "99.99";
                LabverT = "99.99";
                LabverP = "99.99";
                LabverPr = "99.99";
                LabverQM = "99.99";
                LabverW = "99.99";

                // Set Lable Labs Version
                try
                {
                    try
                    {

                        for (Int16 i = 0; i < ParsicMaster.Rows.Count; i++)
                        {
                            
                            if (Convert.ToDouble(LabverC) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionC"].ToString()))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionC"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverC = ParsicMaster.Rows[i]["DBCurruntVersionC"].ToString();
                                }
                            }

                            if (Convert.ToDouble(LabverQ) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionQ"]))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionQ"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverQ = ParsicMaster.Rows[i]["DBCurruntVersionQ"].ToString();
                                }
                            }

                            if (Convert.ToDouble(LabverS) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionS"]))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionS"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverS = ParsicMaster.Rows[i]["DBCurruntVersionS"].ToString();
                                }
                            }



                            if (Convert.ToDouble(LabverJ) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionJ"]))
                            {

                                if ((ParsicMaster.Rows[i]["DBCurruntVersionJ"].ToString() != "0") & (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverJ = ParsicMaster.Rows[i]["DBCurruntVersionJ"].ToString();
                                }
                            }

                            if (Convert.ToDouble(LabverT) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionT"]))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionT"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverT = ParsicMaster.Rows[i]["DBCurruntVersionT"].ToString();
                                }
                            }

                            if (Convert.ToDouble(LabverP) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionP"]))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionP"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverP = ParsicMaster.Rows[i]["DBCurruntVersionP"].ToString();
                                }
                            }

                            if (Convert.ToDouble(LabverPr) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionPr"]))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionPr"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverPr = ParsicMaster.Rows[i]["DBCurruntVersionPr"].ToString();
                                }
                            }

                            if (Convert.ToDouble(LabverQM) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionQM"]))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionQM"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverQM = ParsicMaster.Rows[i]["DBCurruntVersionQM"].ToString();
                                }
                            }
                            if (Convert.ToDouble(LabverW) > Convert.ToDouble(ParsicMaster.Rows[i]["DBCurruntVersionW"]))
                            {
                                if ((ParsicMaster.Rows[i]["DBCurruntVersionW"].ToString() != "0") && (Convert.ToBoolean(ParsicMaster.Rows[i]["Is_Present"]) == true))
                                {
                                    LabverW = ParsicMaster.Rows[i]["DBCurruntVersionW"].ToString();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToString().Contains("nput string was not in a correct forma"))
                        {
                            MessageBox.Show("قرار دهید English سیستم خود را  Region ، Region لطفا در تنضیمات ");
                        }

                    }

                    var LabVerList = new List<string>();
                    LabVerList.Add(LabverC);
                    LabVerList.Add(LabverQ);
                    LabVerList.Add(LabverS);
                    LabVerList.Add(LabverJ);
                    LabVerList.Add(LabverT);
                    LabVerList.Add(LabverP);
                    LabVerList.Add(LabverPr);
                    LabVerList.Add(LabverQM);
                    LabVerList.Add(LabverW);
                    LabVerList.Distinct();
                    LabVerList.Sort();

                }
                catch (Exception ex)
                {
                }
                // Set Lable Labs Version\




                LabVersion.Add(Convert.ToDouble(LabverC));
                LabVersion.Add(Convert.ToDouble(LabverQ));
                LabVersion.Add(Convert.ToDouble(LabverS));
                LabVersion.Add(Convert.ToDouble(LabverJ));
                LabVersion.Add(Convert.ToDouble(LabverT));
                LabVersion.Add(Convert.ToDouble(LabverP));
                LabVersion.Add(Convert.ToDouble(LabverPr));
                LabVersion.Add(Convert.ToDouble(LabverQM));
                LabVersion.Add(Convert.ToDouble(LabverW));
                //LVersion.Add(11.12);

                LabVersion.Sort();

                List<double> distinctWords = new List<double>(LabVersion.Distinct());
                LabVersion.Clear();
                int count = 1;
                for (int i = 0; i < distinctWords.Count; i++)
                {
                    if (distinctWords[i].ToString() == "0" || distinctWords[i].ToString() == "99.99")
                    {
                        distinctWords.RemoveAt(i);
                    }
                    else
                    {
                        LabVersion.Add(distinctWords[i]);

                    }

                }

                if( LabverC == "99.99")
                {
                    LabverC = "0";
                }
                if (LabverQ == "99.99")
                {
                    LabverQ = "0";
                }
                if (LabverS == "99.99")
                {
                    LabverS = "0";
                }
                if (LabverJ == "99.99")
                {
                    LabverJ = "0";
                }
                if (LabverT == "99.99")
                {
                    LabverT = "0";
                }
                if (LabverP == "99.99")
                {
                    LabverP = "0";
                }
                if (LabverPr == "99.99")
                {
                    LabverPr = "0";
                }
                if (LabverQM == "99.99")
                {
                    LabverQM = "0";
                }
                if (LabverW == "99.99")
                {
                    LabverW = "0";
                }

                //Lbl_Ver_1.Text = LabVersion[LabVersion.Count() - 1];
                //Lbl_Ver_2.Text = LabVersion[LabVersion.Count - 2];
                //Lbl_Ver_3.Text = LabVersion[LabVersion.Count - 3];
                //Lbl_Ver_4.Text = LabVersion[LabVersion.Count - 4];
                //Lbl_Ver_5.Text = LabVersion[LabVersion.Count - 5];
                //Lbl_Ver_6.Text = "...";
            }
            catch
            {

            }
            
        }

        public void SetCheckLabVersions(string LabverC)
        {
            try
            {
                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1C.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6C.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6C, LabverC);
                    }
                }
                catch
                {
                }

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1Q.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6Q.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Q, LabverC);
                    }
                }
                catch
                {
                }

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1S.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6S.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6C, LabverC);
                    }
                }
                catch
                {
                }

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1J.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6J.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6J, LabverC);
                    }
                }
                catch
                {
                }

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1T.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6T.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6T, LabverC);
                    }
                }
                catch
                {
                }

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1P.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6P.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6P, LabverC);
                    }
                }
                catch
                {
                }

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1Pr.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6Pr.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Pr, LabverC);
                    }
                }
                catch
                {
                }

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1Pr.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6QM.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Pr, LabverC);
                    }
                }
                catch
                {
                }
            }
            catch
            {
            }
        }


        public void FeatchLastVersionsFromCloud()
        {
            try
            {

                LastCloudVersion = "99.99";
                try
                {
                    if (Cbox_LastVersions.Items.Count == 0)
                    {
                        LastCloudVersion = "99.99";
                    }
                    else if (Cbox_LastVersions.Text != "" && Cbox_LastVersions.Text != "0")
                    {
                        LastCloudVersion = Cbox_LastVersions.Text;
                    }
                    if (LastVersion != "")
                    {
                        LastCloudVersion = LastVersion;
                    }
                }
                catch(Exception ex)
                {
                    UpdaterFunctions.SaveTextExeption("ERoooooor 10123:" + ex.Message.ToString());
                }


                List<double> Version = new List<double>();
                ////DTV = PWS.Get_LastVersions(Get_UserName(), Get_Password());
                //DTV = PWS.Get_LastVersions(Get_UserName(), Get_Password(), LastCloudVersion);

                DTV = GetCloudVersion(LastCloudVersion);

                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["Centeral"])); } catch { DTV.Rows[0]["Centeral"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["QC"])); } catch { DTV.Rows[0]["QC"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["Storage"])); } catch { DTV.Rows[0]["Storage"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["Journal"])); } catch { DTV.Rows[0]["Journal"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["Temperature"])); } catch { DTV.Rows[0]["Temperature"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"])); } catch { DTV.Rows[0]["ParsicLabAndroid"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["PrinterCacher"])); } catch { DTV.Rows[0]["PrinterCacher"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["QMatic"])); } catch { DTV.Rows[0]["QMatic"] = 0; Version.Add(0); }
                try { Version.Add(Convert.ToDouble(DTV.Rows[0]["Web"])); } catch { DTV.Rows[0]["Web"] = 0; Version.Add(0); }


                Version.Sort();
                LastCloudVersion = Version[Version.Count - 1].ToString();

                List<double> FirstVersions = new List<double>();

                for (int i = 0; i < Version.Count - 1; i++)
                {
                    if (Version[i].ToString() == "0")
                    {

                    }
                    else
                    {
                        FirstVersions.Add(Version[i]);
                    }
                }
                //double FLastver = Convert.ToDouble(LastCloudVersion);
                //Cbox_LastVersions.Items.Clear();
                //Cbox_LastVersions.Text = LastCloudVersion;
                //for (int i = 0; i < 30; i++)
                //{
                //    string s = "0.01";
                //    if (FLastver.ToString().Length == 4)
                //    {
                //        s = FLastver + "0";
                //    }
                //    else
                //    {
                //        s = FLastver.ToString();
                //    }
                //    Cbox_LastVersions.Items.Add(s);
                //    FLastver = FLastver - 0.01;
                //}

                string DownVer = (FirstVersions[0] - 0.03).ToString();

                FirstToLastVer = UpdaterFunctions.GetListOfExeVersion(Convert.ToDouble(DownVer));//.Tables["VerInfo"]
                try
                {
                    DTCloudVerID.Columns.Add("AppName");
                    DTCloudVerID.Columns.Add("VerID");
                }
                catch
                {
                    DTCloudVerID.Clear();
                }

                    Boolean check = false;
                    for (int i = 0; i < DTV.Columns.Count; i++)
                    {
                        string ver = DTV.Rows[0][i].ToString();
                        for (int j = 0; j < FirstToLastVer.Rows.Count - 1; j++)
                        {
                            if (FirstToLastVer.Rows[j]["VersionNumber"].ToString() == ver)
                            {
                                DTCloudVerID.Rows.Add(DTV.Columns[i], FirstToLastVer.Rows[j]["Prk_VersionRelease_AutoID"]);
                                check = true;
                                break;
                            }
                        }
                        if (check == false)
                        {
                            DTCloudVerID.Rows.Add(DTV.Columns[i], -1);

                        }
                        check = false;
                    }
                }
                catch
                {

                }

            }

        public DataTable GetCloudVersion(string LastVersion)
        {
            DataTable MyDT = new DataTable("DT");
            DataTable DTLock = new DataTable("DT");
            DataTable DTUnLock = new DataTable("DT");
            string CenteralVer = "";
            MyDT = PWS.Get_LastVersions(Get_UserName(), Get_Password(), LastVersion);
            try
            {
                DTLock = PWS.Get_LastVersions2(Get_UserName(), Get_Password(), LastVersion, 0, "99.99");
                MyDT = DTLock.Copy();
                //MyDT.Rows.Clear();
                CenteralVer = DTLock.Rows[0]["Centeral"].ToString();
                DTUnLock = PWS.Get_LastVersions2(Get_UserName(), Get_Password(), LastVersion, 1, CenteralVer);
                //MyDT = DTUnLock.Copy();
                MyDT.Rows[0]["Centeral"] = CenteralVer;
                for (int i = 1; i < DTUnLock.Columns.Count ; i++)
                {
                    if(Convert.ToDouble(CenteralVer) >= Convert.ToDouble(DTUnLock.Rows[0][i]))
                    {
                        MyDT.Rows[0][i] = DTUnLock.Rows[0][i].ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("در دریافت ورژن ها از سرور ابری مشکلی بوجود آمده است", "خطا");
            }


            return MyDT;
        }

        public void SetCheckLabVersions()
        {
            try
            {

                try
                {
                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2C.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1C.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6C.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6C, LabverC);

                    }
                }
                catch
                {

                }

                try
                {

                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2Q.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1Q.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6Q.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Q, LabverC);

                    }

                }
                catch
                {

                }

                try
                {

                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2S.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1S.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6S.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6C, LabverC);

                    }
                }
                catch
                {

                }

                try
                {

                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2J.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1J.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6J.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6J, LabverC);

                    }
                }
                catch
                {

                }

                try
                {

                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2T.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1T.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6T.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6T, LabverC);

                    }
                }
                catch
                {

                }

                try
                {

                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2P.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1P.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6P.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6P, LabverC);

                    }

                }
                catch
                {

                }

                try
                {

                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1Pr.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6Pr.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Pr, LabverC);

                    }

                }
                catch
                {

                }

                try
                {

                    if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_5Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                    {
                        Pbox_Ver_4Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                    {
                        Pbox_Ver_3Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                    {
                        Pbox_Ver_2Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                    {
                        Pbox_Ver_1Pr.Visible = true;
                    }
                    else
                    {
                        Pbox_Ver_6QM.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Pr, LabverC);

                    }

                }
                catch
                {

                }

            }
            catch
            {

            }

        }

        public void SetCheckCloudVersions()
        {
            try
            {

                if (DTV.Rows[0]["Centeral"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Centeral"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6C.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6C, DTV.Rows[0]["Centeral"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5C.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4C.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3C.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2C.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1C.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["Centeral"]))
                {
                    Chk_CentralChk.Checked = true;
                }



                if (DTV.Rows[0]["QC"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["QC"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6Q.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6Q, DTV.Rows[0]["QC"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5Q.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4Q.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3Q.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2Q.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1Q.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["QC"]))
                {
                    Chk_QCChk.Checked = true;
                }



                if (DTV.Rows[0]["Storage"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Storage"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6S.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6S, DTV.Rows[0]["Storage"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5S.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4S.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3S.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2S.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1S.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["Storage"]))
                {
                    Chk_StorageChk.Checked = true;
                }



                if (DTV.Rows[0]["Journal"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Journal"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6J.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6J, DTV.Rows[0]["Journal"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5J.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4J.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3J.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2J.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1J.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["Journal"]))
                {
                    Chk_JournalChk.Checked = true;
                }



                if (DTV.Rows[0]["Temperature"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Temperature"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6T.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6T, DTV.Rows[0]["Temperature"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5T.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4T.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3T.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2T.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1T.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["Temperature"]))
                {
                    Chk_TemperatureChk.Checked = true;
                }



                if (DTV.Rows[0]["ParsicLabAndroid"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6P.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6P, DTV.Rows[0]["ParsicLabAndroid"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5P.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4P.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3P.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2P.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1P.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]))
                {
                    Chk_ParsicLabAndroidChk.Checked = true;
                }



                if (DTV.Rows[0]["PrinterCacher"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6Pr.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6Pr, DTV.Rows[0]["PrinterCacher"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2Pr.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1Pr.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]))
                {
                    Chk_PrinterCacherChk.Checked = true;
                }


                if (DTV.Rows[0]["QMatic"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["QMatic"]) < Convert.ToDouble(Lbl_Ver5.Text))
                {
                    Pbox_Ver6QM.Visible = true;
                    toolTip1.SetToolTip(this.Pbox_Ver6QM, DTV.Rows[0]["QMatic"].ToString());

                }
                else
                {
                    if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver5QM.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver4.Text))
                    {
                        Pbox_Ver4QM.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver3.Text))
                    {
                        Pbox_Ver3QM.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver2.Text))
                    {
                        Pbox_Ver2QM.Visible = true;
                    }
                    else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver1.Text))
                    {
                        Pbox_Ver1QM.Visible = true;
                    }
                }
                if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["QMatic"]))
                {
                    //Chk_QMaticChk.Checked = true;
                }





            }
            catch
            {

            }

        }


        public void SetCheckLabCloudVersion()
        {
            try
            {
                try
                {
                    if (LabverC != "0" && Convert.ToDouble(LabverC) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6C.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6C, LabverC);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5C.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4C.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3C.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2C.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverC) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1C.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6C.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6C, LabverC);

                        }
                    }
                    if (DTV.Rows[0]["Centeral"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Centeral"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6C.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6C, DTV.Rows[0]["Centeral"].ToString());
                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5C.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4C.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3C.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2C.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Centeral"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1C.Visible = true;
                        }

                    }
                    if (Convert.ToDouble(LabverC) < Convert.ToDouble(DTV.Rows[0]["Centeral"]))
                    {
                        if (LabverC != "0")
                        {
                            Chk_CentralChk.Checked = true;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                try { 

                    if (LabverQ != "0" && Convert.ToDouble(LabverQ) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6Q.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Q, LabverQ);
                    }
                    else
                    {
                        if (Convert.ToDouble(LabverQ) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5Q.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQ) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4Q.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQ) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3Q.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQ) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2Q.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQ) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1Q.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6Q.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6Q, LabverQ);
                        }
                    }
                    if (DTV.Rows[0]["QC"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["QC"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6Q.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6Q, DTV.Rows[0]["QC"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5Q.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4Q.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3Q.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2Q.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QC"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1Q.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverQ) < Convert.ToDouble(DTV.Rows[0]["QC"]))
                    {
                        if (LabverQ != "0")
                        {
                            Chk_QCChk.Checked = true;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                try { 
                    if (LabverS != "0" && Convert.ToDouble(LabverS) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6S.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6S, LabverS);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverS) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5S.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverS) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4S.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverS) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3S.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverS) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2S.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverS) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1S.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6S.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6S, LabverS);
                        }
                    }
                    if (DTV.Rows[0]["Storage"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Storage"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6S.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6S, DTV.Rows[0]["Storage"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5S.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4S.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3S.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2S.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Storage"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1S.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverS) < Convert.ToDouble(DTV.Rows[0]["Storage"]))
                    {
                        if (LabverS != "0")
                        {
                            Chk_StorageChk.Checked = true;
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                
                try { 
                    if (LabverJ != "0" && Convert.ToDouble(LabverJ) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6J.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6J, LabverJ);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverJ) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5J.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverJ) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4J.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverJ) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3J.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverJ) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2J.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverJ) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1J.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6J.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6J, LabverJ);
                        }
                    }
                    if (DTV.Rows[0]["Journal"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Journal"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6J.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6J, DTV.Rows[0]["Journal"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5J.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4J.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3J.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2J.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Journal"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1J.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverJ) < Convert.ToDouble(DTV.Rows[0]["Journal"]))
                    {
                        if (LabverJ != "0")
                        {
                            Chk_JournalChk.Checked = true;
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                
                try { 
                    if (LabverT != "0" && Convert.ToDouble(LabverT) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6T.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6T, LabverT);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverT) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5T.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverT) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4T.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverT) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3T.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverT) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2T.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverT) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1T.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6T.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6T, LabverT);
                        }
                    }
                    if (DTV.Rows[0]["Temperature"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Temperature"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6T.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6T, DTV.Rows[0]["Temperature"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5T.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4T.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3T.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2T.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Temperature"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1T.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverT) < Convert.ToDouble(DTV.Rows[0]["Temperature"]))
                    {
                        if (LabverT != "0")
                        {
                            //Chk_TemperatureChk.Checked = true;
                        }
                    }

                }
                catch (Exception ex)
                {

                }

                try { 
                    if (LabverP != "0" && Convert.ToDouble(LabverP) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6P.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6P, LabverP);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverP) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5P.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverP) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4P.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverP) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3P.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverP) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2P.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverP) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1P.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6P.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6P, LabverP);
                        }
                    }
                    if (DTV.Rows[0]["ParsicLabAndroid"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6P.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6P, DTV.Rows[0]["ParsicLabAndroid"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5P.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4P.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3P.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2P.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1P.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverP) < Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]))
                    {
                        string Path = "";
                        Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                        System.IO.DirectoryInfo di = new DirectoryInfo(Path);
                        foreach (DirectoryInfo d in di.GetDirectories())
                        {
                            try
                            {
                                if (d.Name.Contains("ParsicLabAndroid"))
                                {
                                    IsFromIIS = true;
                                    Chk_ParsicLabAndroidChk.Checked = true;

                                    break;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if(Chk_ParsicLabAndroidChk.Checked == true)
                        {
                            DataTable LabInfo = new DataTable();
                            LabInfo = UpdaterFunctions.GetCloudLabInfo(Convert.ToInt32(LabID));
                            if (LabInfo.Rows.Count == 0)
                            {
                                Chk_ParsicLabAndroidChk.Checked = false;
                                UpdaterFunctions.SaveTextExeption("اطلاعات ابری برای لب اندروید یافت نشد");
                            }
                            else
                            {
                                string IISLabAndroidURL = LabInfo.Rows[0]["Str_ServiceURL"].ToString();
                                if(IISLabAndroidURL == "")
                                {
                                    Chk_ParsicLabAndroidChk.Checked = false;
                                    UpdaterFunctions.SaveTextExeption("آدرس لب اندروید در تیکتینگ وجود ندارد");
                                }
                                else
                                {
                                    string MyPort = UpdaterFunctions.FindPort(IISLabAndroidURL);
                                    if (MyPort == "")
                                    {
                                        UpdaterFunctions.SaveTextExeption("پورتی برای لب اندروید یافت نشد، فرمت آدرس لب اندروید در تیکتینگ را چک نمایید");
                                    }
                                    string MySiteName = UpdaterFunctions.FindIISSiteName(MyPort);
                                    if (MySiteName == "")
                                    {
                                        Chk_ParsicLabAndroidChk.Checked = false;
                                        UpdaterFunctions.SaveTextExeption("نام سایت با پورت مورد نظر یافت نشد\r\nPort = " + MyPort);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                try { 
                    if (LabverPr != "0" && Convert.ToDouble(LabverPr) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6Pr.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6Pr, LabverPr);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverPr) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverPr) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverPr) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverPr) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverPr) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1Pr.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6Pr.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6Pr, LabverPr);
                        }
                    }
                    if (DTV.Rows[0]["PrinterCacher"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6Pr.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6Pr, DTV.Rows[0]["PrinterCacher"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2Pr.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1Pr.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverPr) < Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]))
                {
                        if (LabverPr != "0")
                        {
                            Chk_PrinterCacherChk.Checked = true;
                        }
                }


                }
                catch (Exception ex)
                {

                }
                try { 

                    if (LabverQM != "0" && Convert.ToDouble(LabverQM) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6QM.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6QM, LabverQM);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverQM) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5QM.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQM) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4QM.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQM) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3QM.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQM) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2QM.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverQM) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1QM.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6QM.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6QM, LabverQM);
                        }

                    }
                    if (DTV.Rows[0]["QMatic"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["QMatic"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6QM.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6QM, DTV.Rows[0]["QMatic"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5QM.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4QM.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3QM.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2QM.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["QMatic"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1QM.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverQM) < Convert.ToDouble(DTV.Rows[0]["QMatic"]))
                    {
                        if(LabverQM != "0")
                        {
                            //Chk_QMaticChk.Checked = true;
                        }
                    }


                }
                catch (Exception ex)
                {

                }
                try
                {
                    if (LabverW != "0" && Convert.ToDouble(LabverW) < Convert.ToDouble(Lbl_Ver_5.Text))
                    {
                        Pbox_Ver_6W.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver_6W, LabverW);

                    }
                    else
                    {
                        if (Convert.ToDouble(LabverW) == Convert.ToDouble(Lbl_Ver_5.Text))
                        {
                            Pbox_Ver_5W.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverW) == Convert.ToDouble(Lbl_Ver_4.Text))
                        {
                            Pbox_Ver_4W.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverW) == Convert.ToDouble(Lbl_Ver_3.Text))
                        {
                            Pbox_Ver_3W.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverW) == Convert.ToDouble(Lbl_Ver_2.Text))
                        {
                            Pbox_Ver_2W.Visible = true;
                        }
                        else if (Convert.ToDouble(LabverW) == Convert.ToDouble(Lbl_Ver_1.Text))
                        {
                            Pbox_Ver_1W.Visible = true;
                        }
                        else
                        {
                            Pbox_Ver_6W.Visible = true;
                            toolTip1.SetToolTip(this.Pbox_Ver_6W, LabverW);
                        }

                    }
                    if (DTV.Rows[0]["Web"].ToString() != "0" && Convert.ToDouble(DTV.Rows[0]["Web"]) < Convert.ToDouble(Lbl_Ver5.Text))
                    {
                        Pbox_Ver6W.Visible = true;
                        toolTip1.SetToolTip(this.Pbox_Ver6W, DTV.Rows[0]["Web"].ToString());

                    }
                    else
                    {
                        if (Convert.ToDouble(DTV.Rows[0]["Web"]) == Convert.ToDouble(Lbl_Ver5.Text))
                        {
                            Pbox_Ver5W.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Web"]) == Convert.ToDouble(Lbl_Ver4.Text))
                        {
                            Pbox_Ver4W.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Web"]) == Convert.ToDouble(Lbl_Ver3.Text))
                        {
                            Pbox_Ver3W.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Web"]) == Convert.ToDouble(Lbl_Ver2.Text))
                        {
                            Pbox_Ver2W.Visible = true;
                        }
                        else if (Convert.ToDouble(DTV.Rows[0]["Web"]) == Convert.ToDouble(Lbl_Ver1.Text))
                        {
                            Pbox_Ver1W.Visible = true;
                        }
                    }
                    if (Convert.ToDouble(LabverW) < Convert.ToDouble(DTV.Rows[0]["Web"]))
                    {
                        string Path = "";
                        Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                        System.IO.DirectoryInfo di = new DirectoryInfo(Path);
                        foreach (DirectoryInfo d in di.GetDirectories())
                        {
                            try
                            {
                                if (d.Name.Contains("LabOnline"))
                                {
                                    IsFromIIS = true;
                                    Chk_WebChk.Checked = true;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (Chk_WebChk.Checked == true)
                        {
                            DataTable LabInfo = new DataTable();
                            LabInfo = UpdaterFunctions.GetCloudLabInfo(Convert.ToInt32(LabID));
                            if (LabInfo.Rows.Count == 0)
                            {
                                Chk_WebChk.Checked = false;
                                UpdaterFunctions.SaveTextExeption("اطلاعات ابری برای لب آنلاین یافت نشد");
                            }
                            else
                            {
                                string IISLabOnlineURL = LabInfo.Rows[0]["Str_WebSiteAddress"].ToString();
                                if (IISLabOnlineURL == "")
                                {
                                    Chk_WebChk.Checked = false;
                                    UpdaterFunctions.SaveTextExeption("آدرس لب آنلاین در تیکتینگ وجود ندارد");
                                }
                                else
                                {
                                    string MyPort = UpdaterFunctions.FindPort(IISLabOnlineURL);
                                    if(MyPort == "")
                                    {
                                        UpdaterFunctions.SaveTextExeption("پورتی برای لب آنلاین یافت نشد، فرمت آدرس لب آنلاین در تیکتینگ را چک نمایید");
                                    }

                                    string MySiteName = UpdaterFunctions.FindIISSiteName(MyPort);
                                    if (MySiteName == "")
                                    {
                                        Chk_WebChk.Checked = false;
                                        UpdaterFunctions.SaveTextExeption("نام سایت با پورت مورد نظر یافت نشد\r\nPort = " + MyPort);
                                    }
                                }
                            }


                        }
                    }

                    

                }
                catch (Exception ex)
                {

                }

                if (Chk_Lock.Checked)
                {
                    double CloudCVer = Convert.ToDouble(DTV.Rows[0]["Centeral"]);
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["QC"]))
                    {
                        Chk_QCChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["Storage"]))
                    {
                        Chk_StorageChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["Journal"]))
                    {
                        Chk_JournalChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["Temperature"]))
                    {
                        Chk_TemperatureChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]))
                    {
                        Chk_ParsicLabAndroidChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["ParsicLabAndroid"]))
                    {
                        Chk_ParsicLabAndroidChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["QMatic"]))
                    {
                        Chk_QMaticChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["PrinterCacher"]))
                    {
                        Chk_PrinterCacherChk.Checked = false;
                    }
                    if (Convert.ToDouble(CloudCVer) < Convert.ToDouble(DTV.Rows[0]["web"]))
                    {
                        Chk_WebChk.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
            }


        }

        public void SetChecksInFormForLabAndCloudVersions()
        {
            try
                {
                //Set Versions Lable
                Lbl_Ver1.Text = LastCloudVersion;
                if ((Convert.ToDouble(LastCloudVersion)).ToString().Length == 5) { Lbl_Ver1.Text = (Convert.ToDouble(LastCloudVersion)).ToString(); } else { Lbl_Ver1.Text = (Convert.ToDouble(LastCloudVersion)).ToString() + "0"; }
                if ((Convert.ToDouble(LastCloudVersion) - 00.01).ToString().Length == 5) { Lbl_Ver2.Text = (Convert.ToDouble(LastCloudVersion) - 00.01).ToString(); } else { Lbl_Ver2.Text = (Convert.ToDouble(LastCloudVersion) - 00.01).ToString() + "0"; }
                if ((Convert.ToDouble(LastCloudVersion) - 00.02).ToString().Length == 5) { Lbl_Ver3.Text = (Convert.ToDouble(LastCloudVersion) - 00.02).ToString(); } else { Lbl_Ver3.Text = (Convert.ToDouble(LastCloudVersion) - 00.02).ToString() + "0"; }
                if ((Convert.ToDouble(LastCloudVersion) - 00.03).ToString().Length == 5) { Lbl_Ver4.Text = (Convert.ToDouble(LastCloudVersion) - 00.03).ToString(); } else { Lbl_Ver4.Text = (Convert.ToDouble(LastCloudVersion) - 00.03).ToString() + "0"; }
                if ((Convert.ToDouble(LastCloudVersion) - 00.04).ToString().Length == 5) { Lbl_Ver5.Text = (Convert.ToDouble(LastCloudVersion) - 00.04).ToString(); } else { Lbl_Ver5.Text = (Convert.ToDouble(LastCloudVersion) - 00.04).ToString() + "0"; }
                Lbl_Ver6.Text = "...";
                double LastVer = 0.01;
                try
                {
                    if (LabVersion[LabVersion.Count - 1].ToString().Length == 4) { Lbl_Ver_1.Text = LabVersion[LabVersion.Count - 1].ToString() + "0"; } else { Lbl_Ver_1.Text = LabVersion[LabVersion.Count - 1].ToString(); };
                    LastVer = Convert.ToDouble(LabVersion[LabVersion.Count - 1]);
                }
                catch
                {

                }

                try
                {
                    if (LabVersion[LabVersion.Count - 2].ToString().Length == 4) { Lbl_Ver_2.Text = LabVersion[LabVersion.Count - 2].ToString() + "0"; } else { Lbl_Ver_2.Text = LabVersion[LabVersion.Count - 2].ToString(); };
                    LastVer = Convert.ToDouble(LabVersion[LabVersion.Count - 2]);
                }
                catch
                {
                    LastVer = LastVer - 0.01;
                    Lbl_Ver_2.Text = (LastVer).ToString();
                }



                try
                {
                    if (LabVersion[LabVersion.Count - 3].ToString().Length == 4) { Lbl_Ver_3.Text = LabVersion[LabVersion.Count - 3].ToString() + "0"; } else { Lbl_Ver_3.Text = LabVersion[LabVersion.Count - 3].ToString(); };
                    LastVer = Convert.ToDouble(LabVersion[LabVersion.Count - 3]);

                }
                catch
                {
                    LastVer = LastVer - 0.01;
                    Lbl_Ver_3.Text = (LastVer).ToString();
                }


                try
                {
                    if (LabVersion[LabVersion.Count - 4].ToString().Length == 4) { Lbl_Ver_4.Text = LabVersion[LabVersion.Count - 4].ToString() + "0"; } else { Lbl_Ver_4.Text = LabVersion[LabVersion.Count - 4].ToString(); };
                    LastVer = Convert.ToDouble(LabVersion[LabVersion.Count - 4]);
                }
                catch
                {
                    LastVer = LastVer - 0.01;
                    Lbl_Ver_4.Text = (LastVer).ToString();
                }

                try
                {
                    if (LabVersion[LabVersion.Count - 5].ToString().Length == 4) { Lbl_Ver_5.Text = LabVersion[LabVersion.Count - 5].ToString() + "0"; } else { Lbl_Ver_5.Text = LabVersion[LabVersion.Count - 5].ToString(); };
                    LastVer = Convert.ToDouble(LabVersion[LabVersion.Count - 5]);
                }
                catch
                {
                    LastVer = LastVer - 0.01;
                    Lbl_Ver_5.Text = (LastVer).ToString();
                }

                Lbl_Ver_6.Text = "...";
            }
            catch(Exception ex)
            {
                UpdaterFunctions.SaveTextExeption("Error : " + ex.Message.ToString());
            }
          

            //if (LabVersion[5].ToString().Length == 4) { Lbl_Ver_6.Text = LabVersion[5].ToString() + "0"; } else { Lbl_Ver_6.Text = LabVersion[5].ToString(); };
            //Set Versions Lable\

            try
            {
                Clear();
                if (Convert.ToDouble(LabverC) < 11.21) //////11.21
                {
                    //SetCheckLabVersions();

                    //SetCheckCloudVersions();
                    SetCheckLabCloudVersion();

                }
                else
                {
                    SetCheckLabCloudVersion();
                }
            }
            catch
            {

            }

        }
        public void Clear()
        {
            try
            {

                Chk_CentralChk.Checked = false;
                Chk_QCChk.Checked = false;
                Chk_StorageChk.Checked = false;
                Chk_JournalChk.Checked = false;
                Chk_TemperatureChk.Checked = false;
                Chk_ParsicLabAndroidChk.Checked = false;
                Chk_PrinterCacherChk.Checked = false;
                Chk_QMaticChk.Checked = false;
                Chk_WebChk.Checked = false;

                Pbox_Ver_1C.Visible = false;
                Pbox_Ver_2C.Visible = false;
                Pbox_Ver_3C.Visible = false;
                Pbox_Ver_4C.Visible = false;
                Pbox_Ver_5C.Visible = false;
                Pbox_Ver_6C.Visible = false;

                Pbox_Ver_1Q.Visible = false;
                Pbox_Ver_2Q.Visible = false;
                Pbox_Ver_3Q.Visible = false;
                Pbox_Ver_4Q.Visible = false;
                Pbox_Ver_5Q.Visible = false;
                Pbox_Ver_6Q.Visible = false;

                Pbox_Ver_1S.Visible = false;
                Pbox_Ver_2S.Visible = false;
                Pbox_Ver_3S.Visible = false;
                Pbox_Ver_4S.Visible = false;
                Pbox_Ver_5S.Visible = false;
                Pbox_Ver_6S.Visible = false;

                Pbox_Ver_1J.Visible = false;
                Pbox_Ver_2J.Visible = false;
                Pbox_Ver_3J.Visible = false;
                Pbox_Ver_4J.Visible = false;
                Pbox_Ver_5J.Visible = false;
                Pbox_Ver_6J.Visible = false;

                Pbox_Ver_1T.Visible = false;
                Pbox_Ver_2T.Visible = false;
                Pbox_Ver_3T.Visible = false;
                Pbox_Ver_4T.Visible = false;
                Pbox_Ver_5T.Visible = false;
                Pbox_Ver_6T.Visible = false;

                Pbox_Ver_1P.Visible = false;
                Pbox_Ver_2P.Visible = false;
                Pbox_Ver_3P.Visible = false;
                Pbox_Ver_4P.Visible = false;
                Pbox_Ver_5P.Visible = false;
                Pbox_Ver_6P.Visible = false;

                Pbox_Ver_1Pr.Visible = false;
                Pbox_Ver_2Pr.Visible = false;
                Pbox_Ver_3Pr.Visible = false;
                Pbox_Ver_4Pr.Visible = false;
                Pbox_Ver_5Pr.Visible = false;
                Pbox_Ver_6Pr.Visible = false;

                Pbox_Ver_1QM.Visible = false;
                Pbox_Ver_2QM.Visible = false;
                Pbox_Ver_3QM.Visible = false;
                Pbox_Ver_4QM.Visible = false;
                Pbox_Ver_5QM.Visible = false;
                Pbox_Ver_6QM.Visible = false;

                Pbox_Ver_1W.Visible = false;
                Pbox_Ver_2W.Visible = false;
                Pbox_Ver_3W.Visible = false;
                Pbox_Ver_4W.Visible = false;
                Pbox_Ver_5W.Visible = false;
                Pbox_Ver_6W.Visible = false;






                Pbox_Ver1C.Visible = false;
                Pbox_Ver2C.Visible = false;
                Pbox_Ver3C.Visible = false;
                Pbox_Ver4C.Visible = false;
                Pbox_Ver5C.Visible = false;
                Pbox_Ver6C.Visible = false;

                Pbox_Ver1Q.Visible = false;
                Pbox_Ver2Q.Visible = false;
                Pbox_Ver3Q.Visible = false;
                Pbox_Ver4Q.Visible = false;
                Pbox_Ver5Q.Visible = false;
                Pbox_Ver6Q.Visible = false;

                Pbox_Ver1S.Visible = false;
                Pbox_Ver2S.Visible = false;
                Pbox_Ver3S.Visible = false;
                Pbox_Ver4S.Visible = false;
                Pbox_Ver5S.Visible = false;
                Pbox_Ver6S.Visible = false;

                Pbox_Ver1J.Visible = false;
                Pbox_Ver2J.Visible = false;
                Pbox_Ver3J.Visible = false;
                Pbox_Ver4J.Visible = false;
                Pbox_Ver5J.Visible = false;
                Pbox_Ver6J.Visible = false;

                Pbox_Ver1T.Visible = false;
                Pbox_Ver2T.Visible = false;
                Pbox_Ver3T.Visible = false;
                Pbox_Ver4T.Visible = false;
                Pbox_Ver5T.Visible = false;
                Pbox_Ver6T.Visible = false;

                Pbox_Ver1P.Visible = false;
                Pbox_Ver2P.Visible = false;
                Pbox_Ver3P.Visible = false;
                Pbox_Ver4P.Visible = false;
                Pbox_Ver5P.Visible = false;
                Pbox_Ver6P.Visible = false;

                Pbox_Ver1Pr.Visible = false;
                Pbox_Ver2Pr.Visible = false;
                Pbox_Ver3Pr.Visible = false;
                Pbox_Ver4Pr.Visible = false;
                Pbox_Ver5Pr.Visible = false;
                Pbox_Ver6Pr.Visible = false;

                Pbox_Ver1QM.Visible = false;
                Pbox_Ver2QM.Visible = false;
                Pbox_Ver3QM.Visible = false;
                Pbox_Ver4QM.Visible = false;
                Pbox_Ver5QM.Visible = false;
                Pbox_Ver6QM.Visible = false;

                Pbox_Ver1W.Visible = false;
                Pbox_Ver2W.Visible = false;
                Pbox_Ver3W.Visible = false;
                Pbox_Ver4W.Visible = false;
                Pbox_Ver5W.Visible = false;
                Pbox_Ver6W.Visible = false;
            }
            catch
            {

            }
        }
        public void Docheck()
        {
            DTV = new DataTable("Versions");


            //ListOfExeFile
            ListOfExeFile();
            //ListOfExeFile\


            //Find Lab Version
            FindLabVersions();
            //Find Lab Version\



            //FeatchLastVersionsFromCloud\
            FeatchLastVersionsFromCloud();
            //FeatchLastVersionsFromCloud\




            //SetChecksInFormForLabAndCloudVersions
            SetChecksInFormForLabAndCloudVersions();
            //SetChecksInFormForLabAndCloudVersions\

        }

        private void Cbox_LastVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            Docheck();
        }

        private void Btn_DiscSpace_Click(object sender, EventArgs e)
        {

            string Space = "";
            var drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                try
                {
                    DriveInfo dDrive = new DriveInfo(drive);
                    if (dDrive.IsReady)
                    {
                        // Calculate the percentage free space
                        string freeSpacePerc = (Convert.ToInt32((dDrive.AvailableFreeSpace / (float)dDrive.TotalSize) * 100)).ToString();
                        // Calculate the percentage free space \
                        if (freeSpacePerc.Length == 1)
                        {
                            freeSpacePerc += "0";
                        }
                        Space += "Name : " + dDrive.Name + "     Format : " + dDrive.DriveFormat + "     Total : " + (((((float)dDrive.TotalSize)/1024)/1024)/1024) + "     Available : " + (((((float)dDrive.AvailableFreeSpace) / 1024) / 1024) / 1024) + "     Free Space percentage : " + freeSpacePerc + "%" + "\r\n";
                    }
                }
                catch (Exception ex)
                {
                }
            }
            Frm_MessageBox MyMessagebox = new Frm_MessageBox(Space, "اطلاعات فضای ذخیره سازی");
            MyMessagebox.ShowDialog();
        }

        private void Chk_Lock_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }
    }
}

