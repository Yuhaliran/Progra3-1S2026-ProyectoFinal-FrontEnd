namespace Progra3.ProyectoFinal.Frontend.Util
{
    public class ListaCircularDoble<T>
    {
        public Nodo<T> Cabeza { get; private set; }
        public int Cantidad { get; private set; }

        public void InsertarAlFinal(T valor)
        {
            var nuevoNodo = new Nodo<T>(valor);

            if (Cabeza == null)
            {
                Cabeza = nuevoNodo;
                Cabeza.Siguiente = Cabeza;
                Cabeza.Anterior = Cabeza;
            }
            else
            {
                var ultimo = Cabeza.Anterior;
                ultimo.Siguiente = nuevoNodo;
                nuevoNodo.Anterior = ultimo;
                nuevoNodo.Siguiente = Cabeza;
                Cabeza.Anterior = nuevoNodo;
            }

            Cantidad++;
        }

        public List<T> ObtenerListaCompleta()
        {
            var lista = new List<T>();
            if (Cabeza == null) return lista;

            var actual = Cabeza;
            do
            {
                lista.Add(actual.Valor);
                actual = actual.Siguiente;
            } while (actual != Cabeza);

            return lista;
        }
    }
}
