using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Data;
using BibliotecaApi.Models;


namespace BibliotecaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {

        private readonly AppDbContext _context;
        public LibrosController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Libro>>> GetLibros()
        {
            return await _context.Libros.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Libro>> GetLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            return libro;
        }

        [HttpGet("Buscar_titulo/{titulo}")]
        public async Task<ActionResult<Libro>> GetLibrotitulo(string titulo)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Titulo == titulo);
            if (libro == null)
            {
                return NotFound();
            }
            return libro;
        }

        [HttpGet("Buscar_estado/{estado}")]
        public async Task<ActionResult<IEnumerable<Libro>>> GetLibrosEstado(string estado)
        {
            var libros = await _context.Libros.Where(l => l.Estado == estado).ToListAsync();
            if (libros == null || libros.Count == 0)
            {
                return NotFound();
            }
            return libros;
        }




        [HttpPost]
        public async Task<ActionResult<CrearLibro>> PostLibro(CrearLibro crearLibro )
        {
            var libro = new Libro
            {
                Descripcion = crearLibro.Descripcion,
                Titulo = crearLibro.Titulo,
                Autor = crearLibro.Autor,
                Anio_Publicacion = crearLibro.Anio_Publicacion,
                Estado = "Disponible",
                Genero = crearLibro.Genero,
                Imagen_Portada = crearLibro.Imagen_Portada
            };

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            return Ok($"El libro '{libro.Titulo}' ha sido agregado correctamente.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLibro(int id, Libro libro)
        {
            if (id != libro.Id_Libro)
            {
                return BadRequest("El ID del libro no coincide.");
            }
            var libroExistente = await _context.Libros.FindAsync(id);
            if (libroExistente == null)
            {
                return NotFound("El libro no existe.");
            }

            libroExistente.Titulo = libro.Titulo;
            libroExistente.Descripcion = libro.Descripcion;
            libroExistente.Autor = libro.Autor;
            libroExistente.Genero = libro.Genero;
            libroExistente.Anio_Publicacion = libro.Anio_Publicacion;
            libroExistente.Estado = libro.Estado;
            libroExistente.Imagen_Portada = libro.Imagen_Portada;

            await _context.SaveChangesAsync();
            return Ok($"El libro '{libro.Titulo}' ha sido actualizado correctamente.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
                return NotFound("El libro no existe.");

            if (libro.Estado == "Prestado")
                return BadRequest("No se puede eliminar el libro porque está prestado.");

            var prestamos = _context.Prestamos
            .Where(p => p.Id_Libro == id);

            _context.Prestamos.RemoveRange(prestamos);

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return Ok($"El libro '{libro.Titulo}' ha sido eliminado correctamente.");
        }

        [HttpGet("con-usuario")]
        public async Task<ActionResult> GetLibrosConUsuario()
        {
            var libros = await _context.Libros
                .Select(l => new
                {
                    l.Id_Libro,
                    l.Titulo,
                    l.Autor,
                    l.Anio_Publicacion,
                    l.Genero,
                    l.Descripcion,
                    l.Imagen_Portada,
                    l.Estado,

                    UsuarioPrestamo = _context.Prestamos
                        .Where(p => p.Id_Libro == l.Id_Libro && p.Estado == "Activo")
                        .Select(p => p.Usuario.Nombre)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(libros);
        }

        [HttpGet("con-usuario/estado/{estado}")]
        public async Task<ActionResult> GetLibrosConUsuarioPorEstado(string estado)
        {
            var libros = await _context.Libros
                .Where(l => l.Estado == estado)
                .Select(l => new
                {
                    l.Id_Libro,
                    l.Titulo,
                    l.Autor,
                    l.Anio_Publicacion,
                    l.Genero,
                    l.Descripcion,
                    l.Imagen_Portada,
                    l.Estado,

                    UsuarioPrestamo = _context.Prestamos
                        .Where(p => p.Id_Libro == l.Id_Libro && p.Estado == "Activo")
                        .Select(p => p.Usuario.Nombre)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(libros);
        }
    }
}
