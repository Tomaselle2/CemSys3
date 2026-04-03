using CemSys3.DTOs.PlantillaTramite;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;
using iText.Forms.Form.Element;
using Microsoft.AspNetCore.Mvc;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace CemSys3.Business.PlantillaTramite
{
    public class PlantillaTramiteService : IPlantillaTramite
    {
        private readonly AppDbContext _context;

        public PlantillaTramiteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Add(PlantillaTramiteDTO dto)
        {
            Models.PlantillasTramite plantilla = new Models.PlantillasTramite
            {
                TipoTramiteId = dto.TipoTramiteId,
                Nombre = dto.Nombre,
                Contenido = dto.Contenido,
                TipoEscenario = dto.TipoEscenario,
                Activo = true,
                FechaModificacion = DateTime.UtcNow
            };

            await _context.PlantillasTramites.AddAsync(plantilla);

            return await _context.SaveChangesAsync();
        }

        public async Task<PlantillaTramiteDTO> Get(int id)
        {
            Models.PlantillasTramite plantilla = await _context.PlantillasTramites.FindAsync(id) ?? throw new Exception("Plantilla de trámite no encontrada");

            PlantillaTramiteDTO dto = new PlantillaTramiteDTO
            {
                Id = plantilla.Id,
                TipoTramiteId = plantilla.TipoTramiteId,
                Nombre = plantilla.Nombre,
                Contenido = plantilla.Contenido,
                TipoEscenario = plantilla.TipoEscenario,
                Activo = plantilla.Activo,
                FechaModificacion = plantilla.FechaModificacion
            };
            return dto;
        }

        public async Task<int> Update(PlantillaTramiteDTO dto)
        {
            Models.PlantillasTramite plantilla = await _context.PlantillasTramites.FindAsync(dto.Id) ?? throw new Exception("Plantilla de trámite no encontrada");


            plantilla.TipoTramiteId = dto.TipoTramiteId;
            plantilla.Nombre = dto.Nombre;
            plantilla.Contenido = dto.Contenido;
            plantilla.TipoEscenario = dto.TipoEscenario;
            plantilla.Activo = true;
            plantilla.FechaModificacion = DateTime.UtcNow;
            

            _context.PlantillasTramites.Update(plantilla);

            return await _context.SaveChangesAsync();
        }



        //🧠 PASO 1: Elegir plantilla correcta
        //    var plantilla = await _context.PlantillasTramite
        //.Where(p => p.tipoTramiteId == tramite.TipoTramiteId
        //         && p.tipoEscenario == tipoCambio) // 🔥 importante
        //.FirstOrDefaultAsync();


        //PASO 2: Armar variables
        //        var valores = new Dictionary<string, string>
        //{
        //    { "TitularesActuales", titularesActuales },
        //    { "NuevosTitulares", nuevosTitulares },
        //    { "Parcela", parcela.Descripcion },
        //    { "Fecha", DateTime.Now.ToString("dd/MM/yyyy") }
        //};


        //PASO 3: Reemplazar
        //string htmlGenerado = GenerarDocumento(plantilla.contenido, valores);

        //🧠 PASO 4: Mandar a la vista
        //        var vm = new DocumentoEditableVM
        //        {
        //            TramiteId = tramite.Id,
        //            HtmlContenido = htmlGenerado
        //        };

        //return View(vm);

        //6. EL USUARIO EDITA
        //        <textarea name = "HtmlContenido" >
        //    @Html.Raw(Model.HtmlContenido)
        //</ textarea >

        //📄 7. GENERAR PDF
        //[HttpPost]
        //public IActionResult GenerarPdf(DocumentoEditableVM model)
        //{
        //    var pdf = _pdfService.GenerarDesdeHtml(model.HtmlContenido);

        //    return File(pdf, "application/pdf");
        //}

    }
}
