using System.Collections;
using ROA.Utilities.Models;

namespace ROA.Rest.API.Mappers;

public interface IMapper
{

}

public interface IMapper<TEntity, TDto> : IMapper
    where TEntity : class
    where TDto : class
{
    IEnumerable<TEntity> MapCollectionFromDto<TItem>(IEnumerable<TDto> model, Action<TDto, TItem>? extra = null, TItem? destination = null) where TItem : class, TEntity;
    IEnumerable<TDto> MapCollectionToDto(IEnumerable<TEntity> source, Action<TEntity, TDto>? extra = null);

    IEnumerable<TDto> MapCollectionToDto(IEnumerable source, Action<TEntity, TDto>? extra = null);
    IEnumerable<TItem> MapCollectionToDto<TItem>(IEnumerable<TEntity> source, Action<TEntity, TItem>? extra = null) where TItem : class, TDto;
    PagedList<TDto> MapPagedListToDto(PagedList<TEntity> source, Action<TEntity, TDto>? extra = null);
    PagedCursorList<TDto> MapPagedCursorListToDto(PagedCursorList<TEntity> source, Action<TEntity, TDto>? extra = null);
    PagedSearchCursorList<TDto> MapPagedSearchCursorListToDto(PagedSearchCursorList<TEntity> source, Action<TEntity, TDto>? extra = null);
    TEntity MapFromDto(TDto model, Action<TDto, TEntity>? extra = null, TEntity? destination = null);
    TEntity MapFromDto<TItem>(TDto model, Action<TDto, TItem>? extra = null, TItem? destination = null) where TItem : class, TEntity;
    TDto MapToDto(TEntity source, Action<TEntity, TDto>? extra = null);
    TItem MapToDto<TItem>(TEntity source, Action<TEntity, TItem>? extra = null, TItem? destination = null) where TItem : class, TDto;
}

