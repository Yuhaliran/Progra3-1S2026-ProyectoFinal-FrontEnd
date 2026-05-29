namespace Progra3.ProyectoFinal.Frontend.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Progra3.ProyectoFinal.Frontend.Models;
    using Progra3.ProyectoFinal.Frontend.Models.Response.Libros;
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

            return RedirectToAction(nameof(DetallesLibro), new { isbn = request.ISBN });
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
