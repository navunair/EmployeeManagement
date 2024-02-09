namespace EmployeeManagement.Model
{
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
    
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set;}
        public int ManagerId { get; set; }
        public string Type { get; set; }
        public int Days { get; set; }
        public LeaveStatus Status { get; set; }
    }
}
