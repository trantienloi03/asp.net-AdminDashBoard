namespace SV21T1020484.Web.Models
{
    public class PaginationSearchInput
    {
        public int Page { get; set; } =1;
        public int PageSize { get; set; }
        public String SearchValue { get; set; }
    }
}
