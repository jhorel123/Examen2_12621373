using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Api.Tienda.Endpoints
{
    internal class ClienteFunction
    {
        [Function("InsertarPersona")]
        [ColingAuthorize(AplicacionRoles.Admin)]
        [OpenApiOperation("InsertarPersona", "InsertarPersona", Description = "Inserta una nueva persona.")]
        [OpenApiRequestBody("application/json", typeof(Cliente), Description = "Datos de la persona a insertar")]
        public async Task<HttpResponseData> InsertarPersona([HttpTrigger(AuthorizationLevel.Function, "post", Route = "insertarpersona")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar persona.");
            try
            {
                var per = await req.ReadFromJsonAsync<Cliente>() ?? throw new Exception("Debe ingresar una persona con todos sus datos");
                bool seGuardo = await ClienteLogic.InsertarPersona(per);
                if (seGuardo)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }

        }
    }
}
