using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo
{
    [ApiController]
    [Route("[controller]")]
    public class BusinessController : Controller
    {
        
        [Triggerable(nameof(SendEmails))]
        [Cron("*/1 * * * *")]
        public IActionResult SendEmails()
        {
            return Ok("sending email...");
        }
    }
}