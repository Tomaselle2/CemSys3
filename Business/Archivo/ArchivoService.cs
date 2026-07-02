using CemSys3.DTOs.Archivo;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Archivo;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Archivo
{
    public class ArchivoService : IArchivo
    {
        private readonly AppDbContext _context;

        public ArchivoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(ArchivoDTO dto)
        {
            byte[] contenido = Array.Empty<byte>();

            using (var ms = new MemoryStream())
            {
                if(dto.Archivo != null)
                {
                    await dto.Archivo.CopyToAsync(ms);
                    contenido = ms.ToArray();
                }
            }
            
            Models.Archivo archivo = new Models.Archivo();

            if (dto.Archivo != null)
            {
                archivo.CategoriaArchivo = dto.CategoriaArchivo;
                archivo.TramiteId = dto.TramiteId <= 0 ? null : dto.TramiteId;
                archivo.NombreArchivo = Path.GetFileName(dto.Archivo.FileName);
                archivo.TipoArchivo = dto.MimeType ?? "application/octet-stream";
                archivo.TamanoBytes = dto.Archivo.Length;
                archivo.Contenido = contenido;
                archivo.Descripcion = dto.Descripcion?.Trim();
                archivo.FechaCreacion = DateTime.Now;
                archivo.Visibilidad = true;
            }

            await _context.Archivos.AddAsync(archivo);
            await _context.SaveChangesAsync();
        }

        public async Task AddDesdeBytes(ArchivoDTO dto)
        {
            Models.Archivo archivo = new Models.Archivo
            {
                CategoriaArchivo = dto.CategoriaArchivo,
                TramiteId = dto.TramiteId <= 0 ? null : dto.TramiteId,
                NombreArchivo = dto.NombreArchivo,
                TipoArchivo = dto.MimeType ?? "application/octet-stream",
                TamanoBytes = dto.Contenido?.Length ?? 0,
                Contenido = dto.Contenido ?? Array.Empty<byte>(),
                Descripcion = dto.Descripcion?.Trim(),
                FechaCreacion = DateTime.Now,
                Visibilidad = true
            };

            await _context.Archivos.AddAsync(archivo);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid archivoId)
        {
            Models.Archivo archivo = await _context.Archivos.FindAsync(archivoId) ?? throw new Exception("No se encontro el archivo");

            _context.Archivos.Remove(archivo);

            await _context.SaveChangesAsync();
        }

        public async Task<ArchivoDTO> Get(Guid archivoId)
        {
            Models.Archivo archivo = await _context.Archivos.FindAsync(archivoId) ?? throw new Exception("No se encontro el archivo");

            return new ArchivoDTO
            {
                Id = archivo.Id,
                NombreArchivo = archivo.NombreArchivo,
                TipoArchivo = archivo.TipoArchivo,
                Descripcion = archivo.Descripcion,
                Visibilidad = archivo.Visibilidad,
                Contenido = archivo.Contenido
            };
        }

        public async Task<IEnumerable<ArchivoDTO>> GetAllByTramiteId(int tramiteId)
        {
            return await _context.Archivos.Where(a => a.TramiteId == tramiteId).AsNoTracking().OrderByDescending(f=>f.FechaCreacion).Select(a => new ArchivoDTO {
                Id = a.Id,
                CategoriaArchivo = a.CategoriaArchivo,
                TramiteId = a.TramiteId,
                NombreArchivo = a.NombreArchivo,
                TipoArchivo = a.TipoArchivo,
                Descripcion = a.Descripcion,
                FechaCreacion = a.FechaCreacion,
                Visibilidad = a.Visibilidad,
            }).ToListAsync();
        }

        public async Task<IEnumerable<ArchivoDTO>> GetDocumentacionSistema()
        {
            IEnumerable<ArchivoDTO> dto = new List<ArchivoDTO>();

            dto = await _context.Archivos
            .Where(a =>
             a.TramiteId == null &&
             a.CategoriaArchivo == ((int)CategoriaArchivosEnum.DocumentacionCemSys).ToString() || a.CategoriaArchivo == ((int)CategoriaArchivosEnum.Tarifaria).ToString()).Select(a => new ArchivoDTO
             {
                 Id = a.Id,
                 CategoriaArchivo = a.CategoriaArchivo,
                 TramiteId = a.TramiteId,
                 NombreArchivo = a.NombreArchivo,
                 TipoArchivo = a.TipoArchivo,
                 Descripcion = a.Descripcion,
                 FechaCreacion = a.FechaCreacion,
                 Visibilidad = a.Visibilidad,
             }).ToListAsync();

            return dto;
        }

        public async Task Update(ArchivoDTO dto)
        {
            Models.Archivo archivo = await _context.Archivos.FindAsync(dto.Id) ?? throw new Exception("No se encontro el archivo");

            archivo.Descripcion = dto.Descripcion?.Trim();
            archivo.CategoriaArchivo = dto.CategoriaArchivo;

            if(dto.Archivo != null && dto.Archivo.Length > 0)
            {
                var extension = Path.GetExtension(dto.Archivo.FileName).ToLower();
                string mimeType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".pdf" => "application/pdf",
                    _ => "application/octet-stream"
                };

                using (var ms = new MemoryStream())
                {
                    await dto.Archivo.CopyToAsync(ms);
                    archivo.Contenido = ms.ToArray();
                }

                archivo.NombreArchivo = Path.GetFileName(dto.Archivo.FileName);
                archivo.TipoArchivo = mimeType;
                archivo.TamanoBytes = dto.Archivo.Length;
            }

            await _context.SaveChangesAsync();
        }
    }
}
