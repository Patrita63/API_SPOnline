using Manage.Job.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

// https://stackoverflow.com/questions/42186888/using-sharepoint-csom-inside-net-core-application
//Download the nuget package https://www.nuget.org/packages/Microsoft.SharePointOnline.CSOM/
//Extract it to a subfolder in your project eg; Dependencies
//Reference the correct assemblies

//netcore45\Microsoft.SharePoint.Client.Portable.dll netcore45\Microsoft.SharePoint.Client.Runtime.Portable.dll net45\Microsoft.SharePoint.Client.Runtime.Windows.dll netcore45\Microsoft.SharePoint.Client.Runtime.WindowsStore.dll

//Oh and if you already have that nuget package do an uninstall-package on it.

using Microsoft.SharePoint.Client;
using ClassLibraryLogging;
// using Microsoft.SharePoint.Client.Portable;
// using Microsoft.SharePoint.Client.Runtime.Portable;

namespace Manage.Job.Utilities
{
    public class SPHelper
    {
        private const string PassPhrase = "c8e76e3d-45be-4adc-a0de-a01281f2aa8b";

        public Web web { get; set; }
        public ClientContext ctx { get; set; }

        protected tb_Referente oReferente = null;
        protected tb_JobScheduler oJobScheduler = null;
        protected tb_JobManager oJobManager = null;

        //protected tb_Richiedente oRichiedente = null;
        //protected tb_Segnalazione oSegnalazione = null;
        //protected tbAcc_Area oArea = null;
        //protected tbAcc_Priorita oPriorita = null;
        //// protected tbAcc_TipoSegnalazione oTipoSegnalazione = null;
        //protected tbAcc_Tipologia oTipologia = null;
        //protected tbAcc_Stato oStato = null;

        // https://stackoverflow.com/questions/49803245/sharepoint-csom-authentication-issue-with-net-core
        public async Task<string> getContextAsync(string siteUrl)
        {
            string sMsg = string.Empty;
            ctx = null;
            try
            {
                KeyInfo keyInfo = new KeyInfo(Helper.InfoKey, Helper.InfoIv);
                EncryptionService oMEPwd = new EncryptionService(keyInfo);

                //var str = "***";
                //var enc = oMEPwd.Encrypt(str);
                //var dec = oMEPwd.Decrypt(enc);
                //string ret = ($"str: {str}, enc: {enc}, dec: {dec}");

                ctx = new ClientContext(siteUrl);
                {
                    ctx.AuthenticationMode = ClientAuthenticationMode.Default;

                    //Source: System.Security.Cryptography.Algorithms Message: BlockSize must be 128 in this implementation.
                    //StackTrace:    at System.Security.Cryptography.RijndaelManaged.set_BlockSize(Int32 value)
                    //at Manage.Job.Utilities.StringCipher.Decrypt(String cipherText, String passPhrase)
                    //at Manage.Job.Utilities.SPHelper.getContextAsync(String siteUrl)

                    //SecureString passWord = new SecureString();
                    //foreach (char c in Helper.Pwd.ToCharArray()) passWord.AppendChar(c);

                    //< add key = "ImpersonateUserName" value = "p.tardiolobonifazi@vivasoft.it" />
                    //< add key = "ImpersonatePwd" value = "3KSoHoHFEv7pC5yCe3vN9aCbhpAiRaU+yWhnsLwTK3Lj5fonEsaPmZ1nVtSKrNJSj7ZHho8AaGayrp/VrMmb00r4WiF/tWjPiWQsNLXlzndtzbxYpOW4XTh7hFItLuBd" />

                    //Helper.ImpersonateUserName = "p.tardiolobonifazi@vivasoft.it";
                    //Helper.ImpersonatePwd = "3KSoHoHFEv7pC5yCe3vN9aCbhpAiRaU+yWhnsLwTK3Lj5fonEsaPmZ1nVtSKrNJSj7ZHho8AaGayrp/VrMmb00r4WiF/tWjPiWQsNLXlzndtzbxYpOW4XTh7hFItLuBd";

                    // https://github.com/dotnet/runtime/issues/895
                    // .NET Core does not support AES/Rijndael with a block size other than 128
                    // Helper.Pwd = StringCipher.Decrypt(Helper.ImpersonatePwd, PassPhrase);

                    ctx.Credentials = new SharePointOnlineCredentials(Helper.ImpersonateUserName, oMEPwd.Decrypt(Helper.ImpersonatePwd));

                    web = ctx.Web;
                    ctx.Load(web);

                    // https://stackoverflow.com/questions/42186888/using-sharepoint-csom-inside-net-core-application
                    Task spTask = ctx.ExecuteQueryAsync();
                    await Task.WhenAll(spTask); 
                }
            }
            catch (Exception ex)
            {
                sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal("getContext", sMsg);
            }
            return sMsg;
        }

