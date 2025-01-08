using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using URL_Shortener.Data.Models;

namespace URL_Shortener.Data.Services
{
    public sealed class URLService
    {

        public readonly string _connString;

        public URLService(IConfiguration config)
        { 
            _connString = config.GetConnectionString("database") ?? throw new NullReferenceException(nameof(config));
        }

        public async Task<IEnumerable<URL>> GetAll()
        {
            var selectStatement = "SELECT * FROM \"URL\"";

            using var connection = new NpgsqlConnection(_connString);
            connection.Open();

            var urls = await connection.QueryAsync<URL>(selectStatement);
            return urls;
        }

        public async Task<URL> ShortenURL(string originalUrl, string IPAddress)
        {
            using var connection = new NpgsqlConnection(_connString);

            var insertStatement = "INSERT INTO \"URL\" (\"IPAddress\", \"OriginalURL\", \"ShortenedURL\") VALUES (@IPAddress, @OriginalURL, @ShortenedURL) returning id;";

            var newShortnedUrl = new URL
            {
                IPAddress = IPAddress,
                OriginalURL = originalUrl,
                ShortenedURL = shortenedUrlGenerator()
            };

            var createdId = await connection.ExecuteScalarAsync(insertStatement, newShortnedUrl);

            newShortnedUrl.ShortenedURL += createdId.ToString();

            var updateStatement = "UPDATE \"URL\" SET \"ShortenedURL\" = @updatedShortenedUrl WHERE id = @id";

            var affected = await connection.ExecuteAsync(updateStatement, new { updatedShortenedUrl = newShortnedUrl.ShortenedURL, id = createdId });

            return newShortnedUrl;
        }

        public async Task<URL> GetURL(string shortenedUrl)
        {
            var urlId = Int32.Parse(shortenedUrl.Substring(8));
            var selectStatement = "SELECT * FROM \"URL\" WHERE \"ShortenedURL\" = @url AND id = @id";

            using var connection = new NpgsqlConnection(_connString);
            var url = await connection.QueryFirstAsync<URL>(selectStatement, new { url = shortenedUrl, id = urlId });

            return url;
        }


        private string shortenedUrlGenerator()
        {
            const string Charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return string.Create<object?>(8, null, static (chars, _) => Random.Shared.GetItems(Charset, chars));
        }

    }
}
