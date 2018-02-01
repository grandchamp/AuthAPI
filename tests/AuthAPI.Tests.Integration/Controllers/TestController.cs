using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Tests.Integration.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        [Authorize]
        public IActionResult Get() => Ok();
    }
}
