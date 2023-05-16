using System;
using Microsoft.Win32;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ParsicAutoUpdater.Classes
{
    public class clsRegistry
    {
        public string ReadFromRegistry(int intBaseKey, string RegistryPath, string KeyName)
        {
            RegistryKey RegKey = null;
            string val = string.Empty;

            switch (intBaseKey)
            {
                case 1:
                    RegKey = Registry.ClassesRoot;
                    break;
                case 2:
                    RegKey = Registry.CurrentConfig;
                    break;
                case 3:
                    RegKey = Registry.CurrentUser;
                    break;
                case 4:
                    RegKey = Registry.DynData;
                    break;
                case 5:
                    RegKey = Registry.LocalMachine;
                    break;
                case 6:
                    RegKey = Registry.PerformanceData;
                    break;
                case 7:
                    RegKey = Registry.Users;
                    break;
                default:
                    RegKey = Registry.CurrentUser;
                    break;
            }
            try
            {
                RegKey = RegKey.OpenSubKey(RegistryPath);
                object TMP = RegKey.GetValue(KeyName);
                if (TMP != null)
                {
                    val = TMP.ToString();
                }
            }
            catch
            {
                val = string.Empty;
            }
            return val.ToString();
        }

        public bool SetRegistryKeyValue(int intBaseKey, string SubKey, string KeyName, Object KeyValue)
        {

            RegistryKey RegKey = null;
            bool boolResult;

            switch (intBaseKey)
            {
                case 1:
                    RegKey = Registry.ClassesRoot;
                    break;
                case 2:
                    RegKey = Registry.CurrentConfig;
                    break;
                case 3:
                    RegKey = Registry.CurrentUser;
                    break;
                case 4:
                    RegKey = Registry.DynData;
                    break;
                case 5:
                    RegKey = Registry.LocalMachine;
                    break;
                case 6:
                    RegKey = Registry.PerformanceData;
                    break;
                case 7:
                    RegKey = Registry.Users;
                    break;
                default:
                    RegKey = Registry.CurrentUser;
                    break;
            }

            try
            {
                RegistryKey tmpRegKey = RegKey.OpenSubKey(SubKey, true);
                if (tmpRegKey != null)
                    RegKey = tmpRegKey;
                else
                {
                    RegKey = RegKey.CreateSubKey(SubKey);
                }
                RegKey.SetValue(KeyName, KeyValue);
                boolResult = true;
            }
            catch
            {
                boolResult = false;
            }

            return boolResult;
        }
    }
}
