using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Tienda.contratos
{
    public interface IClienteLogic
    {
        public Task<bool> InsertarPersona(Cliente cliente);
       /* public Task<bool> ModificarPersona(Persona persona, int id);
        public Task<bool> EliminarPersona(int id);
        public Task<List<Persona>> ListarPersonaTodos();
        public Task<Persona> ObtenerPersonaById(int id);*/
    }
}
