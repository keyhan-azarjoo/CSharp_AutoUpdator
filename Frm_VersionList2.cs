
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using UpdaterClasses;

    namespace ParsicAutoUpdater
{



    public partial class Frm_VersionList2 : Form
        {
        //╔═══════════════════ Variabel ═════════════════════╗
        //║                                                  ║
        //║                                                  ║
#region "Variabel"


        string connection = "";
        string ComputerName = "";
        string DB_ServerName = "";
        string DB_Name = "";
        string DB_Username = "";
        string DB_Password = "";
        string LabID = "";
        string _Parsicuserid = "";
        string VersionNumberC, VersionNumberQ, VersionNumberS, VersionNumberJ, VersionNumberT, VersionNumberP, VersionNumberPr, VersionNumberQM, VersionNumberW = "0";

        String VersionNumbers = "";
        string From;
        string To;
        string CenteralVersion = "";
        string DbEXEVersion = "";
        string ScriptVersion = "";
        string ConfermerPerson = "";
        string ReceivedInfo = "";
        string IISLabAndroidURL = "";
        string IISLabOnlineURL = "";

        Boolean Rollback = false;
        Boolean _Chk_Centeral = false;
        Boolean _Chk_QC = false;
        Boolean _Chk_Storage = false;
        Boolean _Chk_Journal = false;
        Boolean _Chk_Temperature = false;
        Boolean _Chk_ParsicLabAndroid = false;
        Boolean _Chk_PrinterCacher = false;
        Boolean _Chk_QMatic = false;
        Boolean _Chk_Web = false;
        Boolean Exe = false;
        Boolean Scripts = false;
        Boolean ShowForm = false;
        Boolean TherIsTruDb = false;
        Boolean WithBackup = false;
        Boolean ReplaceParsicLab = false;

        Int32 CenteralVerID = -1;
        Int32 QCVerID = -1;
        Int32 StorageVerID = -1;
        Int32 JournalVerID = -1;
        Int32 TemperatureVerID = -1;
        Int32 ParsicLabAndroidVerID = -1;
        Int32 PrinterCacherVerID = -1;
        Int32 QMaticVerID = -1;
        Int32 WebVerID = -1;

        Int32 TextStartIndexError = 0;
        Int32 TextStartIndexRollBack = 0;
        Int32 TextStartIndexCommit = 0;
        Int32 TextStartIndexWarning = 0;
        int Parsicuserid = 0;
        Int32 VersionRelease_ID = 0;
        double DBversion = 10.01;
        double DB_version = 10.01;
        double _From;

        DataSet DS;
        DataTable Dt;
        DataTable DTParsicMaster;
        DataTable DtScripts;
        DataTable DtDescriptions;

        GetAndInsertVersionInDB UpdaterFunctions;

        List<string> GetODBCInfoList;

        private bool user_is_from_Parsic;
        public delegate void SetprogressBar1(int Num);
        public delegate void SetMaxprogressBar1(int Num);
        public delegate void AddprogressBar1(int Num);
        public delegate void SetLbl_CenteralVersion(string Text);
        public delegate void SetLbl_DbEXEVersion(string Text);
        public delegate void SetLbl_ScriptVersion(string Text);
        public delegate void SetFrmHiden(Boolean IsHiden);

        Thread thread;

        public delegate void TxtError(string Text);
        public delegate void TxtExeError(string Text);
        public delegate void SetTxt_EventLog(string Text);
        public delegate void SetChangeRollBackExitButtonVisible(Boolean b);
        public delegate void MyDelegate(string Text);

        MyDelegate MyDL;

        #endregion
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝


        //╔═══════════════════ Load ═════════════════════════╗
        //║                                                  ║
        //║                                                  ║
#region "Load"


