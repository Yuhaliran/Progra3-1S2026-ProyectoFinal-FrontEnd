namespace Progra3.ProyectoFinal.Frontend.Util
{
    public class Cola<T>
    {
        private class NodoCola
        {
            public T Valor { get; set; }
            public NodoCola? Siguiente { get; set; }

            public NodoCola(T valor)
            {
                Valor = valor;
            }
        }

        private NodoCola? _frente;
        private NodoCola? _final;
        public int Cantidad { get; private set; }

        public void Encolar(T valor)
        {
            var nuevoNodo = new NodoCola(valor);
            if (_final == null)
            {
                _frente = nuevoNodo;
                _final = nuevoNodo;
            }
            else
            {
                _final.Siguiente = nuevoNodo;
                _final = nuevoNodo;
            }
            Cantidad++;
        }

        public T Desencolar()
        {
            if (EstaVacia())
            {
                throw new InvalidOperationException("La cola está vacía.");
            }
            T valor = _frente!.Valor;
            _frente = _frente.Siguiente;
            if (_frente == null)
            {
                _final = null;
            }
            Cantidad--;
            return valor;
        }

        public T Frente()
        {
            if (EstaVacia())
            {
                throw new InvalidOperationException("La cola está vacía.");
            }
            return _frente!.Valor;
        }

        public bool EstaVacia()
        {
            return _frente == null;
        }

        public List<T> ObtenerListaCompleta()
        {
            var lista = new List<T>();
            var actual = _frente;
            while (actual != null)
            {
                lista.Add(actual.Valor);
                actual = actual.Siguiente;
            }
            return lista;
        }
    }
}
