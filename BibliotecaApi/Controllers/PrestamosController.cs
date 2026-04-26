using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Data;
using BibliotecaApi.Models;

namespace BibliotecaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PrestamosController(AppDbContext context)
        {
            _context = context;
        }

        //Obtener todos los préstamos
        [HttpGet]
        public async Task<ActionResult> GetPrestamos()
        {
            var resultado = await _context.Prestamos
                .Select(p => new
                {
                    Libro = p.Libro.Titulo,
                    Usuario = p.Usuario.Nombre,
                    Estado = p.Estado,
                    Fecha_Prestamo = p.Fecha_Prestamo.ToString("dd/MM/yyyy"),
                    Fecha_devolución = p.Fecha_Devolucion.HasValue ? p.Fecha_Devolucion.Value.ToString("dd/MM/yyyy") : "No devuelto"
                })
                .ToListAsync();

            return Ok(resultado);
        }

        //Obtener un préstamo por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> GetPrestamo(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id_Prestamo == id);

            if (prestamo == null)
                return NotFound();

            return prestamo;
        }


        //Crear préstamo
        [HttpPost]
        public async Task<ActionResult> CrearPrestamo(CrearPrestamoDto dto)
        {
            var libro = await _context.Libros.FindAsync(dto.Id_Libro);
            if (libro == null)
                return BadRequest("El libro no existe");

            var usuario = await _context.Usuarios.FindAsync(dto.Id_Usuario);
            if (usuario == null)
                return BadRequest("El usuario no existe");

            if (libro.Estado != "Disponible")
                return BadRequest("El libro no está disponible");

            var prestamo = new Prestamo
            {
                Id_Libro = dto.Id_Libro,
                Id_Usuario = dto.Id_Usuario,
                Fecha_Prestamo = DateTime.Now,
                Estado = "Activo"
            };

            libro.Estado = "Prestado";

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            return Ok("Préstamo creado");
        }

        //Devolver libro
        [HttpPut("devolver/libro/{idLibro}")]
        public async Task<ActionResult> DevolverPorLibro(int idLibro)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id_Libro == idLibro && p.Estado == "Activo");

            if (prestamo == null)
                return NotFound("No hay préstamo activo para este libro");

            prestamo.Fecha_Devolucion = DateTime.Now;
            prestamo.Estado = "Devuelto";

            prestamo.Libro.Estado = "Disponible";

            await _context.SaveChangesAsync();

            return Ok("Libro devuelto correctamente");
        }

        //Préstamos activos
        [HttpGet("activos")]
        public async Task<ActionResult> GetActivos()
        {
            var resultado = await _context.Prestamos
                .Where(p => p.Estado == "Activo")
                .Select(p => new
                {
                    Libro = p.Libro.Titulo,
                    Usuario = p.Usuario.Nombre,
                    FechaPrestamo = p.Fecha_Prestamo
                })
                .ToListAsync();

            return Ok(resultado);
        }

        //Historial por usuario
        [HttpGet("usuario/{id}")]
        public async Task<ActionResult> GetPorUsuario(int id)
        {
            var resultado = await _context.Prestamos
                .Where(p => p.Id_Usuario == id)
                .Select(p => new
                {
                    Libro = p.Libro.Titulo,
                    FechaPrestamo = p.Fecha_Prestamo,
                    FechaDevolucion = p.Fecha_Devolucion,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Ok(resultado);
        }

        


    }
}
