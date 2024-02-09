using EmployeeManagement.DTO;
using EmployeeManagement.Model;

namespace EmployeeManagement.Data
{
    public class DataStore
    {
        private readonly Dictionary<int, Employee> _emp;
        private int _empId = 1004;
        private int _leaveId = 0;
        public DataStore()
        {
            _emp = new Dictionary<int, Employee>();
            _emp.Add(1001, new Employee { Id = 1001, Name = "admin", Password = "admin", Role = Roles.SuperAdmin, ManagerId = 0, Leaves = new List<Leave>() });
            _emp.Add(1002, new Employee { Id = 1002, Name = "manager", Password = "manager", Role = Roles.Manager, ManagerId = 1001, Leaves = new List<Leave>() });
            _emp.Add(1003, new Employee { Id = 1003, Name = "employee", Password = "employee", Role = Roles.Employee, ManagerId = 1002, Leaves = new List<Leave>() });
        }

        public void AddEmp(Employee emp)
        {
            emp.Id = _empId++;
            emp.Leaves = new List<Leave>();
            _emp.Add(emp.Id,emp);
        }

        public Employee? GetEmp(int empId)
        {
            _emp.TryGetValue(empId, out Employee? emp);
            return emp;
        }
        public IEnumerable<Employee> GetAll(int mid)
        {
            return _emp.Values.Where(e => e.ManagerId == mid);
        }
        public Employee? FindEmp(string name, string pwd)
        {
            return _emp.Values.FirstOrDefault(e => e.Name == name && e.Password == pwd);
        }
        public bool ApplyLeave(int empId, LeaveAppDto leaveAppDto)
        {
            var employee = _emp[empId];
            var leaveRequest = new Leave
            {
                Id = ++_leaveId,
                EmployeeId = empId,
                Days = leaveAppDto.days,
                Type = leaveAppDto.type,
                Status = LeaveStatus.Pending,
            };
            employee.Leaves.Add(leaveRequest);
            return true;
        }
        public bool LeaveStatusUpdate(int mId, int empId, int leaveReqId, string status)
        {
            var employee = _emp[empId];
            var leaveRequest = employee.Leaves.Find(l => l.Id == leaveReqId);
            if(leaveRequest == null ) 
            {
                return false;
            }
            if(employee.ManagerId != mId) { return false; }
            if(status.ToLower() == "approved") { leaveRequest.Status = LeaveStatus.Approved; }
            else if(status.ToLower() == "rejected") { leaveRequest.Status = LeaveStatus.Rejected; }
            else return false;

            return true;
        }
    }
}
