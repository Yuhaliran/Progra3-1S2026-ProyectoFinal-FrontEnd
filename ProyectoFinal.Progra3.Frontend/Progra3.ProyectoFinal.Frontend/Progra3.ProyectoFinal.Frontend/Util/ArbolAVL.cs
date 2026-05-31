namespace Progra3.ProyectoFinal.Frontend.Util
{
    public class ArbolAVL<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class NodoAVL
        {
            public TKey Clave { get; set; }
            public List<TValue> Valores { get; set; }
            public NodoAVL? Izquierdo { get; set; }
            public NodoAVL? Derecho { get; set; }
            public int Altura { get; set; }

            public NodoAVL(TKey clave, TValue valor)
            {
                Clave = clave;
                Valores = new List<TValue> { valor };
                Altura = 1;
            }
        }

        private NodoAVL? raiz;

        private int obtenerAltura(NodoAVL? nodoActual)
        {
            return nodoActual?.Altura ?? 0;
        }

        private int obtenerFactorEquilibrio(NodoAVL? nodoActual)
        {
            if (nodoActual == null) return 0;
            return obtenerAltura(nodoActual.Izquierdo) - obtenerAltura(nodoActual.Derecho);
        }

        private NodoAVL rotarDerecha(NodoAVL y)
        {
            NodoAVL x = y.Izquierdo!;
            NodoAVL? t2 = x.Derecho;

            x.Derecho = y;
            y.Izquierdo = t2;

            y.Altura = Math.Max(obtenerAltura(y.Izquierdo), obtenerAltura(y.Derecho)) + 1;
            x.Altura = Math.Max(obtenerAltura(x.Izquierdo), obtenerAltura(x.Derecho)) + 1;

            return x;
        }

        private NodoAVL rotarIzquierda(NodoAVL x)
        {
            NodoAVL y = x.Derecho!;
            NodoAVL? t2 = y.Izquierdo;

            y.Izquierdo = x;
            x.Derecho = t2;

            x.Altura = Math.Max(obtenerAltura(x.Izquierdo), obtenerAltura(x.Derecho)) + 1;
            y.Altura = Math.Max(obtenerAltura(y.Izquierdo), obtenerAltura(y.Derecho)) + 1;

            return y;
        }

        public void Insertar(TKey clave, TValue valor)
        {
            raiz = insertarInterno(raiz, clave, valor);
        }

        private NodoAVL insertarInterno(NodoAVL? nodo, TKey clave, TValue valor)
        {
            if (nodo == null)
            {
                return new NodoAVL(clave, valor);
            }

            int comparacion = clave.CompareTo(nodo.Clave);

            if (comparacion < 0)
            {
                nodo.Izquierdo = insertarInterno(nodo.Izquierdo, clave, valor);
            }
            else if (comparacion > 0)
            {
                nodo.Derecho = insertarInterno(nodo.Derecho, clave, valor);
            }
            else
            {
                nodo.Valores.Add(valor);
                return nodo;
            }

            nodo.Altura = Math.Max(obtenerAltura(nodo.Izquierdo), obtenerAltura(nodo.Derecho)) + 1;

            int factorEquilibrio = obtenerFactorEquilibrio(nodo);

            if (factorEquilibrio > 1 && clave.CompareTo(nodo.Izquierdo!.Clave) < 0)
            {
                return rotarDerecha(nodo);
            }

            if (factorEquilibrio < -1 && clave.CompareTo(nodo.Derecho!.Clave) > 0)
            {
                return rotarIzquierda(nodo);
            }

            if (factorEquilibrio > 1 && clave.CompareTo(nodo.Izquierdo!.Clave) > 0)
            {
                nodo.Izquierdo = rotarIzquierda(nodo.Izquierdo);
                return rotarDerecha(nodo);
            }

            if (factorEquilibrio < -1 && clave.CompareTo(nodo.Derecho!.Clave) < 0)
            {
                nodo.Derecho = rotarDerecha(nodo.Derecho);
                return rotarIzquierda(nodo);
            }

            return nodo;
        }

        public void ObtenerRango(TKey min, TKey max, List<TValue> resultado)
        {
            obtenerRangoInterno(raiz, min, max, resultado);
        }

        private void obtenerRangoInterno(NodoAVL? nodo, TKey min, TKey max, List<TValue> resultado)
        {
            if (nodo == null)
            {
                return;
            }

            if (nodo.Clave.CompareTo(min) > 0)
            {
                obtenerRangoInterno(nodo.Izquierdo, min, max, resultado);
            }

            if (nodo.Clave.CompareTo(min) >= 0 && nodo.Clave.CompareTo(max) <= 0)
            {
                resultado.AddRange(nodo.Valores);
            }

            if (nodo.Clave.CompareTo(max) < 0)
            {
                obtenerRangoInterno(nodo.Derecho, min, max, resultado);
            }
        }
    }
}
