using EmployeeManagement.Data;
using EmployeeManagement.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using EmployeeManagement.Model;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private DataStore _employeeData;

        public EmployeeController(DataStore employeData) 
        {
            _employeeData = employeData;        
        }
        
        [HttpGet]
        [Authorize(Roles  = "Manager,SuperAdmin")]
        public IActionResult GetAllEmpolyees()
        {
            var userId = GetCurrentUserId();
            //Console.Write(userId);
            var employees = _employeeData.GetAll(userId);
            return Ok(employees);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDTO)
        {
            var employee = _employeeData.FindEmp(loginDTO.Name, loginDTO.Password);
            if (employee == null)
            {
                return Unauthorized("Invaid Username or Password");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name , employee.Name),
                new Claim(ClaimTypes.Role , employee.Role.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Ok("Logged In Successfully");
        }
        [HttpGet("id")]
        [Authorize]
        public IActionResult GetEmpById(int id) 
        {
            if(GetCurrentUserId() != id && !User.IsInRole(Roles.SuperAdmin.ToString()) && !User.IsInRole(Roles.Manager.ToString()))
            {
                return Forbid("Unauthorised to view details");
            }
            var employee = _employeeData.GetEmp(id);
            return Ok(employee);
        }
        [HttpPost("managers")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult AddManager([FromBody] ManagerDto managerDto) 
        {
            var Manager = new Employee
            {
                Name = managerDto.Name,
                Password = managerDto.Password,
                Role = Roles.Manager,
                ManagerId = 1001,
            };
            _employeeData.AddEmp(Manager);
            return Ok("Manager Added");
        }
        [HttpPost()]
        [Authorize(Roles = "Manager")]
        public IActionResult AddEmployee([FromBody] EmployeeDto empDto)
        {
            var Employee = new Employee
            {
                Name = empDto.Name,
                Password = empDto.Password,
                Role = Roles.Employee,
                ManagerId = GetCurrentUserId(),
            };
            _employeeData.AddEmp(Employee);
            return Ok("Employee Added");
        }
        [HttpPost("apply_leave")]
        [Authorize(Roles = "Employee")]
        public IActionResult ApplyLeave([FromBody] LeaveAppDto leaveDto)
        {
            var empId = GetCurrentUserId();
            var status = _employeeData.ApplyLeave(empId, leaveDto);
            if (!status)
            {
                return BadRequest("Failed to Apply Leave");
            }
            return Ok("Leave applied");
        }
        [HttpPost("update_leave_status")]
        [Authorize(Roles = "Manager")]
        public IActionResult UpdateLeaveStatus([FromBody] LeaveStatusDto lDto)
        {
            var mId = GetCurrentUserId();
            var status = _employeeData.LeaveStatusUpdate(mId, lDto.EmployeeId, lDto.LeaveId, lDto.Status);
            if (!status)
            {
                return BadRequest("Failed to change Leave Status");
            }
            return Ok("Leave Status changed");
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }


    }
}
