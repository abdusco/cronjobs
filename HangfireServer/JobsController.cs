using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HangfireServer
{
    [ApiController]
    [Route("/api/[controller]")]
    public class JobsController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> RegisterJobs()
        {
            return Ok();
        }
    }
}