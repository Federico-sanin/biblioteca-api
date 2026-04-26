using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaApi.Models
{
    public class Prestamo
    {
        [Key]
        public int Id_Prestamo { get; set; }

        public int Id_Libro { get; set; }

        [ForeignKey("Id_Libro")]
        public Libro Libro { get; set; }

        public int Id_Usuario { get; set; }

        [ForeignKey("Id_Usuario")]
        public Usuario Usuario { get; set; }

        public DateTime Fecha_Prestamo { get; set; }
        public DateTime? Fecha_Devolucion { get; set; }

        public string? Estado { get; set; }
    }
}
