using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryLogging;
using Manage.Job.Models;
using Manage.Job.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Manage.Job.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SPJobSchedulerController : ControllerBase
    {
        protected SPHelper oSP = null;

        // GET: api/<SPJobScheduler>
        [HttpGet]
        public async Task<IEnumerable<tb_JobScheduler>> Get()
        {
            List<tb_JobScheduler> listRetJobScheduler = null;

            string siteUrl = Helper.SiteUrlSpOnline; // "https://vivasoft.sharepoint.com/";
            string LoginUserName = "Utente";

            // Helper.ListaJobSchedulerGuid = "43b32e9b-7595-4ebd-a0ca-d2878c85025a";
            try
            {
                oSP = new SPHelper();
                string sRet = await oSP.getContextAsync(siteUrl);

                if (string.IsNullOrEmpty(sRet))
                {
                    listRetJobScheduler = await oSP.getJobToScheduleAsync(Helper.ListaJobSchedulerGuid, LoginUserName);

                    if (listRetJobScheduler == null)
                    {
                        SeriLogging.LogError(LoginUserName, "listRetJobScheduler == null");
                    }
                    else
                    {
                        SeriLogging.LogInformation(LoginUserName, "ok");
                    }
                }
                else
                {
                    SeriLogging.LogError(LoginUserName, sRet);
                }
            }
            catch (Exception ex)
            {
                string sMsg = string.Empty;
                sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }

            oSP.clearContext();

            return listRetJobScheduler;
        }

        //// GET: api/<SPJobScheduler>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<SPJobScheduler>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SPJobScheduler>
        [HttpPost]
        public async Task<IEnumerable<tb_JobManager>> createManager([FromBody] string value)
        {
            List<tb_JobScheduler> listRetJobScheduler = null;
            List<tb_JobManager> listRetJobManager = null;

            string siteUrl = Helper.SiteUrlSpOnline; // "https://vivasoft.sharepoint.com/";
            string LoginUserName = "Utente";
            string statoJob = string.Empty;
            tb_JobManager oJobManager = null;
            try
            {
                statoJob = value;       // DA PUBBLICARE
                oSP = new SPHelper();
                string sRet = await oSP.getContextAsync(siteUrl);

                if (string.IsNullOrEmpty(sRet))
                {
                    listRetJobScheduler = await oSP.getJobsByStatoJob(Helper.ListaJobSchedulerGuid, LoginUserName, statoJob);

                    if (listRetJobScheduler == null)
                    {
                        SeriLogging.LogError(LoginUserName, "listRetJobScheduler == null");
                    }
                    else
                    {
                        SeriLogging.LogInformation(LoginUserName, "listRetJobScheduler ok");

                        string ret = string.Empty;
                        foreach(var item in listRetJobScheduler)
                        {
                            switch(item.Ripetizione)
                            {
                                case "Singola":
                                    oJobManager = new tb_JobManager();
                                    oJobManager.Nome = item.Nome;
                                    oJobManager.Descrizione = item.Descrizione;
                                    oJobManager.DataInizioJob = item.DataInizioJob;
                                    oJobManager.DataFineJob = item.DataFineJob;
                                    oJobManager.TipoJob = item.TipoJob;
                                    oJobManager.Ripetizione = item.Ripetizione;
                                    oJobManager.StatoJob = "Pubblicato";
                                    oJobManager.Referente1 = item.Referente1;
                                    oJobManager.Referente1Valore = item.Referente1Valore;
                                    oJobManager.Referente2 = item.Referente2;
                                    oJobManager.Referente2Valore = item.Referente2Valore;
                                    oJobManager.Referente3 = item.Referente3;
                                    oJobManager.Referente3Valore = item.Referente3Valore;
                                    oJobManager.Referente4 = item.Referente4;
                                    oJobManager.Referente4Valore = item.Referente4Valore;
                                    oJobManager.Referente5 = item.Referente5;
                                    oJobManager.Referente5Valore = item.Referente5Valore;

                                    oJobManager.Autore = item.Autore;
                                    oJobManager.DataCreazione = item.DataCreazione;
                                    oJobManager.AutoreUltimaModifica = item.AutoreUltimaModifica;
                                    oJobManager.DataUltimaModifica = item.DataUltimaModifica;

                                    ret = await oSP.createJobManagerAsync(Helper.ListaJobManagerGuid, oJobManager, LoginUserName);
                                    break;
                                case "Giornaliera":

                                    break;
                                case "Settimanale":

                                    break;
                                case "Mensile":

                                    break;
                                case "Annuale":

                                    break;
                            }
                        }
                    }

                    listRetJobManager = await oSP.getJobManagerAsync(Helper.ListaJobManagerGuid, LoginUserName);
                    if (listRetJobManager == null)
                    {
                        SeriLogging.LogError(LoginUserName, "listRetJobManager == null");
                    }
                    else
                    {
                        SeriLogging.LogInformation(LoginUserName, "listRetJobManager ok");
                    }
                }
                else
                {
                    SeriLogging.LogError(LoginUserName, sRet);
                }
            }
            catch (Exception ex)
            {
                string sMsg = string.Empty;
                sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }

            oSP.clearContext();

            return listRetJobManager;
        }


        // PUT api/<SPJobScheduler>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SPJobScheduler>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
