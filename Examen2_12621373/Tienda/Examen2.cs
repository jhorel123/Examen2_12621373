using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Shared;
using System.Net;
using Tienda;

namespace Ventas
{
    public class Examen2
    {
        private readonly ILogger<Examen2> _logger;
        private readonly Contexto _contexto;

        public Examen2(ILogger<Examen2> logger, Contexto contexto)
        {
            _contexto = contexto;
            _logger = logger;
        }

        [Function("RegistrarPedido")]
        [OpenApiOperation("RegistrarPedido", "Registrar Pedido", Description = "Registra un nuevo pedido con sus detalles")]
        [OpenApiRequestBody("application/json", typeof(Pedido), Description = "Datos del nuevo pedido con sus detalles")]
        public async Task<HttpResponseData> RegistrarPedido([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registrarPedido")] HttpRequestData req)
        {
            _logger.LogInformation("Registrando un nuevo pedido.");
            var pedido = await req.ReadFromJsonAsync<Pedido>() ?? throw new Exception("Debe ingresar un Pedido con todos sus datos");

            _contexto.Pedidos.Add(pedido);
            await _contexto.SaveChangesAsync();

            var respuesta = req.CreateResponse(HttpStatusCode.OK);
            return respuesta;
        }

        [Function("ListarReportePedidos")]
        [OpenApiOperation("ListarReportePedidos", "Listar Reporte de Pedidos por Cliente")]
        public async Task<IActionResult> ListarReportePedidosPorCliente(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarReportePedidos")] HttpRequestData req)
        {
            _logger.LogInformation("Listando reporte de pedidos por cliente.");
            var reporte = await _contexto.Detalles
                .Select(x => new ReportePedidoCliente
                {
                    NombreCliente = x.Pedido.Cliente.Nombre,
                    FechaPedido = x.Pedido.Fecha,
                    NombrePedido = x.Producto.Nombre
                })
                .ToListAsync();
            return new OkObjectResult(reporte);
        }

        [Function("ListarTop3Productos")]
        [OpenApiOperation("ListarTop3Productos", "Listar los 3 Productos Más Pedidos")]
        public async Task<IActionResult> ListarTop3ProductosMasPedidos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarTop3Productos")] HttpRequestData req)
        {
            _logger.LogInformation("Listando los 3 productos más pedidos.");

            var productosMasPedidos = await _contexto.Detalles
                .GroupBy(d => d.idProducto)
                .Select(g => new ProductoMasPedidosDto
                {
                    NombreProducto = g.FirstOrDefault().Producto.Nombre,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(3)
                .ToListAsync();
            return new OkObjectResult(productosMasPedidos);
        }

        [Function("EliminarClienteCascada")]
        [OpenApiOperation("EliminarClienteCascada", "Eliminar Cliente en Cascada")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del cliente", Description = "El ID del cliente a eliminar en cascada", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> EliminarClienteCascada(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminarClienteCascada/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Eliminando cliente en cascada");
            var cliente = await _contexto.Clientes.FindAsync(id);
            _contexto.Clientes.Remove(cliente);
            await _contexto.SaveChangesAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync("Cliente eliminado");
            return response;
        }

        [Function("ListarTopProductos")]
        [OpenApiOperation("ListarTopProductos", "Listar los Productos Más Pedidos según un Rango de Fechas")]
        [OpenApiParameter(name: "fechaInicio", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Summary = "Fecha de inicio", Description = "Fecha de inicio del rango de fechas")]
        [OpenApiParameter(name: "fechaFin", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Summary = "Fecha de fin", Description = "Fecha de fin del rango de fechas")]
        public async Task<IActionResult> ListarTopProductosMasPedidos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarTopProductos")] HttpRequestData req,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            _logger.LogInformation("Listando los productos más pedidos.");

            var topProductos = await _contexto.Detalles
                .Where(d => d.Pedido.Fecha >= fechaInicio && d.Pedido.Fecha <= fechaFin)
                .GroupBy(d => d.idProducto)
                .Select(g => new ProductoMasPedidosDto
                {
                    NombreProducto = g.FirstOrDefault().Producto.Nombre,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToListAsync();

            return new OkObjectResult(topProductos);
        }
    }

    public class ReportePedidoCliente
    {
        public string NombreCliente { get; set; }
        public DateTime FechaPedido { get; set; }
        public string NombrePedido { get; set; }
    }

    public class ProductoMasPedidosDto
    {
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class dtoProductomasVendido1
    {
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
