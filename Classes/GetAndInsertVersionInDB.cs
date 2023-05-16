

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UpdaterClasses
{
    public class GetAndInsertVersionInDB
    {

        //╔═══════════════════ Variabel ═════════════════════╗
        //║                                                  ║
        //║                                                  ║
        #region "Variabel"
        DataSet DS = new DataSet();
        int ParsicUserID = 0;
        string Version = "";
        int State = 0;
        string Description = "";
        string prerequirement = "";
        Boolean Chk_Centeral = false;
        Boolean Chk_QC = false;
        Boolean Chk_Storage = false;
        Boolean Chk_Journal = false;
        Boolean Chk_Temperature = false;

        Boolean RollBack = false;
        Int64 VersionReleaseID = -1;
        Int32 counter = 0;
        string CenteralPath = "";
        string QCPath = "";
        string StoragePath = "";
        string JournalPath = "";
        string TemperaturePath = "";
        string CenteralDescription = "";
        string QCDescription = "";
        string StorageDescription = "";
        string JournalDescription = "";
        string TemperatureDescription = "";

        public delegate void DelegateEvent(string s);
        public event DelegateEvent SendBackLog;

        MyWebservice.Service1SoapClient TiketingWebService;
        Parsic.Business.Security.Cls_Encryption MySecurity;

        SqlConnection conID;
        SqlTransaction sqlTran;


        SqlConnection DScon;
        SqlTransaction sqlDSTran;
        SqlCommand DScommand;
        #endregion
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝


        //╔══════════════════ Constructor ═══════════════════╗
        //║                                                  ║
        //║                                                  ║
        #region "Constructor"
        public GetAndInsertVersionInDB(int _ParsicUserID)
        {
            ParsicUserID = _ParsicUserID;
            TiketingWebService = new MyWebservice.Service1SoapClient();
            MySecurity = new Parsic.Business.Security.Cls_Encryption();
        }
        #endregion
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝


        //╔════════════════════ Function ════════════════════╗
        //║                                                  ║
        //║                                                  ║
        #region Function


        public string[] UpdateOrAddVersionReleaseFiles(int _VersionReleaseID, string _Version, int _State, string _Description, string _prerequirement, Boolean _Chk_Centeral, Boolean _Chk_QC, Boolean _Chk_Storage, Boolean _Chk_Journal, Boolean _Chk_Temperature, string _CenteralPath, string _CenteralDescription, string _QCPath, string _QCDescription, string _StoragePath, string _StorageDescription, string _JournalPath, string _JournalDescription, string _TemperaturePath, string _TemperatureDescription)
        {
            int ErrorAndDeleteVersionRelease = 0;
            string[] ReturnMessage = new string[5];
            Version = _Version;
            State = _State;
            Description = _Description;
            prerequirement = _prerequirement;
            Chk_Centeral = _Chk_Centeral;
            Chk_QC = _Chk_QC;
            Chk_Storage = _Chk_Storage;
            Chk_Journal = _Chk_Journal;
            Chk_Temperature = _Chk_Temperature;

            CenteralPath = _CenteralPath;
            QCPath = _QCPath;
            StoragePath = _StoragePath;
            JournalPath = _JournalPath;
            TemperaturePath = _TemperaturePath;

            FindPaths();// if path was empty find them

            // Get Descriptions
            CenteralDescription = _CenteralDescription;
            QCDescription = _QCDescription;
            StorageDescription = _StorageDescription;
            JournalDescription = _JournalDescription;
            TemperatureDescription = _TemperatureDescription;
            // Get Descriptions\

            VersionReleaseID = _VersionReleaseID;

            if (VersionReleaseID == -1)
            {
                return null;
            }
            int VersionReleaseVApp = -1;
            try
            {
                DataTable _Dt = new DataTable(); //.Tables["SpecialVerRelVApp"]
                _Dt = GetSpecialVersionInfoFromVersionReleaseVApp(VersionReleaseID);
                VersionReleaseVApp = Convert.ToInt16(_Dt.Rows[0]["Prk_VersionReleaseVApplication_AutoID"]);
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                VersionReleaseVApp = -1;
                //return ReturnMessage;
            }

            //if (VersionReleaseVApp != -1)
            //{
            //    Delete_VRVA_VRVAF(VersionReleaseVApp);
            //}


            Delete_VRVA_VRVAF(VersionReleaseVApp);

            string Fpath = "";
            string ver = "";
            string date = DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "." + DateTime.Now.Minute.ToString() + "." + DateTime.Now.Second.ToString() + ".zip";
            string Filename = "";


            if (Chk_Centeral == true)
            {
                if (CenteralPath != "")
                {
                    // save splits bytes in sql
                    ver = Version;
                    Filename = "Centeral" + "/" + ver + "/" + date;
                    string zipname = "-" + ver + "-" + date;
                    string SizeOfWholeByts = "";
                    byte[] WholeByte;
                    Fpath = ZipDirectori(CenteralPath, zipname);
                    if (Fpath == "")
                        ReturnMessage[0] = "Centeral: Your Access to the path is denied";
                    else
                    {
                        WholeByte = StreamFile(Fpath);
                        if (WholeByte == null)
                            ReturnMessage[0] = "Centeral: error in fetch bytes";
                        else
                        {
                            SizeOfWholeByts = WholeByte.Length.ToString();
                            string check = CalculateChecksum(WholeByte);
                            Int64 VersionReleaseVApplicationID = InsertToVersionReleaseVApplication(VersionReleaseID, 1, Filename, SizeOfWholeByts, check, CenteralDescription);
                            if (VersionReleaseVApplicationID == -1)
                                ReturnMessage[0] = "Centeral: error in Save In VersionReleaseVApplication";
                            else
                            {
                                int SaveInDB = SplitFiles(WholeByte, VersionReleaseVApplicationID);
                                if (SaveInDB == -1)
                                {
                                    if (Delete_VRVA_VRVAF(VersionReleaseVApplicationID))
                                    {
                                        ReturnMessage[0] = "Centeral: error in Save In VersionReleaseVApplicationFile And Wrong Information For This Version Have Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    else
                                    {
                                        ReturnMessage[0] = "Centeral: error in Save In VersionReleaseVApplicationFile And the Wrong Information For This Version Have Not Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    ErrorAndDeleteVersionRelease += 1;
                                }
                            }
                        }
                    }
                    DeleteZipFile(Fpath);
                    if (ReturnMessage[0] == null)
                        ReturnMessage[0] = "Centeral: SuccessFul";
                }
                else
                {
                    ReturnMessage[0] = "Centeral: Path Is Emoty";
                }
            }
            else
            {
                ReturnMessage[0] = "Centeral: Not Checked";
            }

            if (Chk_QC == true)
            {
                if (QCPath != "")
                {
                    // save splits bytes in sql
                    ver = Version;
                    Filename = "QC" + "/" + ver + "/" + date;
                    string zipname = "-" + ver + "-" + date;
                    string SizeOfWholeByts = "";
                    byte[] WholeByte;
                    Fpath = ZipDirectori(QCPath, zipname);
                    if (Fpath == "")
                        ReturnMessage[1] = "QC: Your Access to the path is denied";
                    else
                    {
                        WholeByte = StreamFile(Fpath);
                        if (WholeByte == null)
                            ReturnMessage[1] = "QC: error in fetch bytes";
                        else
                        {
                            SizeOfWholeByts = WholeByte.Length.ToString();
                            string check = CalculateChecksum(WholeByte);
                            Int64 VersionReleaseVApplicationID = InsertToVersionReleaseVApplication(VersionReleaseID, 2, Filename, SizeOfWholeByts, check, QCDescription);
                            if (VersionReleaseVApplicationID == -1)
                                ReturnMessage[1] = "QC: error in Save In VersionReleaseVApplication";
                            else
                            {
                                int SaveInDB = SplitFiles(WholeByte, VersionReleaseVApplicationID);
                                if (SaveInDB == -1)
                                {
                                    if (Delete_VRVA_VRVAF(VersionReleaseVApplicationID))
                                    {
                                        ReturnMessage[1] = "QC: error in Save In VersionReleaseVApplicationFile And Wrong Information For This Version Have Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    else
                                    {
                                        ReturnMessage[1] = "QC: error in Save In VersionReleaseVApplicationFile And the Wrong Information For This Version Have Not Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    ErrorAndDeleteVersionRelease += 1;
                                }
                            }
                        }
                    }
                    DeleteZipFile(Fpath);
                    if (ReturnMessage[1] == null)
                        ReturnMessage[1] = "QC: SuccessFul";
                }
                else
                {
                    ReturnMessage[1] = "QC: Path Is Emoty";
                }
            }
            else
            {
                ReturnMessage[1] = "QC: Not Checked";
            }

            if (Chk_Storage == true)
            {
                if (StoragePath != "")
                {
                    // save splits bytes sql
                    ver = Version;
                    Filename = "Storage" + "/" + ver + "/" + date;
                    string zipname = "-" + ver + "-" + date;
                    string SizeOfWholeByts = "";
                    byte[] WholeByte;
                    Fpath = ZipDirectori(StoragePath, zipname);
                    if (Fpath == "")
                        ReturnMessage[2] = "Storage: Your Access to the path is denied";
                    else
                    {
                        WholeByte = StreamFile(Fpath);
                        if (WholeByte == null)
                            ReturnMessage[2] = "Storage: error in fetch bytes";
                        else
                        {
                            SizeOfWholeByts = WholeByte.Length.ToString();
                            string check = CalculateChecksum(WholeByte);
                            Int64 VersionReleaseVApplicationID = InsertToVersionReleaseVApplication(VersionReleaseID, 3, Filename, SizeOfWholeByts, check, StorageDescription);
                            if (VersionReleaseVApplicationID == -1)
                                ReturnMessage[2] = "Storage: error in Save In VersionReleaseVApplication";
                            else
                            {
                                int SaveInDB = SplitFiles(WholeByte, VersionReleaseVApplicationID);
                                if (SaveInDB == -1)
                                {
                                    if (Delete_VRVA_VRVAF(VersionReleaseVApplicationID))
                                    {
                                        ReturnMessage[2] = "Storage: error in Save In VersionReleaseVApplicationFile And Wrong Information For This Version Have Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    else
                                    {
                                        ReturnMessage[2] = "Storage: error in Save In VersionReleaseVApplicationFile And the Wrong Information For This Version Have Not Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    ErrorAndDeleteVersionRelease += 1;
                                }
                            }
                        }
                    }
                    DeleteZipFile(Fpath);
                    if (ReturnMessage[2] == null)
                        ReturnMessage[2] = "Storage: SuccessFul";
                }
                else
                {
                    ReturnMessage[2] = "Storage: Path Is Emoty";
                }
            }
            else
            {
                ReturnMessage[2] = "Storage: Not Checked";
            }

            if (Chk_Journal == true)
            {
                if (JournalPath != "")
                {
                    // save splits bytes in sql
                    ver = Version;
                    Filename = "Journal" + "/" + ver + "/" + date;
                    string zipname = "-" + ver + "-" + date;
                    string SizeOfWholeByts = "";
                    byte[] WholeByte;
                    Fpath = ZipDirectori(JournalPath, zipname);
                    if (Fpath == "")
                        ReturnMessage[3] = "Journal: Your Access to the path is denied";
                    else
                    {
                        WholeByte = StreamFile(Fpath);
                        if (WholeByte == null)
                            ReturnMessage[3] = "Journal: error in fetch bytes";
                        else
                        {
                            SizeOfWholeByts = WholeByte.Length.ToString();
                            string check = CalculateChecksum(WholeByte);
                            Int64 VersionReleaseVApplicationID = InsertToVersionReleaseVApplication(VersionReleaseID, 4, Filename, SizeOfWholeByts, check, JournalDescription);
                            if (VersionReleaseVApplicationID == -1)
                                ReturnMessage[3] = "Journal: error in Save In VersionReleaseVApplication";
                            else
                            {
                                int SaveInDB = SplitFiles(WholeByte, VersionReleaseVApplicationID);
                                if (SaveInDB == -1)
                                {
                                    if (Delete_VRVA_VRVAF(VersionReleaseVApplicationID))
                                    {
                                        ReturnMessage[3] = "Journal: error in Save In VersionReleaseVApplicationFile And Wrong Information For This Version Have Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    else
                                    {
                                        ReturnMessage[3] = "Journal: error in Save In VersionReleaseVApplicationFile And the Wrong Information For This Version Have Not Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    ErrorAndDeleteVersionRelease += 1;
                                }
                            }
                        }
                    }
                    DeleteZipFile(Fpath);
                    if (ReturnMessage[3] == null)
                        ReturnMessage[3] = "Journal: SuccessFul";
                }
                else
                {
                    ReturnMessage[3] = "Journal: Path Is Emoty";
                }
            }
            else
            {
                ReturnMessage[3] = "Journal: Not Checked";
            }

            if (Chk_Temperature == true)
            {
                if (TemperaturePath != "")
                {
                    // save splits bytes in sql
                    ver = Version;
                    Filename = "Temperature" + "/" + ver + "/" + date;
                    string zipname = "-" + ver + "-" + date;
                    string SizeOfWholeByts = "";
                    byte[] WholeByte;
                    Fpath = ZipDirectori(TemperaturePath, zipname);
                    if (Fpath == "")
                        ReturnMessage[4] = "Temperature: Your Access to the path is denied";
                    else
                    {
                        WholeByte = StreamFile(Fpath);
                        if (WholeByte == null)
                            ReturnMessage[4] = "Temperature: error in fetch bytes";
                        else
                        {
                            SizeOfWholeByts = WholeByte.Length.ToString();
                            string check = CalculateChecksum(WholeByte);
                            Int64 VersionReleaseVApplicationID = InsertToVersionReleaseVApplication(VersionReleaseID, 5, Filename, SizeOfWholeByts, check, TemperatureDescription);
                            if (VersionReleaseVApplicationID == -1)
                                ReturnMessage[4] = "Temperature: error in Save In VersionReleaseVApplication";
                            else
                            {
                                int SaveInDB = SplitFiles(WholeByte, VersionReleaseVApplicationID);
                                if (SaveInDB == -1)
                                {
                                    if (Delete_VRVA_VRVAF(VersionReleaseVApplicationID))
                                    {
                                        ReturnMessage[4] = "Temperature: error in Save In VersionReleaseVApplicationFile And Wrong Information For This Version Have Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    else
                                    {
                                        ReturnMessage[4] = "Temperature: error in Save In VersionReleaseVApplicationFile And the Wrong Information For This Version Have Not Deleted From Tbl_VersionReleaseVApp , Tbl_VersionReleaseVAppFile ";
                                    }
                                    ErrorAndDeleteVersionRelease += 1;
                                }
                            }
                        }
                    }
                    DeleteZipFile(Fpath);
                    if (ReturnMessage[4] == null)
                        ReturnMessage[4] = "Temperature: SuccessFul";
                }
                else
                {
                    ReturnMessage[4] = "Temperature: Path Is Emoty";
                }
            }
            else
            {
                ReturnMessage[4] = "Temperature: Not Checked";
            }


            return ReturnMessage;
        }


        public Boolean AddVersionReleaseScript(int VersionReleaseID, int ParsicUserID, int Order, string Script, string Description)
        {
            try
            {
                return InsertScript(VersionReleaseID, ParsicUserID, Order, Script.Replace("\n", "!@#$%^&**&^%$#@!").Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), Description.Replace(Environment.NewLine, "!@#$%^&**&^%$#@!").Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"));
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean AddVersionReleaseDescription(int VersionReleaseID, int ParsicUserID, string Title, string Description, Int16 Order, Boolean Visibility, Boolean LerningTips, int code)
        {
            try
            {
                return InsertDescription(VersionReleaseID, ParsicUserID, Title.Replace(Environment.NewLine, "!@#$%^&**&^%$#@!").Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), Description.Replace(Environment.NewLine, "!@#$%^&**&^%$#@!").Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), Order, Visibility, LerningTips, code);
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public DataTable FetchScriptsFromTo(string From, string To)
        {
            DataTable DT = new DataTable();
            DT = GetScriptVersions(From, To);
            return DT;
        }
        public DataTable FetchDescriptionsFromTo(string From, string To)
        {
            DataTable DT = new DataTable();
            DT = GetDescriptionsVersions(From, To);
            return DT;
        }
        public string SaveDescriptionAndScripts(string From, string To, int ParsicUserID, int VersionRelease_ID, string connectionTrueDB, Boolean User_Is_From_Parsic)
        {
            counter = 0;
            string LabID = "0";


            string ComputerName = System.Environment.MachineName.ToString();
            DataTable DTParsicMaster = new DataTable();
            try
            {
                List<string> GetODBCInfoList = new List<string>();
                GetODBCInfoList = GetODBCInformation();

                if (GetODBCInfoList.Count == 1)
                    return GetODBCInfoList[0];

                DTParsicMaster = UpdateGridView(GetODBCInfoList);
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "Error : " + EX.Message.ToString();
            }

            LabID = GetLabID(DTParsicMaster);
            if (LabID == "0")
                return "برای آزمایشگاه آیدی ثبت نشده است";

            if (SendLocalVersionReleaseVLog(VersionRelease_ID, ParsicUserID, From, To, "Start", "Scripts", 0, 0, ComputerName, "", false, false, 0, connectionTrueDB, User_Is_From_Parsic, Convert.ToInt16(LabID)) == false)
            {

                return "ارتباط با جدول لاگ های بانک فعال آزمایشگاه،سرور دیتابیس، یا بانک ابری،اینترنت، مشکلی وجود دارد ";
            }

            SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, "", false, From + "-" + To, 0, 0, "Start", "Scripts", connectionTrueDB);
            SendBackLog("Start : From: " + From + " To: " + To);
            List<string> SqlServerDrives = new List<string>();
            string Connection = MakeConnectionString(DTParsicMaster.Rows[0]["DBList_Server"].ToString(), DTParsicMaster.Rows[0]["DBList_Name"].ToString(), DTParsicMaster.Rows[0]["DBList_Username"].ToString(), DTParsicMaster.Rows[0]["DBList_Password"].ToString());
            if (Connection == "")
                return "در پیدا کردن کانکشن استرینگ بانک فعال مشکلی وجود دارد";
            SqlServerDrives = GetSQLServerSystemDrivesBySQLCode(Connection);
            if (SqlServerDrives.Count == 0)
                return "در پیدا کردن درایور های سیستم اس کیو ال سرور خطایی وجود دارد";
            int ServerDriveCount = 0;
            Boolean BackIsOk = true;
            string BackUpAns = "";
            foreach (string str in SqlServerDrives)
            {
                BackIsOk = true;
                string path = str + "AutoBackupBeforVersion";
                CreateFolderInSQLServerSystemBySQLCode(path, Connection);
                SendBackLog("BackUp : Start ");
                SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", "", false, From + "-" + To, 0, 0, "Try", "BackUp", connectionTrueDB);
                for (int i = 0; i < DTParsicMaster.Rows.Count; i++)
                {
                    SendBackLog("BackUp : Try  -  " + path + @"\" + DTParsicMaster.Rows[i]["DBList_Name"].ToString() + " ...");
                    BackUpAns = GetDbBackup(DTParsicMaster.Rows[i]["DBList_Name"].ToString(), path + @"\", Connection, 2);
                    if (BackUpAns != "Ok")
                    {
                        BackIsOk = false;
                        SendBackLog("BackUp : BackUp Error: " + DTParsicMaster.Rows[i]["DBList_Name"].ToString());
                        SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DTParsicMaster.Rows[i]["DBList_Name"].ToString(), false, From + "-" + To, 0, 0, BackUpAns, "BackUp - Path : " + path, connectionTrueDB);
                        break;
                    }
                    SendBackLog("BackUp : Ok  -  " + path + @"\" + DTParsicMaster.Rows[i]["DBList_Name"].ToString() + " ...");
                    SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DTParsicMaster.Rows[i]["DBList_Name"].ToString(), false, From + "-" + To, 0, 0, BackUpAns, "BackUp - Path : " + path, connectionTrueDB);
                }
                if (BackIsOk == true)
                    break;
                if (BackUpAns.Contains("There is not enough space") == false)
                    break;
            }
            SendBackLog("BackUp : Finish");
            if (BackIsOk == false)
                return "در گرفتن بکاپ بانک ها مشکلی وجود دارد " + "\r\n" + BackUpAns;

            try
            {
                SendBackLog("Scripts : Start");
                string DB_ServerName = "";
                string DB_Name = "";
                string DB_Username = "";
                string DB_Password = "";

                string LocalConnection = "";
                double DB_version = 10.01;
                Int32 WholePart = 0;
                Boolean ThisIsTrueDB = false;
                DataTable DtScripts = new DataTable();
                DataTable DtDescriptions = new DataTable();

                if (DTParsicMaster.Rows.Count == 0)
                    return "هیچ بانکی وجود ندارد";

                for (Int16 i = 0; i < DTParsicMaster.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(DTParsicMaster.Rows[i]["DBList_IsActive"]) == true)
                    {
                        ThisIsTrueDB = true; break;
                        //LocalConnection = MakeConnectionString(DTParsicMaster.Rows[i]["DBList_Server"].ToString(), DTParsicMaster.Rows[i]["DBList_Name"].ToString(), DTParsicMaster.Rows[i]["DBList_Username"].ToString(), DTParsicMaster.Rows[i]["DBList_Password"].ToString());
                    }
                }
                if (ThisIsTrueDB == false)
                {
                    return "بانک فعالی وجود ندارد";
                }



                DtScripts = FetchScriptsFromTo(From, To); // Fetch Scripts From a Version To Another Version 

                DtDescriptions = FetchDescriptionsFromTo(From, To); // Fetch Description From a Version To Another Version 


                if (From == To)
                {
                    return "اسکریپت مورد نظر در حال حاظر بر روی بانک اصلی اعمال شده است";
                }
                if (Convert.ToDouble(To) < Convert.ToDouble(From))
                    return "اسکریپت فعلی بانک پیش فرض از اسکریپتی که میخواهید اعمال کنید بالاتر است";



                if (DtScripts == null && DtDescriptions == null)
                    return "در واکشی اطلاعات توضیحات و اسکریپت ها از بانک ابری مشکلی وجود دارد ";
                if (DtScripts == null)
                    return "در واکشی اطلاعات توضیحات از بانک ابری مشکلی وجود دارد ";
                if (DtDescriptions == null)
                    return "در واکشی اطلاعات اسکریپت ها از بانک ابری مشکلی وجود دارد ";

                if (DtScripts.Rows.Count == 0)
                    return "هیچ فیلد اسکریپتی برای ورژن" + From + " به " + To + "در بانک ابری واکشی نشده است";

                DScon = new SqlConnection();
                DScommand = new SqlCommand();
                try
                {
                    DScon.ConnectionString = MakeConnectionString(DTParsicMaster.Rows[0]["DBList_Server"].ToString(), DTParsicMaster.Rows[0]["DBList_Name"].ToString(), DTParsicMaster.Rows[0]["DBList_Username"].ToString(), DTParsicMaster.Rows[0]["DBList_Password"].ToString());
                    if (TestConnectionToDB(DScon) == false)
                    {
                        return "بانک " + DB_Name + "در دسترس نمیباشد، لطفا پارسیک مستر آزمایشگاه را با بانک های موجود چک نمایید";
                    }
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return "Error : " + EX.Message.ToString();
                }

                try
                {
                    DScommand = DScon.CreateCommand();
                    DScommand.CommandTimeout = 200000;
                    DScon.Open();
                    sqlDSTran = DScon.BeginTransaction();
                    DScommand.Transaction = sqlDSTran;
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return "Error : " + EX.Message.ToString();
                }
                try
                {
                    WholePart = Convert.ToInt32(DtScripts.Rows.Count) * Convert.ToInt32(DTParsicMaster.Rows.Count) + Convert.ToInt32(DTParsicMaster.Rows.Count) * 2 + 2;
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                }
                try
                {
                    foreach (DataRow dr in DTParsicMaster.Rows)
                    {
                        if ((dr["DBList_IsActive"].ToString() != "True"))
                        {
                            // Run For Whole Tables Except Default DB





                            DB_ServerName = dr["DBList_Server"].ToString();
                            DB_Name = dr["DBList_Name"].ToString();
                            DB_Username = dr["DBList_Username"].ToString();
                            DB_Password = dr["DBList_Password"].ToString();
                            DB_version = Convert.ToDouble(dr["DBCurruntVersion"]);
                            //SendBackLog("Scripts: Try - Server Name : "+ DB_ServerName + "DataBase Name : " + DB_Name);
                            SendLocalVersionReleaseVLog(VersionRelease_ID, ParsicUserID, From, To, "Try", "Scripts", 0, 0, ComputerName, DB_Name, false, false, 0, connectionTrueDB, User_Is_From_Parsic, Convert.ToInt16(LabID));
                            SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, DB_Name, false, From + "-" + To, 0, 0, "Try", "Scripts", connectionTrueDB);
                            string ans = SaveDescriptionScriptsInner(DtScripts, DtDescriptions, false, DB_Name, DB_version.ToString(), ParsicUserID, connectionTrueDB, WholePart);
                            if (ans != "OK")
                            {
                                DScon.Close();
                                return ans;
                            }
                        }
                    }

                    foreach (DataRow dr in DTParsicMaster.Rows)
                    {
                        if ((dr["DBList_IsActive"].ToString() == "True"))
                        {
                            // Run Only For Default DB
                            DB_ServerName = dr["DBList_Server"].ToString();
                            DB_Name = dr["DBList_Name"].ToString();
                            DB_Username = dr["DBList_Username"].ToString();
                            DB_Password = dr["DBList_Password"].ToString();
                            DB_version = Convert.ToDouble(dr["DBCurruntVersion"]);

                            SendLocalVersionReleaseVLog(VersionRelease_ID, ParsicUserID, From, To, "Try", "", 0, 0, ComputerName, DB_Name, true, false, 0, connectionTrueDB, User_Is_From_Parsic, Convert.ToInt16(LabID));
                            SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, DB_Name, false, From + "-" + To, 0, 0, "Try", "", connectionTrueDB);





                            string ans = SaveDescriptionScriptsInner(DtScripts, DtDescriptions, true, DB_Name, DB_version.ToString(), ParsicUserID, connectionTrueDB, WholePart);
                            if (ans != "OK")
                            {

                                DScon.Close();

                                return ans;
                            }
                        }
                    }
                    // Evry Thing is Ok
                    SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, DB_Name, false, From + "-" + To, 0, 0, "Success Ful", "Scripts", connectionTrueDB);
                    sqlDSTran.Commit();
                    DScon.Close();
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return EX.Message.ToString();
                }
                return "Ok";
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "Error : " + EX.Message.ToString();
            }

        }

        public string SaveDescriptionAndScripts(string From, string To, int ParsicUserID, int VersionRelease_ID, string connectionTrueDB, Boolean User_Is_From_Parsic, DataTable _DT_parsicMaster)
        {
            counter = 0;
            string LabID = "0";


            string ComputerName = System.Environment.MachineName.ToString();
            DataTable DTParsicMaster = new DataTable();
            //try
            //{
            //    List<string> GetODBCInfoList = new List<string>();
            //    GetODBCInfoList = GetODBCInformation();

            //    if (GetODBCInfoList.Count == 1)
            //        return GetODBCInfoList[0];

            //    DTParsicMaster = UpdateGridView(GetODBCInfoList);
            //}
            //catch (Exception EX)
            //{
            //    return "Error : " + EX.Message.ToString();
            //}
            DTParsicMaster = _DT_parsicMaster;
            LabID = GetLabID(DTParsicMaster);
            if (LabID == "0")
                return "برای آزمایشگاه آیدی ثبت نشده است";

            if (SendLocalVersionReleaseVLog(VersionRelease_ID, ParsicUserID, From, To, "Start", "Scripts", 0, 0, ComputerName, "", false, false, 0, connectionTrueDB, User_Is_From_Parsic, Convert.ToInt16(LabID)) == false)
            {

                return "ارتباط با جدول لاگ های بانک فعال آزمایشگاه یا بانک ابری،اینترنت، مشکلی وجود دارد " + "\r\nVersionReleaseVLogs" + "\r\n" + connectionTrueDB;
            }

            SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, "", false, From + "-" + To, 0, 0, "Start", "Scripts", connectionTrueDB);
            SendBackLog("\r\n\r\nScript Start : From: " + From + " To: " + To);
            List<string> SqlServerDrives = new List<string>();
            string Connection = MakeConnectionString(DTParsicMaster.Rows[0]["DBList_Server"].ToString(), DTParsicMaster.Rows[0]["DBList_Name"].ToString(), DTParsicMaster.Rows[0]["DBList_Username"].ToString(), DTParsicMaster.Rows[0]["DBList_Password"].ToString());
            if (Connection == "")
                return "در پیدا کردن کانکشن استرینگ بانک فعال مشکلی وجود دارد";
            SqlServerDrives = GetSQLServerSystemDrivesBySQLCode(Connection);
            if (SqlServerDrives.Count == 0)
                return "در پیدا کردن درایور های سیستم اس کیو ال سرور خطایی وجود دارد";
            int ServerDriveCount = 0;
            Boolean BackIsOk = true;
            string BackUpAns = "";
            foreach (string str in SqlServerDrives)
            {
                BackIsOk = true;
                string path = str + "AutoBackupBeforVersion";
                CreateFolderInSQLServerSystemBySQLCode(path, Connection);
                SendBackLog("BackUp : Start ");
                SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", "", false, From + "-" + To, 0, 0, "Try", "BackUp", connectionTrueDB);
                for (int i = 0; i < DTParsicMaster.Rows.Count; i++)
                {
                    SendBackLog("BackUp : Try  -  " + path + @"\" + DTParsicMaster.Rows[i]["DBList_Name"].ToString() + " ...");
                    BackUpAns = GetDbBackup(DTParsicMaster.Rows[i]["DBList_Name"].ToString(), path + @"\", Connection, 2);
                    if (BackUpAns != "Ok")
                    {
                        BackIsOk = false;
                        SendBackLog("BackUp : BackUp Error: " + DTParsicMaster.Rows[i]["DBList_Name"].ToString());
                        SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DTParsicMaster.Rows[i]["DBList_Name"].ToString(), false, From + "-" + To, 0, 0, BackUpAns, "BackUp - Path : " + path, connectionTrueDB);
                        break;
                    }
                    SendBackLog("BackUp : Ok  -  " + path + @"\" + DTParsicMaster.Rows[i]["DBList_Name"].ToString() + " ...");
                    SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DTParsicMaster.Rows[i]["DBList_Name"].ToString(), false, From + "-" + To, 0, 0, BackUpAns, "BackUp - Path : " + path, connectionTrueDB);
                }
                if (BackIsOk == true)
                    break;
                if (BackUpAns.Contains("There is not enough space") == false)
                    break;
            }
            SendBackLog("BackUp : Finish");
            if (BackIsOk == false)
                return "در گرفتن بکاپ بانک ها مشکلی وجود دارد " + "\r\n" + BackUpAns;

            string DB_ServerName = "";
            try
            {
                SendBackLog("Scripts : Start");
                string DB_Name = "";
                string DB_Username = "";
                string DB_Password = "";

                string LocalConnection = "";
                double DB_version = 10.01;
                Int32 WholePart = 0;
                Boolean ThisIsTrueDB = false;
                DataTable DtScripts = new DataTable();
                DataTable DtDescriptions = new DataTable();

                if (DTParsicMaster.Rows.Count == 0)
                    return "هیچ بانکی وجود ندارد";

                for (Int16 i = 0; i < DTParsicMaster.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(DTParsicMaster.Rows[i]["DBList_IsActive"]) == true)
                    {
                        ThisIsTrueDB = true; break;
                        //LocalConnection = MakeConnectionString(DTParsicMaster.Rows[i]["DBList_Server"].ToString(), DTParsicMaster.Rows[i]["DBList_Name"].ToString(), DTParsicMaster.Rows[i]["DBList_Username"].ToString(), DTParsicMaster.Rows[i]["DBList_Password"].ToString());
                    }
                }
                if (ThisIsTrueDB == false)
                {
                    return "بانک فعالی وجود ندارد";
                }



                DtScripts = FetchScriptsFromTo(From, To); // Fetch Scripts From a Version To Another Version 

                DtDescriptions = FetchDescriptionsFromTo(From, To); // Fetch Description From a Version To Another Version 


                if (From == To)
                {
                    return "اسکریپت مورد نظر در حال حاظر بر روی بانک اصلی اعمال شده است";
                }
                if (Convert.ToDouble(To) < Convert.ToDouble(From))
                    return "اسکریپت فعلی بانک پیش فرض از اسکریپتی که میخواهید اعمال کنید بالاتر است";



                if (DtScripts == null && DtDescriptions == null)
                    return "در واکشی اطلاعات توضیحات و اسکریپت ها از بانک ابری مشکلی وجود دارد ";
                if (DtScripts == null)
                    return "در واکشی اطلاعات توضیحات از بانک ابری مشکلی وجود دارد ";
                if (DtDescriptions == null)
                    return "در واکشی اطلاعات اسکریپت ها از بانک ابری مشکلی وجود دارد ";

                if (DtScripts.Rows.Count == 0)
                    return "هیچ فیلد اسکریپتی برای ورژن" + From + " به " + To + "در بانک ابری واکشی نشده است";

                DScon = new SqlConnection();
                DScommand = new SqlCommand();
                try
                {
                    DScon.ConnectionString = MakeConnectionString(DTParsicMaster.Rows[0]["DBList_Server"].ToString(), DTParsicMaster.Rows[0]["DBList_Name"].ToString(), DTParsicMaster.Rows[0]["DBList_Username"].ToString(), DTParsicMaster.Rows[0]["DBList_Password"].ToString());
                    if (TestConnectionToDB(DScon) == false)
                    {
                        return "بانک " + DB_Name + "در دسترس نمیباشد، لطفا پارسیک مستر آزمایشگاه را با بانک های موجود چک نمایید";
                    }
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return "Error : " + EX.Message.ToString();
                }

                try
                {
                    DScommand = DScon.CreateCommand();
                    DScommand.CommandTimeout = 200000;
                    DScon.Open();
                    sqlDSTran = DScon.BeginTransaction();
                    DScommand.Transaction = sqlDSTran;
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return "Error : " + EX.Message.ToString();
                }
                try
                {
                    WholePart = Convert.ToInt32(DtScripts.Rows.Count) * Convert.ToInt32(DTParsicMaster.Rows.Count) + Convert.ToInt32(DTParsicMaster.Rows.Count) * 2 + 2;
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                }
                try
                {
                    try
                    {
                        string connection = "";
                        try
                        {
                            foreach (DataRow dr in DTParsicMaster.Rows)
                            {
                                if ((dr["DBList_IsActive"].ToString() == "True"))
                                {
                                    DB_Name = dr["DBList_Name"].ToString();
                                    break;
                                }
                            }
                        }
                        catch (Exception EX)
                        {
                            //MessageBox.Show("Error 102 : " + EX.Message.ToString());
                            SaveTextExeption(EX.Message.ToString());
                        }

                        string Ansver = "";

                        //int VersionRelease_ID = Convert.ToInt16(Dt.Rows[Dg_VersionList.CurrentRow.Index]["Prk_VersionRelease_AutoID"]);

                    }
                    catch
                    {

                    }
                    foreach (DataRow dr in DTParsicMaster.Rows)
                    {
                        if ((dr["DBList_IsActive"].ToString() != "True"))
                        {
                            // Run For Whole Tables Except Default DB





                            DB_ServerName = dr["DBList_Server"].ToString();
                            DB_Name = dr["DBList_Name"].ToString();
                            DB_Username = dr["DBList_Username"].ToString();
                            DB_Password = dr["DBList_Password"].ToString();
                            DB_version = Convert.ToDouble(dr["DBCurruntVersion"]);
                            //SendBackLog("Scripts: Try - Server Name : "+ DB_ServerName + "DataBase Name : " + DB_Name);
                            SendLocalVersionReleaseVLog(VersionRelease_ID, ParsicUserID, From, To, "Try", "Scripts", 0, 0, ComputerName, DB_Name, false, false, 0, connectionTrueDB, User_Is_From_Parsic, Convert.ToInt16(LabID));
                            SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, DB_Name, false, From + "-" + To, 0, 0, "Try", "Scripts", connectionTrueDB);
                            if (Parsic_user_send_log(User_Is_From_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(ParsicUserID), 0, " اسکریپت ") == false)
                            {
                                SaveTextExeption("Error in Send Log to Cloud");
                                return "Error in Send Log to Cloud";
                            }
                            string ans = SaveDescriptionScriptsInner(DtScripts, DtDescriptions, false, DB_Name, DB_version.ToString(), ParsicUserID, connectionTrueDB, WholePart);
                            if (ans != "OK")
                            {
                                DScon.Close();
                                return ans;
                            }
                            if (Parsic_user_send_log(User_Is_From_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(ParsicUserID), 1, " اسکریپت ") == false)
                            {
                                SaveTextExeption("Error in Send Log to Cloud");
                                return "Error in Send Log to Cloud";
                            }
                        }
                    }

                    foreach (DataRow dr in DTParsicMaster.Rows)
                    {
                        if ((dr["DBList_IsActive"].ToString() == "True"))
                        {
                            // Run Only For Default DB
                            DB_ServerName = dr["DBList_Server"].ToString();
                            DB_Name = dr["DBList_Name"].ToString();
                            DB_Username = dr["DBList_Username"].ToString();
                            DB_Password = dr["DBList_Password"].ToString();
                            DB_version = Convert.ToDouble(dr["DBCurruntVersion"]);

                            SendLocalVersionReleaseVLog(VersionRelease_ID, ParsicUserID, From, To, "Try", "", 0, 0, ComputerName, DB_Name, true, false, 0, connectionTrueDB, User_Is_From_Parsic, Convert.ToInt16(LabID));
                            SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, DB_Name, false, From + "-" + To, 0, 0, "Try", "", connectionTrueDB);
                            if (Parsic_user_send_log(User_Is_From_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(ParsicUserID), 0, " اسکریپت ") == false)
                            {
                                SaveTextExeption("Error in Send Log to Cloud");
                                return "Error in Send Log to Cloud";
                            }
                            string ans = SaveDescriptionScriptsInner(DtScripts, DtDescriptions, true, DB_Name, DB_version.ToString(), ParsicUserID, connectionTrueDB, WholePart);
                            if (ans != "OK")
                            {
                                DScon.Close();
                                return ans;
                            }
                            if (Parsic_user_send_log(User_Is_From_Parsic, From, To, DB_Name, Convert.ToInt32(LabID), Convert.ToInt32(ParsicUserID), 1, " اسکریپت ") == false)
                            {
                                SaveTextExeption("Error in Send Log to Cloud");
                                return "Error in Send Log to Cloud";
                            }
                        }
                    }
                    // Evry Thing is Ok
                    SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, ComputerName, DB_Name, false, From + "-" + To, 0, 0, "Success Ful", "Scripts", connectionTrueDB);
                    sqlDSTran.Commit();
                    DScon.Close();
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return EX.Message.ToString();
                }
                return "Ok";
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "Error : " + EX.Message.ToString();
            }

        }


        // Important and Main Functions\








        // whole of version to gather in a function for update EXE
        public Boolean SaveExeVersionFromCloudToLabDb(DataTable DT, int RowIndex, Boolean Chk_Centeral, Boolean Chk_QC, Boolean Chk_Storage, Boolean Chk_Journal, string LocalConnection)
        {
            int OpperationIsOk = 0;
            int VersionReleaseVApplication = 0;
            int SoftwareApplication = 0;
            int CountOfEachApp = 0;
            int VerNum = Convert.ToInt16(DT.Rows[RowIndex]["Prk_VersionRelease_AutoID"]);
            string VersionNumber = "";
            VersionNumber = DT.Rows[RowIndex]["VersionNumber"].ToString();
            DT = GetVersionCountFromDataBase(VerNum); //Tables["PackageCount"]
            int WholeCount = 0;
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                if (Chk_Centeral = true && Convert.ToInt16(DT.Rows[i]["Frk_SoftwareApplication"]) == 1)
                    WholeCount += Convert.ToInt16(DT.Rows[i]["CountOfEachApp"]);
                if (Chk_QC = true && Convert.ToInt16(DT.Rows[i]["Frk_SoftwareApplication"]) == 2)
                    WholeCount += Convert.ToInt16(DT.Rows[i]["CountOfEachApp"]);
                if (Chk_Storage = true && Convert.ToInt16(DT.Rows[i]["Frk_SoftwareApplication"]) == 3)
                    WholeCount += Convert.ToInt16(DT.Rows[i]["CountOfEachApp"]);
                if (Chk_Journal = true && Convert.ToInt16(DT.Rows[i]["Frk_SoftwareApplication"]) == 4)
                    WholeCount += Convert.ToInt16(DT.Rows[i]["CountOfEachApp"]);
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                VersionReleaseVApplication = Convert.ToInt16(DT.Rows[i]["Prk_VersionReleaseVApplication_AutoID"]);
                SoftwareApplication = Convert.ToInt16(DT.Rows[i]["Frk_SoftwareApplication"]);
                CountOfEachApp = Convert.ToInt16(DT.Rows[i]["CountOfEachApp"]);
                string Filename1 = DT.Rows[i]["Str_FileName"].ToString();
                string Filename = Filename1.Replace("/", "-");
                switch (SoftwareApplication)
                {
                    case 1:
                        if (Chk_Centeral)
                        {
                            string Type = "Centeral";
                            if (FeatchAndSavePackage(VersionReleaseVApplication, CountOfEachApp, Type, Filename, VersionNumber, LocalConnection))
                                OpperationIsOk += 1;
                        }
                        break;
                    case 2:
                        if (Chk_QC)
                        {
                        }
                        break;
                    case 3:
                        if (Chk_Storage)
                        {
                        }
                        break;
                    case 4:
                        if (Chk_Journal)
                        {
                        }
                        break;
                }
            }
            if (OpperationIsOk == 1)// just Centeral
                return true;
            else
                return false;
        }
        // whole of version to gather in a function for update EXE\










        // save 1 Mg , 1 Mg of versions in a function for update EXE and show how much remaind
        private Boolean FeatchAndSavePackage(int VersionReleaseVApplication, int CountOfEachApp, string Type, string Filename, string VersionNumber, string connection)
        {
            conID = new SqlConnection(connection);

            conID.Open();
            sqlTran = conID.BeginTransaction();

            Boolean FierstDel = false;
            try
            {
                string vernn = GetDBVersion("Centeral", connection);
                if (vernn == VersionNumber || vernn == "")
                {
                    FierstDel = true;
                    DeleteFromDB(1, Type, "", connection);
                }
                int CheckRepet = 0;
                int CheckRepet1 = 0;
                for (int i = 0; i < CountOfEachApp; i++)
                {
                    byte[] Package;
                    DataTable dt2 = new DataTable();// Tables["EachPart"]
                    dt2 = GetVersionInfoFileFromDataBase(VersionReleaseVApplication, i);
                    Package = (byte[])dt2.Rows[0]["Bin_FileContent"];
                    string CRC = dt2.Rows[0]["Str_CRC"].ToString();
                    string FileSize = dt2.Rows[0]["Str_PartSize"].ToString();
                    //for each part(exept last part) run one time 
                    string check = CalculateChecksum(Package);
                    if (CRC == check)
                    {
                        if (SaveVersionInLabDBVComit(Type, Filename, FileSize, CRC, VersionNumber, i, Package, "", connection) == false)
                        {
                            i--;
                            CheckRepet1++;
                            if (CheckRepet1 == 5)
                            {
                                //DeleteFromDB(2, Type, Filename, connection);
                                try
                                {
                                    sqlTran.Rollback();
                                }
                                catch (Exception EX)
                                {
                                    SaveTextExeption(EX.Message.ToString());
                                }
                                conID.Close();
                                return false;
                            }


                        }
                        else
                        {
                            CheckRepet1 = 0;// Not Catch In Loop If There area Mistake
                        }

                        CheckRepet = 0;
                    }
                    else
                    {
                        i--;
                        CheckRepet++;
                        if (CheckRepet == 5)
                        {
                            //DeleteFromDB(2, Type, Filename, connection);
                            try
                            {
                                sqlTran.Rollback();
                            }
                            catch (Exception EX)
                            {
                                SaveTextExeption(EX.Message.ToString());
                            }
                            conID.Close();
                            return false;
                        }
                    }
                }

                sqlTran.Commit();
                conID.Close();
                if (FierstDel == false)
                    DeleteFromDB(3, Type, Filename, connection);

                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                try
                {
                    sqlTran.Rollback();
                    conID.Close();
                }
                catch (Exception eX)
                {
                    SaveTextExeption(eX.Message.ToString());
                }
                return false;
            }
        }
        // save 1 Mg , 1 Mg of versions in a function for update EXE and show how much remaind\









        // in Local DB
        public Boolean SaveVersionInLabDB(string Type, string FileName, string FileSize, string CRC, string VersionNumber, int part, byte[] Contant, string Description, string connection)
        {
            //string query = "INSERT INTO Tbl_VersionFiles (Str_Type, Str_FileName, Str_FileSize ,Str_CRC ,Str_VersionNo , Int_PartNo, Bin_FileContent, Str_Description ) VALUES ( @Str_Type, @Str_FileName , @Str_FileSize, @Str_CRC ,@Str_VersionNo, @Int_PartNo, @Bin_FileContent , @Str_Description )";
            SqlConnection con = new SqlConnection(connection);
            try
            {
                string query = "execute SP_Insert_VersionFiles @Str_Type = @Str_Typec, @Str_FileName = @Str_FileNamec, @Str_FileSize = @Str_FileSizec, @Str_CRC = @Str_CRCc, @Str_VersionNo = @Str_VersionNoc, @Int_PartNo = @Int_PartNoc, @Bin_FileContent = @Bin_FileContentc, @Str_Description = @Str_Descriptionc";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Str_Typec", Type);
                cmd.Parameters.AddWithValue("@Str_FileNamec", FileName);
                cmd.Parameters.AddWithValue("@Str_FileSizec", FileSize);
                cmd.Parameters.AddWithValue("@Str_CRCc", CRC);
                cmd.Parameters.AddWithValue("@Str_VersionNoc", VersionNumber);
                cmd.Parameters.AddWithValue("@Int_PartNoc", part);
                cmd.Parameters.AddWithValue("@Bin_FileContentc", Contant);
                cmd.Parameters.AddWithValue("@Str_Descriptionc", Description);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return false;
            }
        }
        public Boolean SaveVersionInLabDBVComit(string Type, string FileName, string FileSize, string CRC, string VersionNumber, int part, byte[] Contant, string Description, string connection)
        {
            //string query = "INSERT INTO Tbl_VersionFiles (Str_Type, Str_FileName, Str_FileSize ,Str_CRC ,Str_VersionNo , Int_PartNo, Bin_FileContent, Str_Description ) VALUES ( @Str_Type, @Str_FileName , @Str_FileSize, @Str_CRC ,@Str_VersionNo, @Int_PartNo, @Bin_FileContent , @Str_Description )";
            try
            {
                SqlCommand command = conID.CreateCommand();
                command.Transaction = sqlTran;

                string query = "execute SP_Insert_VersionFiles @Str_Type = @Str_Typec, @Str_FileName = @Str_FileNamec, @Str_FileSize = @Str_FileSizec, @Str_CRC = @Str_CRCc, @Str_VersionNo = @Str_VersionNoc, @Int_PartNo = @Int_PartNoc, @Bin_FileContent = @Bin_FileContentc, @Str_Description = @Str_Descriptionc";


                command.Parameters.AddWithValue("@Str_Typec", Type);
                command.Parameters.AddWithValue("@Str_FileNamec", FileName);
                command.Parameters.AddWithValue("@Str_FileSizec", FileSize);
                command.Parameters.AddWithValue("@Str_CRCc", CRC);
                command.Parameters.AddWithValue("@Str_VersionNoc", VersionNumber);
                command.Parameters.AddWithValue("@Int_PartNoc", part);
                command.Parameters.AddWithValue("@Bin_FileContentc", Contant);
                command.Parameters.AddWithValue("@Str_Descriptionc", Description);
                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean DeleteFromDB(Int16 mode, string type, string Name, string connection)
        {
            SqlConnection con = new SqlConnection(connection);
            if (mode == 1)
            {
                //delete last files in DB
                try
                {
                    con.Open();
                    string query1 = "execute SP_Delete_VersionFiles @Str_Type = '" + type + "', @int_state= 1 ";
                    SqlCommand cmd1 = new SqlCommand(query1, con);
                    cmd1.ExecuteNonQuery();
                    con.Close();
                    return true;
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    con.Close();
                    return false;
                }
                //delete last files in DB\
            }
            else if (mode == 2)
            {
                //delete New files in DB where name = Name
                try
                {
                    con.Open();
                    string query1 = "execute SP_Delete_VersionFiles @Str_Type = '" + type + "', @Str_Name = '" + Name + "', @int_state= 2 ";
                    SqlCommand cmd1 = new SqlCommand(query1, con);
                    cmd1.ExecuteNonQuery();
                    con.Close();
                    return true;
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    con.Close();
                    return false;
                }
                //delete last files in DB\
            }
            else if (mode == 3)
            {
                //delete old files in DB where name != Name
                try
                {
                    con.Open();
                    string query1 = "execute SP_Delete_VersionFiles @Str_Type = '" + type + "', @Str_Name = '" + Name + "', @int_state= 3 ";
                    SqlCommand cmd1 = new SqlCommand(query1, con);
                    cmd1.ExecuteNonQuery();
                    con.Close();
                    return true;
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    con.Close();
                    return false;
                }
                //delete last files in DB\
            }
            return false;
        }
        public string GetDBVersion(string type, string connection)
        {
            // find the version of file from DB
            SqlConnection con = new SqlConnection(connection);
            try
            {
                //SqlConnection con = new SqlConnection(@"Data Source=Keyhan\AZARJOO2014;Initial Catalog=VersionFiles;User ID=sa;Password=who");
                DataSet ds = new DataSet();
                con.Open();
                SqlCommand command = new SqlCommand("execute SP_Get_VersionFiles @Str_Type = '" + type + "', @int_state = 2", con);
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds, type);
                con.Close();
                DataRow dr = ds.Tables[type].Rows[0];
                return dr["Str_VersionNo"].ToString();
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return "";
            }
        }
        public string GetDBScriptVersion(string connection)
        {
            // find the version of Scripts from DB
            SqlConnection con = new SqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                SqlCommand command = new SqlCommand("select option_value from cTBL_Option where PRK_Option = 1", con);
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds, "ScriptVersion");
                con.Close();
                DataRow dr = ds.Tables["ScriptVersion"].Rows[0];
                return dr["option_value"].ToString();
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return "";
            }
        }
        public List<string> GetSQLServerSystemDrivesBySQLCode(string connection)
        {
            List<string> DriveList = new List<string>();
            // find the version of Scripts from DB
            SqlConnection con = new SqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();

                con.Open();
                SqlCommand command = new SqlCommand("execute SPC_GetServerDirectories", con);
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds, "Drives");
                con.Close();
                DataTable dt = ds.Tables["Drives"];
                for (int i = 0; i < dt.Rows.Count; i++)
                    DriveList.Add(dt.Rows[i]["Name"].ToString());
                return DriveList;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return null;
            }
        }
        public Boolean CreateFolderInSQLServerSystemBySQLCode(string Path, string connection)
        {
            SqlConnection con = new SqlConnection(connection);
            try
            {
                con.Open();
                string query = "EXEC master.dbo.xp_create_subdir '" + @Path + "'"; ;
                SqlCommand cmd1 = new SqlCommand(query, con);
                cmd1.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return false;
            }
        }
        public String GetDbBackup(string DbName, string Path, string connection, int MinTimeOut = 2)
        {
            SqlConnection con = new SqlConnection(connection);
            try
            {
                con.Open();
                string query = "execute SP_Create_Restore_DbBackup @Str_DbName = '" + DbName + "', @Str_Path = '" + Path + "' , @int_state = 1";
                SqlCommand cmd1 = new SqlCommand(query, con);
                cmd1.CommandTimeout = MinTimeOut * 60 * 1000;
                cmd1.ExecuteNonQuery();
                con.Close();
                return "Ok";
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return "Error : " + EX.Message.ToString();
            }
        }
        public string GetLabID(DataTable DTParsicMaster)
        {
            string ConnectionString = "";
            string DB_ServerName = "";
            string DB_Name = "";
            string DB_Username = "";
            string DB_Password = "";
            string Lab_id = "";
            try
            {
                //find lab_id
                foreach (DataRow dr in DTParsicMaster.Rows)
                {
                    if ((dr["DBList_IsActive"].ToString() == "True"))
                    {
                        DB_ServerName = dr["DBList_Server"].ToString();
                        DB_Name = dr["DBList_Name"].ToString();
                        DB_Username = dr["DBList_Username"].ToString();
                        DB_Password = dr["DBList_Password"].ToString();
                        ConnectionString = MakeConnectionString(DB_ServerName, DB_Name, DB_Username, DB_Password);
                    }
                }
                SqlConnection con = new SqlConnection(ConnectionString);

                string CommandText = "select Top 1 ISNULL(Option_value,0)  from tbl_option where Option_ID = 'ParsicLabID'";
                con.Open();
                try
                {
                    SqlCommand Cmd = new SqlCommand(CommandText, con);
                    Lab_id = Cmd.ExecuteScalar().ToString();
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    Lab_id = "0";
                }
                con.Close();

                //find lab_id\
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                Lab_id = "0";
            }

            return Lab_id;
        }
        private Boolean TestConnectionToDB(SqlConnection cn)
        {
            try
            {
                cn.Open();
                cn.Close();
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }





        public Boolean SendLocalVersionReleaseVLog(int VersionReleaseID, int ParsicUserID, string VersionFrom, string VersionTo, string ErrorLog, string Description, int AppType, int LogType, string ComputerName, string DataBaseName, Boolean IsActiveDB, Boolean IsEXE, Int64 ExeSize, string connection, Boolean user_is_from_Parsic, int LabID)
        {
            // APP Type 1 = Centeral, 2 = QC, 3 = Storage, 4 = Journal
            // LogType  0 = Try , 1 = Success
            // IsEXE    0 = Sripts, 1 = EXE
            //SendBackLog("DbName : "+DataBaseName + " ----- From: " + VersionFrom + " ----- To: " + VersionTo + " ----- Is Exe?: " + IsEXE.ToString() + " ----- Descriptiond: " + Description + " -----Log: "+ ErrorLog);
            SqlConnection con = new SqlConnection(connection);
            try
            {
                string query = "execute SP_Insert_VersionReleaseVLogs @Frk_VersionReleaseID = " + VersionReleaseID + ", @Frk_ParsicUserID = " + ParsicUserID + ", @Str_VersionFrom = '" + VersionFrom + "', @Str_VersionTo = '" + VersionTo + "', @Str_ErrorLog = N'" + ErrorLog.Replace("'", "''") + "', @Str_Description = N'" + Description + "', @Int_AppType = " + AppType + ", @Int_LogType = " + LogType + ", @Str_ComputerName = N'" + ComputerName + "',@Str_DataBaseName = N'" + DataBaseName + "', @Bit_IsActiveDB = " + IsActiveDB + ", @Bit_IsEXE = " + IsEXE + ", @Str_ExeSize = '" + ExeSize.ToString() + "', @Int_State = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                SendBackLog("Error Exception(Local DB) : " + EX.Message.ToString());
                return false;
            }
            if (user_is_from_Parsic == false)
            {
                try
                {
                    //return Updator_Boolean_Function(11, LabID, ParsicUserID, VersionFrom, VersionTo, Description, ErrorLog.Replace(Environment.NewLine,"!@#$%^&**&^%$#@!").Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), LogType, AppType, ComputerName, DataBaseName, IsActiveDB, IsEXE, ExeSize);
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return false;
                }
            }
            return true;
        }
        public Boolean SendLocalVersionReleaseVSplitsSubLogs(int ParsicUserID, string ComputerName, string DataBaseName, Boolean IsEXE, string Version, int Number, int WholeCount, string ErrorLog, string Description, string connection)
        {
            // IsEXE    0 = Sripts, 1 = EXE

            SqlConnection con = new SqlConnection(connection);
            try
            {
                string query = "execute SP_Insert_VersionReleaseVLogs @Frk_ParsicUserID = " + ParsicUserID + ", @Str_ComputerName = N'" + ComputerName + "', @Str_DataBaseName = N'" + DataBaseName + "', @Bit_IsEXE = " + IsEXE + ", @Str_Version = '" + Version + "', @Int_Order = " + Number + ", @Int_WholeCount = " + WholeCount + ", @Str_ErrorLog = N'" + ErrorLog.Replace("'", "''") + "', @Str_Description = N'" + Description + "', @Int_State = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return false;
            }
        }

        public Boolean TruncateVersionReleaseLogsandSubLogs(string connection)
        {
            SqlConnection con = new SqlConnection(connection);
            try
            {
                string query = "Truncate Table Tbl_VersionReleaseVSplitsSubLogs ";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                con.Close();
                return false;
            }
        }


        public string SaveDescriptionScriptsInner(DataTable DtScripts, DataTable DtDescriptions, Boolean IsDefultDB, string DBName, string DBVersion, int ParsicUserID, string LocalConnection, Int32 WholeCount = 0)
        {
            try
            {
                if (IsDefultDB)
                {
                    // Just run For Default DB
                    string RetDescription = SaveDescriptions(DtDescriptions, DBName, ParsicUserID, LocalConnection);// save Descriptions
                    if (RetDescription == "OK")
                    {
                        string RetScripts = SaveScripts(DtScripts, DBName, DBVersion, ParsicUserID, LocalConnection, WholeCount);// Do Scripts
                        if (RetScripts == "OK")
                        {
                            return "OK";
                        }
                        else
                        {
                            return RetScripts;
                        }
                    }
                    else
                    {
                        return RetDescription;
                    }
                }
                else
                {
                    //Run For Whole Data Base Except Default Data Base
                    string RetScripts = SaveScripts(DtScripts, DBName, DBVersion, ParsicUserID, LocalConnection, WholeCount);// Do Scripts
                    if (RetScripts == "OK")
                    {
                        return "OK";
                    }
                    else
                    {
                        return RetScripts;
                    }
                }
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "Error : " + EX.Message.ToString();
            }

        }
        public string SaveDescriptions(DataTable DtDescriptions, string DBName, int ParsicUserID, string LogConnection)
        {
            // Save Description Just On Default Data Base
            string VersionNumber = "";
            string Title = "";
            string Description = "";
            int Order = 0;
            Boolean Visibility = false;
            Boolean LerningTips = false;
            SendBackLog("DbName : " + DBName + " Start downloading Lab Descriptions ");
            try
            {

                int Code = 0;
                for (int i = 0; i < DtDescriptions.Rows.Count; i++)
                {
                    VersionNumber = DtDescriptions.Rows[i]["Str_VersionNumber"].ToString();
                    Title = DtDescriptions.Rows[i]["str_Title"].ToString();
                    Description = DtDescriptions.Rows[i]["str_Description"].ToString();
                    Order = Convert.ToInt16(DtDescriptions.Rows[i]["int_Order"]);
                    Visibility = Convert.ToBoolean(DtDescriptions.Rows[i]["bit_Visibility"]);
                    LerningTips = Convert.ToBoolean(DtDescriptions.Rows[i]["bit_LerningTips"]);
                    Code = Convert.ToInt16(DtDescriptions.Rows[i]["int_Code"]);
                    string tmp = "USE [" + DBName + "]\r\n\r\n" + "execute SP_Insert_VersionDescriptions @frk_ParsicUserID = " + ParsicUserID + ", @Str_VersionNumber = '" + VersionNumber + "', @str_Title = '" + Title + "', @str_Description = '" + Description + "', @int_Order = " + Order + ", @bit_Visibility = " + Visibility + ", @bit_LerningTips = " + LerningTips + ", @int_Code = " + Code + "";
                    DScommand.CommandText = tmp;

                    if (RollBack == false)
                    {
                        DScommand.ExecuteNonQuery();
                    }
                    else
                    {
                        sqlDSTran.Rollback();
                        string strLogMessage = "RollBack : Description, database " + DBName + ":" + DateTime.Now.ToString() + ".\r\n\r\n ";
                        return strLogMessage;
                    }


                }

                SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DBName, false, VersionNumber, 0, 0, "OK", "Description", LogConnection);
                SendBackLog("DbName : " + DBName + " Finis downloading Lab Descriptions");
                return "OK";
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                sqlDSTran.Rollback();
                string strLogMessage = "Error For Save Description on database " + DBName + ":" + DateTime.Now.ToString() + ".\r\n\r\nVersion Number: " + VersionNumber + " - Description No: " + Order + ".\r\n\r\nOriginal Error Message:\r\n\r\n" + EX.Message.ToString();
                SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DBName, false, VersionNumber, 0, 0, strLogMessage, "Description", LogConnection);
                return strLogMessage;
            }
        }
        public string SaveScripts(DataTable DtScriptions, string DBName, string DBVersion, int ParsicUserID, string LogConnection, Int32 WholCount = 0)
        {
            // Execute Scripts On Data Bases
            string Script = "";
            string VerNumber = "";
            string ScriptNum = "";

            try
            {
                SendBackLog("Scripts : Start DbName : " + DBName + "     " + VerNumber + "");
                foreach (DataRow dr in DtScriptions.Rows)
                {
                    bool SendLog = false;
                    if (Convert.ToDouble(DBVersion) < Convert.ToDouble(dr["Str_VersionNumber"]))
                    {
                        SendLog = true;
                        Script = dr["str_Script"].ToString().Replace("!@#$%^&**&^%$#@!", Environment.NewLine).Replace("$#@!!@#$", "'").Replace("*&^%%^&*", "-");
                        VerNumber = dr["Str_VersionNumber"].ToString();
                        ScriptNum = dr["int_Order"].ToString();
                        string tmp = "USE [" + DBName + "]\r\n\r\n" + Script;
                        string[] Splitter = { "GO ", "Go ", "gO ", "go ", " GO", " Go", " gO", " go", "\nGO", "\nGo", "\ngO", "\ngo", "GO\n", "Go\n", "gO\n", "go\n" };
                        string[] Commands = tmp.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string SubCommand in Commands)
                        {
                            try
                            {
                                DScommand.CommandText = SubCommand;
                                if (RollBack == false)
                                {
                                    DScommand.ExecuteNonQuery();
                                }
                                else
                                {
                                    sqlDSTran.Rollback();
                                    string strLogMessage = "RollBack : Scripts, database " + DBName + ":" + DateTime.Now.ToString() + ".\r\n\r\n";
                                    return strLogMessage;
                                }

                            }
                            catch (Exception EX)
                            {
                                SaveTextExeption(EX.Message.ToString());
                                sqlDSTran.Rollback();
                                string strLogMessage = "Error on database " + DBName + ":" + DateTime.Now.ToString() + ".\r\n\r\nVersion Number: " + VerNumber + " - Script No: " + ScriptNum + ".\r\n\r\n Original Command is:\r\n\r\n" + Script + "\r\n\r\nOriginal Error Message:\r\n\r\n" + EX.Message.ToString();
                                return strLogMessage;
                            }
                        }

                    }
                    if (SendLog == true)
                    {
                        SendBackLog("Scripts : DbName : " + DBName + "      Version: " + VerNumber + "     SCriptNumber : " + ScriptNum + "");
                        SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DBName, false, VerNumber, counter, WholCount, "OK", "Scripts", LogConnection);
                    }
                    counter++;
                }
                SendBackLog("Scripts : Finish DbName : " + DBName + "     " + VerNumber + "");
                return "OK";
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                sqlDSTran.Rollback();
                string strLogMessage = "Scripts : Error on database " + DBName + ":" + DateTime.Now.ToString() + ".\r\n\r\nVersion Number: " + VerNumber + " - Script No: " + ScriptNum + ".\r\n\r\n Original Command is:\r\n\r\n" + Script + "\r\n\r\nOriginal Error Message:\r\n\r\n" + EX.Message.ToString();
                SendLocalVersionReleaseVSplitsSubLogs(ParsicUserID, "", DBName, false, VerNumber, 0, 0, strLogMessage, "Scripts", LogConnection);
                return strLogMessage;
            }
        }
        // in Local DB\








        // other Function / inner Function
        public void FindPaths()
        {
            //Find Paths
            if (CenteralPath == "")
                CenteralPath = FindFolder("Centeral");
            if (QCPath == "")
                QCPath = FindFolder("QC");
            if (StoragePath == "")
                StoragePath = FindFolder("Storage");
            if (JournalPath == "")
                JournalPath = FindFolder("Machines");
            //Find Paths\
        }
        public string FindFolder(string FolderName)
        {
            //find direct we look for
            try
            {

                if (System.IO.Directory.Exists("C:\\Program Files (x86)\\ParsicPol\\" + FolderName))
                {
                    return "C:\\Program Files (x86)\\ParsicPol\\" + FolderName;
                }
                else if (System.IO.Directory.Exists("C:\\Program Files\\ParsicPol\\" + FolderName))
                {
                    return "C:\\Program Files\\ParsicPol\\" + FolderName;
                }
                else
                {
                    string[] drives = System.IO.Directory.GetLogicalDrives();
                    foreach (string str in drives)
                    {
                        if (System.IO.Directory.Exists(str + "Program Files (x86)\\ParsicPol\\" + FolderName))
                        {
                            return str + "Program Files (x86)\\ParsicPol\\" + FolderName;
                        }
                        else if (System.IO.Directory.Exists(str + "Program Files\\ParsicPol\\" + FolderName))
                        {
                            return str + "Program Files\\ParsicPol\\" + FolderName;
                        }



                    }
                }
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "";
            }

            return "";
        }
        public string GetVersion(string filepath)
        {
            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(filepath);
                string ver = versionInfo.ProductVersion;


                string version = "";
                int i = 0;
                foreach (char c in ver)
                {
                    version += c;
                    i++;
                    if (i == 5)
                    {
                        break;
                    }
                }

                if (1 < Convert.ToDouble(version) && Convert.ToDouble(version) < 30)
                {
                    return ver;
                }
                else
                {
                    return ver;
                }
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            return "";
        }
        public string CalculateChecksum(byte[] byteToCalculate)
        {
            //CheackSum
            try
            {
                int checksum = 0;
                foreach (byte chData in byteToCalculate)
                {
                    checksum += chData;
                }
                checksum &= 0xff;
                return checksum.ToString("X2");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                //MessageBox.Show("error in cheack sum \r\n ", "errore");
            }
            return null;
        }
        public int SplitFiles(byte[] ByteFile, Int64 VersionReleaseVApplicationID)
        {
            // split a big array of byto to smal part And Save In DB
            try
            {
                Int64 lenght = 0;
                Int64 CountOfSplites = 0;
                int Error = 0;
                Int64 Counter = 0;
                Int64 SplitSize = 1048576;
                Int64 LenghtOfLastPacage;
                lenght = ByteFile.Length;
                CountOfSplites = lenght / SplitSize;
                LenghtOfLastPacage = lenght % SplitSize;
                byte[] buffer = new byte[SplitSize];
                byte[] buffefSplit = new byte[LenghtOfLastPacage];
                int j = 0;
                for (Int64 k = 0; k < lenght; k++)
                {
                    byte b = ByteFile[k];
                    if (j == SplitSize && Counter <= CountOfSplites)
                    {
                        //for each part(exept last part) run one time 
                        string check = CalculateChecksum(buffer);
                        if (InsertEachPart(buffer, VersionReleaseVApplicationID, check, Counter, SplitSize.ToString()))
                        {

                        }
                        else
                        {

                            k -= SplitSize + 1;
                            Counter--;
                            Error++;
                            b = ByteFile[k];
                            if (Error >= 5)
                                break;
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        j = 0;
                        Counter++;
                    }

                    if (Counter < CountOfSplites)
                    {
                        //for each part 
                        buffer[j] = b;
                        j++;
                    }
                    else
                    {
                        //for last part
                        buffefSplit[j] = b;
                        j++;
                    }
                }

                if (j == LenghtOfLastPacage && Counter == CountOfSplites)
                {
                    //for last patr run one time
                    string check = CalculateChecksum(buffefSplit);
                    InsertEachPart(buffefSplit, VersionReleaseVApplicationID, check, Counter, LenghtOfLastPacage.ToString());
                    Array.Clear(buffefSplit, 0, buffefSplit.Length);
                    j = 0;
                }
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return -1;
            }
            return 1;
            // split a big array of byto to smal part And Save In DB\
        }
        public byte[] StreamFile(string filepath)
        {
            //Convert file to byte[]
            try
            {
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                byte[] ImageData = new byte[fs.Length];
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                return ImageData;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public string ZipDirectori(string path, string FileName)
        {
            try
            {
                //ZIP Directory
                string startPath = @path;
                string zipPath = @path + FileName;
                ZipFile.CreateFromDirectory(startPath, zipPath);
                return zipPath;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "";
            }
        }

        public int DeleteZipFile(string Path)
        {
            //Delete Zip File
            try
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return 0;
            }
            return 0;
        }
        public string MakeConnectionString(string DB_ServerName, string DB_Name, string DB_Username, string DB_Password)
        {
            string connection = @"Data Source=" + DB_ServerName + ";Initial Catalog=" + DB_Name + ";User ID=" + DB_Username + ";Password=" + DB_Password + ";Connect Timeout=10000;";
            return connection;
        }
        public Boolean TransactionRollback()
        {
            try
            {
                RollBack = true;
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }


        public List<string> GetODBCInformation()
        {

            List<string> Result = new List<string>();
            clsRegistry reg = new clsRegistry();
            string strInstanceName = reg.ReadFromRegistry(int.Parse(ConfigurationManager.AppSettings["RegistryBaseKey"]), ConfigurationManager.AppSettings["RegistryPath"], ConfigurationManager.AppSettings["InstanceNameKeyName"]);
            string strDBName = reg.ReadFromRegistry(int.Parse(ConfigurationManager.AppSettings["RegistryBaseKey"]), ConfigurationManager.AppSettings["RegistryPath"], ConfigurationManager.AppSettings["DBNameKeyName"]);
            string strUserName = reg.ReadFromRegistry(int.Parse(ConfigurationManager.AppSettings["RegistryBaseKey"]), ConfigurationManager.AppSettings["RegistryPath"], ConfigurationManager.AppSettings["UserNameKeyName"]);
            string strPassword = ConfigurationManager.AppSettings["Password"].ToString();
            string strTableName = ConfigurationManager.AppSettings["TableName"].ToString();
            string strSourceFieldName = ConfigurationManager.AppSettings["SourceFieldName"].ToString();
            string strTargetFieldName = ConfigurationManager.AppSettings["TargetFieldName"].ToString();
            string strSourceFieldValueForInstanceName = ConfigurationManager.AppSettings["SourceFieldValueForInstanceName"].ToString();
            string strSourceFieldValueForDBName = ConfigurationManager.AppSettings["SourceFieldValueForDBName"].ToString();
            string strSourceFieldValueForTableName = ConfigurationManager.AppSettings["SourceFieldValueForTableName"].ToString();
            string strSourceFieldValueForUserName = ConfigurationManager.AppSettings["SourceFieldValueForUserName"].ToString();
            string strSourceFieldValueForPassword = ConfigurationManager.AppSettings["SourceFieldValueForPassword"].ToString();
            string strSourceFieldValueForMustBackup = ConfigurationManager.AppSettings["SourceFieldValueForMustBackup"].ToString();

            if (!CheckTBLOptionsExistancy(strInstanceName, strDBName, strUserName, strPassword))
            {
                Result.Add("در واکشی اطلاعات او دی بی سی (اطلاعات ریجستری) خطا رخ داده است ");
                return Result;
            }
            try
            {
                Result.Add(GetValue(strInstanceName, strDBName, strUserName, strPassword, strTableName, strSourceFieldName, strSourceFieldValueForInstanceName, strTargetFieldName)); // InstanceName

                Result.Add(GetValue(strInstanceName, strDBName, strUserName, strPassword, strTableName, strSourceFieldName, strSourceFieldValueForDBName, strTargetFieldName)); // DBName

                Result.Add(GetValue(strInstanceName, strDBName, strUserName, strPassword, strTableName, strSourceFieldName, strSourceFieldValueForTableName, strTargetFieldName)); // TableName

                Result.Add(GetValue(strInstanceName, strDBName, strUserName, strPassword, strTableName, strSourceFieldName, strSourceFieldValueForUserName, strTargetFieldName)); // UserName

                Result.Add(GetValue(strInstanceName, strDBName, strUserName, strPassword, strTableName, strSourceFieldName, strSourceFieldValueForPassword, strTargetFieldName)); // Password

                Result.Add(GetValue(strInstanceName, strDBName, strUserName, strPassword, strTableName, strSourceFieldName, strSourceFieldValueForMustBackup, strTargetFieldName)); // MustBackup
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                Result.Clear();
                Result.Add("در ارتباط با پارسیک مستر و بانک های آن مشکلی بوجود آمده است");
                return Result;
            }
            return Result;

        }
        public Boolean CheckTBLOptionsExistancy(string InstanceName, string DBName, string UserName, string Password)
        {
            SqlConnection scon = new SqlConnection(MakeConnectionString(InstanceName, DBName, UserName, Password));
            SqlCommand scom = new SqlCommand();
            scom.Connection = scon;
            scom.CommandType = CommandType.Text;
            string strCommand = "SET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO\r\nUSE [DB_parsicmaster]\r\nGO\r\nif not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TBL_Options]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)\r\n begin\r\nCREATE TABLE [dbo].[TBL_Options]([OptionName] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,\r\n[OptionValue] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL) ON [PRIMARY]\r\ninsert into TBL_Options (OptionName,OptionValue) values ('UpdateArchivePath','')\r\ninsert into TBL_Options (OptionName,OptionValue) values ('OutputPath','')\r\ninsert into TBL_Options (OptionName,OptionValue) values ('DBListInstanceName','" + InstanceName + "')\r\ninsert into TBL_Options (OptionName,OptionValue) values ('DBListDBName','" + DBName + "')\r\ninsert into TBL_Options (OptionName,OptionValue) values ('DBListTableName','TBL_DBList')\r\ninsert into TBL_Options (OptionName,OptionValue) values ('DBListUserName','sa')\r\ninsert into TBL_Options (OptionName,OptionValue) values ('DBListPassword','who')\r\ninsert into TBL_Options (OptionName,OptionValue) values ('DBListMustBackup','True')\r\nend";
            string[] CommandsList = new string[15];
            string[] Splitter = { "\r\nGO" };
            CommandsList = strCommand.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                scon.Open();
                foreach (string strComm in CommandsList)
                {
                    scom.CommandText = strComm;
                    scom.ExecuteNonQuery();
                }

                scom.CommandText = "select Count(*) from TBL_Options where (OptionName = 'Backup_Path')";
                if (int.Parse(scom.ExecuteScalar().ToString()) != 1)
                {
                    scom.CommandText = "insert into TBL_Options (OptionName,OptionValue) values ('Backup_Path','E:\\Parsic_Ver_Backup')";
                    scom.ExecuteNonQuery();
                }

                scon.Close();
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }

        }
        public string GetValue(string DBInstanceName, string DBName, string UserID, string Password, string TableName, string SourceFieldName, string SourceFieldValue, string TargetFieldName)
        {
            SqlConnection SCon = new SqlConnection("Data Source=" + DBInstanceName + ";Initial Catalog=" + DBName + ";User ID=" + UserID + ";password=" + Password + ";");//Connect Timeout=5;");
            SqlCommand SCom = new SqlCommand();
            //SCom.CommandTimeout = 5;
            SCom.Connection = SCon;
            SCom.CommandType = CommandType.Text;
            SCom.CommandText = "Select " + TargetFieldName + " From " + TableName + " where (" + SourceFieldName + " = '" + SourceFieldValue + "')";
            object Result = string.Empty;
            try
            {

                SCom.Connection.Open();
                Result = SCom.ExecuteScalar().ToString();
                SCom.Connection.Close();
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "Error : " + DBInstanceName + EX.Message;
            }

            return Result.ToString();
        }
        public DataTable UpdateGridView(List<string> ODBCInfo)
        {
            DataTable DT = new DataTable();
            try
            {
                if (ODBCInfo.Count == 1)
                    return null;

                DT = GetDBList(ODBCInfo[0], ODBCInfo[1], ODBCInfo[2], ODBCInfo[3], ODBCInfo[4]);

                if (DT == null)
                    return null;

                DT.Columns.Add("Select", typeof(int));
                DT.Columns.Add("DBCurruntVersion");
                DT.Columns.Add("ToVersion");
                DT.Columns.Add("str_DBList_CreationDate");
                //DS.Tables.Add("ToVersion");
                //DS.Tables["ToVersion"].Columns.Add("ToVersion", typeof(string));



                string dblDBVersion = string.Empty;
                int intCounter2 = 0;
                string strErrorDBs = string.Empty;
                while (intCounter2 < DT.Rows.Count)
                {


                    dblDBVersion = GetDBScriptVersion(MakeConnectionString((string)(DT.Rows[intCounter2]["DBList_Server"]), DT.Rows[intCounter2]["DBList_Name"].ToString(), DT.Rows[intCounter2]["DBList_Username"].ToString(), DT.Rows[intCounter2]["DBList_Password"].ToString()));
                    try
                    {
                        if (Math.Round(double.Parse(dblDBVersion), 3) == -1)
                        {
                            strErrorDBs += " - " + DT.Rows[intCounter2]["DBList_Name"].ToString();
                            DT.Rows[intCounter2].Delete();
                            DT.Rows[intCounter2].AcceptChanges();

                        }
                        else
                        {
                            DT.Rows[intCounter2]["Select"] = false;
                            DT.Rows[intCounter2]["DBCurruntVersion"] = dblDBVersion;
                            DT.Rows[intCounter2].AcceptChanges();
                            intCounter2++;
                        }
                    }
                    catch (Exception EX)
                    {








                        SaveTextExeption(EX.Message.ToString());
                        if (dblDBVersion == "-1")
                        {

                            strErrorDBs += " - " + DT.Rows[intCounter2]["DBList_Name"].ToString();
                            DT.Rows[intCounter2].Delete();
                            DT.Rows[intCounter2].AcceptChanges();

                        }
                        else
                        {

                            DT.Rows[intCounter2]["Select"] = false;
                            DT.Rows[intCounter2]["DBCurruntVersion"] = dblDBVersion;
                            DT.Rows[intCounter2].AcceptChanges();
                            intCounter2++;
                        }
                    }



                }

                foreach (DataRow dr in DT.Rows)




                {

                    DateTime TMP_Date_Time;
                    if (DateTime.TryParse(dr["DBList_CreationDate"].ToString(), out TMP_Date_Time))
                    {
                        dr["str_DBList_CreationDate"] = TMP_Date_Time.Year.ToString("0000") + "/" + TMP_Date_Time.Month.ToString("00") + "/" + TMP_Date_Time.Day.ToString("00");

                    }
                    else
                    {
                        dr["str_DBList_CreationDate"] = string.Empty;
                    }
                    dr.AcceptChanges();
                }


                DT.AcceptChanges();

            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            return DT;
        }
        public DataTable GetDBList(string DBInstanceName, string DBName, string TablaName, string UserID, string Password)
        {

            SqlDataAdapter SDA = new SqlDataAdapter("Select * From " + TablaName, MakeConnectionString(DBInstanceName, DBName, UserID, Password));
            DataTable DT = new DataTable();
            try
            {

                SDA.Fill(DT);
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
            return DT;
        }
        public Boolean SaveTextExeption(string Text)
        {
            try
            {
                Text = DateTime.Now + "\r\n----------------------------------------------------------------\r\n" + Text + "\r\n\r\n";
                File.AppendAllText(@"C:\ParsicWebTemp\AutoUpdaterErrorLog.txt", Text + Environment.NewLine);
                return true;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }

        }
        public string GetIPAddress()
        {
            //find Ip Valid From 2 site
            string ip = "0";
            try
            {
                ip = new WebClient().DownloadString("https://icanhazip.com");
                return ip;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            try
            {
                String address = "";
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    address = stream.ReadToEnd();
                }
                int first = address.IndexOf("Address: ") + 9;
                int last = address.LastIndexOf("</body>");
                address = address.Substring(first, last - first);
                ip = address + "\n";
                return ip;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "0";
            }

        }

        public string Get_UserName()
        {
            // Return "hbb" & Now.Year & "parsic" & Now.Month
            return "Updater";
        }
        private string Get_Password()
        {
            try
            {
                string MyKey = "rPGhgO2urgm1k09RTU4cIIIbISbFKUXI2UqVGsYo91RXIZOHH3YsMmBmFWQbN7rhl8ds5GaMR/PXCUGOfaiDIFFIY1T49/uXg0frUHIJ1cArqch1zCJW6L78oqrtEEtcJk1arEy8F1Q=";
                DateTime MyTime = DateTime.Now;
                string MyDate = MyTime.Year + MyTime.Month.ToString().PadLeft(2, '0') + MyTime.Day.ToString().PadLeft(2, '0');
                string MyPass = "Parsic;" + MyTime.Second.ToString().PadLeft(2, '0') + ";Hbb;" + MyTime.Millisecond.ToString().PadLeft(3, '0') + ";!@#;" + MyDate + ";Raz;" + MyDate + MyTime.Second.ToString().PadLeft(2, '0') + MyTime.Millisecond.ToString().PadLeft(3, '0');

                MyPass = MySecurity.EncryptData(MyPass, MyKey);

                return MyPass;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return "";
            }
        }
        // other Function\  / inner Function\





        // interface Function between Ws function  and internal function

        public DataTable VersionInfoFileFromDataBase(int VersionReleaseVApplication, int Part)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(2, VersionReleaseVApplication.ToString(), Part.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public DataTable VersionCountFromDataBase(Int64 VersionNumberID)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(3, VersionNumberID.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            return _Dt;
        }
        public DataTable GetSpecialVersionInfoFromVersionNumber(string VerNum)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(4, VerNum.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public DataTable GetSpecialVersionInfoFromVersionReleaseVApp(Int64 VersionReleaseID)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(5, VersionReleaseID.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public DataTable GetListOfExeVersion(double _DBversion)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(1, _DBversion.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public DataTable GetListOfScriptVersion(double _DBversion)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(1, _DBversion.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public DataTable GetVersionCountFromDataBase(Int64 _VersionNumberID)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(3, _VersionNumberID.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            return _Dt;
        }
        public DataTable GetVersionInfoFileFromDataBase(int _VersionReleaseVApplication, int _Part)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(2, _VersionReleaseVApplication.ToString(), _Part.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public DataTable GetExistExe(Int64 VersionReleaseID)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(11, VersionReleaseID.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            return _Dt;
        }
        public DataTable GetScripts(Int64 VersionReleaseID)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(7, VersionReleaseID.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            return _Dt;
        }
        public DataTable GetDescription(Int64 VersionReleaseID)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(8, VersionReleaseID.ToString());
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
            }
            return _Dt;
        }
        public DataTable GetScriptVersions(String From, String To)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(9, From, To);
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }
        public DataTable GetDescriptionsVersions(String From, String To)
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(10, From, To);
                return _Dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }


        public Int64 InsertVersionReleaseInDB(int ParsicUser, string VersionNumber, int status, string Description, string prerequirement)
        {
            try
            {
                Int64 i = Updator_Int64_Function(1, ParsicUser.ToString(), VersionNumber, status.ToString(), Description.Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), prerequirement);
                return i;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return -1;
            }
        }
        public Int64 InsertToVersionReleaseVApplication(Int64 VersionReleaseID, int SoftwareApplication, string FileName, string FileSize, string CRC, string Description)
        {
            try
            {
                Int64 i = Updator_Int64_Function(2, VersionReleaseID.ToString(), SoftwareApplication.ToString(), FileName, FileSize, CRC, Description.Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"));
                return i;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return -1;
            }

        }


        public Boolean InsertEachPart(byte[] buffer, Int64 VersionReleaseVApplicationID, String check, Int64 Counter, string SplitSize)
        {
            try
            {
                return Updator_Boolean_Function(1, buffer, VersionReleaseVApplicationID, check, Counter, SplitSize, "", "", "", "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean Delete_VRVA_VRVAF(Int64 VersionReleaseVAppID)
        {
            //Delete VersionReleaseVApplication And VersionReleaseVApplicationFiles
            int i = 0;
            try
            {
                if (Updator_Boolean_Function(2, VersionReleaseVAppID, "", "", "", "", "", "", "", "", "", "", "", ""))
                {
                    i = 1;
                }
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
            if (i == 1)
            {
                try
                {
                    if (Updator_Boolean_Function(5, VersionReleaseVAppID, "", "", "", "", "", "", "", "", "", "", "", ""))
                    {
                        i = 2;
                    }
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return false;
                }
            }
            return true;
            if (i == 2)
            {
                return true;
            }
            return false;
        }
        public Boolean Delete_VR(Int64 VersionReleaseID)
        {
            //Delete VersionReleaseVApplication And VersionReleaseVApplicationFiles
            try
            {
                return Updator_Boolean_Function(3, VersionReleaseID, "", "", "", "", "", "", "", "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean Update_VersionRelease(Int64 VersionRelease, int ParsicUser, string VersionNumber, int statuse, string Description, string prerequirement)
        {
            //Update VersionRelease
            try
            {
                return Updator_Boolean_Function(4, VersionRelease, ParsicUser, VersionNumber, statuse, Description.Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), prerequirement, "", "", "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean Update_VersionReleaseWarning(Int64 VersionRelease, int ParsicUser, string Warning, string prerequirement)
        {
            //Update VersionRelease
            try
            {

                return Updator_Boolean_Function(10, VersionRelease, ParsicUser, Warning.Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), prerequirement, "", "", "", "", "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean InsertScript(int VersionReleaseID, int ParsicUserID, int Order, string Script, string Description)
        {   //Insert VersionReleaseScripts
            try
            {

                return Updator_Boolean_Function(6, VersionReleaseID, ParsicUserID, Order, Script.Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), Description.Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), "", "", "", "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean InsertDescription(int VersionReleaseID, int ParsicUserID, string Title, string Description, Int16 Order, Boolean Visibility, Boolean LerningTips, int code)
        {   //Insert VersionReleaseDescriptions
            try
            {
                //Updator_GetSysteminformation_Boolean(9, VersionReleaseID, ParsicUserID, Title, Description, Order, Visibility, LerningTips, code)
                return Updator_Boolean_Function(9, VersionReleaseID, ParsicUserID, Title, Description.Replace("'", "$#@!!@#$").Replace("-", "*&^%%^&*"), Order, Visibility, LerningTips, code, "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }
        public Boolean Delete_VRS(Int32 VersionReleaseID)
        {    //Delete VersionReleaseScripts For a special VersionReleasID
            try
            {
                return Updator_Boolean_Function(7, VersionReleaseID, "", "", "", "", "", "", "", "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }

        }
        public Boolean Delete_VRD(Int32 VersionReleaseID)
        {    //Delete VersionReleaseDescription For a special VersionReleasID
            try
            {
                return Updator_Boolean_Function(8, VersionReleaseID, "", "", "", "", "", "", "", "", "", "", "", "");
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }

        }
        public List<string> GetAllVersionsFromDataBase()
        {
            DataTable _Dt = new DataTable();
            try
            {
                _Dt = Updator_DataTable_GetFunction(6);

                List<string> ListOfVersion = new List<string>();
                List<double> ListOfVersiondouble = new List<double>();
                double NewVer;
                try
                {
                    for (int i = 0; i < _Dt.Rows.Count; i++)
                    {
                        ListOfVersiondouble.Add(Convert.ToDouble(_Dt.Rows[i]["Str_VersionNumber"]));
                    }
                    if (ListOfVersiondouble != null)
                        ListOfVersiondouble.Sort();

                    //ListOfVersion.Add((NewVer + 0.01).ToString("00.00"));

                    int j = ListOfVersiondouble.Count - 1;
                    NewVer = Convert.ToDouble(ListOfVersiondouble[j]);
                    ListOfVersion.Add((NewVer + 0.01).ToString("00.00"));
                    for (int i = j; i >= 0; i--)
                    {

                        ListOfVersion.Add(ListOfVersiondouble[i].ToString("0.00"));
                    }
                    //NewVer = Convert.ToDouble(ListOfVersiondouble[j]);
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    NewVer = Convert.ToDouble("3.99");
                    ListOfVersion.Add((NewVer + 0.01).ToString("0.00"));
                }
                return ListOfVersion;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }
        }

        // interface Function between Ws function  and internal function\





        // Connection To WebService
        public bool Parsic_user_send_log(Boolean user_is_from_Parsic, string version_from, string version_to, String DBname, int LabID, int Parsic_user_id, int _state = 0, string TXT = "")
        {
            string ComputerName = System.Environment.MachineName;
            if (user_is_from_Parsic == true)
            {
                //user is from parsic company and does not need to save this version and user try to does an example, so it not real\

            }
            else
            {
                //save log
                string Lab_id = LabID.ToString();
                if (Lab_id == "")
                {
                    return false;
                }
                string msg = "";
                // save log is real
                try
                {
                    //send log for version started stage
                    if (_state == 0)
                    {
                        MyWebservice.Service1SoapClient cc = new MyWebservice.Service1SoapClient();
                        if (cc.SaveLabVersionLog(Get_UserName(), Get_Password(), Convert.ToInt16(Lab_id), Parsic_user_id, version_from, version_to, ComputerName, "اقدام برای ورژن جدید" + TXT, DBname, 0, ref msg))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    //send log for version started stage\
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return false;
                }
                try
                {
                    //send log for version finished stage
                    if (_state == 1)
                    {
                        MyWebservice.Service1SoapClient cc = new MyWebservice.Service1SoapClient();
                        if (cc.SaveLabVersionLog(Get_UserName(), Get_Password(), Convert.ToInt16(Lab_id), Parsic_user_id, version_from, version_to, ComputerName, " ورژن جدید زده شد" + TXT, DBname, 1, ref msg))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (_state == 2)
                    {
                        MyWebservice.Service1SoapClient cc = new MyWebservice.Service1SoapClient();
                        if (cc.SaveLabVersionLog(Get_UserName(), Get_Password(), Convert.ToInt16(Lab_id), Parsic_user_id, version_from, version_to, ComputerName, " در ورژن زدن خطایی رخ داده است، تمام عملیات های انجام شده کنسل شد" + TXT, DBname, 1, ref msg))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (_state == 3)
                    {
                        MyWebservice.Service1SoapClient cc = new MyWebservice.Service1SoapClient();
                        if (cc.SaveLabVersionLog(Get_UserName(), Get_Password(), Convert.ToInt16(Lab_id), Parsic_user_id, version_from, version_to, ComputerName, " ورژن آزمایشگاه با موفقیت تغییر یافت" + TXT, DBname, 1, ref msg))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (_state == 4)
                    {
                        MyWebservice.Service1SoapClient cc = new MyWebservice.Service1SoapClient();
                        if (cc.SaveLabVersionLog(Get_UserName(), Get_Password(), Convert.ToInt16(Lab_id), Parsic_user_id, version_from, version_to, ComputerName, "تایید از طرف : " + TXT, DBname, 0, ref msg))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    //send log for version finished stage\
                }
                catch (Exception EX)
                {
                    SaveTextExeption(EX.Message.ToString());
                    return false;
                }
            }
            return false;
            //\save log

        }

        // to mace change easy if a function name change
        public DataTable Updator_DataTable_GetFunction(int Mode, string Parameter1 = "", string Parameter2 = "", string Parameter3 = "")
        {
            try
            {
                //MyWebTest.MyTestTicketingWS t = new MyWebTest.MyTestTicketingWS();
                //DataTable dt = new DataTable();
                //dt = t.Updator_GetSysteminformation_DataTable(Get_UserName(), Get_Password(), Mode, Parameter1, Parameter2, Parameter3);
                //return dt;
                DataTable dt = new DataTable();
                dt = TiketingWebService.Updator_GetSysteminformation_DataTable(Get_UserName(), Get_Password(), Mode, Parameter1, Parameter2, Parameter3);
                return dt;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return null;
            }

        }
        public Int64 Updator_Int64_Function(int Mode, string Parameter1 = "", string Parameter2 = "", string Parameter3 = "", string Parameter4 = "", string Parameter5 = "", string Parameter6 = "")
        {

            try
            {
                //MyWebTest.MyTestTicketingWS t = new MyWebTest.MyTestTicketingWS();
                //Int64 ans;
                //ans = t.Updator_GetSysteminformation_Int64(Get_UserName(), Get_Password(), Mode, Parameter1, Parameter2, Parameter3, Parameter4, Parameter5, Parameter6);
                //return ans;
                Int64 ans;
                ans = TiketingWebService.Updator_GetSysteminformation_Int64(Get_UserName(), Get_Password(), Mode, Parameter1, Parameter2, Parameter3, Parameter4, Parameter5, Parameter6);
                return ans;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return -1;
            }
        }
        public Boolean Updator_Boolean_Function(int Mode, object Parameter1, object Parameter2, object Parameter3, object Parameter4, object Parameter5, object Parameter6, object Parameter7, object Parameter8, object Parameter9, object Parameter10, object Parameter11, object Parameter12, object Parameter13)
        {
            try
            {
                //MyWebTest.MyTestTicketingWS t = new MyWebTest.MyTestTicketingWS();
                //Boolean ans = false;
                //ans = t.Updator_GetSysteminformation_Boolean(Get_UserName(), Get_Password(), Mode, Parameter1, Parameter2, Parameter3, Parameter4, Parameter5, Parameter6, Parameter7, Parameter8, Parameter9, Parameter10, Parameter11, Parameter12, Parameter13);
                //return ans;
                Boolean ans;
                ans = TiketingWebService.Updator_GetSysteminformation_Boolean(Get_UserName(), Get_Password(), Mode, Parameter1, Parameter2, Parameter3, Parameter4, Parameter5, Parameter6, Parameter7, Parameter8, Parameter9, Parameter10, Parameter11, Parameter12, Parameter13);
                return ans;
            }
            catch (Exception EX)
            {
                SaveTextExeption(EX.Message.ToString());
                return false;
            }
        }

        // to mace change easy if a function name change\ and // What hapend in WebServise which call 



        //      <WebMethod()>
        //Public Function Updator_GetSysteminformation_DataTable(UserName As String, Password As String, ByVal Mode As Integer, ByVal Parameter1 As String, ByVal Parameter2 As String, Parameter3 As String) As DataTable

        //    Dim ocm As SqlClient.SqlConnection
        //    Dim Dt As DataTable
        //    Try

        //        DisableService()

        //        If UserName.Contains("-") Or UserName.Contains("'") Or Password.Contains("-") Or Password.Contains("'") Or Parameter1.Contains("-") Or Parameter1.Contains("'") Or Parameter2.Contains("-") Or Parameter2.Contains("'") Or Parameter3.Contains("-") Or Parameter3.Contains("'") Then
        //            Throw New Exception(ErrorSystem(ErrorList.Err1_InvalidInputParameter))
        //        End If

        //        If Not Athentication(UserName, Password) Then
        //            Throw New Exception("دسترسی برای اتصال به سرور پارسیپل مجاز نمی باشد")
        //        End If

        //        ocm = New SqlClient.SqlConnection(ConnectionString)

        //        If Not ocm.State = ConnectionState.Open Then
        //            ocm.Open()
        //        End If


        //        Select Case Mode

        //            Case 1

        //                Dim Adp As New SqlClient.SqlDataAdapter("execute  SP_Get_versionInfo @int_state = 0 , @Flo_VersionNum = " & Convert.ToDouble(Parameter1) & "", ocm)
        //                Dt = New DataTable("VerInfo")
        //                Adp.Fill(Dt)

        //            Case 2

        //                Dim Adp As New SqlClient.SqlDataAdapter("execute  SP_Get_versionInfo @int_state = 2 , @int_VersionReleaseVApplication = " & Convert.ToInt16(Parameter1) & " , @PartNo = " & Convert.ToInt16(Parameter2) & "", ocm)
        //                Dt = New DataTable("EachPart")
        //                Adp.Fill(Dt)

        //            Case 3

        //                Dim Adp As New SqlClient.SqlDataAdapter("execute  SP_Get_versionInfo @int_state = 1 , @int_VersionRelease = " & Convert.ToInt64(Parameter1) & "", ocm)
        //                Dt = New DataTable("PackageCount")
        //                Adp.Fill(Dt)

        //            Case 4

        //                Dim Adp As New SqlClient.SqlDataAdapter("execute  SP_Get_versionInfo @int_state = 12 , @str_VersionNumber = '" & Parameter1 & "'", ocm)
        //                Dt = New DataTable("SpecialVer")
        //                Adp.Fill(Dt)

        //            Case 5

        //                Dim Adp As New SqlClient.SqlDataAdapter("execute  SP_Get_versionInfo @int_state = 13 , @int_VersionRelease = " & Convert.ToInt64(Parameter1) & "", ocm)
        //                Dt = New DataTable("SpecialVerRelVApp")
        //                Adp.Fill(Dt)

        //            Case 6

        //                Dim Adp As New SqlClient.SqlDataAdapter("execute  SP_Get_versionInfo @int_state = 14", ocm)
        //                Dt = New DataTable("AllVersion")
        //                Adp.Fill(Dt)

        //            Case 7

        //                Dim Adp As New SqlClient.SqlDataAdapter("execute  SP_Get_versionInfo @int_state = 15", @int_VersionRelease = " & Convert.ToInt64(Parameter1) & "", ocm)
        //                Dt = New DataTable("AllScripts")
        //                Adp.Fill(Dt)


        //        End Select


        //        SafeClose(ocm)
        //        Return Dt

        //    Catch ex As Exception
        //        SafeClose(ocm)
        //        ExceptionLog(ex)
        //        Return New DataTable("")
        //    End Try

        //End Function

        //<WebMethod()>
        //Public Function Updator_GetSysteminformation_Int64(UserName As String, Password As String, ByVal Mode As Integer, ByVal Parameter1 As String, ByVal Parameter2 As String, Parameter3 As String, Parameter4 As String, Parameter5 As String, Parameter6 As String) As Int64

        //    Dim ocm As SqlClient.SqlConnection
        //    Dim RetValue As Int64

        //    Try

        //        DisableService()

        //        If UserName.Contains("-") Or UserName.Contains("'") Or Password.Contains("-") Or Password.Contains("'") Or Parameter1.Contains("-") Or Parameter1.Contains("'") Or Parameter2.Contains("-") Or Parameter2.Contains("'") Or Parameter3.Contains("-") Or Parameter3.Contains("'") Or Parameter4.Contains("-") Or Parameter4.Contains("'") Or Parameter5.Contains("-") Or Parameter5.Contains("'") Or Parameter6.Contains("-") Or Parameter6.Contains("'") Then
        //            Throw New Exception(ErrorSystem(ErrorList.Err1_InvalidInputParameter))
        //        End If

        //        If Not Athentication(UserName, Password) Then
        //            Throw New Exception("دسترسی برای اتصال به سرور پارسیپل مجاز نمی باشد")
        //        End If

        //        ocm = New SqlClient.SqlConnection(ConnectionString)

        //        If Not ocm.State = ConnectionState.Open Then
        //            ocm.Open()
        //        End If

        //        Dim cmd As New System.Data.SqlClient.SqlCommand("", ocm)

        //        Select Case Mode

        //            Case 1

        //                Dim ParsicUser As Integer = Convert.ToInt16(Parameter1)
        //                Dim VersionNumber As String = Parameter2
        //                Dim status As Integer = Convert.ToInt16(Parameter3)
        //                Dim Description As String = Parameter4
        //                Dim prerequirement As String = Parameter5
        //                Dim query As String = "execute SP_Insert_VersionInformation @Frk_ParsicUser = " & ParsicUser & " , @Str_VersionNumber = N'" & VersionNumber & "', @Int_status = " & status & " , @Str_Description = N'" & Description & "', @Str_prerequirement = N'" & prerequirement & "', @int_state = 0"

        //                cmd.CommandText = query
        //                RetValue = cmd.ExecuteScalar()

        //            Case 2

        //                Dim VersionReleaseID As Int64 = Convert.ToInt64(Parameter1)
        //                Dim SoftwareApplication As Integer = Convert.ToInt16(Parameter2)
        //                Dim FileName As String = Parameter3
        //                Dim FileSize As String = Parameter4
        //                Dim CRC As String = Parameter5
        //                Dim Description As String = Parameter6
        //                Dim query As String = "execute  SP_Insert_VersionInformation @Frk_VersionRelease = " & VersionReleaseID & " , @Frk_SoftwareApplication = " & SoftwareApplication & ", @Str_FileName = '" & FileName & "' , @Str_FileSize = '" & FileSize & "' , @Str_CRC = '" & CRC & "', @Str_Description = N'" & Description & "', @int_state = 1"

        //                cmd.CommandText = query
        //                RetValue = cmd.ExecuteScalar()

        //        End Select

        //        SafeClose(ocm)
        //        Return RetValue

        //    Catch ex As Exception
        //        SafeClose(ocm)
        //        ExceptionLog(ex)
        //        Return -1
        //    End Try

        //End Function

        //<WebMethod()>
        //Public Function Updator_GetSysteminformation_Boolean(UserName As String, Password As String, ByVal Mode As Integer, ByVal Parameter1 As Object, ByVal Parameter2 As Object, Parameter3 As Object, Parameter4 As Object, Parameter5 As Object, Parameter6 As Object) As Boolean

        //    Dim ocm As SqlClient.SqlConnection
        //    Dim RetValue As Boolean = False

        //    Try

        //        DisableService()

        //        If UserName.Contains("-") Or UserName.Contains("'") Or Password.Contains("-") Or Password.Contains("'") Or Parameter2.ToString.Contains("-") Or Parameter2.ToString.Contains("'") Or Parameter3.ToString.Contains("-") Or Parameter3.ToString.Contains("'") Or Parameter4.ToString.Contains("-") Or Parameter4.ToString.Contains("'") Or Parameter5.ToString.Contains("-") Or Parameter5.ToString.Contains("'") Or Parameter6.ToString.Contains("-") Or Parameter6.ToString.Contains("'") Then
        //            Throw New Exception(ErrorSystem(ErrorList.Err1_InvalidInputParameter))
        //        End If

        //        If Not Athentication(UserName, Password) Then
        //            Throw New Exception("دسترسی برای اتصال به سرور پارسیپل مجاز نمی باشد")
        //        End If

        //        ocm = New SqlClient.SqlConnection(ConnectionString)

        //        If Not ocm.State = ConnectionState.Open Then
        //            ocm.Open()
        //        End If

        //        Dim cmd As New System.Data.SqlClient.SqlCommand("", ocm)

        //        Select Case Mode

        //            Case 1

        //                Dim buffer As Byte() = CType(Parameter1, Byte())
        //                Dim VersionReleaseVApplicationID As Int64 = Convert.ToInt64(Parameter2)
        //                Dim check As String = Parameter3.ToString()
        //                Dim Counter As Int64 = Convert.ToInt64(Parameter4)
        //                Dim SplitSize As String = Parameter5.ToString()
        //                Dim query As String = "execute  SP_Insert_VersionInformation @Frk_VersionReleaseVApplication = @_Frk_VersionReleaseVApplication, @Str_CRC = @_Str_CRC, @Int_PartNo = @_Int_PartNo, @Bin_FileContent = @_Bin_FileContent, @Str_PartSize = @_Str_PartSize, @int_state = @_int_state"
        //                cmd.Parameters.AddWithValue("@_Frk_VersionReleaseVApplication", VersionReleaseVApplicationID)
        //                cmd.Parameters.AddWithValue("@_int_state", 2)
        //                cmd.Parameters.AddWithValue("@_Str_CRC", check)
        //                cmd.Parameters.AddWithValue("@_Int_PartNo", Counter)
        //                cmd.Parameters.AddWithValue("@_Bin_FileContent", buffer)
        //                cmd.Parameters.AddWithValue("@_Str_PartSize", SplitSize)

        //                cmd.CommandText = query
        //                cmd.ExecuteNonQuery()
        //                RetValue = True

        //            Case 2

        //                Dim VersionReleaseVAppID As Int64 = Convert.ToInt64(Parameter1)
        //                Dim query As String = "execute  SP_Delete_versionInfo @int_state = 1 , @VersionReleaseVApplication = " & VersionReleaseVAppID & ""

        //                cmd.CommandText = query
        //                cmd.ExecuteNonQuery()
        //                RetValue = True


        //            Case 3

        //                Dim VersionReleaseID As Int64 = Convert.ToInt64(Parameter1)
        //                Dim query As String = "execute  SP_Delete_versionInfo @int_state = 0 , @int_VersionRelease = " & VersionReleaseID & ""

        //                cmd.CommandText = query
        //                cmd.ExecuteNonQuery()
        //                RetValue = True


        //            Case 4

        //                Dim VersionRelease As Int64 = Convert.ToInt64(Parameter1)
        //                Dim ParsicUser As Integer = Convert.ToInt16(Parameter2)
        //                Dim VersionNumber As String = Parameter3.ToString()
        //                Dim Status As String = Parameter4.ToString()
        //                Dim Description As String = Parameter5.ToString()
        //                Dim prerequirement As String = Parameter6.ToString()
        //                Dim query As String = "execute SP_Update_VersionInfo @Prk_VersionRelease_AutoID = " & VersionRelease & " , @Frk_ParsicUser = " & ParsicUser & ", @Str_VersionNumber ='" & VersionNumber & "', @Int_status = " & Status & " , @Str_Description ='" & Description & "', @Str_prerequirement ='" & prerequirement & "', @int_state = 0 "

        //                cmd.CommandText = query
        //                cmd.ExecuteNonQuery()
        //                RetValue = True

        //            Case 5

        //                Dim VersionReleaseVAppID As Int64 = Convert.ToInt64(Parameter1)
        //                Dim query As String = "execute  SP_Delete_versionInfo @int_state = 2 , @VersionReleaseVApplication = " & VersionReleaseVAppID & ""

        //                cmd.CommandText = query
        //                cmd.ExecuteNonQuery()
        //                RetValue = True

        //            Case 6

        //                Dim VersionReleaseID As Int64 = Convert.ToInt64(Parameter1)
        //                Dim ParsicUserID As Int16 = Convert.ToInt16(Parameter2)
        //                Dim Order As Int16 = Convert.ToInt16(Parameter3)
        //                Dim Script As String = Parameter4.ToString()
        //                Dim Description As String = Parameter5.ToString()
        //                Dim query As String = "execute SP_Insert_VersionInformation @Frk_VersionRelease = " & VersionReleaseID & " , @Frk_ParsicUser = " & ParsicUserID & ", @int_Order = " & Order & " , @Bin_Script = N'" & Script & "' , @Str_Description = N'" & Description & "', @int_state = 3"

        //                cmd.CommandText = query
        //                cmd.ExecuteNonQuery()
        //                RetValue = True

        //            Case 7

        //                Dim VersionReleaseID = Convert.ToInt64(Parameter1)
        //                Dim query As String = "execute  SP_Delete_versionInfo @int_state = 3 , @int_VersionRelease = " & VersionReleaseID & ""

        //                cmd.CommandText = query
        //                cmd.ExecuteNonQuery()
        //                RetValue = True


        //        End Select

        //        SafeClose(ocm)
        //        Return RetValue

        //    Catch ex As Exception
        //        SafeClose(ocm)
        //        ExceptionLog(ex)
        //        Return False
        //    End Try

        //End Function

        // Connection To WebService\

        #endregion Function
        //║                                                  ║
        //║                                                  ║
        //╚══════════════════════════════════════════════════╝
    }
}


