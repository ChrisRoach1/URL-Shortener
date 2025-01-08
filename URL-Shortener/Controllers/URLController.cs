using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URL_Shortener.Data.Models;
using URL_Shortener.Data.Services;

namespace URL_Shortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLController : ControllerBase
    {
        private readonly ILogger<URLController> _logger;
        private readonly URLService _urlService;

        public URLController(ILogger<URLController> logger, URLService service)
        {
            _logger = logger;
            _urlService = service;
        }

        [HttpGet(Name = "GetURLs")]
        public async Task<ActionResult<IEnumerable<URL>>> Get()
        {
            var urls = await _urlService.GetAll();

            return Ok(Url);
        }
    }
}
