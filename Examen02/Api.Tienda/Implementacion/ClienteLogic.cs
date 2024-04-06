using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Tienda.Implementacion
{
    public class ClienteLogic
    {

        public async Task<bool> InsertarPersona(ClienteLogic cliente)
        {
            try
            {
                contexto.Cliente.Add(cliente);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
