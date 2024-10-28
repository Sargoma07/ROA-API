using ROA.Infrastructure.Mapper;

namespace ROA.Payment.API.Mappers
{
    public interface IMapperFactory
    {
        T GetMapper<T>() where T : IMapper;
    }

    class MapperFactory(IServiceProvider container) : IMapperFactory
    {
        public static void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPaymentMapper, PaymentMapper>();
            serviceCollection.AddSingleton<IAccountMapper, AccountMapper>();
            
            // self-register
            serviceCollection.AddSingleton<IMapperFactory, MapperFactory>();
        }


        public T GetMapper<T>()
            where T : IMapper
        {
            return container.GetRequiredService<T>();
        }
    }
}