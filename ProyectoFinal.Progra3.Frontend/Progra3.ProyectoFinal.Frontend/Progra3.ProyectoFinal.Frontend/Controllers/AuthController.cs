using Microsoft.AspNetCore.Mvc;
using Progra3.ProyectoFinal.Frontend.Models.Usuarios;
using System.Text.Json;
using System.Text;

namespace Progra3.ProyectoFinal.Frontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUsuario modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var cliente = this.httpClientFactory.CreateClient("XandriaAPI");

            var json = JsonSerializer.Serialize(modelo);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync("auth/login", contenido);

            if (respuesta.IsSuccessStatusCode)
            {
                //aqui pondria el jwt, si tuviera uno

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas. Por favor intente de nuevo.");
                return View(modelo);
            }
        }
    }
}
