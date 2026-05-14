namespace Progra3.ProyectoFinal.Frontend.Controllers
{
    using System.Text;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Progra3.ProyectoFinal.Frontend.Models.Usuarios;

    public class UsuariosController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UsuariosController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearUsuario modelo)
        {
            modelo.IdRol = 1;

            ModelState.Remove("IdRol");

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var cliente = this.httpClientFactory.CreateClient("XandriaAPI");

            var json = JsonSerializer.Serialize(modelo);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync("usuarios/crear", contenido);

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Usuario Registrado";
                return RedirectToAction("Crear"); 
            }
            else
            {
                var errorResponse = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error del servidor: {errorResponse}");
                return View(modelo);
            }
        }
    }
}