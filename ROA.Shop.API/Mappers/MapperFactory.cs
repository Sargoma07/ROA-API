using ROA.Infrastructure.Mapper;

namespace ROA.Shop.API.Mappers
{
    public interface IMapperFactory
    {
        T GetMapper<T>() where T : IMapper;
    }

    class MapperFactory(IServiceProvider container) : IMapperFactory
    {
        public static void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IItemPriceMapper, ItemPriceMapper>();
            
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