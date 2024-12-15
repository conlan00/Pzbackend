using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        [HttpPost("report")]
        public IActionResult ReportIssue()
        {
            return Ok(new { IssueId = Guid.NewGuid(), Status = "Reported" });
        }

        [HttpPost("create")]
        public IActionResult CreateIssue()
        {
            return Ok(new { Message = "Issue created and sent to support." });
        }
    }
}
