using System.ComponentModel.DataAnnotations;

namespace Progra3.ProyectoFinal.Frontend.Models.Usuarios
{
    public class LoginUsuario
    {
        [Required(ErrorMessage = "El Correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
