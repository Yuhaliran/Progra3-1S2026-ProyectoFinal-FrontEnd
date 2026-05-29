namespace Progra3.ProyectoFinal.Frontend.Models.Request.Bitacora
{
    public class ColaLecturaRequest
    {
        public string ISBN { get; set; } = string.Empty;
        public int IdEstadoLectura { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool? MeGusto { get; set; }
    }
}
