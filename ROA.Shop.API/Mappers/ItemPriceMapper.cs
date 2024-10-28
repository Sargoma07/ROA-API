using AutoMapper;
using ROA.Infrastructure.Mapper;
using ROA.Shop.API.Domain;
using ROA.Shop.API.Models;

namespace ROA.Shop.API.Mappers;

public interface IItemPriceMapper : IMapper<ItemPrice, ItemPriceModel>;

internal class ItemPriceMapper : AbstractMapper<ItemPrice, ItemPriceModel>, IItemPriceMapper
{
    protected override AutoMapper.IMapper Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ItemPrice, ItemPriceModel>()
                .ForMember(m => m.Currency,
                    c => c.MapFrom(e =>
                        e.Details.SingleOrDefault() != null ? e.Details.SingleOrDefault()!.Currency : string.Empty))
                .ForMember(m => m.Price,
                    c => c.MapFrom(e =>
                        e.Details.SingleOrDefault() != null ? e.Details.SingleOrDefault()!.Price : 0))
                .ReverseMap();
        });


        return config.CreateMapper();
    }
}