// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.getElementById('theme-toggle');
    
    if (toggleBtn) {
        const esTemaClaro = document.body.classList.contains('tema-claro');
        toggleBtn.textContent = esTemaClaro ? '☀️' : '🌙';

        toggleBtn.addEventListener('click', () => {
            document.body.classList.toggle('tema-claro');
            const esClaro = document.body.classList.contains('tema-claro');
            
            localStorage.setItem('tema', esClaro ? 'claro' : 'oscuro');
            
            toggleBtn.textContent = esClaro ? '☀️' : '🌙';
        });
    }

    const sidebarToggle = document.getElementById('sidebar-toggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', () => {
            document.body.classList.toggle('sidebar-collapsed');
            const esColapsado = document.body.classList.contains('sidebar-collapsed');
            localStorage.setItem('sidebar-collapsed', esColapsado ? 'true' : 'false');
        });
    }

    const items = document.querySelectorAll('.libro-item');
    const btnPrev = document.getElementById('btn-carrusel-prev');
    const btnNext = document.getElementById('btn-carrusel-next');
    
    if (items.length > 0) {
        let currentIndex = 0;
        const total = items.length;

        const infoTitulo = document.getElementById('carrusel-info-titulo');
        const infoAutor = document.getElementById('carrusel-info-autor');
        const infoIsbn = document.getElementById('carrusel-info-isbn');

        function actualizarCarrusel() {
            items.forEach((item, index) => {
                item.classList.remove('active', 'prev', 'next', 'far-prev', 'far-next');

                if (index === currentIndex) {
                    item.classList.add('active');
                } else if (index === (currentIndex - 1 + total) % total) {
                    item.classList.add('prev');
                } else if (index === (currentIndex + 1) % total) {
                    item.classList.add('next');
                } else if (index === (currentIndex - 2 + total) % total) {
                    item.classList.add('far-prev');
                } else if (index === (currentIndex + 2) % total) {
                    item.classList.add('far-next');
                }
            });

            const activeItem = items[currentIndex];
            if (activeItem) {
                if (infoTitulo) infoTitulo.textContent = activeItem.getAttribute('data-titulo') || '';
                if (infoAutor) infoAutor.textContent = activeItem.getAttribute('data-autor') || '';
                if (infoIsbn) infoIsbn.textContent = `ISBN: ${activeItem.getAttribute('data-isbn') || ''}`;
            }
        }

        if (btnPrev) {
            btnPrev.addEventListener('click', () => {
                currentIndex = (currentIndex - 1 + total) % total;
                actualizarCarrusel();
            });
        }

        if (btnNext) {
            btnNext.addEventListener('click', () => {
                currentIndex = (currentIndex + 1) % total;
                actualizarCarrusel();
            });
        }

        items.forEach((item, index) => {
            item.addEventListener('click', () => {
                if (index !== currentIndex) {
                    currentIndex = index;
                    actualizarCarrusel();
                }
            });
        });

        actualizarCarrusel();
    }
});
