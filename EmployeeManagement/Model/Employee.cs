namespace EmployeeManagement.Model
{
    public enum Roles
    {
        SuperAdmin,
        Manager,
        Employee
    }
    public class Employee
    {
        
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ManagerId { get; set; }
        public string? Password { get; set; }

        public Roles Role { get; set; }

        public List<Leave> Leaves { get; set; }

    }
}
