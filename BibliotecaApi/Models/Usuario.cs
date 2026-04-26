using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models
{
    public class Usuario
    {
        [Key]
        public int Id_Usuario { get; set; }
        public string? Nombre { get; set; }
    }
}
