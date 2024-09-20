using AutoMapper;
using ROA.Infrastructure.Mapper;
using ROA.Payment.API.Domain;
using ROA.Payment.API.Models;

namespace ROA.Payment.API.Mappers;

public interface IAccountMapper : IMapper<Account, AccountModel>;


internal class AccountMapper : AbstractMapper<Account, AccountModel>, IAccountMapper
{
    protected override AutoMapper.IMapper Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Account, AccountModel>();


            cfg.CreateMap<Account.BalanceData, AccountModel.BalanceDataModel>()
                .ReverseMap();
        });


        return config.CreateMapper();
    }
}