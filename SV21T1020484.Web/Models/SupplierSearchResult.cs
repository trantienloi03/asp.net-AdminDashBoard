using SV21T1020484.DomainModels;

namespace SV21T1020484.Web.Models
{
    public class SupplierSearchResult: PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
