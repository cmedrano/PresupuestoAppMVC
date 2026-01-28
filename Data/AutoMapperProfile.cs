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
            CreateMap<Budget, BudgetResponseDTO>()
                    // Mapear el ID del tipo de rubro
                    .ForMember(dest => dest.RubroTypeId, opt => opt.MapFrom(src => src.RubroTypeId))
                    .ForMember(dest => dest.tipoRubroNombre, opt => opt.MapFrom(src => src.tipoRubro.nombreRubro));
            CreateMap<CreateBudgetViewRequest, Budget>();
            CreateMap<UpdateBudgetViewRequest, Budget>();

            // Gasto
            CreateMap<Gasto, GastoResponseDto>()
                    .ForMember(dest => dest.RubroTypeNombre, opt => opt.MapFrom(src => src.RubroType.nombreRubro))
                    .ForMember(dest => dest.CuentaNombre, opt => opt.MapFrom(src => src.Cuenta.nombreCuenta))
                    .ForMember(d => d.Tipo, o => o.MapFrom(_ => "Gasto"));

            // Diario
            CreateMap<Diary, DiaryResponseDto>()
                    .ForMember(dest => dest.RubroTypeNombre, opt => opt.MapFrom(src => src.RubroType.nombreRubro))
                    .ForMember(dest => dest.CuentaNombre, opt => opt.MapFrom(src => src.Cuenta.nombreCuenta));

            // Mapeo para Gastos
            CreateMap<CreateGastoViewRequest, Gasto>()
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Fecha, DateTimeKind.Utc)));

            CreateMap<UpdateGastoViewRequest, Gasto>()
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Fecha, DateTimeKind.Utc)));

            CreateMap<Cuenta, CuentaResponseDto>();

            CreateMap<Income, GastoResponseDto>()
                .ForMember(dest => dest.RubroTypeId, opt => opt.MapFrom(_ => 34)) // rubro fijo
                .ForMember(d => d.Tipo, o => o.MapFrom(_ => "Ingreso"))
                .ForMember(dest => dest.CuentaNombre, opt => opt.MapFrom(src => src.Cuenta.nombreCuenta))
                .ForMember(dest => dest.Monto, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Nota, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.Date));

        }
    }
}
