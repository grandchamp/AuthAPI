using AuthAPI.Samples.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAPI.Samples.Mvc.Controllers
{
    [Route("test"), Authorize]
    public class TestController : Controller
    {
        public IActionResult Get() => Ok();

        [HttpPost]
        public IActionResult Post([FromBody]DummyModel model) => Ok();
    }
}
