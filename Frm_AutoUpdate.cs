using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
    public partial class Frm_AutoUpdate : Form
    {


        DataTable Dt = new DataTable();
        string CRC = "";
        string Size = "";
        Int32 Count = 0;
        string AppVer = "";
        string DbVer = "";
        string ZipPath = "";
        Boolean CancelBit = false;
        Boolean PauseBit = false;


        Thread MyThread;
        GetAndInsertVersionInDB UpdaterFunctions = new GetAndInsertVersionInDB(0);
        Int32 VersionReleaseVApplicationID;

        public Frm_AutoUpdate()
        {
            InitializeComponent();
        }
        public Frm_AutoUpdate(DataTable _Dt, string _AppVer, string _DbVer)
        {
            InitializeComponent();
            Dt = _Dt;
            AppVer = _AppVer;
            DbVer = _DbVer;
        }

        private void Frm_AutoUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                MyThread = new Thread(() => Download());
                MyThread.IsBackground = true;
                MyThread.Start();
            }
            catch (Exception EX)
            {
                UpdaterFunctions.SaveTextExeption("Error Download Thread : " + EX.Message.ToString());
                MessageBox.Show("Error In Download Files Thread : " + EX.Message.ToString());
            }

        }


        public void Download()
        {
            long SplitSize = 1048576;
            byte[] buffer = new byte[SplitSize + 1];
            UpdaterFunctions = new GetAndInsertVersionInDB(0);
            string SavePath = FindExeDirectPath().Replace("/", @"\");
            string Name = Dt.Rows[0]["Str_FileName"].ToString().Replace("/", "_");
            VersionReleaseVApplicationID = Convert.ToInt32(Dt.Rows[0]["Prk_VersionReleaseVApplication_AutoID"]);
            CRC = Dt.Rows[0]["Str_CRC"].ToString();
            Size = Dt.Rows[0]["Str_FileSize"].ToString();
            Count = Convert.ToInt32(Dt.Rows[0]["Count"]);


            SetTxt_LblSize((Convert.ToInt64(Size) / 1024 / 1024).ToString() + " MB");
            SetTxt_LblCount(Count.ToString());
            SetTxt_LblAppVer(AppVer);
            SetTxt_LblDbVer(DbVer);
            Set_PbarMaximom(Count - 1);
            ZipPath = SavePath + Name;
            int i = 0;
            short j = 0;
            var dt1 = new DataTable();
            SetTxt_SaveExeLogEvent("Start Downloading ... ");
            SetTxt_SaveExeLogEvent("Path : " + SavePath + Name);
            var loopTo = Count - 1;
            for (i = 0; i <= loopTo; i++)
            {
                for (j = 0; j <= 10; j++)
                {
                    try
                    {
                        dt1 = UpdaterFunctions.GetVersionInfoFileFromDataBase(VersionReleaseVApplicationID, i);
                        buffer = (byte[])dt1.Rows[0]["Bin_FileContent"]; 

                        SetTxt_SaveExeLogEvent((i + 1).ToString() + "    OF    " + Count.ToString());
                        break;
                    }
                    catch (Exception ex)
                    {
                        SetTxt_SaveExeLogEvent("Error In Downloud File Number " + (i + 1).ToString() + " Try Number " + (j + 1).ToString());
                        Thread.Sleep(5000);
                    }

                    if (j == 9)
                    {
                        SetTxt_SaveExeLogEvent("Error In Download File,Try 10 Time, Please Try Agan Later");
                        break;
                    }
                }

                if (j != 9)
                {
                    //FileStream oFileStream;
                    if (i == 0)
                    {
                        //oFileStream = new FileStream(SavePath + Name, FileMode.Create);
                        //oFileStream.Close();
                        ByteArrayToFile(SavePath + Name, buffer, FileMode.Create);
                    }
                    else
                    {
                        //oFileStream = new FileStream(SavePath + Name, FileMode.Append);
                        //oFileStream.Close();
                        ByteArrayToFile(SavePath + Name, buffer, FileMode.Append);

                    }

                    //oFileStream.Write(buffer, 0, buffer.Length);
                    //oFileStream.Close();
                    Set_PbarValue(i);
                }
                else
                {
                    DeleteFile(SavePath + Name);
                    break;
                }

                if (CancelBit)
                {
                    DeleteFile(SavePath + Name);
                    break;
                }

                while (PauseBit)
                    Thread.Sleep(1000);
            }

            if (j != 9)
            {
                if (CancelBit)
                {
                    SetTxt_SaveExeLogEvent("Download Canceled");
                    Application.Exit();
                    //return false;
                }
                else
                {
                    byte[] WholeByte = new byte[Convert.ToInt64(Size + 1)];
                    byte[] buffer2 = new byte[SplitSize + 1];
                    WholeByte = UpdaterFunctions.StreamFile(SavePath + Name);
                    string check = UpdaterFunctions.CalculateChecksum(WholeByte);
                    if (check == CRC)
                    {
                        SetTxt_SaveExeLogEvent("Download Finish");
                        // SetTxt_SaveExeLogEvent("Check Sun is Correct")

                        if (MessageBox.Show(" فایل ها با موفقیت دانلود شد آیا میخواهید برنامه را بروز کنید؟", "بروز رسانی", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            var pHelp = new ProcessStartInfo();
                            pHelp.FileName = FindExePath() + "AutoUpdaterCLU.exe";
                            pHelp.Arguments = AppVer + " " + DbVer + " " + ZipPath.Replace(" ", "|");
                            pHelp.UseShellExecute = true;
                            pHelp.WindowStyle = ProcessWindowStyle.Hidden;
                            pHelp.WindowStyle = ProcessWindowStyle.Normal;
                            var proc = Process.Start(pHelp);
                        }

                        //return true;
                    }
                    else
                    {
                        SetTxt_SaveExeLogEvent("Check Sum is Wrong, The File Have Been Deleted");
                        DeleteFile(SavePath + Name);
                        MessageBox.Show("اطلاعات دریافتی با اطلاعات اصلی هماهنگ نمیباشد، لطفا دوباره تلاش فرمایید");
                    }
                }
            }
            else
            {
                SetTxt_SaveExeLogEvent("Error in receive file");
                MessageBox.Show("خطا در دریافت اطلاعات", "خطا");
            }

            //return false;
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray, FileMode Mode)
        {
            try
            {
                using (var fs = new FileStream(fileName, Mode, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        public delegate void SetStatusCallback(string message);
        public void SetTxt_LblSize(string message)
        {
            if (this.Lbl_Size.InvokeRequired)
            {

                this.Invoke(new SetStatusCallback(SetTxt_LblSize),
                     message);
            }
            else
                this.Lbl_Size.Text = message;
        }

        public delegate void SetStatusCallbackCount(string message);
        public void SetTxt_LblCount(string message)
        {
            if (this.Lbl_Count.InvokeRequired)
            {

                this.Invoke(new SetStatusCallbackCount(SetTxt_LblCount),
                     message);
            }
            else
                this.Lbl_Count.Text = message;
        }

        public delegate void SetStatusCallbackAppVer(string message);
        public void SetTxt_LblAppVer(string message)
        {
            if (this.Lbl_AppVersion.InvokeRequired)
            {

                this.Invoke(new SetStatusCallbackAppVer(SetTxt_LblAppVer),
                     message);
            }
            else
                this.Lbl_AppVersion.Text = message;
        }

        public delegate void SetStatusCallbackDbVer(string message);
        public void SetTxt_LblDbVer(string message)
        {
            if (this.Lbl_NewVersion.InvokeRequired)
            {

                this.Invoke(new SetStatusCallbackDbVer(SetTxt_LblDbVer),
                     message);
            }
            else
                this.Lbl_NewVersion.Text = message;
        }

        public delegate void SetStatusCallbackDbVerSaveExeLogEvent(string message);
        public void SetTxt_SaveExeLogEvent(string message)
        {
            if (this.Txt_ScriptError.InvokeRequired)
            {

                this.Invoke(new SetStatusCallbackDbVerSaveExeLogEvent(SetTxt_SaveExeLogEvent),
                     message);
            }
            else
                this.Txt_ScriptError.Text = this.Txt_ScriptError.Text + "\r\n\r\n" + message;
        }

        public delegate void SetStatusCallbackPbarMaximom(Int32 Num);
        public void Set_PbarMaximom(Int32 Num)
        {
            if (this.Lbl_Size.InvokeRequired)
            {

                this.Invoke(new SetStatusCallbackPbarMaximom(Set_PbarMaximom),
                     Num);
            }
            else
                this.Pbar_Download.Maximum = Num;
        }

        public delegate void SetStatusCallbackPbarValue(Int32 Num);
        public void Set_PbarValue(Int32 Num)
        {
            if (this.Lbl_Size.InvokeRequired)
            {

                this.Invoke(new SetStatusCallbackPbarValue(Set_PbarValue),
                     Num);
            }
            else
                this.Pbar_Download.Value = Num;
        }




        public string FindExeDirectPath()
        {
            string exePath = Application.ExecutablePath;
            int k = 0;
            int M = 0;
            for (int j = 0, loopTo = exePath.Length - 1; j <= loopTo; j++)
            {
                if (Convert.ToString(exePath[j]) == @"\")
                {
                    k = j;
                }
            }

            for (int j = 0, loopTo1 = k - 1; j <= loopTo1; j++)
            {
                if (Convert.ToString(exePath[j]) == @"\")
                {
                    M = j;
                }
            }

            return exePath.Substring(0, M + 1);
        }

        public string FindExePath()
        {
            string exePath = Application.ExecutablePath;
            int k = 0;
            for (int j = 0, loopTo = exePath.Length - 1; j <= loopTo; j++)
            {
                if (Convert.ToString(exePath[j]) == @"\")
                {
                    k = j;
                }
            }

            return exePath.Substring(0, k + 1);
        }


        public bool DeleteFile(string Address)
        {
            try
            {
                //file
                //Computer.FileSystem.DeleteFile(Address);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            CancelBit = true;
        }

        private void Btn_Pause_Click(object sender, EventArgs e)
        {
            if (Btn_Pause.Text == "Pause")
            {
                PauseBit = true;
                Btn_Pause.Text = "Resume";
            }
            else
            {
                PauseBit = false;
                Btn_Pause.Text = "Pause";
            }
        }

        private void Btn_MoreInfo_Click(object sender, EventArgs e)
        {
            if (Btn_MoreInfo.Text == "More Info")
            {
                this.Height = 400;
                Btn_MoreInfo.Text = "Less Info";
            }
            else
            {
                this.Height = 135;
                Btn_MoreInfo.Text = "More Info";
            }
        }
    }
}
