using Microsoft.AspNetCore.Mvc;

namespace RestService.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {

        [HttpGet]
        public string Index()
        {
            return "Service is up and running";
        }
    }
}
