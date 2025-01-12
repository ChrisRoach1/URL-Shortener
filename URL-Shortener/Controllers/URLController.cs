using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ActionResult<URL>> Post([FromBody]string originalUrl)
        {
            var url = await _urlService.ShortenURL(originalUrl, HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "");
            return Ok(url);
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
