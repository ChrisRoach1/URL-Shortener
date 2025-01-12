using System.Text.Json;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using URL_Shortener.Data.Models;

namespace URL_Shortener.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly HttpClient _httpClient;

    public List<URL>? URLs { get; set; } = new();

    [TempData]
    public string ShortenedURLResult { get; set; } = string.Empty;

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _httpClient = clientFactory.CreateClient("api");
    }

    public async Task OnGet(string shortenedUrl)
    {
         if (!String.IsNullOrEmpty(shortenedUrl))
         {
             var originalUrlResponse = await _httpClient.GetAsync($"api/URL/{shortenedUrl}");

             if (originalUrlResponse.IsSuccessStatusCode)
             {
                 var url = await originalUrlResponse.Content.ReadFromJsonAsync<URL>();

                 Response.Redirect(url.OriginalURL);
             }
             else
             {
                 Response.Redirect("/");
             }
         }
    }

    public async Task<IActionResult> OnPost(string originalUrl)
    {
        var returnUrl = await _httpClient.PostAsJsonAsync("api/URL/", originalUrl);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var url = await returnUrl.Content.ReadFromJsonAsync<URL>(options);

        ShortenedURLResult = HttpContext.Request.GetDisplayUrl() + url.ShortenedURL;
        TempData["Response"] = ShortenedURLResult;
        return RedirectToPage("Response");
    }

}