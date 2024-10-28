using AutoMapper;
using ROA.Infrastructure.Mapper;
using ROA.Payment.API.Models;

namespace ROA.Payment.API.Mappers;

public interface IPaymentMapper : IMapper<Domain.Payment, PaymentModel>;

internal class PaymentMapper : AbstractMapper<Domain.Payment, PaymentModel>, IPaymentMapper
{
    protected override AutoMapper.IMapper Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Domain.Payment, PaymentModel>()
                .ReverseMap()
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.Status, y => y.Ignore())
                .ForMember(x => x.TotalDetails, y => y.Ignore());

            cfg.CreateMap<Domain.Payment.PaymentOrder, PaymentModel.OrderModel>()
                .ReverseMap();

            cfg.CreateMap<Domain.Payment.OrderLine, PaymentModel.OrderLineModel>()
                .ReverseMap()
                .ForMember(x => x.PricePerUnit, y => y.Ignore());

            cfg.CreateMap<Domain.Payment.PaymentTotalDetails, PaymentModel.TotalDetailsModel>()
                .ReverseMap();
        });


        return config.CreateMapper();
    }
}