        public Frm_VersionList2(string _Info, string _ConfermerPerson, Boolean _Show)
        {
            if (_Show == false)
            {
                this.Hide();
                this.HidenThis(true);
            }
            ReceivedInfo = _Info;
            ConfermerPerson = _ConfermerPerson;
            DS = new DataSet();
            Dt = new DataTable("Dt");
            DTParsicMaster = new DataTable("DTParsicMaster");
            DtScripts = new DataTable("DtScripts");
            DtDescriptions = new DataTable("DtDescriptions");

            InitializeComponent();

            ShowForm = _Show;

            string Info = _Info.Replace("#@!", " ");

            String[] MyCommand = Info.Split(new String[] { "!@#" }, StringSplitOptions.None);
            _Parsicuserid = MyCommand[0].Replace("UserId#", "");
            VersionRelease_ID = Convert.ToInt32(MyCommand[1].Replace("VerID#", ""));
            VersionNumbers = MyCommand[2].Replace("VerNum#", "");
            string[] vers = new string[10];
            vers = VersionNumbers.Split(',');
            try
            {
                VersionNumberC = vers[0];
                VersionNumberQ = vers[1];
                VersionNumberS = vers[2];
                VersionNumberJ = vers[3];
                VersionNumberT = vers[4];
                VersionNumberP = vers[5];
                VersionNumberPr = vers[6];
                VersionNumberQM = vers[7];
                VersionNumberW = vers[8];

            }
            catch
            {

            }


            UpdaterFunctions = new GetAndInsertVersionInDB(Convert.ToInt32(_Parsicuserid));
            UpdaterFunctions.SaveTextExeption(" Start _ Info : " + _Info);
            ComputerName = System.Environment.MachineName.ToString();
            try
            {
                GetODBCInfoList = new List<string>();
                GetODBCInfoList.Add(MyCommand[3].Replace("Server#", "")); //InstanceName
                GetODBCInfoList.Add(MyCommand[4].Replace("DbName#", "")); // DBName
                GetODBCInfoList.Add(MyCommand[5].Replace("TblName#", "")); // TableName
                GetODBCInfoList.Add(MyCommand[6].Replace("User#", "")); // UserName
                GetODBCInfoList.Add(MyCommand[7].Replace("Pass#", "")); // Password
                if (MyCommand[8].Replace("Exe#", "") == "1") { Exe = true; Chk_Exe.Checked = true; } else { Exe = false; Chk_Exe.Checked = false; }
                if (MyCommand[9].Replace("Scripts#", "") == "1") { Scripts = true; Chk_Scripts.Checked = true; } else { Scripts = false; Chk_Scripts.Checked = false; }
                if (MyCommand[10].Replace("Central#", "") == "1") { _Chk_Centeral = true; } else { _Chk_Centeral = false; }
                if (MyCommand[11].Replace("QC#", "") == "1") { _Chk_QC = true; } else { _Chk_QC = false; }
                if (MyCommand[12].Replace("Storage#", "") == "1") { _Chk_Storage = true; } else { _Chk_Storage = false; }
                if (MyCommand[13].Replace("Jurnul#", "") == "1") { _Chk_Journal = true; } else { _Chk_Journal = false; }
                if (MyCommand[14].Replace("Temperature#", "") == "1") { _Chk_Temperature = true; } else { _Chk_Temperature = false; }
                if (MyCommand[15].Replace("ParsicLabAndroid#", "") == "1") { _Chk_ParsicLabAndroid = true; } else { _Chk_ParsicLabAndroid = false; }
                if (MyCommand[16].Replace("PrinterCacher#", "") == "1") { _Chk_PrinterCacher = true; } else { _Chk_PrinterCacher = false; }
                if (MyCommand[17].Replace("QMatic#", "") == "1") { _Chk_QMatic = true; } else { _Chk_QMatic = false; }
                if (MyCommand[18].Replace("Web#", "") == "1") { _Chk_Web = true; } else { _Chk_Web = false; }



                if (MyCommand[19].Replace("BackUp#", "") == "1") { WithBackup = true; } else { WithBackup = false; }
                CenteralVerID = Convert.ToInt32(MyCommand[20].Replace("CentralVer#", ""));
                QCVerID = Convert.ToInt32(MyCommand[21].Replace("QCVer#", ""));
                StorageVerID = Convert.ToInt32(MyCommand[22].Replace("StorageVer#", ""));
                JournalVerID = Convert.ToInt32(MyCommand[23].Replace("JournalVer#", ""));
                TemperatureVerID = Convert.ToInt32(MyCommand[24].Replace("TemperatureVer#", ""));
                ParsicLabAndroidVerID = Convert.ToInt32(MyCommand[25].Replace("ParsicLabAndroidVer#", ""));
                PrinterCacherVerID = Convert.ToInt32(MyCommand[26].Replace("PrinterCacherVer#", ""));
                QMaticVerID = Convert.ToInt32(MyCommand[27].Replace("QMaticVer#", ""));
                WebVerID = Convert.ToInt32(MyCommand[28].Replace("WebVer#", ""));

                if (_Chk_Centeral == false) { CenteralVerID = -1; }
                if (_Chk_QC == false) { QCVerID = -1; }
                if (_Chk_Storage == false) { StorageVerID = -1; }
                if (_Chk_Journal == false) { JournalVerID = -1; }
                if (_Chk_Temperature == false) { TemperatureVerID = -1; }
                if (_Chk_ParsicLabAndroid == false) { ParsicLabAndroidVerID = -1; }
                if (_Chk_PrinterCacher == false) { PrinterCacherVerID = -1; }
                if (_Chk_QMatic == false) {QMaticVerID = -1; }
                if (_Chk_Web == false) { WebVerID = -1; }

                //GetODBCInfoList = UpdaterFunctions.GetODBCInformation();

                DTParsicMaster = UpdaterFunctions.UpdateGridView(GetODBCInfoList);

                LabID = UpdaterFunctions.GetLabID(DTParsicMaster);

                try
                {
                    foreach (DataRow dr in DTParsicMaster.Rows)
                    {
                        if((dr["Is_Present"].ToString() == "True"))
                        {
                            if ((dr["DBList_IsActive"].ToString() == "True"))
                            {
                                DB_ServerName = dr["DBList_Server"].ToString();
                                DB_Name = dr["DBList_Name"].ToString();
                                DB_Username = dr["DBList_Username"].ToString();
                                DB_Password = dr["DBList_Password"].ToString();
                                DBversion = Convert.ToDouble(dr["DBCurruntVersionC"]);
                                TherIsTruDb = true;
                                connection = UpdaterFunctions.MakeConnectionString(DB_ServerName, DB_Name, DB_Username, DB_Password, 10);

                            }
                            else
                            {
                                //DB_ServerName = dr["DBList_Server"].ToString();
                                //DB_Name = dr["DBList_Name"].ToString();
                                //DB_Username = dr["DBList_Username"].ToString();
                                //DB_Password = dr["DBList_Password"].ToString();
                                //string connnn = UpdaterFunctions.MakeConnectionString(DB_ServerName, DB_Name, DB_Username, DB_Password, 5);
                                //UpdaterFunctions.SaveTextExeption("Sub requirement : DB = " + DB_ServerName + "     " + Prerequirement(Convert.ToInt32(_Parsicuserid), connnn, 1));
                            }
                        }
                        
                    }
                    try
                    {

                    
                    UpdaterFunctions.TruncateVersionReleaseLogsandSubLogs(connection);
                    UpdaterFunctions.SaveTextExeption("Connection  : " + connection);
                    //UpdaterFunctions.SaveTextExeption("requirement : " + Prerequirement(Convert.ToInt32(_Parsicuserid), connection, 0));
                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Convert.ToInt32(_Parsicuserid), "", VersionNumberC, "Start", "EXE _ Info : " + Info.Replace("!@#", " ").Replace("#", ":"), -1, 0, ComputerName, "", false, true, 0, connection, false, Convert.ToInt32(LabID));
                    }
                    catch (Exception ex3)
                    {

                    }
                }
                catch (Exception EX)
                {
                    //MessageBox.Show("Error 1 : " + EX.Message.ToString());
                    UpdaterFunctions.SaveTextExeption("Error In Find DB_ParsicMaster or execute requirement : " + EX.Message.ToString());
                }

                try
                {
                    SqlConnection co = new SqlConnection(connection);
                    try
                    {
                        co.Open();
                        co.Close();
                    }
                    catch
                    {
                        UpdaterFunctions.SaveTextExeption("Error In ConnectionString, Pleas Check IIS HashCode");
                        if (ShowForm)
                        {
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                }
                catch
                {
                }
                try
                {
                    From = DTParsicMaster.Rows[0]["DBCurruntVersionC"].ToString();
                    _From = Convert.ToDouble(From);
                    To = VersionNumberC;
                    user_is_from_Parsic = IsFromLab();
                    if (Exe)
                    {
                        //int VersionRelease_ID = Convert.ToInt16(Dt.Rows[Dg_VersionList.CurrentRow.Index]["Prk_VersionRelease_AutoID"]);
                        if (UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(_Parsicuserid),0, 0, " فایل ") == false)
                        {
                            UpdaterFunctions.SaveTextExeption("Error in Send Log to Cloud");
                            SetTxt_EXeEvent("Error in Send Log to Cloud");
                            SetTxt_EXeEvent(" Operation Finished");
                            if (ShowForm)
                            {
                                ChangeRollBackExitButtonVisible(true);
                                MessageBox.Show("ارور در ارسال لاگ به ابر ها", "ERROR");
                            }
                            else
                            {
                                Application.Exit();
                            }
                        }
                    }
                }
                catch
                {
                }
                if (TherIsTruDb == true)
                {
                }
                else
                {
                    SetTxt_EXeEvent("بانک فعالی وجود ندارد");
                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Convert.ToInt32(_Parsicuserid), "", VersionNumberC, "Error", "برای این کار حتما باید یک بانک فعال وجود داشته باشد ", -1, 1, ComputerName, "", false, true, 0, connection, false, Convert.ToInt32(LabID));
                    SetTxt_EXeEvent("Operation Finished");
                    if (ShowForm)
                    {
                        ChangeRollBackExitButtonVisible(true);
                        MessageBox.Show("بانک فعالی وجود ندارد", "ERROR");
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
            catch (Exception EX)
            {
                //MessageBox.Show("خطا در ارتباط با بانک پارسیک مستر برای آپدیت اتوماتیک" + "\r\n" + "لطفا با شرکت پارسیپل تماس و اطلاع دهید که در فرایند ورژن زدن مشکلی بوجود آمده است" + "021-44804472");
                UpdaterFunctions.SaveTextExeption("خطا در ارتباط با بانک پارسیک مستر برای آپدیت اتوماتیک");
                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                try
                {
                    SetTxt_EXeEvent("Operation Finished");
                    if (ShowForm)
                    {
                        ChangeRollBackExitButtonVisible(true);
                        MessageBox.Show("خطا در ارتباط با بانک پارسیک مستر برای آپدیت اتوماتیک، او دی بی سی را چک نمایید", "ERROR");
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
                catch
                {
                }
                Application.Exit();
            }
            Parsicuserid = Convert.ToInt32(_Parsicuserid);
            //DS = _DS;
        }

        public Frm_VersionList2()
        {
            InitializeComponent();
        }

        private void Frm_VersionList2_Load(object sender, EventArgs e)
        {
            if (ShowForm == false)
            {
                this.Hide();
                this.HidenThis(true);
                
            }
                
            Chk_Centeral.Checked = _Chk_Centeral;
            Chk_QC.Checked = _Chk_QC;
            Chk_Storage.Checked = _Chk_Storage;
            Chk_Journal.Checked = _Chk_Journal;
            Chk_Temperature.Checked = _Chk_Temperature;
            Chk_ParsicLabAndroid.Checked = _Chk_ParsicLabAndroid;
            Chk_PrinterCacher.Checked = _Chk_PrinterCacher;
            Chk_QMatic.Checked = _Chk_QMatic;
            Chk_Web.Checked = _Chk_Web;

            //MessageBox.Show("0\r\n" + args[1] + "\r\n" + args[2] + "\r\n" + args[3] + "\r\n" + args[4] + "\r\n" + args[5] + "\r\n" + args[6] + "\r\n" + args[7] + "\r\n" + args[8] + "\r\n" + args[9] + "\r\n" + args[10] + "\r\n" + args[11] + "\r\n" + args[12] + "\r\n" + args[13] + "\r\n" + args[14] + "\r\n");

            string CentralPath = UpdaterFunctions.FindFolder("Centeral");

            Lbl_CenteralVersion.Text = UpdaterFunctions.GetVersion(CentralPath + "\\CenteralApp.exe");
            Lbl_DbVersion.Text = UpdaterFunctions.GetDBVersion("Centeral", connection);
            Lbl_ScriptVersion.Text = UpdaterFunctions.GetDBScriptVersion(connection,1);
            SetCenteralVersion(UpdaterFunctions.GetVersion(CentralPath + "\\CenteralApp.exe"));
            SetDbEXEVersion(UpdaterFunctions.GetDBVersion("Centeral", connection));
            SetScriptVersion(UpdaterFunctions.GetDBScriptVersion(connection,1));


            //List Of Exe File
            try
            {
                Dt = UpdaterFunctions.GetListOfExeVersion(DBversion);//.Tables["VerInfo"]
                Dg_VersionList.DataSource = Dt;
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
                catch (Exception EX)
                {
                    UpdaterFunctions.SaveTextExeption("Error 2 : " + EX.Message.ToString());
                    //MessageBox.Show("Error 2 : " + EX.Message.ToString());
                }
            }
            catch (Exception EX)
            {
                UpdaterFunctions.SaveTextExeption("Error 3 : " + EX.Message.ToString());
                //MessageBox.Show("Error 3 : " + EX.Message.ToString());
            }
            if (ShowForm == false)
            {
                this.Hide();
                this.HidenThis(true);
            }
            
            if (Exe)
            {
                try
                {
                    thread = new Thread(() => SaveExe());
                    thread.IsBackground = true;
                    thread.Start();
                    Dg_VersionList.Enabled = false;
                }
                catch (Exception EX)
                {
                    UpdaterFunctions.SaveTextExeption("Error 4-1 : " + EX.Message.ToString());
                    //MessageBox.Show("Error 4 : " + EX.Message.ToString());
                }
            }
            else if (Scripts)
            {
                try
                {
                    thread = new Thread(() => ExecuteScripts());
                    thread.IsBackground = true;
                    thread.Start();
                    Dg_VersionList.Enabled = false;
                }
                catch (Exception EX)
                {
                    UpdaterFunctions.SaveTextExeption("Error 4-2 : " + EX.Message.ToString());
                    //MessageBox.Show("Error 4 : " + EX.Message.ToString());
                }
            }
            else
            {
                try
                {
                    UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip");
                }
                catch { }
                try
                {
                    UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip");
                }
                catch { }
            }
            if (ShowForm == false)
            {
                this.Hide();
                this.HidenThis(true);
            }
        }

        #endregion
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝


        //╔══════════════════════ Form ══════════════════════╗
        //║                                                  ║
        //║                                                  ║
        #region "Form"

        private void Btn_SQLRollBack_Click(object sender, EventArgs e)
        {
            Rollback = true;
            UpdaterFunctions.TransactionRollback();
        }

        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا میخواهید خارج شوید؟", "هشدار", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Btn_SaveEventLog_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "VersionLogEvent.txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (SaveEventLog(Txt_ScriptError.Text, saveFileDialog1.FileName) == true)
                {
                    MessageBox.Show("فایل ذخیره شد", "تایید");
                }
                else
                {
                    MessageBox.Show("خطا در ذخیره فایل", "هشدار");
                }
            }
        }

        private void Txt_ScriptError_TextChanged(object sender, EventArgs e)
        {

            Int64 TL = this.Txt_ScriptError.TextLength;

            Int32 S = Txt_ScriptError.Text.IndexOf("Error");
            Int32 F = 10;



            Txt_ScriptError.SelectionColor = Color.Red;

            Txt_ScriptError.Select(S, F);

            //box.AppendText(text);
            //box.SelectionColor = box.ForeColor;


            Txt_ScriptError.SelectionStart = 100000;
        }

        private void Btn_Retry_Click(object sender, EventArgs e)
        {

            //if (Txt_ScriptError.Text.Contains("Scripts : Start"))
            //{
            //    Frm_MackeUpdateInfo fnl = new Frm_MackeUpdateInfo(Parsicuserid,false);
            //    fnl.Show();
            //}
            //else
            //{
                Frm_MackeUpdateInfo FMUI = new Frm_MackeUpdateInfo(Parsicuserid, false);
                FMUI.Show();
            //}

            this.Hide();

            //Btn_RollBack.Visible =true ;
            //Btn_Exit.Visible = false;
            //Btn_SaveEventLog.Visible = false;
            //Btn_Retry.Visible = false;
            //Rollback = false;



            //try {

            //    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Convert.ToInt32(_Parsicuserid), "", VersionNumberC, "Retry", "", -1, 0, ComputerName, "", false, true, 0, connection, false, Convert.ToInt32(LabID));


            //    DTParsicMaster = UpdaterFunctions.UpdateGridView(GetODBCInfoList);

            //    LabID = UpdaterFunctions.GetLabID(DTParsicMaster);

            //    try
            //    {
            //        foreach (DataRow dr in DTParsicMaster.Rows)
            //        {
            //            if ((dr["DBList_IsActive"].ToString() == "True"))
            //            {
            //                DB_ServerName = dr["DBList_Server"].ToString();
            //                DB_Name = dr["DBList_Name"].ToString();
            //                DB_Username = dr["DBList_Username"].ToString();
            //                DB_Password = dr["DBList_Password"].ToString();
            //                DBversion = Convert.ToDouble(dr["DBCurruntVersion"]);
            //                TherIsTruDb = true;
            //                connection = UpdaterFunctions.MakeConnectionString(DB_ServerName, DB_Name, DB_Username, DB_Password);

            //            }
            //            else
            //            {
            //                DB_ServerName = dr["DBList_Server"].ToString();
            //                DB_Name = dr["DBList_Name"].ToString();
            //                DB_Username = dr["DBList_Username"].ToString();
            //                DB_Password = dr["DBList_Password"].ToString();
            //                string connnn = UpdaterFunctions.MakeConnectionString(DB_ServerName, DB_Name, DB_Username, DB_Password);
            //                UpdaterFunctions.SaveTextExeption("Sub requirement : DB = " + DB_ServerName + "     " + Prerequirement(Convert.ToInt32(_Parsicuserid), connnn, 1));
            //            }
            //        }
            //        UpdaterFunctions.TruncateVersionReleaseLogsandSubLogs(connection);
            //        UpdaterFunctions.SaveTextExeption("Connection  : " + connection);
            //        UpdaterFunctions.SaveTextExeption("requirement : " + Prerequirement(Convert.ToInt32(_Parsicuserid), connection, 0));
            //    }
            //    catch (Exception EX)
            //    {
            //        //MessageBox.Show("Error 1 : " + EX.Message.ToString());
            //        UpdaterFunctions.SaveTextExeption("Error In Find DB_ParsicMaster or execute requirement : " + EX.Message.ToString());
            //    }

            //    try
            //    {
            //        SqlConnection co = new SqlConnection(connection);
            //        try
            //        {
            //            co.Open();
            //            co.Close();
            //        }
            //        catch
            //        {
            //            UpdaterFunctions.SaveTextExeption("Error In ConnectionString, Pleas Check IIS HashCode");
            //            if (ShowForm)
            //            {
            //            }
            //            else
            //            {
            //                Application.Exit();
            //            }
            //        }
            //    }
            //    catch
            //    {
            //    }
            //    try
            //    {
            //        From = DTParsicMaster.Rows[0]["DBCurruntVersion"].ToString();
            //        _From = Convert.ToDouble(From);
            //        To = VersionNumberC;
            //        user_is_from_Parsic = IsFromLab();
            //        if (Exe)
            //        {
            //            //int VersionRelease_ID = Convert.ToInt16(Dt.Rows[Dg_VersionList.CurrentRow.Index]["Prk_VersionRelease_AutoID"]);
            //            if (UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(_Parsicuserid), 0, " فایل ") == false)
            //            {
            //                UpdaterFunctions.SaveTextExeption("Error in Send Log to Cloud");
            //                SetTxt_EXeEvent("Error in Send Log to Cloud");
            //                SetTxt_EXeEvent("Operation Finished");
            //                if (ShowForm)
            //                {
            //                    ChangeRollBackExitButtonVisible(true);
            //                    MessageBox.Show("ارور در ارسال لاگ به ابر ها", "ERROR");
            //                }
            //                else
            //                {
            //                    Application.Exit();
            //                }
            //            }
            //        }
            //    }
            //    catch
            //    {
            //    }
            //    if (TherIsTruDb == true)
            //    {
            //    }
            //    else
            //    {
            //        SetTxt_EXeEvent("بانک فعالی وجود ندارد");
            //        UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Convert.ToInt32(_Parsicuserid), "", VersionNumberC, "Error", "برای این کار حتما باید یک بانک فعال وجود داشته باشد ", -1, 1, ComputerName, "", false, true, 0, connection, false, Convert.ToInt32(LabID));
            //        SetTxt_EXeEvent("Operation Finished");
            //        if (ShowForm)
            //        {
            //            ChangeRollBackExitButtonVisible(true);
            //            MessageBox.Show("بانک فعالی وجود ندارد", "ERROR");
            //        }
            //        else
            //        {
            //            Application.Exit();
            //        }
            //    }
            //}
            //catch (Exception EX)
            //{
            //    //MessageBox.Show("خطا در ارتباط با بانک پارسیک مستر برای آپدیت اتوماتیک" + "\r\n" + "لطفا با شرکت پارسیپل تماس و اطلاع دهید که در فرایند ورژن زدن مشکلی بوجود آمده است" + "021-44804472");
            //    UpdaterFunctions.SaveTextExeption("خطا در ارتباط با بانک پارسیک مستر برای آپدیت اتوماتیک");
            //    UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
            //    try
            //    {
            //        SetTxt_EXeEvent("Operation Finished");
            //        if (ShowForm)
            //        {
            //            ChangeRollBackExitButtonVisible(true);
            //            MessageBox.Show("خطا در ارتباط با بانک پارسیک مستر برای آپدیت اتوماتیک، او دی بی سی را چک نمایید", "ERROR");
            //        }
            //        else
            //        {
            //            Application.Exit();
            //        }
            //    }
            //    catch
            //    {
            //    }
            //    Application.Exit();
            //}
            //Parsicuserid = Convert.ToInt32(_Parsicuserid);




            //if (Exe)
            //{
            //    try
            //    {
            //        thread = new Thread(() => SaveExe());
            //        thread.IsBackground = true;
            //        thread.Start();
            //        Dg_VersionList.Enabled = false;
            //    }
            //    catch (Exception EX)
            //    {
            //        UpdaterFunctions.SaveTextExeption("Error 4-1 : " + EX.Message.ToString());
            //        //MessageBox.Show("Error 4 : " + EX.Message.ToString());
            //    }
            //}
            //else if (Scripts)
            //{
            //    try
            //    {
            //        thread = new Thread(() => ExecuteScripts());
            //        thread.IsBackground = true;
            //        thread.Start();
            //        Dg_VersionList.Enabled = false;
            //    }
            //    catch (Exception EX)
            //    {
            //        UpdaterFunctions.SaveTextExeption("Error 4-2 : " + EX.Message.ToString());
            //        //MessageBox.Show("Error 4 : " + EX.Message.ToString());
            //    }
            //}
            //else
            //{
            //try
            //{
            //    UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip");
            //}
            //catch { }
            //try
            //{
            //    UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip");
            //}
            //catch { }
        }


        #endregion
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝


        //╔════════════════════ Functions ═══════════════════╗
        //║                                                  ║
        //║                                                  ║
        #region "Functions"

        #region EXE

        public void ExecuteScripts()
        {
            EnterVersionScripts(Parsicuserid, VersionRelease_ID, VersionNumberC, GetODBCInfoList);
        }
        public void SaveExe()
        {
            try
            {
                progressBar1.Value = 0;
                string result = "";
                int VersionReleaseVApplication = 0;
                int SoftwareApplication = 0;
                int CountOfEachApp = 0;

                //progressBar1.Value = 0;
                SetprogressBar(0);
                DataTable DTAll = new DataTable();

                if (VersionRelease_ID == -1)
                {
                    try
                    {
                        List<Int32> l = new List<Int32>();

                        l.Add(CenteralVerID);
                        l.Add(QCVerID);
                        l.Add(StorageVerID);
                        l.Add(JournalVerID);
                        l.Add(TemperatureVerID);
                        l.Add(ParsicLabAndroidVerID);
                        l.Add(PrinterCacherVerID);
                        l.Add(QMaticVerID);
                        l.Add(WebVerID);
                        List<Int32> ListOfVer = new List<Int32>(l.Distinct());
                        try { DTAll = UpdaterFunctions.GetVersionCountFromDataBase(100); DTAll.Clear(); } catch { }
                        for (int i = 0; i< ListOfVer.Count; i++)
                        {
                            if(ListOfVer[i] != -1)
                            {
                                DTAll.Merge(UpdaterFunctions.GetVersionCountFromDataBase(ListOfVer[i]));
                            }
                        }
     
                    }
                    catch
                    {

                    }
                }
                else
                {
                   
                    try { DTAll = UpdaterFunctions.GetVersionCountFromDataBase(100); DTAll.Clear(); } catch { }

                    DTAll = UpdaterFunctions.GetVersionCountFromDataBase(VersionRelease_ID); //Tables["PackageCount"]

                }

               
                Dt = DTAll.Copy();
                Dt.Clear();
  
                int WholeCount = 0;
                UInt64 WholeSize = 0;
                string[] CheckSum = new string[11] ;
                for (int i = 0; i < DTAll.Rows.Count; i++)
                {
                    if (Chk_Centeral.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 1 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == CenteralVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("Centeral", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Central Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[1] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = DTAll.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);
                        }
                        //Check Exe Is Ok\





                    }
                    if (Chk_QC.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 2 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == QCVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("QC", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading QC Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["str_FileSize"]);
                            CheckSum[2] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);
                        }
                        //Check Exe Is Ok\

                    }
                    if (Chk_Storage.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 3 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == StorageVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("Storage", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Storage Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[3] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);
                        }
                        //Check Exe Is Ok\

                    }
                    if (Chk_Journal.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 4 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == JournalVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("Journal", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Journal Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[4] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);
                        }
                        //Check Exe Is Ok\

                    }
                    if (Chk_Temperature.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 5 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == TemperatureVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("Temperature", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Temperature Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[5] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);
                        }
                        //Check Exe Is Ok\

                    }
                    if (Chk_ParsicLabAndroid.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 6 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == ParsicLabAndroidVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("ParsicLabAndroid", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Parsic Lab Android Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[6] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);

                            // Stop IIS For Replace DLLs
                            try
                            {
                                SetTxt_EXeEvent("ParsicLabAndroid : Stop IIS For Replace DLLs");
                                UpdaterFunctions.StartOrStopService("Stop", "W3SVC");
                                ReplaceParsicLab = true;
                            }
                            catch
                            {
                            }
                            // Stop IIS For Replace DLLs\

                            //Check Exe Is Ok\
                        }
                    }
                    if (Chk_PrinterCacher.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 10 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == PrinterCacherVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("PrinterCacher", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Printer Cacher Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[7] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);

                            //// Stop IIS For Replace DLLs
                            //try
                            //{
                            //    SetTxt_EXeEvent("Printer Cacher : Stop IIS For Replace DLLs");
                            //    UpdaterFunctions.StartOrStopService("Stop", "W3SVC");
                            //    ReplaceParsicLab = true;
                            //}
                            //catch
                            //{
                            //}
                            //// Stop IIS For Replace DLLs\

                            //Check Exe Is Ok\
                        }
                    }
                    if (Chk_QMatic.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 11 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == QMaticVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("QMatic", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading QMatic Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[8] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);

                            //// Stop IIS For Replace DLLs
                            //try
                            //{
                            //    SetTxt_EXeEvent("QMatic : Stop IIS For Replace DLLs");
                            //    UpdaterFunctions.StartOrStopService("Stop", "W3SVC");
                            //    ReplaceParsicLab = true;
                            //}
                            //catch
                            //{
                            //}
                            //// Stop IIS For Replace DLLs\

                            //Check Exe Is Ok\
                        }
                    }
                    if (Chk_Web.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 12 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == WebVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("Web", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Web Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[9] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = Dt.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);

                            // Stop IIS For Replace DLLs
                            try
                            {
                                SetTxt_EXeEvent("Web : Stop IIS For Replace DLLs");
                                UpdaterFunctions.StartOrStopService("Stop", "W3SVC");
                                ReplaceParsicLab = true;
                            }
                            catch
                            {
                            }
                            // Stop IIS For Replace DLLs\

                            //Check Exe Is Ok\
                        }
                    }
                    if (Chk_Centeral.Checked == true && Convert.ToInt16(DTAll.Rows[i]["Frk_SoftwareApplication"]) == 13 && (Convert.ToInt16(DTAll.Rows[i]["Frk_VersionRelease"]) == CenteralVerID || VersionRelease_ID != -1))
                    {
                        // Check Exe Is Ok
                        Int64 MySize = 0;
                        try
                        {
                            DataTable CDT = new DataTable();
                            CDT = UpdaterFunctions.GetDBVersionInfo("Centeral64", connection);
                            for (Int16 k = 0; k < CDT.Rows.Count; k++)
                            {
                                MySize += Convert.ToInt64(CDT.Rows[k]["Str_FileSize"]);
                            }
                        }
                        catch { }
                        if (Convert.ToInt64(DTAll.Rows[i]["Str_FileSize"]) == MySize && VersionRelease_ID == -1)
                        {
                            SetTxt_EXeEvent("Downloading Central64 Exe:  Ther are same file with same Size in database");
                        }
                        else
                        {
                            WholeCount += Convert.ToInt16(DTAll.Rows[i]["CountOfEachApp"]);
                            WholeSize += Convert.ToUInt64(DTAll.Rows[i]["Str_FileSize"]);
                            CheckSum[10] = DTAll.Rows[i]["Str_CRC"].ToString();
                            DataRow row = DTAll.NewRow();
                            row = DTAll.Rows[i];
                            Dt.Rows.Add(row.ItemArray);
                        }
                        //Check Exe Is Ok\

                    }
                }
                //progressBar1.Maximum = WholeCount;
                string Filename = "";
                string ParsicLabAndroidFilename = "";
                string WebFilename = "";
                SetMaxprogressBar(WholeCount);
                if(WholeCount != 0)
                { 
                    SetTxt_EXeEvent("Downloading Exe:   Start    Count : " + WholeCount + "    Version To : " + VersionNumberC);
                }
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    VersionReleaseVApplication = Convert.ToInt16(Dt.Rows[i]["Prk_VersionReleaseVApplication_AutoID"]);
                    SoftwareApplication = Convert.ToInt16(Dt.Rows[i]["Frk_SoftwareApplication"]);
                    CountOfEachApp = Convert.ToInt16(Dt.Rows[i]["CountOfEachApp"]);
                    string Filename1 = Dt.Rows[i]["Str_FileName"].ToString();
                    Filename = Filename1.Replace("/", "-");

                    Int64 Size = 0;
                    switch (SoftwareApplication)
                    {
                        case 1:
                            if (Chk_Centeral.Checked)
                            {
                                string Type = "Centeral";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[1], connection))
                                    result += "\r\nCenteral : OK";
                                else
                                {
                                    DataTable __DT = new DataTable("__DT");
                                    result += "\r\nCenteral : Error";
                                    try
                                    {
                                        SqlConnection con = new SqlConnection(connection);
                                        con.Open();
                                        SqlCommand cmd = new SqlCommand("select top 1 * From Tbl_VersionReleaseVLogs order by Prk_VersionReleaseLogs_AutoID desc", con);
                                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                                        cmd.ExecuteNonQuery();
                                        ad.Fill(__DT);
                                        con.Close();
                                        result += "\r\nLog: " + __DT.Rows[0]["Str_ErrorLog"];
                                    }
                                    catch (Exception EX)
                                    {
                                        UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                                        //MessageBox.Show("Error 5 : " + EX.Message.ToString());
                                    }

                                }
                            }
                            break;
                        case 2:
                            if (Chk_QC.Checked)
                            {
                                string Type = "QC";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[2], connection))
                                    result += "\r\nQC : OK";
                                else
                                    result += "\r\nQC : Error";
                            }
                            break;
                        case 3:
                            if (Chk_Storage.Checked)
                            {
                                string Type = "Storage";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[3], connection))
                                    result += "\r\nStorage : OK";
                                else
                                    result += "\r\nStorage : Error";
                            }
                            break;
                        case 4:
                            if (Chk_Journal.Checked)
                            {
                                string Type = "Journal";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[4], connection))
                                    result += "\r\nJournal : OK";
                                else
                                    result += "\r\nJournal : Error";
                            }
                            break;
                        case 5:
                            if (Chk_Temperature.Checked)
                            {
                                string Type = "Temperature";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[5], connection))
                                    result += "\r\nTemperature : OK";
                                else
                                    result += "\r\nTemperature : Error";
                            }
                            break;
                        case 6:
                            if (Chk_ParsicLabAndroid.Checked)
                            {
                                string FN = Dt.Rows[i]["Str_FileName"].ToString();
                                ParsicLabAndroidFilename = FN.Replace("/", "-");
                                string Type = "ParsicLabAndroid";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[6], connection))
                                    result += "\r\nParsicLabAndroid : OK";
                                else
                                    result += "\r\nParsicLabAndroid : Error";
                            }
                            break;
                        case 10:
                            if (Chk_PrinterCacher.Checked)
                            {
                                string Type = "PrinterCacher";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[7], connection))
                                    result += "\r\nPrinterCacher : OK";
                                else
                                    result += "\r\nPrinterCacher : Error";
                            }
                            break;
                        case 11:
                            if (Chk_QMatic.Checked)
                            {
                                string Type = "QMatic";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[8], connection))
                                    result += "\r\nQMatic : OK";
                                else
                                    result += "\r\nQMatic : Error";
                            }
                            break;
                        case 12:
                            if (Chk_Web.Checked)
                            {
                                string FN = Dt.Rows[i]["Str_FileName"].ToString();
                                WebFilename = FN.Replace("/", "-");
                                string Type = "Web";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[9], connection))
                                    result += "\r\nWeb : OK";
                                else
                                    result += "\r\nWeb : Error";
                            }
                            break;

                        case 13:
                            if (Chk_Centeral.Checked)
                            {
                                string Type = "Centeral64";
                                Size = Convert.ToInt64(Dt.Rows[i]["str_FileSize"]);
                                if (FeatchAndSavePackageWithoutTransaction(VersionRelease_ID, VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumberC, DB_Name, Size, CheckSum[10], connection))
                                    result += "\r\nCenteral64 : OK";
                                else
                                {
                                    DataTable __DT = new DataTable("__DT");
                                    result += "\r\nCenteral64 : Error";
                                    try
                                    {
                                        SqlConnection con = new SqlConnection(connection);
                                        con.Open();
                                        SqlCommand cmd = new SqlCommand("select top 1 * From Tbl_VersionReleaseVLogs order by Prk_VersionReleaseLogs_AutoID desc", con);
                                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                                        cmd.ExecuteNonQuery();
                                        ad.Fill(__DT);
                                        con.Close();
                                        result += "\r\nLog: " + __DT.Rows[0]["Str_ErrorLog"];
                                    }
                                    catch (Exception EX)
                                    {
                                        UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                                        //MessageBox.Show("Error 5 : " + EX.Message.ToString());
                                    }

                                }
                            }
                            break;

                    }
                }

                //progressBar1.Value = progressBar1.Maximum;
                SetprogressBar(progressBar1.Maximum);
                if (result == "")
                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Convert.ToInt32(_Parsicuserid), "", VersionNumberC, "Error", "هیچ برنامه ای انتخواب نکرده اید ", -1, 1, ComputerName, "", false, true, 0, connection, false, Convert.ToInt32(LabID));
                else
                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Convert.ToInt32(_Parsicuserid), "", VersionNumberC, "Finish", result, -1, 1, ComputerName, "", false, true, 0, connection, false, Convert.ToInt32(LabID));

                UpdaterFunctions.SaveTextExeption("Exe Finish.      " + result);
                if (Rollback == false)
                {
                    if (WholeCount != 0)
                    {
                        SetTxt_EXeEvent("Downloading Exe:   Finish    Version To : " + VersionNumberC + "         " + result.Replace("\r\n", "     "));
                    }
                }
                //MessageBox.Show(result,"نتیجه");
                //}
                //HidenThis(true);
                if (result.Contains("Error"))
                {
                    if (Rollback == false)
                    {
                        SetTxt_EXeEvent("Ther is an error in downloading exe file for Version " + VersionNumberC + "     Scripts canceled");
                        UpdaterFunctions.SaveTextExeption("Error in download EXE, Pleas Try again Later");
                    }
                    SetTxt_EXeEvent("Operation Finished");

                    if (ShowForm)
                    {
                        ChangeRollBackExitButtonVisible(true);
                        MessageBox.Show("خطا در دانلود فایل های اگزه، لطفا دوباره تلاش نمایید.\r\n این خطا ممکن است بدلیل عدم ارتباط با سرور ابری پارسیپل بوجود آمده باشد", "ERROR");
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
                else
                {
                    try
                    {
                        string From = _From.ToString();
                        string To = VersionNumberC;

                        //int VersionRelease_ID = Convert.ToInt16(Dt.Rows[Dg_VersionList.CurrentRow.Index]["Prk_VersionRelease_AutoID"]);
                        UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(_Parsicuserid),0, 1, " فایل ");

                    }
                    catch
                    {

                    }


                    if (Chk_ParsicLabAndroid.Checked && result.Contains("ParsicLabAndroid : OK"))
                    {
                        try
                        {

                            ReplaceParsicLab = true;
                            SetTxt_EXeEvent("ParsicLabAndroid Start To Replace");
                            UpdaterFunctions.StartOrStopService("Stop", "W3SVC");

                            string Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                            if (Path != "")
                            {
                                string subans = ReplaceLabAndroidFolder(ParsicLabAndroidFilename, Path);
                                try
                                {
                                    if (!Directory.Exists(Path + "\\ParsicLabAndroid_Archive"))
                                    {
                                        Directory.CreateDirectory(Path + "\\ParsicLabAndroid_Archive");
                                    }
                                    if (Directory.Exists(Path + "\\ParsicLabAndroid_Archive\\" + ParsicLabAndroidFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", "")))
                                    {
                                        UpdaterFunctions.clearFolder(Path + "\\ParsicLabAndroid_Archive\\" + ParsicLabAndroidFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""));
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(Path + "\\ParsicLabAndroid_Archive\\" + ParsicLabAndroidFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""));
                                    }
                                    UpdaterFunctions.CopyDirectory(Path + "\\" + ParsicLabAndroidFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""), Path + "\\ParsicLabAndroid_Archive\\" + ParsicLabAndroidFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""));
                                    try
                                    {
                                        UpdaterFunctions.clearFolder(Path + "\\" + ParsicLabAndroidFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""));
                                        Directory.Delete(Path + "\\" + ParsicLabAndroidFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""));
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    SetTxt_EXeEvent("ParsicLabAndroid Folder Archived");

                                }
                                catch
                                {

                                }
                                SetTxt_EXeEvent("ParsicLabAndroid Finish, Ansver : " + subans);

                                if(subans != "OK")
                                {
                                    ParsicLabAndroidVerID = -1;
                                    UpdaterFunctions.clearFolder(Path + "\\ParsicLabAndroid");
                                    UpdaterFunctions.ExtractZipFile(Path + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip", Path + "\\ParsicLabAndroid");
                                    if (ShowForm)
                                    {
                                        MessageBox.Show(subans, "ParsicLabAndroid");
                                    }
                                }
                            }

                            if (!Chk_Web.Checked)
                            {
                                UpdaterFunctions.StartOrStopService("Start", "W3SVC");
                                System.Diagnostics.Process.Start(IISLabAndroidURL);
                            }
                        }
                        catch
                        {

                        }
                    }





                    if (Chk_Web.Checked && result.Contains("Web : OK"))
                    {
                        try
                        {
                            ReplaceParsicLab = true;
                            SetTxt_EXeEvent("Web Start To Replace");
                            UpdaterFunctions.StartOrStopService("Stop", "W3SVC");
                            string Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                            string subans = "";
                            if (Path != "")
                            {
                                subans = ReplaceLabOnlineFolder(WebFilename, Path);
                                try
                                {
                                    if (!Directory.Exists(Path + "\\ParsicLabOnline_Archive"))
                                    {
                                        Directory.CreateDirectory(Path + "\\ParsicLabOnline_Archive");
                                    }
                                    if (Directory.Exists(Path + "\\ParsicLabOnline_Archive\\" + WebFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", "").Replace("Web","LabOnline")))
                                    {
                                        UpdaterFunctions.clearFolder(Path + "\\ParsicLabOnline_Archive\\" + WebFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", "").Replace(".zip", "").Replace("Web", "LabOnline"));
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(Path + "\\ParsicLabOnline_Archive\\" + WebFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", "").Replace(".zip", "").Replace("Web", "LabOnline"));
                                    }
                                    UpdaterFunctions.CopyDirectory(Path + "\\" + WebFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""), Path + "\\ParsicLabOnline_Archive\\" + WebFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", "").Replace(".zip", "").Replace("Web", "LabOnline"));
                                    try
                                    {
                                        UpdaterFunctions.clearFolder(Path + "\\" + WebFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""));
                                        Directory.Delete(Path + "\\" + WebFilename.Replace("-", " ").Replace("_", " ").Replace(".zip", ""));
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    SetTxt_EXeEvent("LabOnline Folder Archived");

                                }
                                catch
                                {

                                }
                                SetTxt_EXeEvent("Web Finish, Ansver : " + subans);
                                if (subans != "OK")
                                {
                                    WebVerID = -1;
                                    UpdaterFunctions.clearFolder(Path + "\\LabOnline");
                                    UpdaterFunctions.ExtractZipFile(Path + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip", Path + "\\LabOnline");
                                    if (ShowForm)
                                    {
                                        MessageBox.Show(subans, "LabOnline");
                                    }
                                }
                            }
                            UpdaterFunctions.StartOrStopService("Start", "W3SVC");
                            System.Diagnostics.Process.Start(IISLabOnlineURL.Replace("/Account", "").Replace("UMM", ""));
                            if (Chk_ParsicLabAndroid.Checked)
                            {
                                System.Diagnostics.Process.Start(IISLabAndroidURL);
                            }
                        }
                        catch
                        {

                        }
                    }


                }
                SetTxt_EXeEvent("File Downloading Hase Been Done  \r\n\r\n\r\n");

                    if (Scripts)
                    {
                        EnterVersionScripts(Parsicuserid, VersionRelease_ID, VersionNumberC, GetODBCInfoList);
                    }
                    else
                    {
                    try
                    {
                        UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip");
                    }
                    catch { }
                    try
                    {
                        UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip");
                    }
                    catch { }
                        ChangeRollBackExitButtonVisible(true);
                        SetTxt_EXeEvent("All Operations Finished");
                        if (ShowForm)
                        {
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                
                if (ShowForm)
                {

                    ChangeRollBackExitButtonVisible(true);

                }

                //Eupdate.ShowDialog();
            }
            catch (Exception EX)
            {
                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                SetTxt_EXeEvent("Operation Finished");
                if (ShowForm)
                {
                    MessageBox.Show("Error : " + EX.Message.ToString(), "ERROR");
                    ChangeRollBackExitButtonVisible(true);
                }
                else
                {
                    Application.Exit();
                }

                //Frm_VersionList_Load(sender, e);
                //MessageBox.Show("Error 6 : " + EX.Message.ToString());
            }
        }
        public String ReplaceLabAndroidFolder(string name, string Path)
        {
            string ans = "";
            DataTable LabInfo = new DataTable("DT");
            string NewPathForIIS = Path + "\\ParsicLabAndroid";
            string curentPath = "";
            try
            {
                LabInfo = UpdaterFunctions.GetCloudLabInfo(Convert.ToInt32(LabID));
                if (LabInfo.Rows.Count == 0)
                {
                    SetTxt_EXeEvent("ParsicLabAndroid: Ther is no Info About This Lab In Ticketing");
                    return "ParsicLabAndroid : Ther is no Info About This Lab In Ticketing";
                }
                IISLabAndroidURL = LabInfo.Rows[0]["Str_ServiceURL"].ToString();
                SetTxt_EXeEvent("ParsicLabAndroid : URL : " + IISLabAndroidURL);
                string MyPort = UpdaterFunctions.FindPort(IISLabAndroidURL);
                string MySiteName = UpdaterFunctions.FindIISSiteName(MyPort);
                curentPath = UpdaterFunctions.GetIISSitePath(MySiteName);
                DataTable ConfigDt = new DataTable("DT");


                //Create Zip For Backup
                if (UpdaterFunctions.CreateZipFile(curentPath, Path + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip").Contains("Error"))
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 1.Error :    Create Zip For Backup");
                }
                else
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 1.Create Zip For Backup");
                    UpdaterFunctions.HidenFile(Path + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip");
                }


                //Get Web.config
                string mypath = curentPath + "\\Web.config";
                ConfigDt = UpdaterFunctions.GetWebConfigInfo(mypath);
                if (ConfigDt.Rows.Count == 0)
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 2.Error In Get Old Web.config Information From " + curentPath + "\\Web.config");
                    SetTxt_EXeEvent("ParsicLabAndroid : 2.برنامه به فایل وب کانفیگ دسترسی پیدا نکرد  " + curentPath + "\\Web.config");
                }
                else
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 2.Get Old Web.config Information From " + curentPath + "\\Web.config");
                }


                //Create or clear Folder ParsicLabAndroid
                if (!System.IO.Directory.Exists(Path + "\\ParsicLabAndroid"))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(Path + "\\ParsicLabAndroid");
                        SetTxt_EXeEvent("ParsicLabAndroid : 3.Create Folder : " + Path + "\\ParsicLabAndroid");
                    }
                    catch
                    {
                        SetTxt_EXeEvent("ParsicLabAndroid : 3.Error In Create Folder : " + Path + "\\ParsicLabAndroid");
                    }



                }
                else
                {
                    if (UpdaterFunctions.clearFolder(Path + "\\ParsicLabAndroid"))
                    {
                        SetTxt_EXeEvent("ParsicLabAndroid : 3.Clear " + Path + "\\ParsicLabAndroid ");
                    }
                    else
                    {
                        SetTxt_EXeEvent("ParsicLabAndroid : 3.Error : In Clear " + Path + "\\ParsicLabAndroid ");
                        return "ParsicLabAndroid : Error In Clear Path : " + Path + "\\ParsicLabAndroid ";

                    }
                }


                //replace folder
                ans = UpdaterFunctions.CopyFolder(Path + "\\" + name.Replace(".zip", "").Replace("-", " ").Replace("_", " "), Path + "\\ParsicLabAndroid");
                if (ans == "OK")
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 4.File Replased ");
                }
                else
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 4.Error : In Replasing File    " + ans);
                    return "ParsicLabAndroid : Error In Replacing Files    " + ans;

                }


                //set Web.config
                if (UpdaterFunctions.SetWebConfigInfo(ConfigDt, Path + "\\ParsicLabAndroid\\web.config"))
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 5.Set Web.config In " + Path + "\\ParsicLabAndroid\\web.config");
                }
                else
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : 5.Error In Set Web.config In " + Path + "\\ParsicLabAndroid\\web.config");
                }

            }
            catch
            {

            }


            // Change IIS Site Path
            if(NewPathForIIS != curentPath)
            {
                SetTxt_EXeEvent("ParsicLabAndroid : Nwe Path = " + NewPathForIIS);
                try
                {
                    UpdaterFunctions.SendBackLog -= ReceiveLogEvent;
                    UpdaterFunctions.SendBackLog += ReceiveLogEvent;
                }
                catch
                {

                }
                if (UpdaterFunctions.ChangeIISPathPyURL(IISLabAndroidURL, NewPathForIIS))
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : Replace IIS Path Finished");
                }
                else
                {
                    SetTxt_EXeEvent("ParsicLabAndroid : Error in replacing Path In IIS  ----->  RUN AS ADMINISTRATOR");
                    return "ParsicLabAndroid : Error in replacing Path In IIS\r\n RUN AS ADMINISTRATOR";

                }
            }
            if (ans == "OK")
                return "OK";
            else
                return ans;

        }

        public String ReplaceLabOnlineFolder(string name, string Path)
        {
            string ans = "";
            DataTable LabInfo = new DataTable("DT");
            string NewPathForIIS = Path + "\\LabOnline";
            string curentPath = "";

            try
            {
                LabInfo = UpdaterFunctions.GetCloudLabInfo(Convert.ToInt32(LabID));
                if (LabInfo.Rows.Count == 0)
                {
                    SetTxt_EXeEvent("LabOnline : Ther is no Info About This Lab In Ticketing");
                    return "Lab Online : Ther is no Info About This Lab In Ticketing";
                }
                IISLabOnlineURL = LabInfo.Rows[0]["Str_WebSiteAddress"].ToString();
                SetTxt_EXeEvent("LabOnline : URL : " + IISLabOnlineURL);
                string MyPort = UpdaterFunctions.FindPort(IISLabOnlineURL);
                string MySiteName = UpdaterFunctions.FindIISSiteName(MyPort);
                curentPath = UpdaterFunctions.GetIISSitePath(MySiteName);


                // Chek For Folder Deployment
                //try
                //{
                //    string[] Folders = Directory.GetDirectories(curentPath);
                //    for (int i = 0; i <= Folders.Count(); i++)
                //    {
                //        if (Folders[i].Contains("Deployment"))
                //        {
                //            //curentPath += "\\Deployment";
                //            break;
                //        }
                //    }
                //}
                //catch
                //{

                //}


                //Create Zip For Backup
                if (UpdaterFunctions.CreateZipFile(curentPath, Path + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip").Contains("Error"))
                {
                    SetTxt_EXeEvent("LabOnline : 1.Error Create Zip For Backup");
                }
                else
                {
                    SetTxt_EXeEvent("LabOnline : 1.Create Zip For Backup");
                    UpdaterFunctions.HidenFile(Path + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip");
                }

                // Get Web.Config and Image Folders
                try
                {
                    string Copyans = UpdaterFunctions.CopyFile(curentPath + "\\Web.config", Path + "\\Web.config");
                    if (Copyans == "OK")
                    {
                        SetTxt_EXeEvent("LabOnline : 2.1.Copy Old Web.config From " + curentPath + "\\Web.config to " + Path + "\\Web.config");
                        UpdaterFunctions.HidenFile(Path + "\\Web.config");

                    }
                    else
                    {
                        SetTxt_EXeEvent("LabOnline : 2.1.Error     Didn't Copy Old Web.config from " + curentPath + "\\Web.config \r\n" + Copyans);
                        //return "LabOnline : 2.Error : Copy Old Web.config in " + curentPath + "\\Web.config\r\n" + Copyans;
                    }


                    string Zans = UpdaterFunctions.CreateZipFile(curentPath + "\\Content\\Images", Path + "\\LabAndroidImage.zip");
                    if (Zans.Contains("Error"))
                    {
                        SetTxt_EXeEvent("LabOnline : 2.2.Error     Didn't Create Zip From " + curentPath + "\\Content\\Images   To   " + Path + "\\LabAndroidImage.zip" + Copyans);
                        UpdaterFunctions.DeleteZipFile(Path + "\\LabAndroidImage.zip");
                    }
                    else
                    {
                        SetTxt_EXeEvent("LabOnline : 2.2.Create Zip From " + curentPath + "\\Content\\Images   To   " + Path + "\\LabAndroidImage.zip" + Copyans);
                        UpdaterFunctions.HidenFile(Path + "\\LabAndroidImage.zip");
                    }



                    string Zans2 = UpdaterFunctions.CreateZipFile(curentPath + "\\Content\\Qmatic\\Tabligh", Path + "\\LabAndroidQMATICTabligh.zip");
                    if (Zans2.Contains("Error"))
                    {
                        SetTxt_EXeEvent("LabOnline : 2.3.Error     didn't Create Zip From " + curentPath + "\\Content\\Qmatic\\Tabligh   To   " + Path + "\\LabAndroidQMATICTabligh.zip" + Copyans);
                        UpdaterFunctions.DeleteZipFile(Path + "\\LabAndroidQMATICTabligh.zip");
                    }
                    else
                    {
                        SetTxt_EXeEvent("LabOnline : 2.3.Create Zip From " + curentPath + "\\Content\\Qmatic\\Tabligh   To   " + Path + "\\LabAndroidQMATICTabligh.zip" + Copyans);
                        UpdaterFunctions.HidenFile(Path + "\\LabAndroidQMATICTabligh.zip");
                    }

                }
                catch
                {

                }



                //Create or clear Folder LabOnline
                if (!System.IO.Directory.Exists(Path + "\\LabOnline"))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(Path + "\\LabOnline");
                        SetTxt_EXeEvent("LabOnline : 3.Create Folder : " + Path + "\\LabOnline");
                    }
                    catch
                    {
                        SetTxt_EXeEvent("LabOnline : 3.Error In Create Folder : " + Path + "\\LabOnline");
                    }
                }
                else
                {

                    if (UpdaterFunctions.clearFolder(Path + "\\LabOnline"))
                    {
                        SetTxt_EXeEvent("LabOnline : 3.Clear " + Path + "\\LabOnline ");
                    }
                    else
                    {
                        SetTxt_EXeEvent("LabOnline : 3.Error In Clear " + Path + "\\LabOnline ");
                        return "LabOnline : Error In Clear Path : " + Path + "\\LabOnline ";
                    }

                }


                //Replace Folders
                ans = UpdaterFunctions.CopyFolder(Path + "\\" + name.Replace(".zip", "").Replace("-", " ").Replace("_", " "), Path + "\\LabOnline");
                if (ans == "OK")
                {
                    SetTxt_EXeEvent("LabOnline : 4.Files Replaced");
                }
                else
                {
                    SetTxt_EXeEvent("LabOnline : 4.Error : In Replacing File     " + ans);

                    return "Lab Online : Error In Replacind Fils  " + ans;
                }


                //Set Old Images and Configs
                if (UpdaterFunctions.CopyFile(Path + "\\Web.config", Path + "\\LabOnline\\Web.config") == "OK")
                {
                    SetTxt_EXeEvent("LabOnline : 5.1.Web.config Replaced");
                    UpdaterFunctions.ShowFile(Path + "\\LabOnline\\Web.config");
                    try
                    {
                        System.IO.File.Delete(Path + "\\Web.config");
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    SetTxt_EXeEvent("LabOnline : 5.1.Error     Dont Replace Web.Config From : " + Path + "\\Web.config to : " + Path + "\\LabOnline\\Web.config");
                    //return "Error In Replacing Web.Config From : " + Path + "\\Web.config to : " + Path + "\\LabOnline\\Web.config";
                }

                try
                {
                    if (UpdaterFunctions.clearFolder(Path + "\\LabOnline\\Content\\Images"))
                    {
                        UpdaterFunctions.ShowFile(Path + "\\LabAndroidImage.zip");
                        if (UpdaterFunctions.ExtractZipFile(Path + "\\LabAndroidImage.zip", Path + "\\LabOnline\\Content\\Images") != "")
                        {
                            UpdaterFunctions.DeleteZipFile(Path + "\\LabAndroidImage.zip");
                            SetTxt_EXeEvent("LabOnline : 5.2.Images Folder Replaced -----> " + Path + "\\LabOnline\\Content\\Images");
                        }
                        else
                        {
                            SetTxt_EXeEvent("LabOnline : 5.2.Error Dont Replace Images Folder");
                        }
                    }
                    else
                    {
                        SetTxt_EXeEvent("LabOnline : 5.2.2.Error Dont Replace Images Folder");
                    }
                }
                catch
                {
                    SetTxt_EXeEvent("LabOnline : 5.2.3.Error Dont Replace Images Folder");
                }





                try
                {
                    if (UpdaterFunctions.clearFolder(Path + "\\LabOnline\\Content\\Qmatic\\Tabligh"))
                    {
                        UpdaterFunctions.ShowFile(Path + "\\LabAndroidQMATICTabligh.zip");
                        if (UpdaterFunctions.ExtractZipFile(Path + "\\LabAndroidQMATICTabligh.zip", Path + "\\LabOnline\\Content\\Qmatic\\Tabligh") != "")
                        {
                            UpdaterFunctions.DeleteZipFile(Path + "\\LabAndroidQMATICTabligh.zip");
                            SetTxt_EXeEvent("LabOnline : 5.3.Tabligh Folder Replaced -----> " + Path + "\\LabOnline\\Content\\Qmatic\\Tabligh");
                        }
                        else
                        {
                            SetTxt_EXeEvent("LabOnline : 5.3.Error Dont Replace Tabligh Folder");
                        }
                    }
                    else
                    {
                        SetTxt_EXeEvent("LabOnline : 5.3.2.Error Dont Replace Tabligh Folder");
                    }
                }
                catch
                {
                    SetTxt_EXeEvent("LabOnline : 5.3.3.Error Dont Replace Tabligh Folder");
                }



            }
            catch
            {

            }



            if(NewPathForIIS != curentPath)
            {
                SetTxt_EXeEvent("LabOnline : Nwe Path = " + NewPathForIIS);
                try
                {
                    UpdaterFunctions.SendBackLog -= ReceiveLogEvent;
                    UpdaterFunctions.SendBackLog += ReceiveLogEvent;
                }
                catch
                {

                }
                if (UpdaterFunctions.ChangeIISPathPyURL(IISLabOnlineURL, NewPathForIIS))
                {
                    SetTxt_EXeEvent("LabOnline : Replace IIS Path Finished");
                }
                else
                {
                    SetTxt_EXeEvent("LabOnline : Error in replacing Path In IIS  ----->  RUN AS ADMINISTRATOR");
                    return "LabOnline : Error in replacing Path In IIS\r\n RUN AS ADMINISTRATOR";
                }
            }

            if (ans == "OK")
                return "OK";
            else
                return ans;
        }











        public Boolean FeatchAndSavePackageWithoutTransaction(int VersionRelease_ID, int VersionReleaseVApplication, int CountOfEachApp, string Type, string Filename, string VersionNumberC, string DB_Name, Int64 Size, string WholeCRC, string connection, string Path = "")
        {
            string VersionFrom = "";
            int AppType = 0;
            if (Type == "Centeral")
            {
                AppType = 1;
                VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder(Type) + "\\CenteralApp.exe");
            }

            if (Type == "QC")
            {
                AppType = 2;
                VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder(Type) + "\\Quality_Control.exe");
            }

            if (Type == "Storage")
            {
                AppType = 3;
                VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder(Type) + "\\Storage.exe");
            }
            if (Type == "Journal")
            {
                AppType = 4;
                VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder("Machines") + "\\machineController.exe");
            }
            if (Type == "Temperature")
            {
                AppType = 5;
                //VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder("Machines") + "\\machineController.exe");
                VersionFrom = "";
            }
            if (Type == "ParsicLabAndroid")
            {
                AppType = 6;
                //VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder("Machines") + "\\machineController.exe");
                VersionFrom = "";
            }
            if (Type == "PrinterCacher")
            {
                AppType = 10;
                //VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder("Machines") + "\\machineController.exe");
                VersionFrom = "";
            }
            if (Type == "QMatic")
            {
                AppType = 11;
                //VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder("Machines") + "\\machineController.exe");
                VersionFrom = "";
            }
            if (Type == "Web")
            {
                AppType = 12;
                //VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder("Machines") + "\\machineController.exe");
                VersionFrom = "";
            }
            if (Type == "Centeral64")
            {
                AppType = 13;
                //VersionFrom = UpdaterFunctions.GetVersion(UpdaterFunctions.FindFolder("Machines") + "\\machineController.exe");
                VersionFrom = "";
            }

            //progressBar1.Value = 0;
            try
            {
                ComputerName = System.Environment.MachineName.ToString();
                UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Try", Type, AppType, 0, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, 0, CountOfEachApp, "Try", Type, connection);

            }catch (Exception ex)
            {

            }


            //Boolean FierstDel = false;
            try
            {

                string vernn = UpdaterFunctions.GetDBVersion(Type, connection);
                //if (vernn == VersionNumberC || vernn == "")
                //{
                //    FierstDel = true;
                //    //UpdaterFunctions.DeleteFromDB(1, Type, "", connection);
                //}

                int CheckRepet = 0;
                int CheckRepet1 = 0;
                if (AppType == 6)
                {
                    byte[] WholeImageData = new byte[0];
                    for (int i = 0; i < CountOfEachApp; i++)
                    {
                        if (Rollback)
                        {
                            UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type, Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                            UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type, Type, connection);
                            SetTxt_Error("RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type);
                            return false;
                        }
                        byte[] ImageData;
                        DataTable Dt1 = new DataTable("EachPart"); //Tables["EachPart"]
                        Dt1 = UpdaterFunctions.GetVersionInfoFileFromDataBase(VersionReleaseVApplication, i);

                        ImageData = (byte[])Dt1.Rows[0]["Bin_FileContent"];
                        try
                        {
                            string CRC = Dt1.Rows[0]["Str_CRC"].ToString();
                            string FileSize = Dt1.Rows[0]["Str_PartSize"].ToString();
                            //for each part(exept last part) run one time 
                            string check = UpdaterFunctions.CalculateChecksum(ImageData);
                            if (CRC == check)
                            {
                                var s = new MemoryStream();
                                s.Write(WholeImageData, 0, WholeImageData.Length);
                                s.Write(ImageData, 0, ImageData.Length);
                                WholeImageData = s.ToArray();
                                //progressBar1.Value += 1;
                                //progressBar1.Refresh();
                                AddprogressBar(1);
                                SetTxt_EXeEvent("Downloading " + Type + " Exe:   " + progressBar1.Value + "   OF   " + progressBar1.Maximum);
                                CheckRepet1 = 0;// Not Catch In Loop If There area Mistake
                                CheckRepet = 0;
                            }
                            else
                            {
                                SetTxt_EXeEvent("Downloading " + Type + " Exe:   CheckSum Is Wrong Try " + CheckRepet);
                                i--;
                                CheckRepet++;
                                Thread.Sleep(3000);
                                if (CheckRepet == 5)
                                {
                                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, connection);
                                    return false;
                                }
                            }
                            UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "", Type, connection);

                        }
                        catch
                        {
                            i--;
                            CheckRepet++;
                            SetTxt_EXeEvent("Downloading " + Type + " Exe:   Download Failed Try " + CheckRepet);
                            Thread.Sleep(3000);
                            if (CheckRepet == 5)
                            {
                                UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, connection);
                                return false;
                            }
                        }
                    }

                    Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                    if (Path == "")
                    {
                        UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: ParsicLabAndroid Path is Empty", Type, AppType, 1, ComputerName, "", true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                        UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, "", true, VersionNumberC, 0, CountOfEachApp, "Error: ParsicLabAndroid Path is Empty", Type, connection);
                        SetTxt_Error("Error: ParsicLabAndroid Path is Empty");
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(Path))
                        {
                            try
                            {
                                UpdaterFunctions.WriteZipFile(Filename, Path, WholeImageData);
                                UpdaterFunctions.ExtractFile(Filename, Path, Path);
                                UpdaterFunctions.DeleteZipFile(Path + "\\" + Filename);
                            }
                            catch (Exception EX)
                            {
                                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                                SetTxt_EXeEvent(Type + " Operation Finished");
                            }
                        }
                        else
                        {
                            SetTxt_EXeEvent("Path For Save ParsicLabAndroid not Exist. Pati : " + Path);
                        }
                    }

                }

                else if (AppType == 12)
                {

                    byte[] WholeImageData = new byte[0];
                    for (int i = 0; i < CountOfEachApp; i++)
                    {
                        if (Rollback)
                        {
                            UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type, Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                            UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type, Type, connection);
                            SetTxt_Error("RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type);
                            return false;
                        }
                        byte[] ImageData;
                        DataTable Dt1 = new DataTable("EachPart"); //Tables["EachPart"]
                        Dt1 = UpdaterFunctions.GetVersionInfoFileFromDataBase(VersionReleaseVApplication, i);

                        ImageData = (byte[])Dt1.Rows[0]["Bin_FileContent"];
                        try
                        {
                            string CRC = Dt1.Rows[0]["Str_CRC"].ToString();
                            string FileSize = Dt1.Rows[0]["Str_PartSize"].ToString();
                            //for each part(exept last part) run one time 
                            string check = UpdaterFunctions.CalculateChecksum(ImageData);
                            if (CRC == check)
                            {
                                var s = new MemoryStream();
                                s.Write(WholeImageData, 0, WholeImageData.Length);
                                s.Write(ImageData, 0, ImageData.Length);
                                WholeImageData = s.ToArray();
                                //progressBar1.Value += 1;
                                //progressBar1.Refresh();
                                AddprogressBar(1);
                                SetTxt_EXeEvent("Downloading " + Type + " Exe:   " + progressBar1.Value + "   OF   " + progressBar1.Maximum);
                                CheckRepet1 = 0;// Not Catch In Loop If There area Mistake
                                CheckRepet = 0;
                            }
                            else
                            {
                                SetTxt_EXeEvent("Downloading " + Type + " Exe:   CheckSum Is Wrong Try " + CheckRepet);
                                i--;
                                CheckRepet++;
                                Thread.Sleep(3000);
                                if (CheckRepet == 5)
                                {
                                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, connection);
                                    return false;
                                }
                            }
                            UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "", Type, connection);

                        }
                        catch
                        {
                            i--;
                            CheckRepet++;
                            SetTxt_EXeEvent("Downloading " + Type + " Exe:   Download Failed Try " + CheckRepet);
                            Thread.Sleep(3000);
                            if (CheckRepet == 5)
                            {
                                UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, connection);
                                return false;
                            }
                        }
                    }

                    Path = UpdaterFunctions.FindSpecialPathInAllDrives("web");
                    if (Path == "")
                    {
                        UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: Web Path is Empty", Type, AppType, 1, ComputerName, "", true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                        UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, "", true, VersionNumberC, 0, CountOfEachApp, "Error: Web Path is Empty", Type, connection);
                        SetTxt_Error("Error: Web Path is Empty");
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(Path))
                        {
                            try
                            {
                                UpdaterFunctions.WriteZipFile(Filename, Path, WholeImageData);
                                UpdaterFunctions.ExtractFile(Filename, Path, Path);
                                UpdaterFunctions.DeleteZipFile(Path + "\\" + Filename);
                            }
                            catch (Exception EX)
                            {
                                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                                SetTxt_EXeEvent(Type + " Operation Finished");
                            }
                        }
                        else
                        {
                            SetTxt_EXeEvent("Path For Save Web not Exist. Pati : " + Path);
                        }
                    }

                }

                else
                {
                    int TryCount = 0;
                    byte[] WholeImageData = new byte[0];
                    for (int i = 0; i < CountOfEachApp; i++)
                    {
                        if (Rollback)
                        {
                            UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type, Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                            UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type, Type, connection);
                            SetTxt_Error("RolBack: " + Filename + " , Number --> " + i + " , in --> " + Type);
                            return false;
                        }
                        byte[] Package;
                        DataTable Dt1 = new DataTable("EachPart"); //Tables["EachPart"]
                        Boolean isok = true;
                        try
                        {

                            Dt1 = UpdaterFunctions.GetVersionInfoFileFromDataBase(VersionReleaseVApplication, i);
                            Package = (byte[])Dt1.Rows[0]["Bin_FileContent"];
                            TryCount = 0;
                        }
                        catch (Exception ex)
                        {
                            TryCount++;
                            SetTxt_EXeEvent("Downloading " + Type + " Exe:   Error in row Number : " + i + "     Try : " + TryCount);
                            i--;
                            Thread.Sleep(1000);
                            if (TryCount > 9)
                            {
                                SetTxt_EXeEvent("Downloading " + Type + " Exe:   More Than 10 attempt, Check Parsic WebServer");
                                UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Downloading Exe:   More Than 10 attempt, Check Parsic WebServer", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Downloading Exe:   More Than 10 attempt, Check Parsic WebServer", Type, connection);
                                return false;
                            }
                            continue;
                        }
                        //Package = (byte[])Dt1.Rows[0]["Bin_FileContent"];
                        try
                        {
                            string CRC = Dt1.Rows[0]["Str_CRC"].ToString();
                            string FileSize = Dt1.Rows[0]["Str_PartSize"].ToString();
                            //for each part(exept last part) run one time 
                            string check = UpdaterFunctions.CalculateChecksum(Package);
                            if (CRC == check)
                            {
                                try
                                {
                                    //string FileSize = Dt1.Rows[0]["Str_PartSize"].ToString();
                                    //for each part(exept last part) run one time 
                                    var s = new MemoryStream();
                                    s.Write(WholeImageData, 0, WholeImageData.Length);
                                    s.Write(Package, 0, Package.Length);
                                    WholeImageData = s.ToArray();
                                    //progressBar1.Value += 1;
                                    //progressBar1.Refresh();
                                    isok = true;
                                }
                                catch
                                {
                                    isok = false;
                                }


                                if (isok == false)
                                {
                                    i--;
                                    CheckRepet1++;
                                    Thread.Sleep(1000);
                                    if (CheckRepet1 == 5)
                                    {
                                        //UpdaterFunctions.DeleteFromDB(2, Type, Filename, connection);
                                        UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type, Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                        UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type, Type, connection);
                                        return false;
                                    }
                                }
                                else
                                {
                                    //progressBar1.Value += 1;
                                    //progressBar1.Refresh();
                                    AddprogressBar(1);
                                    SetTxt_EXeEvent("Downloading " + Type + " Exe:   " + progressBar1.Value + "   OF   " + progressBar1.Maximum);
                                    CheckRepet1 = 0;// Not Catch In Loop If There area Mistake
                                }
                                CheckRepet = 0;
                            }
                            else
                            {
                                i--;
                                CheckRepet++;
                                Thread.Sleep(1000);
                                if (CheckRepet == 5)
                                {
                                    //UpdaterFunctions.DeleteFromDB(2, Type, Filename, connection);
                                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, connection);
                                    return false;
                                }
                            }
                            UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "", Type, connection);

                        }
                        catch
                        {
                            i--;
                            CheckRepet++;
                            Thread.Sleep(1000);
                            if (CheckRepet == 5)
                            {
                                //UpdaterFunctions.DeleteFromDB(2, Type, Filename, connection);
                                UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, i, CountOfEachApp, "Error: More than 5 attempt to reached download --> " + Filename + " , Number --> " + i + " , in --> " + Type + " And CRC Is Wrong", Type, connection);
                                return false;
                            }
                        }

                    }
                    //if (FierstDel == false)
                    //    UpdaterFunctions.DeleteFromDB(3, Type, Filename, connection);




                    string InnerWholeCRC = UpdaterFunctions.CalculateChecksum(WholeImageData);

                    if (WholeCRC == InnerWholeCRC)
                    {
                        UpdaterFunctions.SaveWholeVersionInLabDB(Type, Filename, VersionNumberC, -1, WholeImageData, "", connection);

                    }

                    //UpdaterFunctions.DeleteFromDB(1, Type, "", connection);



                }



                try
                {
                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Success Full", Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, 0, CountOfEachApp, "Success Full", Type, connection);
                }catch(Exception ex3)
                {

                }
                return true;
            }
            catch (Exception EX)
            {
                try { 
                //MessageBox.Show("Error 7 : " + EX.Message.ToString());
                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, VersionFrom, VersionNumberC, "Error In Exception : " + EX.Message.ToString(), Type, AppType, 1, ComputerName, DB_Name, true, true, Size, connection, user_is_from_Parsic, Convert.ToInt16(LabID));
                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, DB_Name, true, VersionNumberC, 0, CountOfEachApp, "Error Exception : " + EX.Message.ToString(), Type, connection);
            }
            catch (Exception ex3)
            {

            }
            return false;

            }
        }



        public void SetprogressBar(int Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetprogressBar1(SetprogressBar), Num);
            else
            {
                this.progressBar1.Value = Num;
                progressBar1.Refresh();
            }
        }
        public void SetMaxprogressBar(int Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetMaxprogressBar1(SetMaxprogressBar), Num);
            else
            {
                this.progressBar1.Maximum = Num;
                progressBar1.Refresh();
            }
        }
        public void SetTxt_EXeEvent(string Text = "")
        {
            if (this.InvokeRequired)
                this.Invoke(new TxtExeError(SetTxt_EXeEvent), Text);
            else
            {
                this.Txt_ScriptError.AppendText("\r\n\r\n" + Text);
                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, "", true, "", 0, 0, "EventLog", Text, connection);
            }
        }
        public void AddprogressBar(int Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new AddprogressBar1(AddprogressBar), Num);
            else
            {
                try
                {
                    this.progressBar1.Value += Num;
                    progressBar1.Refresh();
                }
                catch (Exception EX)
                {
                    UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                }

            }
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
        public void SetCenteralVersion(string Text)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetLbl_CenteralVersion(SetCenteralVersion), Text);
            else
                this.Lbl_CenteralVersion.Text = Text;
        }
        public void SetDbEXEVersion(string Text)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetLbl_DbEXEVersion(SetDbEXEVersion), Text);
            else
                this.Lbl_DbVersion.Text = Text;
        }
        public void SetScriptVersion(string Text)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetLbl_ScriptVersion(SetScriptVersion), Text);
            else
                this.Lbl_ScriptVersion.Text = Text;
        }
        public bool IsFromLab()
        {

            try
            {
                // find that user is from Parsic company or in lab ,by ip valid,
                string externalip = UpdaterFunctions.GetIPAddress();


                if (externalip == "185.189.122.57\n")
                {
                    //user connected from parsipol company\
                    return true;
                }
                else if (externalip == "81.16.116.84\n")
                {
                    //user connected from parsipol company\
                    return true;
                }
                else if (externalip == "0")
                {
                    UpdaterFunctions.SaveTextExeption("اتصال به اینترنت برقرار نمیباشد، یا لطفا آدرس های زیر را چک نمایید" + "\r\n" + "https://icanhazip.com or http://checkip.dyndns.org/");
                    //MessageBox.Show("اتصال به اینترنت برقرار نمیباشد، یا لطفا آدرس های زیر را چک نمایید" + "\r\n" + "https://icanhazip.com or http://checkip.dyndns.org/", "هشدار");
                    return false;
                    //error in finding ip Valid
                }
                else
                {


                }
                //\ find that user is from Parsic company or in lab by ip valid
            }
            catch (Exception EX)
            {
                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                string ipstatic = UpdaterFunctions.GetIPAddress();
                if (ipstatic == "185.189.122.57")
                {
                    //user connected from parsipol company\
                    return true;
                }
                else if (ipstatic == "81.16.116.84\n")
                {
                    //user connected from parsipol company\
                    return true;
                }
                else if (ipstatic == "0")
                {
                }
                else
                {
                    return false;
                    // user connected from lab\
                }

            }
            return false;
        }
        public String Prerequirement(int Parsic_user_id, string connection, int type = 0)
        {

            try
            {
                string courendPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Updater11.02.txt";
                string Text = System.IO.File.ReadAllText(@courendPath);

                if (type == 0)
                {
                    try
                    {
                        SqlConnection con = new SqlConnection();
                        con.ConnectionString = connection;
                        Boolean Error = false;
                        //string[] Splitter = { "GO ", "Go ", "gO ", "go ", " GO", " Go", " gO", " go", "\nGO", "\nGo", "\ngO", "\ngo", "GO\n", "Go\n", "gO\n", "go\n" };
                        string[] Splitter = { "\r\nGO\r\n", "\r\nGo\r\n", "\r\ngO\r\n", "\r\ngo\r\n" };

                        string[] Commands = Text.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string SubCommand in Commands)
                        {
                            try
                            {
                                SqlCommand cmd = new SqlCommand(SubCommand, con);
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                            catch (Exception EX)
                            {
                                UpdaterFunctions.SaveTextExeption("Error in Execute Script in DB 1 : " + EX.Message.ToString());
                                Error = true;
                                //MessageBox.Show(EX.Message.ToString());
                            }
                        }
                        if (Error == false)
                        {
                            return "Success Ful";

                        }
                    }
                    catch (Exception EX)
                    {
                        UpdaterFunctions.SaveTextExeption("Error in Execute all Scripts in Updater11.02 next to Install Path : " + EX.Message.ToString());
                        return EX.Message.ToString();
                    }

                }
                else if (type == 1)
                {
                    try
                    {
                        int count = 0;
                        SqlConnection con = new SqlConnection();
                        con.ConnectionString = connection;
                        Boolean Error = false;
                        //string[] Splitter = { "GO ", "Go ", "gO ", "go ", " GO", " Go", " gO", " go", "\nGO", "\nGo", "\ngO", "\ngo", "GO\n", "Go\n", "gO\n", "go\n" };
                        string[] Splitter = { "\r\nGO\r\n", "\r\nGo\r\n", "\r\ngO\r\n", "\r\ngo\r\n" };
                        string[] Commands = Text.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                        con.Open();
                        foreach (string SubCommand in Commands)
                        {
                            if (count == 4)
                            {
                                break;
                            }
                            try
                            {
                                count += 1;
                                SqlCommand cmd = new SqlCommand(SubCommand, con);

                                cmd.ExecuteNonQuery();
                                
                            }
                            catch (Exception EX)
                            {
                                UpdaterFunctions.SaveTextExeption("Error in Execute First And Secend Script In Updater11.02 next to Install Path : " + EX.Message.ToString());
                                Error = true;
                                //MessageBox.Show(EX.Message.ToString());
                            }
                        }
                        con.Close();
                        if (Error == false)
                        {
                            return "Success Ful";
                        }
                    }
                    catch (Exception EX)
                    {
                        UpdaterFunctions.SaveTextExeption("Error in Execute Script 1 and 2 go : " + EX.Message.ToString());
                        return EX.Message.ToString();
                    }

                }
            }
            catch (Exception EX)
            {
                UpdaterFunctions.SaveTextExeption("Error in read or featch Updater11.02 next to install path : " + EX.Message.ToString());
                return EX.Message.ToString();
            }
            return "";
        }

        #endregion EXE


        public void EnterVersionScripts(int _Parsicuserid, Int32 _VersionReleaseID, string _VersionNumberC, List<string> GetODBCInfoList)
        {

            string result = "";
            SqlConnection Myconnection = new SqlConnection(connection);
            Myconnection.Open();
            SqlCommand command = Myconnection.CreateCommand();
            command.CommandTimeout = 10800000;
            SqlTransaction transaction;

            transaction = Myconnection.BeginTransaction("SampleTransaction");

            command.Connection = Myconnection;
            command.Transaction = transaction;

            if (_VersionReleaseID == -1)
            {
                if (Rollback) { }
                else
                {
               
                    if (CenteralVerID != -1)
                    {
                    try
                    {
                        To = VersionNumberC;
                        foreach (DataRow dr in DTParsicMaster.Rows)
                        {
                            if ((dr["Is_Present"].ToString() == "True"))
                            {
                                    if (Convert.ToDouble(dr["DBCurruntVersionC"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                    {
                                        DB_version = Convert.ToDouble(dr["DBCurruntVersionC"]);
                                    }
                                
                                    if (_From > Convert.ToDouble(dr["DBCurruntVersionC"]))
                                    {
                                        _From = Convert.ToDouble(dr["DBCurruntVersionC"]);
                                    }
                                }
                            }
                        }


                    }
                    catch (Exception EX)
                    {
                        //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                        UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                    }
                    if (Rollback) { }
                    else
                    {
                        result = EnterOneVersionScripts(_Parsicuserid, CenteralVerID, "Central", 1, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                    }
                    if (result.Contains("RollBack") || result.ToLower().Contains("error"))
                    {
                        transaction.Rollback();
                        SetTxt_EXeEvent("SqlTransaction RollBacked");
                        Rollback = true;
                    }
                    }
                }
                if (Rollback) { }
                else
                {
                    if (QCVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberQ;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionQ"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionQ"]);
                                        }
                                    
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionQ"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionQ"]);
                                        }
                                    }
                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, QCVerID, "QC", 2, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }
                if (Rollback) { }
                else
                {
                    if (StorageVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberS;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionS"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionS"]);
                                        }
                                    
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionS"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionS"]);
                                        }
                                    }
                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, StorageVerID, "Storage", 3,_VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }
                if (Rollback) { }
                else
                {
                    if (JournalVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberJ;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionJ"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionJ"]);
                                        }
                                   
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionJ"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionJ"]);
                                        }
                                    }
                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, JournalVerID, "Journal", 4, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }
                if (Rollback) { }
                else
                {
                    if (TemperatureVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberT;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionT"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionT"]);
                                        }
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionT"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionT"]);
                                        }
                                    }
                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, TemperatureVerID, "Temperature", 5, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }

                if (Rollback) { }
                else
                {
                    if (ParsicLabAndroidVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberP;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionP"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionP"]);
                                        }
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionP"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionP"]);
                                        }
                                    }
                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, ParsicLabAndroidVerID, "Parsic Lab Android", 6, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }

                if (Rollback) { }
                else
                {
                    if (PrinterCacherVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberPr;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionPr"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionPr"]);
                                        }
                                    
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionPr"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionPr"]);
                                        }
                                    }
                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, PrinterCacherVerID, "Printer Cacher", 10, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }


                if (Rollback) { }
                else
                {
                    if (QMaticVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberQM;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionQM"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionQM"]);
                                        }
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionQM"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionQM"]);
                                        }
                                    }
                                    else
                                    {

                                    }

                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, QMaticVerID, "QMatic", 11, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }

                if (Rollback) { }
                else
                {
                    if (WebVerID != -1)
                    {
                        try
                        {
                            To = VersionNumberW;
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["Is_Present"].ToString() == "True"))
                                {
                                    if (Convert.ToDouble(dr["DBCurruntVersionW"]) != 0)
                                    {
                                        if ((dr["DBList_IsActive"].ToString() == "True"))
                                        {
                                            DB_version = Convert.ToDouble(dr["DBCurruntVersionW"]);
                                        }
                                        if (_From > Convert.ToDouble(dr["DBCurruntVersionW"]))
                                        {
                                            _From = Convert.ToDouble(dr["DBCurruntVersionW"]);
                                        }
                                    }

                                }
                            }


                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                            UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                        }
                        if (Rollback) { }
                        else
                        {
                            result = EnterOneVersionScripts(_Parsicuserid, WebVerID, "Web", 12, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                            if (result.Contains("RollBack"))
                            {
                                transaction.Rollback();
                                SetTxt_EXeEvent("SqlTransaction RollBacked");
                                Rollback = true;
                            }
                        }
                    }
                }








                if (!Rollback)
                {
                    transaction.Commit();
                    SetTxt_EXeEvent("SqlTransaction Commited");


                }


            }
            else
            {
                if (Rollback) { }
                else
                {
                    try
                    {
                        To = VersionNumberC;
                        foreach (DataRow dr in DTParsicMaster.Rows)
                        {
                            if ((dr["Is_Present"].ToString() == "True"))
                            {
                                if ((dr["DBList_IsActive"].ToString() == "True"))
                                {
                                    DB_version = Convert.ToDouble(dr["DBCurruntVersionC"]);
                                }
                                if (_From > Convert.ToDouble(dr["DBCurruntVersionC"]))
                                {
                                    _From = Convert.ToDouble(dr["DBCurruntVersionC"]);
                                }
                            }
                        }


                    }
                    catch (Exception EX)
                    {
                        //MessageBox.Show("Error 101 : " + EX.Message.ToString());
                        UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                    }
                }
                if (Rollback) { } else
                {
                    result = EnterOneVersionScripts(_Parsicuserid, _VersionReleaseID, "All", 0, _VersionNumberC, GetODBCInfoList, Myconnection, command, transaction);
                }
                if (result.Contains("RollBack"))
                {
                    transaction.Rollback();
                    SetTxt_EXeEvent("SqlTransaction RollBacked");
                    Rollback = true;
                }
                if (!Rollback)
                {
                    transaction.Commit();
                }

            }
            if (ShowForm)
            {
                ChangeRollBackExitButtonVisible(true);
                MessageBox.Show("All Operations FINISH");
                SetTxt_EXeEvent("All Operations Finished");
            }
            else
            {
                Application.Exit();
            }


        }

        public string EnterOneVersionScripts(int _Parsicuserid, Int32 _VersionReleaseID,string Type,Int16 AppType, string _VersionNumber, List<string> GetODBCInfoList, SqlConnection SqlCon, SqlCommand SqlCom, SqlTransaction SqlTran)
        {
            Parsicuserid = _Parsicuserid;

            try
            {
                UpdaterFunctions.SendBackLog -= ReceiveLogEvent;
                UpdaterFunctions.SendBackLog += ReceiveLogEvent;
            }
            catch
            {

            }

            VersionNumberC = _VersionNumber;
            VersionRelease_ID = _VersionReleaseID;

            


            string CentralPath = UpdaterFunctions.FindFolder("Centeral");
            CenteralVersion = UpdaterFunctions.GetVersion(CentralPath + "\\CenteralApp.exe");
            DbEXEVersion = UpdaterFunctions.GetDBVersion("Centeral", connection);
            ScriptVersion = UpdaterFunctions.GetDBScriptVersion(connection,1);
            MyDL = new MyDelegate(ReceiveLogEvent);


            SetCenteralVersion(CenteralVersion);
            SetDbEXEVersion(DbEXEVersion);
            SetScriptVersion(ScriptVersion);
            //List Of Scripts
            string ans = GetUpdate(AppType,Type,SqlCon,SqlCom,SqlTran);
            return ans;
        }

        private void Txt_ScriptError_TextChanged_1(object sender, EventArgs e)
        {
            Int32 E = Txt_ScriptError.Text.IndexOf("Error", TextStartIndexError);
            TextStartIndexError = E + 5;
            try
            {
                Txt_ScriptError.Select(E, 5);
                Txt_ScriptError.SelectionColor = Color.Red;
            }
            catch
            {

            }
            Int32 R = Txt_ScriptError.Text.IndexOf("RollBacked", TextStartIndexRollBack);
            TextStartIndexRollBack = R + 10;
            try
            {
                Txt_ScriptError.Select(R, 10);
                Txt_ScriptError.SelectionColor = Color.Red;
            }
            catch
            {

            }
            Int32 C = Txt_ScriptError.Text.IndexOf("Commited", TextStartIndexCommit);
            TextStartIndexCommit = C + 8;
            try
            {
                Txt_ScriptError.Select(C, 8);
                Txt_ScriptError.SelectionColor = Color.Green;
            }
            catch
            {

            }
            //Int32 W = Txt_ScriptError.Text.IndexOf("Warning", TextStartIndexWarning);
            //TextStartIndexWarning = W + 7;
            //try
            //{
            //    Txt_ScriptError.Select(W, 7);
            //    Txt_ScriptError.SelectionColor = Color.Yellow;
            //}
            //catch
            //{

            //}






            Txt_ScriptError.SelectionStart = 100000;




        }

        public void ReceiveLogEvent(string Text)
        {
            SetEventLog(Text + "\r\n");
        }

        public string GetUpdate(Int16 AppType, string Type, SqlConnection SqlCon, SqlCommand SqlCom, SqlTransaction SqlTran)
        {
            try
            {
                //Boolean DoIt = false;
        
                From = _From.ToString();

                string Ansver = "";


                Ansver = InnerSaveDescriptionAndScripts(LabID, From, To, Parsicuserid, VersionRelease_ID,Type, AppType, connection, user_is_from_Parsic, DTParsicMaster , WithBackup, SqlCon, SqlCom, SqlTran);
                if (Ansver == "Ok")
                {
                    for (int i = 0; i <= DTParsicMaster.Rows.Count - 1; i++)
                    {
                        if ((DTParsicMaster.Rows[i]["Is_Present"].ToString() == "True"))
                        {
                            UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, From, To, DTParsicMaster.Rows[i]["DBList_Name"].ToString(), Convert.ToInt32(LabID), Convert.ToInt32(Parsicuserid), AppType, 1, " اسکریپت ");
                        }
                    }
                    //Log
                    try { 
                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, From, To, "Scripts", "Success", 0, 1, ComputerName, DB_Name, false, false, 0, connection, user_is_from_Parsic, Convert.ToInt32(LabID));
                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, "Finish", false, From + "-" + To, 0, 0, "Scripts", "Success", connection);
                }catch (Exception ex3)
            {

            }
            //Log\

            //MessageBox.Show("ورژن با موفقیت به " + To + "تغییر یافت");
            UpdaterFunctions.SaveTextExeption("ورژن با موفقیت به " + To + "تغییر یافت");
                    //UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(_Parsicuserid), 3, "");
                    //Txt_ScriptError.Text = Ansver;
                    if(Ansver != "Ok")
                    {
                        SetTxt_Error(Ansver);
                    }
                    try
                    {
                        UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip");
                    }
                    catch { }
                    try
                    {
                        UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip");
                    }
                    catch { }
                    //UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Parsicuserid, 1);


                    try
                    {
                        UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, "", "", false, "", 0, 0, "EventLog", Ansver.Replace(Environment.NewLine, "!@#$%^&**&^%$#@!").Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), connection);
                        SetTxt_EXeEvent( Type + " Operation Finished   \r\n\r\n\r\n\r\n\r\n\r\n");
                        
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        try
                        {
                            if (Chk_ParsicLabAndroid.Checked)
                            {
                                UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip");
                                SetTxt_EXeEvent("ParsicLabAndroid Files And IIS Path didn't RollBack");
                            }
                            if (Chk_Web.Checked)
                            {
                                UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip");
                                SetTxt_EXeEvent("LabOnline Files And IIS Path didn't RollBack");
                            }
                            UpdaterFunctions.StartOrStopService("Start", "W3SVC");
                        }
                        catch { }

                        //if (Chk_ParsicLabAndroid.Checked && File.Exists(UpdaterFunctions.FindSpecialPathInAllDrives("web\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip")))
                        //{
                        //    int ErrorCount = 0;
                        //    if (UpdaterFunctions.clearFolder(UpdaterFunctions.FindSpecialPathInAllDrives("web\\ParsicLabAndroid"))) { ErrorCount += 1; }
                        //    if (UpdaterFunctions.ExtractZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip", UpdaterFunctions.FindSpecialPathInAllDrives("web\\ParsicLabAndroid")) != "") { ErrorCount += 1; }
                        //    if(UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionParsicLabAndroid.zip") == 1) { ErrorCount += 1; }
                        //    if(ErrorCount >= 2)
                        //    {
                        //        SetTxt_EXeEvent("ParsicLabAndroid File Returned");
                        //    }
                        //}
                        //else
                        //{
                        //    SetTxt_EXeEvent("New ParsicLabAndroid Files Don't Change\r\nPlease Chek ParsicLabOnline Path In IIS");
                        //}

                        //if (Chk_Web.Checked && File.Exists(UpdaterFunctions.FindSpecialPathInAllDrives("web\\BackupbinUntilFinishScriptTransactionLabOnline")))
                        //{
                        //    int ErrorCount = 0;
                        //    if(UpdaterFunctions.clearFolder(UpdaterFunctions.FindSpecialPathInAllDrives("web\\LabOnline"))) { ErrorCount += 1; }
                        //    if (UpdaterFunctions.ExtractZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip", UpdaterFunctions.FindSpecialPathInAllDrives("web\\LabOnline")) != "") { ErrorCount += 1; }
                        //    if (UpdaterFunctions.DeleteZipFile(UpdaterFunctions.FindSpecialPathInAllDrives("web") + "\\BackupbinUntilFinishScriptTransactionLabOnline.zip") == 1) { ErrorCount += 1; }
                        //    if (ErrorCount >= 2)
                        //    {
                        //        SetTxt_EXeEvent("LabOnline File Returned");
                        //    }
                        //}
                        //else
                        //{
                        //    SetTxt_EXeEvent("New LabOnline Files Don't Change\r\nPlease Chek LabOnline Path In IIS");
                        //}
                    }
                    catch { }
                    //Log
                    try {
                    UpdaterFunctions.Parsic_user_send_log(user_is_from_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(_Parsicuserid), AppType, 2 ,"", Ansver);
                    UpdaterFunctions.SendLocalVersionReleaseVLog(VersionRelease_ID, Parsicuserid, From, To, Ansver, "Error", 0, 1, ComputerName, "", false, false, 0, connection, user_is_from_Parsic, Convert.ToInt32(LabID));
                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, "Finish", false, From + "-" + To, 0, 0, Ansver, "Error", connection);
                }catch (Exception ex3)
            {

            }
            //Log\
            UpdaterFunctions.SaveTextExeption("Error : " + Type + " Message : " + Ansver);
                    //MessageBox.Show(Ansver, "خطا در اجرای بانک", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Txt_ScriptError.Text = Ansver;
                    SetTxt_Error(Ansver);
                    try
                    {
                        UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, "", "", false, "", 0, 0, "EventLog", Ansver.Replace(Environment.NewLine, "!@#$%^&**&^%$#@!").Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), connection);
                        SetTxt_EXeEvent(Type + " Operation Finished");
                        if (ShowForm)
                        {
                            //ChangeRollBackExitButtonVisible(true);
                            MessageBox.Show( Type + "  STOP \r\n" + Ansver, "Warning");
                        }
                        else
                        {
                            //Application.Exit();
                        }

                    }
                    catch
                    {

                    }
                }


                Thread.Sleep(100);
                //if (ShowForm)
                //{

                //}
                //else
                //{
                //    Application.Exit();
                //}
                return Ansver;
            }
            catch (Exception EX)
            {
                try
                {
                    //ChangeRollBackExitButtonVisible(true);
                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, ComputerName, "Finish", false, "", 0, 0, EX.Message.ToString(), "Error", connection);
                    SetTxt_EXeEvent(Type + " Operation Finished");
                    Thread.Sleep(10000);
                    if (ShowForm)
                    {
                        //ChangeRollBackExitButtonVisible(true);
                        MessageBox.Show("ERROR 1000");
                    }
                    else
                    {
                        //Application.Exit();
                    }
                }
                catch
                {
                }
                //MessageBox.Show("Error 103 : " + EX.Message.ToString());
                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
            }
            return "Error";
        }

        public void SetTxt_Error(string Text)
        {
            if (this.InvokeRequired)
                this.Invoke(new TxtError(SetTxt_Error), Text);
            else
            {
                this.Txt_ScriptError.AppendText("\r\n\r\n" + Text);
                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, "", "", false, "", 0, 0, "EventLog", Text, connection);
            }
        }

        public void SetEventLog(string Text)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetTxt_EventLog(SetEventLog), Text);
            else
            {
                this.Txt_ScriptError.AppendText("\r\n" + Text);
                UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(Parsicuserid, "", "", false, "", 0, 0, "EventLog", Text, connection);
            }

        }

        public void ChangeRollBackExitButtonVisible(Boolean b)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetChangeRollBackExitButtonVisible(ChangeRollBackExitButtonVisible), b);
            else
            {
                Btn_RollBack.Visible = false;
                Btn_Exit.Visible = true;
                Btn_SaveEventLog.Visible = true;
                Btn_Retry.Visible = true;
            }

        }

        //public string InnerSaveDescriptionAndScripts(string LabID, string From, string To, int ParsicUserID, int VersionRelease_ID,string Type,Int16 AppType, string connectionTrueDB, bool User_Is_From_Parsic, SqlConnection SqlCon, SqlCommand SqlCom, SqlTransaction SqlTran)
        //{
        //    try
        //    {
        //        return UpdaterFunctions.SaveDescriptionAndScripts(LabID, From, To, Parsicuserid, VersionRelease_ID,Type, AppType, connection, user_is_from_Parsic, SqlCon, SqlCom, SqlTran);
        //    }
        //    catch (Exception EX)
        //    {
        //        //MessageBox.Show("Error 104 : " + EX.Message.ToString());
        //        UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
        //        return "Error: " + EX.Message.ToString();
        //    }
        //}

        public string InnerSaveDescriptionAndScripts(string LabID, string From, string To, int ParsicUserID, int VersionRelease_ID,string Type, Int16 AppType, string connectionTrueDB, bool User_Is_From_Parsic, DataTable _DT_ParsicMaster,Boolean WithBackup, SqlConnection SqlCon, SqlCommand SqlCom, SqlTransaction SqlTran)
        {
            try
            {
                return UpdaterFunctions.SaveDescriptionAndScripts(LabID,From, To, Parsicuserid, VersionRelease_ID,Type,AppType, connection, user_is_from_Parsic, _DT_ParsicMaster, WithBackup, SqlCon, SqlCom, SqlTran);
            }
            catch (Exception EX)
            {
                //MessageBox.Show("Error 105 : " + EX.Message.ToString());
                UpdaterFunctions.SaveTextExeption(EX.Message.ToString());
                return "Error: " + EX.Message.ToString();
            }
        }


        public Boolean SaveEventLog(string Text, string Path)
        {
            try
            {
                string _Text = "";
                _Text = "Save Log File Date : " + DateTime.Now + "\r\n" + "Central EXE Version In Computer : " + Lbl_CenteralVersion.Text + "          Central EXE Version In DataBase : " + Lbl_DbVersion.Text + "          DataBase Script Version : " + Lbl_ScriptVersion.Text + "\r\n\r\n";
                _Text = _Text + "EXE : " + Chk_Exe.Checked + "\r\n" + "Scripts : " + Chk_Scripts.Checked + "\r\n" + "Central : " + Chk_Centeral.Checked + "\r\n" + "QC : " + Chk_QC.Checked + "\r\n" + "Storage : " + Chk_Storage.Checked + "\r\n" + "Jurnal : " + Chk_Journal.Checked + "\r\n" + "Temperature : " + Chk_Temperature.Checked + "\r\n" + "Parsic Lab Android : " + Chk_ParsicLabAndroid.Checked + "\r\n\r\n";
                if (ShowForm)
                {
                    _Text = _Text + "Update From : Lab \r\n";
                    _Text = _Text + "Confermer : " + ConfermerPerson + "\r\n\r\n\r\n";
                }
                else
                {
                    _Text = _Text + "Update From : Ticketing \r\n\r\n\r\n";
                }

                Text = _Text + Text + "\r\n\r\n\r\n\r\n";

                File.AppendAllText(Path, Text + Environment.NewLine);
                return true;
            }
            catch (Exception EX)
            {
                return false;
            }

        }


        #endregion
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝





























    }

}