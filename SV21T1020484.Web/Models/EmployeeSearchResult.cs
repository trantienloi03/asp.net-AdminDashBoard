using SV21T1020484.DomainModels;

namespace SV21T1020484.Web.Models
{
    public class EmployeeSearchResult: PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
