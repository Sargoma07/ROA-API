using System;
namespace ROA.Utilities.Models
{
    public class PagedList<T> 
    {
        public PagedList()
        {
        }

        public PagedList(IEnumerable<T> items, int totalItems, int totalPages, int currentPage)
        {
            Items = new List<T>(items);
            TotalItems = totalItems;
            TotalPages = totalPages;
            CurrentPage = currentPage;
        }

        public List<T> Items { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}

