using System.ComponentModel.DataAnnotations;

namespace Progra3.ProyectoFinal.Frontend.Models.Usuarios
{
    public class CrearUsuario
    {
        [Required(ErrorMessage = "El Rol es obligatorio")]
        public int IdRol { get; set; }

        [Required(ErrorMessage = "Los Nombres son obligatorios")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los Apellidos son obligatorios")]
        public string Apellidos { get; set; } = string.Empty;

        public string? DPI { get; set; }

        [Required(ErrorMessage = "El Correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = string.Empty;

        public string? Telefono { get; set; }
    }
}
