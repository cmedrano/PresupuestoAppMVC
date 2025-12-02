using AutoMapper;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeo inverso
            CreateMap<LoginResponseDto, User>().ReverseMap();

            // Rubro
            CreateMap<Rubro, RubroResponseDTO>()
                    // Mapear el ID del tipo de rubro
                    .ForMember(dest => dest.RubroTypeId, opt => opt.MapFrom(src => src.RubroTypeId))
                    .ForMember(dest => dest.tipoRubroNombre, opt => opt.MapFrom(src => src.tipoRubro.nombreRubro));
            CreateMap<CreateRubroViewRequest, Rubro>();
            CreateMap<UpdateRubroViewRequest, Rubro>();

            // Gasto
            CreateMap<Gasto, GastoResponseDto>()
                    .ForMember(dest => dest.RubroTypeNombre, opt => opt.MapFrom(src => src.RubroType.nombreRubro))
                    .ForMember(dest => dest.CuentaNombre, opt => opt.MapFrom(src => src.Cuenta.nombreCuenta));

            // Mapeo para Gastos
            CreateMap<CreateGastoViewRequest, Gasto>()
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Fecha, DateTimeKind.Utc)));

            CreateMap<UpdateGastoViewRequest, Gasto>()
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Fecha, DateTimeKind.Utc)));

            CreateMap<Cuenta, CuentaResponseDto>();

        }
    }
}
