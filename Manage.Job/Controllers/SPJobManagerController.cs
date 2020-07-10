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
    public class SPJobManagerController : ControllerBase
    {
        protected SPHelper oSP = null;

        // GET: api/<SPJobManager>
        [HttpGet]
        public async Task<IEnumerable<tb_JobManager>> Get()
        {
            List<tb_JobManager> listRetJobManager = null;

            string siteUrl = Helper.SiteUrlSpOnline; // "https://vivasoft.sharepoint.com/";
            string LoginUserName = "Utente";

            // Helper.ListaJobManagerGuid = "todo";
            try
            {
                oSP = new SPHelper();
                string sRet = await oSP.getContextAsync(siteUrl);

                if (string.IsNullOrEmpty(sRet))
                {
                    listRetJobManager = await oSP.getJobManagerAsync(Helper.ListaJobManagerGuid, LoginUserName);

                    if (listRetJobManager == null)
                    {
                        SeriLogging.LogError(LoginUserName, "listRetJobManager == null");
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

            return listRetJobManager;
        }

        //// GET: api/<SPJobManagerController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<SPJobManagerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SPJobManagerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SPJobManagerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SPJobManagerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
