using ROA.Infrastructure.Mapper;

namespace ROA.Rest.API.Mappers
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
            serviceCollection.AddSingleton<IPlayerMapper, PlayerMapper>();

            
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