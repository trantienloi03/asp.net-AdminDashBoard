using SV21T1020484.DomainModels;

namespace SV21T1020484.Web.Models
{
    public class CustomerSearchResult: PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}
