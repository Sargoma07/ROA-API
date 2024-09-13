using AutoMapper;
using ROA.Rest.API.Domain;
using ROA.Rest.API.Models;

namespace ROA.Rest.API.Mappers;

public interface IPlayerMapper : IMapper<Player, PlayerModel>;


internal class PlayerMapper : AbstractMapper<Player, PlayerModel>, IPlayerMapper
{
    protected override AutoMapper.IMapper Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Player, PlayerModel>()
                .ReverseMap()
                .ForMember(x => x.Id, y => y.Ignore());
        });


        return config.CreateMapper();
    }
}