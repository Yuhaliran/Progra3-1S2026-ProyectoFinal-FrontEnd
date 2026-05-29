namespace Progra3.ProyectoFinal.Frontend.Models.Response.Bitacora
{
    public class ColaLecturaResponse
    {
        public int IdColaLectura { get; set; }
        public int IdUsuario { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int IdEstadoLectura { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool? MeGusto { get; set; }
        public string NombreEstado { get; set; } = string.Empty;
        public string TituloLibro { get; set; } = string.Empty;
        public string AutorLibro { get; set; } = string.Empty;
    }
}
