using AutoMapper;
using ROA.Infrastructure.Mapper;
using ROA.Inventory.API.Domain;
using ROA.Inventory.API.Models;

namespace ROA.Inventory.API.Mappers;

public interface IInventoryMapper : IMapper<Domain.Inventory, InventoryModel>;

internal class InventoryMapper : AbstractMapper<Domain.Inventory, InventoryModel>, IInventoryMapper
{
    protected override AutoMapper.IMapper Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Domain.Inventory, InventoryModel>()
                .ReverseMap()
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.Type, y => y.Ignore());

            cfg.CreateMap<Domain.Inventory.InventoryItemSlot, InventoryModel.InventoryItemSlotModel>()
                .ReverseMap();

            cfg.CreateMap<Domain.Inventory.InventoryItem, InventoryModel.InventoryItemModel>()
                .ReverseMap();
        });

        return config.CreateMapper();
    }
}