namespace SV21T1020484.DomainModels
{
    public class Supplier
    {
        public int SupplierID {  get; set; }
        public String SupplierName { get; set; }=string.Empty;
        public String ConTactName {  get; set; }=string.Empty;  
        public String Province {  get; set; }=string.Empty;
        public String Address {  get; set; }=string.Empty;
        public String Phone { get; set; } = string.Empty;
        public String Email { get; set; } = string.Empty;
    }
}
