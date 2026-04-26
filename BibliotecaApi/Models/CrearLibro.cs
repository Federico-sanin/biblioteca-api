namespace BibliotecaApi.Models
{
    public class CrearLibro
    {
        public string? Descripcion { get; set; }
        public string? Titulo { get; set; }
        public string? Autor { get; set; }
        public int Anio_Publicacion { get; set; }
        public string? Estado { get; set; } 
        public string? Genero { get; set; }
        public string? Imagen_Portada { get; set; }
    }
}
