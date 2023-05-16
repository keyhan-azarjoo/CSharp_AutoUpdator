using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public partial class Frm_Offline_PreperToUpdate : Form
    {
        Int32 ParsicUserID = -1;
        public delegate void TxtExeError(string Text);
        public delegate void ProxresBarMax(Int32 Num);
        public delegate void ProxresBarVal(Int32 Num);
        string DBName = "";
        string VersionFrom = "";
        string VersionTo = "";
        string LabID = "";
        Boolean FromParsic = false;
        string ComputerName = "";
        Boolean IsExe = true;
        Int32 Number = 0;
        public delegate void MyDelegate(string Text);

        MyDelegate MyDL;

        Thread t;
        Boolean RollBack = false;
        string MyConnection = "";
        UpdaterClasses.GetAndInsertVersionInDB UpdaterFunctions = new UpdaterClasses.GetAndInsertVersionInDB(0);
        DataTable DTParsicMaster = new DataTable("Db_ParsicMaster");
        public Frm_Offline_PreperToUpdate()
        {
            InitializeComponent();
        }
        public Frm_Offline_PreperToUpdate(Int32 _ParsicUserID)
        {
            ParsicUserID = _ParsicUserID;
            InitializeComponent();
        }


        private void Frm_Offline_PreperToUpdate_Load(object sender, EventArgs e)
        {

        }

        private void Btn_GetExePath_Click(object sender, EventArgs e)
        {

            openFileDialog1.FileName = "Not Matter";
            openFileDialog1.Filter = "Zip files (*.Zip)|*.Zip";
            openFileDialog1.Title = "Open Exe File";
            openFileDialog1.ShowDialog();
            Txt_ExePath.Text = openFileDialog1.FileName;

        }

        private void Btn_GetScriptExePath_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "Not Matter";
            openFileDialog1.Filter = "XML File (*.Xml)|*.Xml";
            openFileDialog1.Title = "Open Script File";
            openFileDialog1.ShowDialog();
            Txt_ScriptPath.Text = openFileDialog1.FileName;
        }
        private void Btn_GetDescriptionPath_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "Not Matter";
            openFileDialog1.Filter = "XML File (*.Xml)|*.Xml";
            openFileDialog1.Title = "Open Script File";
            openFileDialog1.ShowDialog();
            Txt_DescriptionPath.Text = openFileDialog1.FileName;
        }


        private void Btn_Run_Click(object sender, EventArgs e)
        {
            t = new Thread(new ThreadStart(Run));
            t.Start();
            if (Txt_ExePath.Text == "" && Txt_ScriptPath.Text == "" && Txt_DescriptionPath.Text == "")
            {
                return;
            }
            Btn_Run.Visible = false;
            Btn_RollBack.Visible = true;


        }

        public void Run()
        {

            ComputerName = System.Environment.MachineName.ToString();
            FromParsic = UpdaterFunctions.IsUserFromParsic();


            if (Txt_ExePath.Text != "")
            {
                if (Txt_ExePath.Text.Contains("ExeFiles_") == false)
                {
                    SetTxt_EXeEvent("لطفا فایل زیپ اگزه را به درستی وارد نمایید");
                }
                else
                {
                    try
                    {
                        MyConnection = MackeConnection();
                        UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "", "Start Exe Offline", -1, -1, ComputerName, DBName, true, true, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                        SetTxt_EXeEvent("شروع ذخیره فایل ها");
                        string Exepath = @Txt_ExePath.Text;
                        string ExeSubpath = "";
                        string FileName = "";

                        string[] ExeSubPath = Exepath.Split();
                        int index = 0;
                        int i = 0;
                        foreach (char c in Exepath)
                        {
                            if (c == '\\')
                            {
                                index = i;
                                FileName = "";
                            }
                            i += 1;
                            FileName += c;
                        }
                        ExeSubpath = Exepath.Substring(0, index);


                        FileName = FileName.Replace(".zip", ".txt");
                        SetTxt_EXeEvent("شروع به باز کردن فایل زیپ");
                        if (UpdaterFunctions.ExtractZipFile(Exepath, ExeSubpath) == "")
                        {
                            SetTxt_EXeEvent("در باز کردن فایل زیپ خطایی وجود دارد");
                        }
                        else
                        {
                            SetTxt_EXeEvent("فایل زیپ باز شد");
                        }

                        byte[] b;
                        using (FileStream fs = File.Open(ExeSubpath + FileName, FileMode.Open))
                        {
                            b = new byte[fs.Length];
                            UTF8Encoding temp = new UTF8Encoding(true);
                            fs.Read(b, 0, b.Length);
                        }
                        SetTxt_EXeEvent("فایل خوانده شد");

                        DataSet DsExe = new DataSet("DT");

                        DsExe.ReadXml(ExeSubpath + FileName);

                        Int32 AllFileCount = DsExe.Tables["ExeFile"].Rows.Count;

                        DsExe.Tables["ExeFile"].Columns.Add("Bin_FileContent");
                        Boolean ExeError = false;
                        for (Int16 j = 0; j < DsExe.Tables["ExeFile"].Rows.Count; j++)
                        {

                            byte[] bb = new byte[Convert.ToInt64(DsExe.Tables["ExeFile"].Rows[j]["Str_PartSize"])];
                            string s = DsExe.Tables["ExeFile"].Rows[j]["ContentBase64"].ToString();
                            DsExe.Tables["ExeFile"].Rows[j]["Bin_FileContent"].ToString();
                            byte[] imageBytes = Convert.FromBase64String(s);
                            DsExe.Tables["ExeFile"].Rows[j]["Bin_FileContent"] = imageBytes;
                            //DsExe.Tables["ExeFile"].Rows[j]["ContentBase64"] = "";
                            string CheckSum = UpdaterFunctions.CalculateChecksum(imageBytes);
                            if (DsExe.Tables["ExeFile"].Rows[j]["Str_CRC"].ToString() != CheckSum)
                            {
                                ExeError = true;
                                MessageBox.Show("فایل ها دارای مشکل میباشند", "هشدار");
                                SetTxt_EXeEvent("فایل دارای مشکل میباشد " + j.ToString());

                                break;
                            }
                        }

                        if (ExeError == false)
                        {
                            SetTxt_EXeEvent("شروع به ذخیره فایل ها در بانک \r\n\r\n\r\n\r\n");
                            string Type = "";
                            for(Int16 l = 0; l < AllFileCount; l++)
                            {
                                if(DsExe.Tables["ExeFile"].Rows[l]["Str_Type"].ToString() == Type)
                                {

                                }
                                else
                                {
                                    UpdaterFunctions.DeleteFromDB(1, DsExe.Tables["ExeFile"].Rows[l]["Str_Type"].ToString(), "", MyConnection);
                                    Type = DsExe.Tables["ExeFile"].Rows[l]["Str_Type"].ToString();
                                }

                            }

                            SetProxresBarMax(AllFileCount);
                            for (int k = 0; k < DsExe.Tables["ExeFile"].Rows.Count; k++)
                            {
                                string MyType = DsExe.Tables["ExeFile"].Rows[k]["Str_Type"].ToString();
                                string MyFileName = DsExe.Tables["ExeFile"].Rows[k]["Str_FileName"].ToString();
                                string MyFileSize = DsExe.Tables["ExeFile"].Rows[k]["Str_PartSize"].ToString();
                                string MyCRS = DsExe.Tables["ExeFile"].Rows[k]["Str_CRC"].ToString();
                                string MyVersionNumber = DsExe.Tables["ExeFile"].Rows[k]["Str_VersionNo"].ToString();
                                VersionTo = MyVersionNumber;
                                int MyPart = Convert.ToInt32(DsExe.Tables["ExeFile"].Rows[k]["Int_PartNo"]);
                                string s = DsExe.Tables["ExeFile"].Rows[k]["ContentBase64"].ToString();
                                byte[] MyContent = Convert.FromBase64String(s);

                                string MyDescription = DsExe.Tables["ExeFile"].Rows[k]["Str_Description"].ToString();


                                if (SaveVersionInLabDBVComit(MyType, MyFileName, MyFileSize, MyCRS, MyVersionNumber, MyPart, MyContent, MyDescription, MyConnection) == false)
                                {
                                    UpdaterFunctions.DeleteFromDB(1, MyType, MyFileName, MyConnection);
                                    SetTxt_EXeEvent("خطا در ثبت اطلاعات");
                                    SetProxresBarVal(progressBar1.Maximum);
                                    MessageBox.Show("خطا در ثبت فایل، لطفا دوباره تلاش نمایید", "هشدار");
                                    break;
                                }
                                if (RollBack)
                                {
                                    UpdaterFunctions.DeleteFromDB(1, MyType, MyFileName, MyConnection);
                                    SetTxt_EXeEvent("\r\n\r\n\r\n\r\n ذخیره باقی فایل ها کنسل شد \r\n\r\n\r\n\r\n");
                                    SetProxresBarVal(progressBar1.Maximum);
                                    break;
                                }
                                string Mess = k.ToString() + " -           " + MyType;
                                string Space = "";
                                for(int j = 0; j < 50 - Mess.Length; j++)
                                {
                                    Space += " ";
                                }
                                SetTxt_EXeEvent(Mess + Space + (Convert.ToInt32(MyPart)+1).ToString());
                                SetProxresBarVal(k + 1);

                            }
                            SetTxt_EXeEvent("\r\n\r\nپایان ذخیره فایل ها \r\n\r\n\r\n\r\n");
                            if (RollBack)
                            {
                                UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "ROLL BACK", "Finish Exe Offline", -1, -1, ComputerName, DBName, true, true, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                            }
                            else
                            {
                                UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "", "Finish Exe Offline", -1, -1, ComputerName, DBName, true, true, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                            }

                            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
                            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
                            SetTxt_EXeEvent("---------------------------------------------   Finish EXE   ----------------------------------------------");
                            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
                            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------\r\n\r\n\r\n\r\n");

                        }


                    }
                    catch (Exception ex)
                    {

                    }
                }
            }


            if (RollBack)
            {
                return;
            }

            IsExe = false;
            if (Txt_DescriptionPath.Text != "")
            {
                if (Txt_DescriptionPath.Text.Contains("Descriptions_") == false)
                {
                    SetTxt_EXeEvent("لطفا فایل توضیحات را به درستی وارد نمایید");
                }
                else
                {
                    UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "", "Start Descriptions Offline", -1, -1, ComputerName, DBName, true, false, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                    try
                    {
                        SetTxt_EXeEvent("شروع عملیات ذخیره توضیحات");

                        if (MyConnection == "")
                        {
                            MyConnection = MackeConnection();
                        }
                        string Descriptionpath = @Txt_DescriptionPath.Text;
                        string[] DescriptionSubPath = Descriptionpath.Split();

                        DataSet DsDescriptions = new DataSet("DT");
                        DsDescriptions.ReadXml(Descriptionpath);
                        SetTxt_EXeEvent("توضیحات بارگزاری شدند");

                        DataTable DTLabVer = new DataTable("DT");
                        DTLabVer = GetLabVersions(MyConnection);
                        string CheckAns = CheckVersionValidationForDescription(DTLabVer, DsDescriptions.Tables["Descriptions"]);
                        if (CheckAns != "OK")
                        {
                            SetTxt_EXeEvent(CheckAns);
                            SetTxt_EXeEvent("پایان عملیات ذخیره توضیحات");
                            UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "ERROR", "Finish Descriptions Offline", -1, -1, ComputerName, DBName, true, false, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                            //t.Suspend();
                        }
                        else
                        {
                            ExecuteAllDescriptions(DTLabVer, DsDescriptions.Tables["Descriptions"], MyConnection);
                            UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "", "Finish Descriptions Offline", -1, -1, ComputerName, DBName, true, false, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }

            }


            if (RollBack)
            {
                return;
            }


            if(Txt_ScriptPath.Text != "")
            {
                if(Txt_ScriptPath.Text.Contains("Scripts_") == false)
                {
                    SetTxt_EXeEvent("لطفا فایل اسکریپت ها را به درستی وارد نمایید");
                }
                else
                {
                    try
                    {
                        SetTxt_EXeEvent("شروع عملیات اسکریپت زدن");

                        if (MyConnection == "")
                        {
                            MyConnection = MackeConnection();
                        }
                        string Scriptpath = @Txt_ScriptPath.Text;
                        string[] ScriptSubPath = Scriptpath.Split();

                        DataSet DsScripts = new DataSet("DT");
                        DsScripts.ReadXml(Scriptpath);
                        SetTxt_EXeEvent("اسکریپت ها بارگزاری شدند");

                        DataTable DTLabVer = new DataTable("DT");
                        DTLabVer = GetLabVersions(MyConnection);
                        string CheckAns = CheckVersionValidationForScripts(DTLabVer, DsScripts.Tables["Scripts"]);
                        if (CheckAns != "OK")
                        {
                            SetTxt_EXeEvent(CheckAns);
                            SetTxt_EXeEvent("پایان عملیات ورژن زدن");
                            t.Suspend();
                            return;
                        }

                        ExecuteAllScripts(DTLabVer, DsScripts.Tables["Scripts"], MyConnection);


                    }
                    catch (Exception ex)
                    {

                    }
                }
                
            }


            SetTxt_EXeEvent("");
            SetTxt_EXeEvent("");
            SetTxt_EXeEvent("");
            SetTxt_EXeEvent("");
            SetTxt_EXeEvent("");

            t.Suspend();
        }



        public string ExecuteAllScripts(DataTable DTLabVersion, DataTable DTScripts, string connection)
        {
            SqlConnection Myconnection = new SqlConnection(connection);
            string ans = "";
            try
            {
                Myconnection.Open();
                SqlCommand command = Myconnection.CreateCommand();
                command.CommandTimeout = 10800000;
                SqlTransaction transaction;
                transaction = Myconnection.BeginTransaction("SampleTransaction");
                command.Connection = Myconnection;
                command.Transaction = transaction;
                 
                Int32 AppType = 0;
                double LabAppVersion = 0;
                string Type = "";
                try
                {
                VersionFrom = DTScripts.Rows[0]["Str_VersionNumber"].ToString();
                VersionTo = DTScripts.Rows[DTScripts.Rows.Count-1]["Str_VersionNumber"].ToString();
                    UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "", "Start Scripts Offline", -1, -1, ComputerName, DBName, true, false, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                }
                catch
                {

                }


                for (Int32 i = 0; i < DTLabVersion.Rows.Count; i++)
                {
                    
                    switch (DTLabVersion.Rows[i]["Option_ID"].ToString())
                    {

                        case "DataBaseVer":
                            AppType = 1;
                            LabAppVersion = Convert.ToDouble(DTLabVersion.Rows[i]["Option_Value"]);
                            Type = "Central";
                            VersionFrom = LabAppVersion.ToString();
                            break;

                        case "QualityControlDB_Ver":
                            AppType = 2;
                            LabAppVersion = Convert.ToDouble(DTLabVersion.Rows[i]["Option_Value"]);
                            Type = "QC";
                            break;

                        case "StorageDB_Ver":
                            AppType = 3;
                            LabAppVersion = Convert.ToDouble(DTLabVersion.Rows[i]["Option_Value"]);
                            Type = "Storage";
                            break;

                        case "JournalDB_Ver":
                            AppType = 4;
                            LabAppVersion = Convert.ToDouble(DTLabVersion.Rows[i]["Option_Value"]);
                            Type = "Journal";
                            break;

                        case "ParsicLabAndroid":
                            AppType = 6;
                            LabAppVersion = 1.1;
                            Type = "ParsicLabAndroid";
                            break;

                        case "PrintCatcherDB_Ver":
                            AppType = 10;
                            LabAppVersion = Convert.ToDouble(DTLabVersion.Rows[i]["Option_Value"]);
                            Type = "PrintCatcher";
                            break;

                        case "QMatic_Ver":
                            AppType = 11;
                            LabAppVersion = 1.1;
                            Type = "QMatic";
                            break;
                    }

                    if( AppType != 0)
                    {
                        string Script = "";
                        string VerNumber = "";
                        string ScriptNum = "";

                        for(Int16 DBNum = 0; DBNum < DTParsicMaster.Rows.Count; DBNum++)
                        {
                            if( Convert.ToBoolean(DTParsicMaster.Rows[DBNum]["Is_Present"]) == true)
                            {
                                DBName = DTParsicMaster.Rows[DBNum]["DBList_Name"].ToString();
                                if(AppType == 1)
                                {
                                    LabAppVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionC"]);
                                }
                                if (AppType == 2)
                                {
                                    LabAppVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionQ"]);
                                }
                                if (AppType == 3)
                                {
                                    LabAppVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionS"]);
                                }
                                if (AppType == 4)
                                {
                                    LabAppVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionJ"]);
                                }
                                if (AppType == 6)
                                {
                                    LabAppVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionP"]);
                                }
                                if (AppType == 10)
                                {
                                    LabAppVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionPr"]);
                                }
                                if (AppType == 11)
                                {
                                    LabAppVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionQM"]);
                                }








                                SetTxt_EXeEvent("\r\n\r\n\r\n نام بانک : " + DBName);
                                SetTxt_EXeEvent("Type : " + Type + "\r\n\r\n\r\n");


                                for (int j = 0; j < DTScripts.Rows.Count; j++)
                                {
                                    if(Convert.ToDouble(DTScripts.Rows[j]["Str_VersionNumber"]) > LabAppVersion && Convert.ToInt32(DTScripts.Rows[j]["Int_AppType"]) == AppType )
                                    {
                                        Script = DTScripts.Rows[j]["str_Script"].ToString().Replace("!@#$%^&**&^%$#@!", Environment.NewLine).Replace("$#@!!@#$", "'").Replace("*&^%%^&*", "-");
                                        VerNumber = DTScripts.Rows[j]["Str_VersionNumber"].ToString();
                                        ScriptNum = DTScripts.Rows[j]["int_Order"].ToString();
                                        string tmp = "USE [" + DBName + "]\r\n\r\n" + Script;
                                        //string[] Splitter = { "GO ", "Go ", "gO ", "go ", " GO", " Go", " gO", " go", "\nGO", "\nGo", "\ngO", "\ngo", "GO\n", "Go\n", "gO\n", "go\n" };
                                        string[] Splitter = { "\r\nGO\r\n", "\r\nGo\r\n", "\r\ngO\r\n", "\r\ngo\r\n" };
                                        string[] Commands = tmp.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

                                        foreach (string SubCommand in Commands)
                                        {
                                            try
                                            {
                                                //DScommand.CommandText = SubCommand;
                                                command.CommandText = SubCommand;
                                                if (RollBack == false)
                                                {
                                                    //DScommand.ExecuteNonQuery();
                                                    command.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    //sqlDSTran.Rollback();
                                                    //SqlTran.Rollback();
                                                    string strLogMessage = "رول بک : " + Type + "    نام بانک : " + DBName + "                   " + DateTime.Now.ToString() + "\r\n\r\n";
                                                    SetTxt_EXeEvent(strLogMessage);
                                                    break;
                                                    //return strLogMessage;
                                                }

                                            }
                                            catch (Exception EX)
                                            {
                                                //SetTxt_EXeEvent("اررور 50 : اسکریپت ها رول بک شد. \r\n" + EX.Message.ToString());
                                                try
                                                {
                                                    RollBack = true;
                                                    //transaction.Rollback();
                                                }
                                                catch
                                                {

                                                }
                                                string strLogMessage = "رول بک :  DBName : " + DBName + "                   " + DateTime.Now.ToString() + ".\r\n\r\nVersion Number : " + VerNumber + "         Type : " + Type + "        Script Number : " + ScriptNum + "\r\n\r\n\r\n\r\n دستور اصلی :\r\n\r\n\r\n\r\n" + Script + "\r\n\r\n\r\n\r\nپیغام خطا :\r\n\r\n\r\n\r\n" + EX.Message.ToString() + "\r\n\r\n\r\n\r\n";
                                                SetTxt_EXeEvent(strLogMessage);
                                                //return strLogMessage;
                                            }
                                        }
                                        if(RollBack)
                                        {
                                            break;
                                        }
                                        SetTxt_EXeEvent("اسکریپت : " + Type + "      نام بانک : " + DBName + "      Version: " + VerNumber + "     Number : " + ScriptNum );

                                    }
                 
                                }
                                if (RollBack)
                                {
                                    break;
                                }
                            }
                        }
                        if (RollBack)
                        {
                            break;
                        }

                    }
                    AppType = 0;

                }


                if (!RollBack)
                {
                    transaction.Commit();
                    //transaction.Rollback();
                    Myconnection.Close();
                    SetTxt_EXeEvent("\r\n\r\n\r\n\r\nتمام اسکریپت های مورد نظر اعمال شدند\r\n\r\n");
                    UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "", "Finish Scripts Offline", -1, -1, ComputerName, DBName, true, false, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                }
                else
                {
                    transaction.Rollback();
                    Myconnection.Close();
                    SetTxt_EXeEvent("\r\n\r\n\r\n\r\nتمام اسکریپت های مورد نظر رول بک شدند\r\n\r\n");
                    UpdaterFunctions.SendLocalVersionReleaseVLog(-1, ParsicUserID, VersionFrom, VersionTo, "ERROR", "Finish Scripts Offline", -1, -1, ComputerName, DBName, true, false, 0, MyConnection, FromParsic, Convert.ToInt32(LabID));

                }



            }
            catch(Exception ex)
            {
                Myconnection.Close();
                SetTxt_EXeEvent("\r\n\r\n\r\n\r\nخطا در ارتباط با بانک آزمایشگاه برای اعمال اسکریپت ها \r\n"+ ex.Message.ToString()  +" \r\n\r\n");

            }

            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
            SetTxt_EXeEvent("--------------------------------------------   Finish Scripts   -------------------------------------------");
            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------\r\n\r\n\r\n\r\n");



            return ans;
        }
        public void ReceiveLogEvent(string Text)
        {
            //SetTxt_EXeEvent(Text);
        }


        public string ExecuteAllDescriptions(DataTable DTLabVersion, DataTable DTDescriptions, string connection)
        {
            SqlConnection Myconnection = new SqlConnection(connection);
            string ans = "";
            try
            {
                Myconnection.Open();
                SqlCommand command = Myconnection.CreateCommand();
                command.CommandTimeout = 10800000;
                SqlTransaction transaction;
                transaction = Myconnection.BeginTransaction("SampleTransaction");
                command.Connection = Myconnection;
                command.Transaction = transaction;

                double LabCentralVersion = 0;


                string Script = "";
                string VerNumber = "";
                string ScriptNum = "";
                string DBName = "";
                for (Int16 DBNum = 0; DBNum < DTParsicMaster.Rows.Count; DBNum++)
                {
                    if (Convert.ToBoolean(DTParsicMaster.Rows[DBNum]["Is_Present"]) == true && Convert.ToBoolean(DTParsicMaster.Rows[DBNum]["DBList_IsActive"]) == true)
                    {
                        DBName = DTParsicMaster.Rows[DBNum]["DBList_Name"].ToString();
                            LabCentralVersion = Convert.ToDouble(DTParsicMaster.Rows[DBNum]["DBCurruntVersionC"]);
                        break;
                    }
                }
                DataTable DT = new DataTable("DT");

                DT = GetLastDescription(connection);
                double lastVersion = Convert.ToDouble(DT.Rows[0]["Str_VersionNumber"]);
                double MyVersion = 0;
                Int32 LastOrder = Convert.ToInt32(DT.Rows[0]["Int_Order"]);
                Int32 MyOrder = 0;

                SetTxt_EXeEvent("آخرین ورژن توضیحات : " + lastVersion.ToString());
                SetTxt_EXeEvent("آخرین شماره توضیحات : " + LastOrder.ToString());
                SetTxt_EXeEvent("درحال بارگزاری توضیحات");

                for (Int32 j = 0; j < DTDescriptions.Rows.Count; j++)
                {
                    MyVersion = Convert.ToDouble(DTDescriptions.Rows[j]["Str_VersionNumber"]);
                    MyOrder = Convert.ToInt32(DTDescriptions.Rows[j]["int_Order"]);
                    
                    if(lastVersion > MyVersion)
                    {
                        DTDescriptions.Rows[j].Delete();
                        j--;
                    }
                    else if(lastVersion == MyVersion)
                    {
                        if(LastOrder >= MyOrder)
                        {
                            DTDescriptions.Rows[j].Delete();
                            j--;
                        }
                        else
                        {
                            SetTxt_EXeEvent("Version : " + MyVersion + "       Number : " + MyOrder);
                        }
                    }
                    else
                    {
                        SetTxt_EXeEvent("Version : " + MyVersion + "       Number : " + MyOrder);
                    }

                }
                if(DTDescriptions.Rows.Count == 0)
                {
                    SetTxt_EXeEvent("توضیحات جدیدی وجود نداد");
                }
                else
                {

                    SetTxt_EXeEvent("توضیحات مورد نیاز بارگزاری شد");
                    try
                    {
                        UpdaterFunctions.SendBackLog -= ReceiveLogEvent;
                        UpdaterFunctions.SendBackLog += ReceiveLogEvent;
                    }
                    catch
                    {

                    }
                    SetTxt_EXeEvent("شروع به ذخیره توضیحات");
                    MyDL = new MyDelegate(ReceiveLogEvent);
                    UpdaterFunctions.SaveDescriptions(DTDescriptions, DBName, -1, "", Myconnection, command, transaction);

                    if (!RollBack)
                    {
                        transaction.Commit();
                        //transaction.Rollback();
                        Myconnection.Close();
                        SetTxt_EXeEvent("\r\n\r\n\r\n\r\nتمام توضیحات مورد نظر اعمال شدند\r\n\r\n");
                    }
                    else
                    {
                        transaction.Rollback();
                        Myconnection.Close();
                        SetTxt_EXeEvent("\r\n\r\n\r\n\r\nتمام توضیحات مورد نظر رول بک شدند\r\n\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Myconnection.Close();
                SetTxt_EXeEvent("\r\n\r\n\r\n\r\nخطا در ارتباط با بانک آزمایشگاه برای ذخیره توضیحات \r\n" + ex.Message.ToString() + " \r\n\r\n");
            }

            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
            SetTxt_EXeEvent("-----------------------------------------   Finish Descriptions   -----------------------------------------");
            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------");
            SetTxt_EXeEvent("-----------------------------------------------------------------------------------------------------------\r\n\r\n\r\n\r\n");



            return ans;
        }



        public string MackeConnection()
        {
            DTParsicMaster = new DataTable("DTParsicMaster");

            List<string> GetODBCInfoList = new List<string>();
            GetODBCInfoList = UpdaterFunctions.GetODBCInformation();
            DTParsicMaster = UpdaterFunctions.UpdateGridView(GetODBCInfoList);

            string DB_ServerName = "";
            string DB_Name = "";
            string DB_Username = "";
            string DB_Password = "";
            
            double DBversion = 0;
            string connection = "";
            LabID = UpdaterFunctions.GetLabID(DTParsicMaster);

            try
            {
                foreach (DataRow dr in DTParsicMaster.Rows)
                {
                    if ((dr["Is_Present"].ToString() == "True"))
                    {
                        if ((dr["DBList_IsActive"].ToString() == "True"))
                        {
                            DB_ServerName = dr["DBList_Server"].ToString();
                            DB_Name = dr["DBList_Name"].ToString();
                            DB_Username = dr["DBList_Username"].ToString();
                            DB_Password = dr["DBList_Password"].ToString();
                            DBversion = Convert.ToDouble(dr["DBCurruntVersionC"]);
                            connection = UpdaterFunctions.MakeConnectionString(DB_ServerName, DB_Name, DB_Username, DB_Password, 10);
                            break;
                        }
                    }
                }
            }
            catch (Exception EX)
            {
                //MessageBox.Show("Error 1 : " + EX.Message.ToString());
                UpdaterFunctions.SaveTextExeption("اررور در ارتباط با بانک پارسیک مستر : " + EX.Message.ToString());
            }
            return connection;
        }


        public Boolean SaveVersionInLabDBVComit(string Type, string FileName, string FileSize, string CRC, string VersionNumber, int part, byte[] Contant, string Description, string connection)
        {
            //string query = "INSERT INTO Tbl_VersionFiles (Str_Type, Str_FileName, Str_FileSize ,Str_CRC ,Str_VersionNo , Int_PartNo, Bin_FileContent, Str_Description ) VALUES ( @Str_Type, @Str_FileName , @Str_FileSize, @Str_CRC ,@Str_VersionNo, @Int_PartNo, @Bin_FileContent , @Str_Description )";
            try
            {
                string query = "execute SP_Insert_VersionFiles @Str_Type = @Str_Typec, @Str_FileName = @Str_FileNamec, @Str_FileSize = @Str_FileSizec, @Str_CRC = @Str_CRCc, @Str_VersionNo = @Str_VersionNoc, @Int_PartNo = @Int_PartNoc, @Bin_FileContent = @Bin_FileContentc, @Str_Description = @Str_Descriptionc";
                SqlConnection Con = new SqlConnection(connection);
                SqlCommand command = new SqlCommand(query,Con);

                command.Parameters.AddWithValue("@Str_Typec", Type);
                command.Parameters.AddWithValue("@Str_FileNamec", FileName);
                command.Parameters.AddWithValue("@Str_FileSizec", FileSize);
                command.Parameters.AddWithValue("@Str_CRCc", CRC);
                command.Parameters.AddWithValue("@Str_VersionNoc", VersionNumber);
                command.Parameters.AddWithValue("@Int_PartNoc", part);
                command.Parameters.AddWithValue("@Bin_FileContentc", Contant);
                command.Parameters.AddWithValue("@Str_Descriptionc", Description);

                Con.Open();

                command.ExecuteNonQuery();
                Con.Close();
                return true;
            }
            catch (Exception EX)
            {
                UpdaterFunctions.SaveTextExeption("Error 28 : " + EX.Message.ToString());
                SetTxt_EXeEvent("اررور در ارتباط با بانک برای ذخیره فایل ها\r\n\r\n" + EX.Message.ToString());
                return false;
            }
        }

        public DataTable GetLabVersions(string Connection)
        {
            DataTable DT = new DataTable();
            SqlConnection con = new SqlConnection(Connection);
            try
            {
                string query = "select * from cTBL_Option";
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();
                ad.Fill(DT);
                con.Close();
            }
            catch(Exception EX)
            {
                SetTxt_EXeEvent("اررور در ارتباط با بانک برای دریافت ورژن های آزمایشگاه\r\n\r\n" + EX.Message.ToString());
                con.Close();
            }
            return DT;
        }

        public DataTable GetLastDescription(string connection)
        {
            DataTable DT = new DataTable();
            SqlConnection con = new SqlConnection(connection);
            try
            {
                string query = "select top 1 * from Tbl_VersionReleaseVDescription  order by prk_VersionReleaseVDescription_AutoID desc";
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();
                ad.Fill(DT);
                con.Close();
            }
            catch (Exception EX)
            {
                SetTxt_EXeEvent("اررور در ارتباط با بانک برای دریافت آخرین توضیحات آزمایشگاه\r\n\r\n" + EX.Message.ToString());
                con.Close();
            }
            return DT;
        }

        public string CheckVersionValidationForScripts(DataTable DTLabVer, DataTable DTOfflineVer)
        {
            string ans = "";

            try
            {
                double LessLabVer = 99.99;
                double LessOfflineVer = 99.99;
                LessOfflineVer = Convert.ToDouble(DTOfflineVer.Rows[0]["Str_VersionNumber"]);
                for(Int16 i = 0; i < DTParsicMaster.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(DTParsicMaster.Rows[i]["Is_Present"]))
                    {
                        if (Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionC"]) < LessLabVer)
                        {
                            LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionC"]);
                        }
                        if (Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionQ"]) < LessLabVer)
                        {
                            LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionQ"]);
                        }
                        if (Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionS"]) < LessLabVer)
                        {
                            LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionS"]);
                        }
                        if (Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionJ"]) < LessLabVer)
                        {
                            LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionJ"]);
                        }
                        //if (Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionP"]) < LessLabVer)
                        //{
                        //    LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionP"]);
                        //}
                        //if (Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionPr"]) < LessLabVer)
                        //{
                        //    LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionPr"]);
                        //}
                        //if (Convert.ToDouble(DTParsicMaster.Rows[i]["DB_CurruntVersionQM"]) < LessLabVer)
                        //{
                        //    LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DB_CurruntVersionQM"]);
                        //}
                    }
                }
                VersionFrom = LessLabVer.ToString();
                if (LessLabVer+1 >= LessOfflineVer)
                {
                    ans = "OK";
                }
                else
                {
                    ans = "پایین ترین ورژن آفلاین " + LessOfflineVer.ToString() + " میباشد، در حالی که پایین ترین ورژن آزمایشگاه " + LessLabVer.ToString() + "میباشد. لطفا از ورژنی پایین تر از ورژن آزمایشگاه شروع نمایید" + "\r\n علاوه بر ورژن ها در جدول آپشن، ورژن پارسیک لب اندروید را نیز چک نمایید";

                }
            }
            catch(Exception EX)
            {

            }

            return ans;
        }

        public string CheckVersionValidationForDescription(DataTable DTLabVer, DataTable DTOfflineVer)
        {
            string ans = "";

            try
            {
                double LessLabVer = 99.99;
                double LessOfflineVer = 99.99;
                LessOfflineVer = Convert.ToDouble(DTOfflineVer.Rows[0]["Str_VersionNumber"]);
                for (Int16 i = 0; i < DTParsicMaster.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(DTParsicMaster.Rows[i]["Is_Present"]) && Convert.ToBoolean(DTParsicMaster.Rows[i]["DBList_IsActive"]))
                    {
                        LessLabVer = Convert.ToDouble(DTParsicMaster.Rows[i]["DBCurruntVersionC"]);
                        break;
                    }
                }
                VersionFrom = LessLabVer.ToString();
                if (LessLabVer >= LessOfflineVer)
                {
                    ans = "OK";
                }
                else
                {
                    ans = "پایین ترین ورژن آفلاین " + LessOfflineVer.ToString() + " میباشد، در حالی که ورژن بانک فعال آزمایشگاه " + LessLabVer.ToString() + "میباشد. لطفا از ورژنی پایین تر از ورژن آزمایشگاه شروع نمایید";
                }
            }
            catch (Exception EX)
            {

            }

            return ans;
        }


        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



 
        public void SetTxt_EXeEvent(string Text = "")
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new TxtExeError(SetTxt_EXeEvent), Text);
            }
            else
            {
                this.Txt_ScriptError.AppendText("\r\n\r\n" + Text);
                try
                {
                    UpdaterFunctions.SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, DBName, IsExe,VersionFrom+"-"+VersionTo, Number,0,"", Text,MyConnection);
                    Number++;

                }
                catch (Exception ex)
                {
                }
            }
        }
        public void SetProxresBarMax(Int32 Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new ProxresBarMax(SetProxresBarMax), Num);
            else
            {
                this.progressBar1.Maximum = Num;
            }
        }
        public void SetProxresBarVal(Int32 Num)
        {
            if (this.InvokeRequired)
                this.Invoke(new ProxresBarVal(SetProxresBarVal), Num);
            else
            {
                this.progressBar1.Value = Num;
            }
        }

        private void Btn_RollBack_Click(object sender, EventArgs e)
        {
            RollBack = true;
        }
    }
}
