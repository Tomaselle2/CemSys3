using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Seccion;
using CemSys3.Enumerables;
using CemSys3.Interfaces;
using CemSys3.Interfaces.Parcela;
using CemSys3.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace CemSys3.Business.Parcela
{
    public class ParcelaService : IParcela
    {
        private readonly AppDbContext _context;
        public ParcelaService(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(SeccionRequestDTO dto)
        {
            switch (dto.TipoParcelaId)
            {
                case 1: // Nicho
                    RegistrarNichos(dto);
                    break;
                case 2: //fosa
                    RegistrarFosas(dto);
                    break;
                case 3: //panteon
                    RegistrarPanteones(dto);
                    break;
            }
        }

        private void RegistrarNichos(SeccionRequestDTO dto)
        {
            int filas = dto.Filas;
            int columnas = dto.NroParcelas / filas;
            int nroNichoContador = 1;

            switch (dto.TipoNumeracionParcelaId)
            {
                case 1: //numeracion nueva
                    for (int i = 1; i <= filas; i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            Models.Parcela nicho = new Models.Parcela();
                            nicho.NroFila = i;
                            nicho.NroParcela = j;
                            nicho.Visibilidad = true;
                            nicho.CantidadDifuntos = 0;
                            nicho.TipoNichoId = (int)TipoNichoEnum.Feretro; //valor por defecto
                            nicho.SeccionId = dto.Id;
                            nicho.TipoParcelaId = dto.TipoParcelaId;
                            nicho.InformacionAdicional = string.Empty;
                            nicho.NombrePanteon = string.Empty;

                            _context.Parcelas.Add(nicho);
                        }
                    }
                    break;
                case 2: //numeracion antigua
                    for (int i = 1; i <= filas; i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            Models.Parcela nicho = new Models.Parcela();
                            nicho.NroFila = i;
                            nicho.NroParcela = nroNichoContador;
                            nicho.Visibilidad = true;
                            nicho.CantidadDifuntos = 0;
                            nicho.TipoNichoId = (int)TipoNichoEnum.Feretro; //valor por defecto
                            nicho.SeccionId = dto.Id;
                            nicho.TipoParcelaId = dto.TipoParcelaId;
                            nicho.InformacionAdicional = string.Empty;
                            nicho.NombrePanteon = string.Empty;

                            _context.Parcelas.Add(nicho);

                            nroNichoContador++; // Aumenta el contador después de cada nicho
                        }
                    }
                    break;
            }
        }

        private void RegistrarFosas(SeccionRequestDTO dto)
        {
            for (int i = 1; i <= dto.NroParcelas; i++)
            {
                Models.Parcela fosa = new Models.Parcela();
                fosa.NroParcela = i;
                fosa.SeccionId = dto.Id;
                fosa.Visibilidad = true;
                fosa.CantidadDifuntos = 0;
                fosa.NroFila = dto.Filas;
                fosa.TipoParcelaId = dto.TipoParcelaId;
                fosa.InformacionAdicional = string.Empty;
                fosa.NombrePanteon = string.Empty;

                _context.Parcelas.Add(fosa);
            }
        }

        private void RegistrarPanteones(SeccionRequestDTO dto)
        {
            for (int i = 1; i <= dto.NroParcelas; i++)
            {
                Models.Parcela panteon = new Models.Parcela();
                panteon.NroParcela = i;
                panteon.SeccionId = dto.Id;
                panteon.Visibilidad = true;
                panteon.CantidadDifuntos = 0;
                panteon.NroFila = dto.Filas;
                panteon.TipoPanteonId = (int)TipoPanteonEnum.ConNichos;
                panteon.TipoParcelaId = dto.TipoParcelaId;
                panteon.InformacionAdicional = string.Empty;
                panteon.NombrePanteon = string.Empty;

                _context.Parcelas.Add(panteon);
            }
        }

    }
}
