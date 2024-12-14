using SV21T1020484.DomainModels;

namespace SV21T1020484.Web.Models
{
    public class CategorySearchResult: PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
