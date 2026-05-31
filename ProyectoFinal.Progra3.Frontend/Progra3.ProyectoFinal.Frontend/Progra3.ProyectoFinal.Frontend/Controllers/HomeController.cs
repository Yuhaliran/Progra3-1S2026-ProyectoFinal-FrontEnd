namespace Progra3.ProyectoFinal.Frontend.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Progra3.ProyectoFinal.Frontend.Models;
    using Progra3.ProyectoFinal.Frontend.Models.Response.Libros;
    using Progra3.ProyectoFinal.Frontend.Models.Response.Bitacora;
    using Progra3.ProyectoFinal.Frontend.Util;
    using System.Diagnostics;
    using System.Text.Json;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> _logger, IHttpClientFactory _httpClientFactory)
        {
            this._logger = _logger;
            this._httpClientFactory = _httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Principal()
        {
            var listaCircular = new ListaCircularDoble<LibroResponse>();

            try
            {
                var cliente = _httpClientFactory.CreateClient("XandriaAPI");
                var respuesta = await cliente.GetAsync("Libros/obtenerTodos");

                if (respuesta.IsSuccessStatusCode)
                {
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var todosLosLibros = JsonSerializer.Deserialize<List<LibroResponse>>(contenido, opciones);

                    if (todosLosLibros != null && todosLosLibros.Count > 0)
                    {
                        var rnd = new Random();
                        var librosAlAzar = todosLosLibros
                            .OrderBy(x => rnd.Next())
                            .Take(10)
                            .ToList();

                        foreach (var libro in librosAlAzar)
                        {
                            listaCircular.InsertarAlFinal(libro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener libros del backend para la vista Principal");
            }

            return View(listaCircular);
        }

        public async Task<IActionResult> DetallesLibro(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return RedirectToAction(nameof(Principal));
            }

            try
            {
                var cliente = _httpClientFactory.CreateClient("XandriaAPI");
                
                var token = Request.Cookies["JwtToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var respuestaLibro = await cliente.GetAsync($"Libros/obtenerPorIsbn/{isbn}");
                
                if (respuestaLibro.IsSuccessStatusCode)
                {
                    var contenidoLibro = await respuestaLibro.Content.ReadAsStringAsync();
                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var libro = JsonSerializer.Deserialize<LibroResponse>(contenidoLibro, opciones);

                    if (libro != null)
                    {
                        ViewBag.Libro = libro;
                        
                        if (!string.IsNullOrEmpty(token))
                        {
                            var respuestaCola = await cliente.GetAsync($"ColaLectura/{isbn}");
                            if (respuestaCola.IsSuccessStatusCode)
                            {
                                var contenidoCola = await respuestaCola.Content.ReadAsStringAsync();
                                var cola = JsonSerializer.Deserialize<Progra3.ProyectoFinal.Frontend.Models.Response.Bitacora.ColaLecturaResponse>(contenidoCola, opciones);
                                ViewBag.ColaLectura = cola;
                            }
                            
                            var respuestaEstados = await cliente.GetAsync($"ColaLectura/estados");
                            if (respuestaEstados.IsSuccessStatusCode)
                            {
                                var contenidoEstados = await respuestaEstados.Content.ReadAsStringAsync();
                                var estados = JsonSerializer.Deserialize<List<Progra3.ProyectoFinal.Frontend.Models.EstadoLectura>>(contenidoEstados, opciones);
                                ViewBag.Estados = estados;
                            }
                        }

                        return View(libro);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalles del libro {isbn}");
            }

            return RedirectToAction(nameof(Principal));
        }

        [HttpPost]
        public async Task<IActionResult> GuardarColaLectura(Progra3.ProyectoFinal.Frontend.Models.Request.Bitacora.ColaLecturaRequest request)
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var cliente = _httpClientFactory.CreateClient("XandriaAPI");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var contenido = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync("ColaLectura", contenido);

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Bitácora actualizada satisfactoriamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Hubo un error al guardar en la bitácora.";
            }

            return RedirectToAction(nameof(DetallesLibro), new { isbn = request.ISBN });
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Principal));
            }

            var resultados = new List<LibroResponse>();
            query = query.Trim();

            try
            {
                var cliente = _httpClientFactory.CreateClient("XandriaAPI");
                var token = Request.Cookies["JwtToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                if (esISBN(query))
                {
                    var respuestaLocal = await cliente.GetAsync($"Libros/obtenerPorIsbn/{query}");
                    if (respuestaLocal.IsSuccessStatusCode)
                    {
                        var contenido = await respuestaLocal.Content.ReadAsStringAsync();
                        var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var libro = JsonSerializer.Deserialize<LibroResponse>(contenido, opciones);
                        if (libro != null)
                        {
                            resultados.Add(libro);
                        }
                    }
                    else
                    {
                        var respuestaUnificada = await cliente.GetAsync($"Libros/buscar-unificado/{query}");
                        if (respuestaUnificada.IsSuccessStatusCode)
                        {
                            var contenido = await respuestaUnificada.Content.ReadAsStringAsync();
                            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var libro = JsonSerializer.Deserialize<LibroResponse>(contenido, opciones);
                            if (libro != null)
                            {
                                resultados.Add(libro);
                            }
                        }
                    }
                }
                else
                {
                    var respuestaLocal = await cliente.GetAsync($"Libros/buscar-local?query={Uri.EscapeDataString(query)}");
                    if (respuestaLocal.IsSuccessStatusCode)
                    {
                        var contenido = await respuestaLocal.Content.ReadAsStringAsync();
                        var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var libros = JsonSerializer.Deserialize<List<LibroResponse>>(contenido, opciones);
                        if (libros != null)
                        {
                            resultados.AddRange(libros);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la búsqueda");
            }

            ViewBag.Query = query;
            return View("ResultadosBusqueda", resultados);
        }

        private bool esISBN(string query)
        {
            if (string.IsNullOrEmpty(query)) return false;
            var clean = query.Replace("-", "").Replace(" ", "").Trim();
            if (clean.Length != 10 && clean.Length != 13) return false;
            for (int i = 0; i < clean.Length; i++)
            {
                if (!char.IsDigit(clean[i]))
                {
                    if (clean.Length == 10 && i == 9 && (clean[i] == 'X' || clean[i] == 'x'))
                    {
                        continue;
                    }
                    return false;
                }
            }
            return true;
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
        public async Task<IActionResult> MiColaDeLectura()
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

            var pila = new Pila<ColaLecturaResponse>();

            try
            {
                var cliente = _httpClientFactory.CreateClient("XandriaAPI");
                cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await cliente.GetAsync("ColaLectura");
                if (respuesta.IsSuccessStatusCode)
                {
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var lista = JsonSerializer.Deserialize<List<ColaLecturaResponse>>(contenido, opciones);

                    if (lista != null)
                    {
                        foreach (var item in lista)
                        {
                            pila.Push(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la cola de lectura del usuario.");
            }

            return View(pila);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarDeCola(string isbn)
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var cliente = _httpClientFactory.CreateClient("XandriaAPI");
                cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await cliente.DeleteAsync($"ColaLectura/{isbn}");

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Libro eliminado de tu cola de lectura.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo eliminar el libro de tu cola de lectura.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el libro {isbn} de la cola de lectura.");
                TempData["ErrorMessage"] = "Ocurrió un error al procesar tu solicitud.";
            }

            return RedirectToAction(nameof(MiColaDeLectura));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