        public void clearContext()
        {
            if (ctx != null)
            {
                ctx.Dispose();
            }
        }

        public async Task<List<tb_Referente>> getReferenteAsync(string listIdName, string LoginUserName)
        {
            List<tb_Referente> listReferente = null;
            try
            {
                List list = web.Lists.GetById(new Guid(listIdName));
                var q = new CamlQuery() { ViewXml = "<View><Query><Where><Gt><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Gt></Where></Query></View>" };
                var listData = list.GetItems(q);
                ctx.Load(listData);
                await ctx.ExecuteQueryAsync();

                listReferente = new List<tb_Referente>();

                foreach (var item in listData)
                {
                    oReferente = new tb_Referente();
                    oReferente.ID = Convert.ToInt32(item.FieldValues["ID"].ToString());
                    oReferente.IdReferente = Convert.ToInt32(item.FieldValues["IdReferente"].ToString());
                    oReferente.Referente = item.FieldValues["Referente"].ToString();

                    oReferente.Amministratore = Convert.ToBoolean(item.FieldValues["Amministratore"].ToString());
                    oReferente.Smistatore = Convert.ToBoolean(item.FieldValues["Smistatore"].ToString());

                    if (item.FieldValues["Email"] != null)
                    {
                        oReferente.Email = item.FieldValues["Email"].ToString();
                    }

                    listReferente.Add(oReferente);
                }
            }
            catch (Exception ex)
            {
                listReferente = null;
                string sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }
            return listReferente;
        }

