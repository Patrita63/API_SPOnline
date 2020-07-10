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
    public class SPReferenteController : ControllerBase
    {
        protected SPHelper oSP = null;

        // GET: api/SPHelper
        [HttpGet]
        public ActionResult<IEnumerable<tb_Referente>> Get()
        {
            List<tb_Referente> listReferente = null;
            string siteUrl = "https://vivasoft.sharepoint.com/";
            string LoginUserName = "Utente";
            Helper.ListaReferenteGuid = "dfc89341-833b-4396-b03b-b8259ea980f7";
            try
            {
                listReferente = new List<tb_Referente>();
                oSP = new SPHelper();
                oSP.getContext(siteUrl);
                listReferente = oSP.getReferente(Helper.ListaReferenteGuid, LoginUserName);

                SeriLogging.LogInformation(LoginUserName, "ok");
            }
            catch (Exception ex)
            {
                string sMsg = string.Empty;
                sMsg = string.Format("Source: {0}{3} Message: {1}{3} StackTrace: {2}{3}", ex.Source, ex.Message, ex.StackTrace, System.Environment.NewLine);
                SeriLogging.LogFatal(LoginUserName, sMsg);
            }

            oSP.clearContext();
            return listReferente;
        }

        //// GET: api/<SPManagerController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<SPManagerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SPManagerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SPManagerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SPManagerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
