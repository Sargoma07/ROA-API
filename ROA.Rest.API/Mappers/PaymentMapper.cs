using AutoMapper;
using ROA.Model;
using ROA.Rest.API.Models;

namespace ROA.Rest.API.Mappers;

public interface IPaymentMapper : IMapper<Payment, PaymentModel>;

internal class PaymentMapper : AbstractMapper<Payment, PaymentModel>, IPaymentMapper
{
    protected override AutoMapper.IMapper Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Payment, PaymentModel>()
                .ReverseMap()
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.Status, y => y.Ignore())
                .ForMember(x => x.TotalDetails, y => y.Ignore());

            cfg.CreateMap<Payment.PaymentOrder, PaymentModel.OrderModel>()
                .ReverseMap();

            cfg.CreateMap<Payment.OrderLine, PaymentModel.OrderLineModel>()
                .ReverseMap()
                .ForMember(x => x.PricePerUnit, y => y.Ignore());

            cfg.CreateMap<Payment.PaymentTotalDetails, PaymentModel.TotalDetailsModel>()
                .ReverseMap();
        });


        return config.CreateMapper();
    }
}