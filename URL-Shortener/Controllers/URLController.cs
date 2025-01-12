using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json.Linq;
using URL_Shortener.Data.Models;
using URL_Shortener.Data.Services;
using ZiggyCreatures.Caching.Fusion;

namespace URL_Shortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLController : ControllerBase
    {
        private readonly ILogger<URLController> _logger;
        private readonly URLService _urlService;
        private readonly IFusionCache _fusionCache;
        public URLController(ILogger<URLController> logger, URLService service, IFusionCache cache)
        {
            _logger = logger;
            _urlService = service;
            _fusionCache = cache;
        }

        [HttpGet(Name = "GetURLs")]
        public async Task<ActionResult<IEnumerable<URL>>> Get()
        {
            var urls = await _urlService.GetAll();

            return Ok(urls);
        }

        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Post([FromBody]string originalUrl)
        {
            try
            {
                var url = await _urlService.ShortenURL(originalUrl, HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "");

                return Ok(url);

            }catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{shortenedUrl}")]
        public async Task<ActionResult<URL>> Get(string shortenedUrl)
        {
            var cacheKey = $"URL_{shortenedUrl}";

            var cachedUrl = await _fusionCache.GetOrSetAsync(cacheKey, async token =>
            {
                var url = await _urlService.GetURL(shortenedUrl);
                return url;
            });

            if (cachedUrl != null)
            {
                return Ok(cachedUrl);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
