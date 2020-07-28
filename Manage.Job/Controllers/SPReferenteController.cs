using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryLogging;
using Manage.Job.Models;
using Manage.Job.Utilities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Manage.Job.Controllers
{
    // TO AVOID CORS:
    // Access to XMLHttpRequest at 'https://localhost:44308/api/movies' from origin 'http://localhost:44301' has been blocked by CORS policy: 
    // Response to preflight request doesn't pass access control check: No 'Access - Control - Allow - Origin' header is present on the requested resource.

    // Se lo metto a livello dell'intero Controller
    // [EnableCors("AllowAllHeaders")]    VA IN ERRORE

    [Route("api/[controller]")]
    [ApiController]
    public class SPReferenteController : ControllerBase
    {
        protected SPHelper oSP = null;

        // https://github.com/dotnet/aspnetcore/issues/17830
        // InvalidOperationException: Endpoint Manage.Job.Controllers.SPReferenteController.Get (Manage.Job) contains CORS metadata, 
        // but a middleware was not found that supports CORS.
        // Configure your application startup by adding app.UseCors() inside the call to Configure(..) in the application startup code.
        // The call to app.UseAuthorization() must appear between app.UseRouting() and app.UseEndpoints(...).
        // Microsoft.AspNetCore.Routing.EndpointMiddleware.ThrowMissingCorsMiddlewareException(Endpoint endpoint)

        // TOLTO
        // [EnableCors("AllowAllHeaders")]

        // GET: api/SPReferente
        [HttpGet]
        public async Task<IEnumerable<tb_Referente>> Get()
        {
            List<tb_Referente> listRetReferente = null;

            string siteUrl = Helper.SiteUrlSpOnline; // "https://vivasoft.sharepoint.com/";
            string LoginUserName = "Utente";    // TODO 

            try
            {
                oSP = new SPHelper();
                string sRet = await oSP.getContextAsync(siteUrl);

                if (string.IsNullOrEmpty(sRet))
                {
                    listRetReferente = await oSP.getReferenteAsync(Helper.ListaReferenteGuid, LoginUserName);

                    if (listRetReferente == null)
                    {
                        SeriLogging.LogError(LoginUserName, "listRetReferente == null");
                    }
                    else
                    {
                        SeriLogging.LogInformation(LoginUserName, "ok");
                        // return (IEnumerable<tb_Referente>)listRetReferente;
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
            // return (IEnumerable<tb_Referente>)listRetReferente;

            return listRetReferente;
        }

        //// GET: api/<SPReferenteController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<SPReferenteController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SPReferenteController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SPReferenteController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SPReferenteController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
