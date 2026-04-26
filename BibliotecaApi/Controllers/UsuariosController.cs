using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Data;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // Crear un nuevo usuario
        [HttpPost]
        public async Task<ActionResult> PostUsuario(CrearUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre es obligatorio");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario creado correctamente",
                id = usuario.Id_Usuario
            });
        }

        // Actualizar un usuario existente
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id_Usuario)
                return BadRequest();

            var existe = await _context.Usuarios.AnyAsync(u => u.Id_Usuario == id);
            if (!existe) return NotFound();

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Obtener los préstamos activos de un usuario
        [HttpGet("{id}/prestamos-activos")]
        public async Task<ActionResult> GetPrestamosActivos(int id)
        {
            var resultado = await _context.Prestamos
                .Where(p => p.Id_Usuario == id && p.Estado == "Activo")
                .Select(p => new
                {
                    Libro = p.Libro.Titulo,
                    FechaPrestamo = p.Fecha_Prestamo
                })
                .ToListAsync();

            return Ok(resultado);
        }



    }
}
