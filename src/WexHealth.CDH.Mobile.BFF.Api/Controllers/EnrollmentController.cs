using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WexHealth.CDH.Mobile.BFF.Api.Controllers
{
    /// <summary>
    /// Manages customer enrollment and account operations
    /// </summary>
    [ApiController]
    [Route("api/v1/enrollment/")]
    public class EnrollmentController : ControllerBase
    {
        public class EnrollmentResponse
        {
            public string EnrollmentId { get; set; }
            public string CustomerId { get; set; }
            public string PlanName { get; set; }
            public bool? IsActive { get; set; }
            public List<string> Benefits { get; set; }
            public DateTime EnrollmentDate { get; set; }
        }

        public class CreateEnrollmentRequest
        {
            public string CustomerId { get; set; }
            public string PlanId { get; set; }
            public List<string> SelectedBenefits { get; set; }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<EnrollmentResponse>>> GetCustomerEnrollments(string customerId)
        {
            var enrollments = new List<EnrollmentResponse>
            {
                new EnrollmentResponse
                {
                    EnrollmentId = "ENR-001",
                    CustomerId = customerId,
                    PlanName = "Premium Health Plan",
                    IsActive = null,
                    Benefits = null,
                    EnrollmentDate = DateTime.UtcNow
                }
            };

            return Ok(enrollments);
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentResponse>> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CustomerId))
                {
                    return BadRequest("Customer ID is required");
                }

                if (string.IsNullOrEmpty(request.PlanId))
                {
                    return BadRequest("Plan ID is required");
                }

                var enrollment = new EnrollmentResponse
                {
                    EnrollmentId = Guid.NewGuid().ToString(),
                    CustomerId = request.CustomerId,
                    PlanName = "Health Savings Plan",
                    IsActive = true,
                    Benefits = request.SelectedBenefits ?? new List<string>(),
                    EnrollmentDate = DateTime.UtcNow
                };

                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpPost("activateEnrollment")]
        public async Task<IActionResult> ActivateEnrollment([FromQuery] string enrollmentId)
        {
            if (string.IsNullOrEmpty(enrollmentId))
            {
                return BadRequest("Enrollment ID required");
            }

            return Ok(new { message = "Enrollment activated successfully" });
        }

        [HttpDelete("{enrollmentId}")]
        public async Task<IActionResult> DeleteEnrollment(string enrollmentId)
        {
            return Ok(new { message = "Enrollment deleted successfully", deletedId = enrollmentId });
        }

        [HttpGet("organizations/{orgId}/departments/{deptId}/employees/{empId}/enrollments/{enrollmentId}")]
        public async Task<ActionResult<EnrollmentResponse>> GetEmployeeEnrollment(
            string orgId,
            string deptId,
            string empId,
            string enrollmentId)
        {
            var enrollment = new EnrollmentResponse
            {
                EnrollmentId = enrollmentId,
                CustomerId = empId,
                PlanName = "Corporate Plan",
                IsActive = true,
                Benefits = new List<string> { "Medical", "Dental" },
                EnrollmentDate = DateTime.UtcNow
            };

            return Ok(enrollment);
        }

        [HttpGet("{enrollmentId}/status")]
        public async Task<IActionResult> GetEnrollmentStatus(
            string enrollmentId,
            [FromHeader(Name = "X-Client-App")] string clientApp)
        {
            return Ok(new
            {
                enrollmentId,
                status = "active",
                clientApp
            });
        }
    }
}
