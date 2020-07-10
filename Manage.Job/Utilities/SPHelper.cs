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
        public Web web { get; set; }
        public ClientContext ctx { get; set; }

        protected tb_Referente oReferente = null;

        //protected tb_Richiedente oRichiedente = null;
        //protected tb_Segnalazione oSegnalazione = null;
        //protected tbAcc_Area oArea = null;
        //protected tbAcc_Priorita oPriorita = null;
        //// protected tbAcc_TipoSegnalazione oTipoSegnalazione = null;
        //protected tbAcc_Tipologia oTipologia = null;
        //protected tbAcc_Stato oStato = null;

        // https://stackoverflow.com/questions/49803245/sharepoint-csom-authentication-issue-with-net-core
        public async Task<string> getContext(string siteUrl)
        {
            string sMsg = string.Empty;
            ctx = null;
            try
            {
                ctx = new ClientContext(siteUrl);
                {
                    //< add key = "ImpersonateUserName" value = "p.tardiolobonifazi@vivasoft.it" />
                    //< add key = "ImpersonatePwd" value = "3KSoHoHFEv7pC5yCe3vN9aCbhpAiRaU+yWhnsLwTK3Lj5fonEsaPmZ1nVtSKrNJSj7ZHho8AaGayrp/VrMmb00r4WiF/tWjPiWQsNLXlzndtzbxYpOW4XTh7hFItLuBd" />

                    Helper.ImpersonateUserName = "p.tardiolobonifazi@vivasoft.it";
                    Helper.Pwd = "3KSoHoHFEv7pC5yCe3vN9aCbhpAiRaU+yWhnsLwTK3Lj5fonEsaPmZ1nVtSKrNJSj7ZHho8AaGayrp/VrMmb00r4WiF/tWjPiWQsNLXlzndtzbxYpOW4XTh7hFItLuBd";
                    ctx.AuthenticationMode = ClientAuthenticationMode.Default;
                    //SecureString passWord = new SecureString();
                    //foreach (char c in Helper.Pwd.ToCharArray()) passWord.AppendChar(c);
                    ctx.Credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(Helper.ImpersonateUserName, Helper.Pwd);

                    web = ctx.Web;
                    ctx.Load(web);
                    await ctx.ExecuteQueryAsync();
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


        public async Task<List<tb_Referente>> getReferente(string listIdName, string LoginUserName)
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

    }
}
