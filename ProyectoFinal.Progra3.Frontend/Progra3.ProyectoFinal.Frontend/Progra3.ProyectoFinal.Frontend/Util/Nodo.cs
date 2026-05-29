namespace Progra3.ProyectoFinal.Frontend.Util
{
    public class Nodo<T>
    {
        public T Valor { get; set; }
        public Nodo<T> Siguiente { get; set; }
        public Nodo<T> Anterior { get; set; }

        public Nodo(T valor)
        {
            Valor = valor;
            Siguiente = this;
            Anterior = this;
        }
    }
}
