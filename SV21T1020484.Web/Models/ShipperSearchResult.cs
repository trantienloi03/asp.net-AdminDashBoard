using SV21T1020484.DomainModels;

namespace SV21T1020484.Web.Models
{
    public class ShipperSearchResult: PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
