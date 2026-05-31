namespace Progra3.ProyectoFinal.Frontend.Util
{
    public class TablaHash<TKey, TValue> where TKey : notnull
    {
        private class EntradaHash
        {
            public TKey Clave { get; set; }
            public TValue Valor { get; set; }
            public EntradaHash? Siguiente { get; set; }

            public EntradaHash(TKey clave, TValue valor)
            {
                Clave = clave;
                Valor = valor;
            }
        }

        private readonly EntradaHash?[] datosHash;
        private readonly int capcidad;
        public int Cantidad { get; private set; }

        public TablaHash(int capacidad = 127)
        {
            capcidad = capacidad > 0 ? capacidad : 127;
            datosHash = new EntradaHash?[capcidad];
        }

        private int ObtenerIndice(TKey clave)
        {
            int hash = clave.GetHashCode();
            int indice = Math.Abs(hash % capcidad);
            return indice;
        }

        public void Insertar(TKey clave, TValue valor)
        {
            int indice = ObtenerIndice(clave);
            var actual = datosHash[indice];

            while (actual != null)
            {
                if (actual.Clave.Equals(clave))
                {
                    actual.Valor = valor;
                    return;
                }
                actual = actual.Siguiente;
            }

            var nuevaEntrada = new EntradaHash(clave, valor)
            {
                Siguiente = datosHash[indice]
            };
            datosHash[indice] = nuevaEntrada;
            Cantidad++;
        }

        public bool Buscar(TKey clave, out TValue valor)
        {
            int indice = ObtenerIndice(clave);
            var actual = datosHash[indice];

            while (actual != null)
            {
                if (actual.Clave.Equals(clave))
                {
                    valor = actual.Valor;
                    return true;
                }
                actual = actual.Siguiente;
            }

            valor = default!;
            return false;
        }

        public bool Eliminar(TKey clave)
        {
            int indice = ObtenerIndice(clave);
            var actual = datosHash[indice];
            EntradaHash? anterior = null;

            while (actual != null)
            {
                if (actual.Clave.Equals(clave))
                {
                    if (anterior == null)
                    {
                        datosHash[indice] = actual.Siguiente;
                    }
                    else
                    {
                        anterior.Siguiente = actual.Siguiente;
                    }
                    Cantidad--;
                    return true;
                }
                anterior = actual;
                actual = actual.Siguiente;
            }

            return false;
        }

        public void Limpiar()
        {
            Array.Clear(datosHash, 0, datosHash.Length);
            Cantidad = 0;
        }
    }
}
