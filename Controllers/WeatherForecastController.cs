using Microsoft.AspNetCore.Mvc;

namespace DimDim.OrdersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "ok", note = "Controller presente apenas para evitar erro de build." });
        }
    }
}
