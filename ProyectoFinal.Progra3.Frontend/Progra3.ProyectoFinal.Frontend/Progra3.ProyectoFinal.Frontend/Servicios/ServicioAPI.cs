namespace Progra3.ProyectoFinal.Frontend.Servicios
{  
    public class ServicioAPI
    {
        public HttpClient Cliente { get; }
        
        public ServicioAPI(HttpClient cliente)
        {
            this.Cliente = cliente;
        }
    }
}
