using Microsoft.AspNetCore.Mvc;

namespace DimDim.OrdersApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("WeatherForecastController ativo, mas não utilizado.");
        }
    }
}
