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

        private int? GetUsuarioIdDesdeToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            try
            {
                var partes = token.Split('.');
                if (partes.Length < 2) return null;
                var payloadB64 = partes[1];
                
                int mod = payloadB64.Length % 4;
                if (mod > 0)
                {
                    payloadB64 += new string('=', 4 - mod);
                }
                
                var payloadBytes = Convert.FromBase64String(payloadB64);
                var json = System.Text.Encoding.UTF8.GetString(payloadBytes);
                var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("IdUsuario", out var idProp))
                {
                    if (idProp.ValueKind == System.Text.Json.JsonValueKind.String && int.TryParse(idProp.GetString(), out var id))
                    {
                        return id;
                    }
                    else if (idProp.ValueKind == System.Text.Json.JsonValueKind.Number && idProp.TryGetInt32(out var idNum))
                    {
                        return idNum;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> DetallePerfil()
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var idUsuario = GetUsuarioIdDesdeToken(token);
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cliente = this.httpClientFactory.CreateClient("XandriaAPI");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await cliente.GetAsync($"Usuarios/obtenerPorId/{idUsuario}");

            if (respuesta.IsSuccessStatusCode)
            {
                var responseContent = await respuesta.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var usuario = JsonSerializer.Deserialize<DetallePerfilViewModel>(responseContent, options);

                if (usuario != null)
                {
                    return View(usuario);
                }
            }

            TempData["ErrorMessage"] = "No se pudieron cargar los datos del perfil.";
            return RedirectToAction("Principal", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DetallePerfil(DetallePerfilViewModel modelo)
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var idUsuario = GetUsuarioIdDesdeToken(token);
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var cliente = this.httpClientFactory.CreateClient("XandriaAPI");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var editarRequest = new
            {
                IdEstado = 1,
                Nombres = modelo.Nombres,
                Apellidos = modelo.Apellidos,
                Telefono = modelo.Telefono
            };

            var json = JsonSerializer.Serialize(editarRequest);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            var respuesta = await cliente.PutAsync($"Usuarios/actualizar/{idUsuario}", contenido);

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                return RedirectToAction("DetallePerfil");
            }
            else
            {
                var errorResponse = await respuesta.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al actualizar el perfil: {errorResponse}");
                return View(modelo);
            }
        }
    }
}