using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json.Linq;
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

            return Ok(urls);
        }

        [EnableRateLimiting("fixed")]
        public async Task<Result<URL>> Post([FromBody]JObject originalUrl)
        {
            string originalUrlString = originalUrl["originalUrl"].ToString();
            try
            {
                var url = await _urlService.ShortenURL(originalUrlString, HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "");

                if (url == null)
                {
                    return Result.Fail<URL>("failed to create shortened URL");
                }

                return Result.Ok<URL>(url);

            }catch(Exception ex)
            {
                return Result.Fail(new Error("unknown error occurred").CausedBy(ex));
            }

        }

        [HttpGet("{shortenedUrl}")]
        public async Task<IActionResult> Get(string shortenedUrl)
        {
            var url = await _urlService.GetURL(shortenedUrl);

            if(url == null)
            {
                return NotFound();
            }
            else
            {
                return StatusCode(302, url.OriginalURL);
            } 
        }
    }
}
