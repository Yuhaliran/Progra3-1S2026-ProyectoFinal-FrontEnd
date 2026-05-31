using System;
using System.Collections.Generic;

namespace Progra3.ProyectoFinal.Frontend.Util
{
    public class Pila<T>
    {
        private class NodoPila
        {
            public T Valor { get; set; }
            public NodoPila? Siguiente { get; set; }

            public NodoPila(T valor)
            {
                Valor = valor;
            }
        }

        private NodoPila? _cima;
        public int Cantidad { get; private set; }

        public void Push(T valor)
        {
            var nuevoNodo = new NodoPila(valor);
            nuevoNodo.Siguiente = _cima;
            _cima = nuevoNodo;
            Cantidad++;
        }

        public T Pop()
        {
            if (EstaVacia())
            {
                throw new InvalidOperationException("La pila está vacía.");
            }
            T valor = _cima!.Valor;
            _cima = _cima.Siguiente;
            Cantidad--;
            return valor;
        }

        public T Peek()
        {
            if (EstaVacia())
            {
                throw new InvalidOperationException("La pila está vacía.");
            }
            return _cima!.Valor;
        }

        public bool EstaVacia()
        {
            return _cima == null;
        }

        public List<T> ObtenerListaCompleta()
        {
            var lista = new List<T>();
            var actual = _cima;
            while (actual != null)
            {
                lista.Add(actual.Valor);
                actual = actual.Siguiente;
            }
            return lista;
        }
    }
}
