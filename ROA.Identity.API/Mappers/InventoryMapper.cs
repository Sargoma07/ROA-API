using AutoMapper;
using ROA.Identity.API.Domain;
using ROA.Identity.API.Models;
using ROA.Infrastructure.Mapper;

namespace ROA.Identity.API.Mappers;

public interface IUserMapper : IMapper<User, SignUpModel>;

internal class UserMapper : AbstractMapper<User, SignUpModel>, IUserMapper
{
    protected override AutoMapper.IMapper Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SignUpModel, User>();
        });

        return config.CreateMapper();
    }
}