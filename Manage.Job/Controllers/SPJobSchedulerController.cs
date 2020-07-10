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

            string siteUrl = "https://vivasoft.sharepoint.com/";
            string LoginUserName = "Utente";
            Helper.ListaReferenteGuid = "43b32e9b-7595-4ebd-a0ca-d2878c85025a";
            try
            {
                oSP = new SPHelper();
                string sRet = await oSP.getContextAsync(siteUrl);

                if (string.IsNullOrEmpty(sRet))
                {
                    listRetJobScheduler = await oSP.getJobToScheduleAsync(Helper.ListaReferenteGuid, LoginUserName);

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
        public void Post([FromBody] string value)
        {
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
