﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage.Job.Utilities
{
    public static class Helper
    {
        public static string SiteUrlSpOnline = string.Empty;
        public static string ImpersonateUserName = string.Empty;
        public static string ImpersonatePwd = string.Empty;
        public static string Pwd = string.Empty;

        public static string ListaReferenteGuid = string.Empty;
        public static string ListaJobSchedulerGuid = string.Empty;
        public static string ListaJobManagerGuid = string.Empty;

        public static string ListaSegnalazioneGuid = string.Empty;
        // public static string DocLibraryAllegatoSegnalazioneGuid = string.Empty;

        public static string MailTest = string.Empty;
        public static string MailPort = "25"; // You can use Port 25 if 587 is blocked
        public static string MailHost = string.Empty;
        public static string MailFrom = string.Empty;

        public static string InfoKey = string.Empty;
        public static string InfoIv = string.Empty;
    }

    //public class JSonAsClassSettings
    //{
    //    public string ImpersonateUserName { get; private set; }

    //    public void getConfigurationSettings(string PassPhrase)
    //    {
    //        ImpersonateUserName = ""; // WebConfigurationManager.AppSettings.Get("PlanID");

    //    }
    //}
}
