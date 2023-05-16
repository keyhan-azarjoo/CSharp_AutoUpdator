
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdaterClasses;

namespace ParsicAutoUpdater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                string Confermer = " ";
                string LastVer = "";
                //string Info = "UserId#27!@#VerID#705!@#VerNum#11.04!@#Server#192.168.1.20\\SQL2012SP2!@#DbName#Db_ParsicMaster!@#TblName#TBL_DBList!@#User#sa!@#Pass#who!@#Exe#1!@#Scripts#1!@#Central#0!@#QC#0!@#Storage#0!@#Jurnul#0!@#Temperature#0!@#ParsicLabAndroid#1!@#BackUp#1";
                string Info = args[1].Replace("**", " ");
                Int16 ParsicUserID = Convert.ToInt16( (Info.Split('~'))[0]);
                LastVer = (Info.Split('~'))[1].ToString();
                Confermer = (Info.Split('~'))[2].ToString();
                //MessageBox.Show(args[0] + "\r\n" + args[1] + "\r\n" + args[2]);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Frm_MackeUpdateInfo(ParsicUserID, true, true, Confermer, LastVer));

            }
            catch (Exception EX)
            {
                try
                {
                    try
                    {
                        try
                        {

                            string[] drives = System.IO.Directory.GetLogicalDrives();
                            int WebCount = 0;
                            int DriveCount = 0;
                            foreach (string str in drives)
                            {
                                DriveCount += 1;
                                if (System.IO.Directory.Exists(str + "web\\"))
                                {
                                    WebCount += 1;
                                }
                            }


                            if (DriveCount > 1)
                            {
                                if (WebCount >= 2)
                                {
                                    if (Directory.Exists("C:\\web"))
                                    {
                                        Directory.Delete("C:\\web", true);
                                    }
                                }
                            }
                        } catch (Exception ex )
                        {

                        }
                        string[] args = Environment.GetCommandLineArgs();
                        string Info = args[1];
                    }
                    catch
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        Application.Run(new Frm_Login());
                    }
                }
                catch
                {
                    GetAndInsertVersionInDB UpdaterFunctions = new GetAndInsertVersionInDB(Convert.ToInt32(0));
                    UpdaterFunctions.SaveTextExeption("Error In Run(First Error) : " + EX.Message.ToString());
                }
            }
            
        }
    }
}
