

namespace SV21T1020484.DomainModels
{
    public class Employee
    {
        public int EmployeeID {  get; set; }
        public String FullName {  get; set; } =string.Empty;
        public DateTime BirthDate { get; set; }
        public String Address { get; set; } = string.Empty ;
        public String Phone {  get; set; } = string.Empty;
        public String Email { get; set; } = String.Empty;
        public String PassWord { get; set; } = string.Empty;
        public String Photo { get; set; } = string.Empty;
        public bool IsWorking { get; set; }
        public string Role { get; set; } = string.Empty;

    }
}
