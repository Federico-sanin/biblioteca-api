using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models
{
    public class Libro
    {
        [Key]
        public int Id_Libro { get; set; }


        public string? Descripcion { get; set; }
        public string? Titulo { get; set; }
        public string? Autor { get; set; }
        public int Anio_Publicacion { get; set; }
        public string? Estado { get; set; }
        public string? Genero { get; set; }
        public string? Imagen_Portada { get; set; }
    }
}
