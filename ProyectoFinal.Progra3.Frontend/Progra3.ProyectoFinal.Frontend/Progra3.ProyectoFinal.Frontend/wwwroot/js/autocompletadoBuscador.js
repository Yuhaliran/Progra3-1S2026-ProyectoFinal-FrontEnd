document.addEventListener('DOMContentLoaded', () => {
    const inputBusqueda = document.getElementById('input-busqueda');
    const contenedorSugerencias = document.getElementById('sugerencias-busqueda');

    if (inputBusqueda && contenedorSugerencias) {
        let temporizadorDebounce;

        inputBusqueda.addEventListener('input', () => {
            clearTimeout(temporizadorDebounce);
            const terminoBusqueda = inputBusqueda.value.trim();

            if (terminoBusqueda.length < 1) {
                contenedorSugerencias.classList.add('d-none');
                contenedorSugerencias.classList.remove('show');
                contenedorSugerencias.innerHTML = '';
                return;
            }

            temporizadorDebounce = setTimeout(() => {
                fetch(`/Home/Sugerencias?palabraABuscar=${encodeURIComponent(terminoBusqueda)}`)
                    .then(respuesta => respuesta.json())
                    .then(datos => {
                        contenedorSugerencias.innerHTML = '';

                        if (datos.length === 0) {
                            contenedorSugerencias.classList.add('d-none');
                            contenedorSugerencias.classList.remove('show');
                            return;
                        }

                        datos.forEach(libro => {
                            const enlaceItem = document.createElement('a');
                            const codigoIsbn = libro.isbn || libro.ISBN || '';
                            const tituloLibro = libro.titulo || libro.Titulo || '';
                            const autorLibro = libro.autor || libro.Autor || '';
                            const portadaLibro = libro.portada || libro.Portada || '';

                            enlaceItem.href = `/Home/DetallesLibro?isbn=${encodeURIComponent(codigoIsbn)}`;
                            enlaceItem.className = 'dropdown-item d-flex align-items-center px-4 py-3 text-white border-bottom';
                            enlaceItem.style.transition = 'background-color 0.2s';
                            enlaceItem.style.borderBottomColor = 'rgba(255,255,255,0.05)';

                            let fuentePortada = '/images/Home-book.webp';
                            if (portadaLibro) {
                                if (portadaLibro.startsWith('http') || portadaLibro.startsWith('/') || portadaLibro.startsWith('~')) {
                                    fuentePortada = portadaLibro;
                                } else if (portadaLibro.includes('base64,')) {
                                    fuentePortada = portadaLibro;
                                } else {
                                    fuentePortada = 'data:image/jpeg;base64,' + portadaLibro;
                                }
                            }

                            enlaceItem.innerHTML = `
                                <img src="${fuentePortada}" alt="${tituloLibro}" class="rounded shadow-sm me-3" style="width: 40px; height: 55px; object-fit: cover;" onerror="this.src='/images/Home-book.webp';">
                                <div class="overflow-hidden flex-grow-1">
                                    <strong class="d-block text-truncate text-white" style="font-size: 0.95rem; font-weight: 600;">${tituloLibro}</strong>
                                    <span class="d-block text-truncate text-muted small" style="color: var(--texto-secundario) !important;">👤 ${autorLibro}</span>
                                    <span class="d-block font-monospace text-muted" style="font-size: 0.72rem;">🔢 ISBN: ${codigoIsbn}</span>
                                </div>
                            `;
                            
                            enlaceItem.addEventListener('mouseenter', () => {
                                enlaceItem.style.backgroundColor = 'var(--fondo-terciario)';
                            });
                            enlaceItem.addEventListener('mouseleave', () => {
                                enlaceItem.style.backgroundColor = 'transparent';
                            });

                            contenedorSugerencias.appendChild(enlaceItem);
                        });

                        contenedorSugerencias.classList.remove('d-none');
                        contenedorSugerencias.classList.add('show');
                    })
                    .catch(error => console.error('Error al traer sugerencias:', error));
            }, 150);
        });

        document.addEventListener('click', (evento) => {
            if (!inputBusqueda.contains(evento.target) && !contenedorSugerencias.contains(evento.target)) {
                contenedorSugerencias.classList.add('d-none');
                contenedorSugerencias.classList.remove('show');
            }
        });

        inputBusqueda.addEventListener('focus', () => {
            if (inputBusqueda.value.trim().length > 0 && contenedorSugerencias.children.length > 0) {
                contenedorSugerencias.classList.remove('d-none');
                contenedorSugerencias.classList.add('show');
            }
        });
    }
});
