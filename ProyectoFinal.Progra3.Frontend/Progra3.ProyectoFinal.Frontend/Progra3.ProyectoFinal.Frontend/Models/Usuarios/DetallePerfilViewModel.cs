using System.ComponentModel.DataAnnotations;

namespace Progra3.ProyectoFinal.Frontend.Models.Usuarios
{
    public class DetallePerfilViewModel
    {
        public int IdUsuario { get; set; }

        public string Rol { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los Nombres son obligatorios")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los Apellidos son obligatorios")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Display(Name = "DPI")]
        public string DPI { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }
}