        public async Task<List<tb_JobScheduler>> getJobToScheduleAsync(string listIdName, string LoginUserName)
        {
            List<tb_JobScheduler> listJobScheduler = null;
            try
            {
                //using (ClientContext ctx = new ClientContext("https://vivasoft.sharepoint.com"))
                //{
                //    Web web = ctx.Web;
                //    List list = web.Lists.GetById(new Guid("43b32e9b-7595-4ebd-a0ca-d2878c85025a"));
                //    var q = new CamlQuery() { ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='GUID0' /></IsNotNull></Where></Query></View>" };
                //    var r = list.GetItems(q);
                //    ctx.Load(r);
                //    ctx.ExecuteQuery();
                //}

                //using (ClientContext ctx = new ClientContext("https://vivasoft.sharepoint.com"))
                //{
                //    Web web = ctx.Web;
                //    List list = web.Lists.GetById(new Guid("43b32e9b-7595-4ebd-a0ca-d2878c85025a"));
                //    var q = new CamlQuery() { ViewXml = "<View><Query><Where><And><IsNotNull><FieldRef Name='GUID0' /></IsNotNull><Eq><FieldRef Name='Stato_x0020_Processo' /><Value Type='Choice'>Da Schedulare</Value></Eq></And></Where></Query></View>" };
                //    var r = list.GetItems(q);
                //    ctx.Load(r);
                //    ctx.ExecuteQuery();
                //}

                List list = web.Lists.GetById(new Guid(listIdName));
                var q = new CamlQuery() { ViewXml = "<View><Query><Where><And><IsNotNull><FieldRef Name='GUID0' /></IsNotNull><Eq><FieldRef Name='Stato_x0020_Job' /><Value Type='Choice'>Da Pubblicare</Value></Eq></And></Where><OrderBy><FieldRef Name='ID' Ascending='False' /></OrderBy></Query></View>" };
                var listData = list.GetItems(q);
                ctx.Load(listData);
                await ctx.ExecuteQueryAsync();

                listJobScheduler = new List<tb_JobScheduler>();

                foreach (var item in listData)
                {
                    oJobScheduler = new tb_JobScheduler();
                    oJobScheduler.ID = Convert.ToInt32(item.FieldValues["ID"].ToString());

                    if (item.FieldValues["GUID0"] != null)
                        oJobScheduler.GUID = item.FieldValues["GUID0"].ToString();

                    if (item.FieldValues["Nome"] != null)
                        oJobScheduler.Nome = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Nome"].ToString());

                    if (item.FieldValues["Descrizione"] != null)
                        oJobScheduler.Descrizione = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Descrizione"].ToString());

                    if (item.FieldValues["Data_x0020_Inizio_x0020_Job"] != null)
                        oJobScheduler.DataInizioJob = Convert.ToDateTime(item.FieldValues["Data_x0020_Inizio_x0020_Job"].ToString());

                    if (item.FieldValues["Data_x0020_Fine_x0020_Job"] != null)
                        oJobScheduler.DataFineJob = Convert.ToDateTime(item.FieldValues["Data_x0020_Fine_x0020_Job"].ToString());

                    if (item.FieldValues["Tipo_x0020_Job"] != null)
                        oJobScheduler.TipoJob = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Tipo_x0020_Job"].ToString());

                    if (item.FieldValues["Ripetizione"] != null)
                        oJobScheduler.Ripetizione = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Ripetizione"].ToString());

                    if (item.FieldValues["Stato_x0020_Job"] != null)
                        oJobScheduler.StatoJob = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Stato_x0020_Job"].ToString());

                    if (item.FieldValues["Author"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Author"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Autore = childId_Value;
                    }
                    
                    if (item.FieldValues["Created_x0020_Date"] != null)
                        oJobScheduler.DataCreazione = Convert.ToDateTime(item.FieldValues["Created_x0020_Date"].ToString());

                    if (item.FieldValues["Editor"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Editor"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.AutoreUltimaModifica = childId_Value;
                    }

                    if (item.FieldValues["Last_x0020_Modified"] != null)
                        oJobScheduler.DataUltimaModifica = Convert.ToDateTime(item.FieldValues["Last_x0020_Modified"].ToString());

                    if (item.FieldValues["Referente1"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente1"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente1Valore = childId_Value;
                        oJobScheduler.Referente1 = childId_Id;
                    }

                    if (item.FieldValues["Referente2"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente2"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente2Valore = childId_Value;
                        oJobScheduler.Referente2 = childId_Id;
                    }

                    if (item.FieldValues["Referente3"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente3"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente3Valore = childId_Value;
                        oJobScheduler.Referente3 = childId_Id;
                    }

                    if (item.FieldValues["Referente4"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente4"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente4Valore = childId_Value;
                        oJobScheduler.Referente4 = childId_Id;
                    }

                    if (item.FieldValues["Referente5"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente5"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente5Valore = childId_Value;
                        oJobScheduler.Referente5 = childId_Id;
                    }

                    listJobScheduler.Add(oJobScheduler);
                }
            }
            catch (Exception ex)
            {
                listJobScheduler = null;
                string sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }
            return listJobScheduler;
        }

        public async Task<List<tb_JobScheduler>> getJobsByStatoJob(string listIdName, string LoginUserName, string statoJob)
        {
            List<tb_JobScheduler> listJobScheduler = null;
            try
            {
                //using (ClientContext ctx = new ClientContext("https://vivasoft.sharepoint.com"))
                //{
                //    Web web = ctx.Web;
                //    List list = web.Lists.GetById(new Guid("43b32e9b-7595-4ebd-a0ca-d2878c85025a"));
                //    var q = new CamlQuery() { ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='GUID0' /></IsNotNull></Where></Query></View>" };
                //    var r = list.GetItems(q);
                //    ctx.Load(r);
                //    ctx.ExecuteQuery();
                //}

                //using (ClientContext ctx = new ClientContext("https://vivasoft.sharepoint.com"))
                //{
                //    Web web = ctx.Web;
                //    List list = web.Lists.GetById(new Guid("43b32e9b-7595-4ebd-a0ca-d2878c85025a"));
                //    var q = new CamlQuery() { ViewXml = "<View><Query><Where><And><IsNotNull><FieldRef Name='GUID0' /></IsNotNull><Eq><FieldRef Name='Stato_x0020_Processo' /><Value Type='Choice'>Da Schedulare</Value></Eq></And></Where></Query></View>" };
                //    var r = list.GetItems(q);
                //    ctx.Load(r);
                //    ctx.ExecuteQuery();
                //}

                List list = web.Lists.GetById(new Guid(listIdName));
                var q = new CamlQuery() { ViewXml = string.Format("<View><Query><Where><And><IsNotNull><FieldRef Name='GUID0' /></IsNotNull><Eq><FieldRef Name='Stato_x0020_Job' /><Value Type='Choice'>{0}</Value></Eq></And></Where><OrderBy><FieldRef Name='ID' Ascending='False' /></OrderBy></Query></View>", statoJob) };
                var listData = list.GetItems(q);
                ctx.Load(listData);
                await ctx.ExecuteQueryAsync();

                listJobScheduler = new List<tb_JobScheduler>();

                foreach (var item in listData)
                {
                    oJobScheduler = new tb_JobScheduler();
                    oJobScheduler.ID = Convert.ToInt32(item.FieldValues["ID"].ToString());

                    if (item.FieldValues["GUID0"] != null)
                        oJobScheduler.GUID = item.FieldValues["GUID0"].ToString();

                    if (item.FieldValues["Nome"] != null)
                        oJobScheduler.Nome = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Nome"].ToString());

                    if (item.FieldValues["Descrizione"] != null)
                        oJobScheduler.Descrizione = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Descrizione"].ToString());

                    if (item.FieldValues["Data_x0020_Inizio_x0020_Job"] != null)
                        oJobScheduler.DataInizioJob = Convert.ToDateTime(item.FieldValues["Data_x0020_Inizio_x0020_Job"].ToString());

                    if (item.FieldValues["Data_x0020_Fine_x0020_Job"] != null)
                        oJobScheduler.DataFineJob = Convert.ToDateTime(item.FieldValues["Data_x0020_Fine_x0020_Job"].ToString());

                    if (item.FieldValues["Tipo_x0020_Job"] != null)
                        oJobScheduler.TipoJob = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Tipo_x0020_Job"].ToString());

                    if (item.FieldValues["Ripetizione"] != null)
                        oJobScheduler.Ripetizione = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Ripetizione"].ToString());

                    if (item.FieldValues["Stato_x0020_Job"] != null)
                        oJobScheduler.StatoJob = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Stato_x0020_Job"].ToString());

                    if (item.FieldValues["Author"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Author"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Autore = childId_Value;
                    }

                    if (item.FieldValues["Created_x0020_Date"] != null)
                        oJobScheduler.DataCreazione = Convert.ToDateTime(item.FieldValues["Created_x0020_Date"].ToString());

                    if (item.FieldValues["Editor"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Editor"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.AutoreUltimaModifica = childId_Value;
                    }

                    if (item.FieldValues["Last_x0020_Modified"] != null)
                        oJobScheduler.DataUltimaModifica = Convert.ToDateTime(item.FieldValues["Last_x0020_Modified"].ToString());

                    if (item.FieldValues["Referente1"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente1"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente1Valore = childId_Value;
                        oJobScheduler.Referente1 = childId_Id;
                    }

                    if (item.FieldValues["Referente2"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente2"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente2Valore = childId_Value;
                        oJobScheduler.Referente2 = childId_Id;
                    }

                    if (item.FieldValues["Referente3"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente3"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente3Valore = childId_Value;
                        oJobScheduler.Referente3 = childId_Id;
                    }

                    if (item.FieldValues["Referente4"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente4"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente4Valore = childId_Value;
                        oJobScheduler.Referente4 = childId_Id;
                    }

                    if (item.FieldValues["Referente5"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente5"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobScheduler.Referente5Valore = childId_Value;
                        oJobScheduler.Referente5 = childId_Id;
                    }

                    listJobScheduler.Add(oJobScheduler);
                }
            }
            catch (Exception ex)
            {
                listJobScheduler = null;
                string sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }
            return listJobScheduler;
        }

        public async Task<List<tb_JobManager>> getJobManagerAsync(string listIdName, string LoginUserName)
        {
            List<tb_JobManager> listJobManager = null;
            try
            {
                List list = web.Lists.GetById(new Guid(listIdName));
                // var q = new CamlQuery() { ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='GUID0' /></IsNotNull></Where></Query></View>" };
                var q = new CamlQuery() { ViewXml = "<View><Query><Where><And><IsNotNull><FieldRef Name='GUID0' /></IsNotNull><Eq><FieldRef Name='Stato_x0020_Processo' /><Value Type='Choice'>Pubblicato</Value></Eq></And></Where></Query></View>" };
                var listData = list.GetItems(q);
                ctx.Load(listData);
                await ctx.ExecuteQueryAsync();

                listJobManager = new List<tb_JobManager>();

                foreach (var item in listData)
                {
                    oJobManager = new tb_JobManager();
                    oJobManager.ID = Convert.ToInt32(item.FieldValues["ID"].ToString());

                    if (item.FieldValues["GUID0"] != null)
                        oJobManager.GUID = item.FieldValues["GUID0"].ToString();

                    if (item.FieldValues["Nome"] != null)
                        oJobManager.Nome = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Nome"].ToString());

                    if (item.FieldValues["Descrizione"] != null)
                        oJobManager.Descrizione = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Descrizione"].ToString());

                    if (item.FieldValues["Data_x0020_Inizio_x0020_Job"] != null)
                        oJobManager.DataInizioJob = Convert.ToDateTime(item.FieldValues["Data_x0020_Inizio_x0020_Job"].ToString());

                    if (item.FieldValues["Data_x0020_Fine_x0020_Job"] != null)
                        oJobManager.DataFineJob = Convert.ToDateTime(item.FieldValues["Data_x0020_Fine_x0020_Job"].ToString());

                    if (item.FieldValues["Tipo_x0020_Job"] != null)
                        oJobManager.TipoJob = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Tipo_x0020_Job"].ToString());

                    if (item.FieldValues["Ripetizione"] != null)
                        oJobManager.Ripetizione = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Ripetizione"].ToString());

                    if (item.FieldValues["Stato_x0020_Job"] != null)
                        oJobManager.StatoJob = System.Web.HttpUtility.HtmlDecode(item.FieldValues["Stato_x0020_Job"].ToString());

                    if (item.FieldValues["Author"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Author"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobManager.Autore = childId_Value;
                    }

                    if (item.FieldValues["Created_x0020_Date"] != null)
                        oJobManager.DataCreazione = Convert.ToDateTime(item.FieldValues["Created_x0020_Date"].ToString());

                    if (item.FieldValues["Editor"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Editor"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobManager.AutoreUltimaModifica = childId_Value;
                    }

                    if (item.FieldValues["Last_x0020_Modified"] != null)
                        oJobManager.DataUltimaModifica = Convert.ToDateTime(item.FieldValues["Last_x0020_Modified"].ToString());

                    if (item.FieldValues["Referente1"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente1"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobManager.Referente1Valore = childId_Value;
                        oJobManager.Referente1 = childId_Id;
                    }

                    if (item.FieldValues["Referente2"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente2"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobManager.Referente2Valore = childId_Value;
                        oJobManager.Referente2 = childId_Id;
                    }

                    if (item.FieldValues["Referente3"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente3"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobManager.Referente3Valore = childId_Value;
                        oJobManager.Referente3 = childId_Id;
                    }

                    if (item.FieldValues["Referente4"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente4"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobManager.Referente4Valore = childId_Value;
                        oJobManager.Referente4 = childId_Id;
                    }

                    if (item.FieldValues["Referente5"] != null)
                    {
                        //Get Lookup Field - Single Value  
                        var childIdField = item.FieldValues["Referente5"] as FieldLookupValue;
                        string childId_Value = string.Empty;
                        int childId_Id = 0;
                        if (childIdField != null)
                        {
                            childId_Value = childIdField.LookupValue;
                            childId_Id = childIdField.LookupId;
                        }

                        oJobManager.Referente5Valore = childId_Value;
                        oJobManager.Referente5 = childId_Id;
                    }

                    listJobManager.Add(oJobManager);
                }
            }
            catch (Exception ex)
            {
                listJobManager = null;
                string sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }
            return listJobManager;
        }

        public async Task<string> createJobManagerAsync(string listIdName, tb_JobManager oJobManager, string LoginUserName)
        {
            string sMsg = string.Empty;
            string DateFormat = string.Empty;

            try
            {
                DateFormat = "MM/dd/yyyy HH:mm"; // "dd/MM/yyyy HH:mm"; // "yyyy/MM/dd"; // "yyyyMMdHHmmss";

                List list = web.Lists.GetById(new Guid(listIdName));

                // adds the new item to the list with ListItemCreationInformation
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem newListItem = list.AddItem(itemCreateInfo);

                newListItem["GUID0"] = oJobManager.GUID;

                newListItem["Nome"] = oJobManager.Nome;
                newListItem["Descrizione"] = oJobManager.Descrizione;
                newListItem["Data_x0020_Inizio_x0020_Job"] = oJobManager.DataInizioJob.ToString(DateFormat);
                newListItem["Data_x0020_Fine_x0020_Job"] = oJobManager.DataFineJob.ToString(DateFormat);

                newListItem["Tipo_x0020_Job"] = oJobManager.TipoJob;
                newListItem["Ripetizione"] = oJobManager.Ripetizione;
                newListItem["Stato_x0020_Job"] = oJobManager.StatoJob;

                FieldLookupValue spvReferente1 = new FieldLookupValue();
                spvReferente1.LookupId = oJobManager.Referente1;
                newListItem["Referente1"] = spvReferente1;
                newListItem.Update();

                FieldLookupValue spvReferente2 = new FieldLookupValue();
                spvReferente2.LookupId = oJobManager.Referente2;
                newListItem["Referente2"] = spvReferente2;
                newListItem.Update();

                FieldLookupValue spvReferente3 = new FieldLookupValue();
                spvReferente3.LookupId = oJobManager.Referente3;
                newListItem["Referente3"] = spvReferente3;
                newListItem.Update();

                FieldLookupValue spvReferente4 = new FieldLookupValue();
                spvReferente4.LookupId = oJobManager.Referente4;
                newListItem["Referente4"] = spvReferente4;
                newListItem.Update();

                FieldLookupValue spvReferente5 = new FieldLookupValue();
                spvReferente5.LookupId = oJobManager.Referente5;
                newListItem["Referente5"] = spvReferente5;
                newListItem.Update();

                // VEDERE SE FUNZIONA
                newListItem["Author"] = oJobManager.Autore;
                newListItem["Editor"] = oJobManager.AutoreUltimaModifica;

                //if (oJobManager.DataCreazione != null)
                //{
                //    newListItem["Created_x0020_Date"] = oJobManager.DataCreazione.ToString(DateFormat);
                //}

                //if (oJobManager.DataUltimaModifica != null)
                //{
                //    newListItem["Last_x0020_Modified"] = oJobManager.DataUltimaModifica.ToString(DateFormat);
                //}

                newListItem.Update();

                // executes the creation of the new list item on SharePoint
                await ctx.ExecuteQueryAsync();

            }
            catch (Exception ex)
            {
                sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }
            return sMsg;
        }

    }
}
