using System.Text.Json;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class ResponseCacheService(IConnectionMultiplexer redis) : IResponseCacheService
{
  // We use redis database no. 0 which is the default. We will use db1 for caching purposes
  private readonly IDatabase _database = redis.GetDatabase(1);
  public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
  {
    // We use the options in order to get the C# object from redis and use the JSON serializer to convert it to a string.
    // Because it is JSON the standard format is camel case
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    var serializedResponse = JsonSerializer.Serialize(response, options); // This converts the response object to a string
    // The chacheKey is used to hold the given string in the serializedResponse for the specified amount of time before redis will delete it from the cache db
    await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
  }

  public async Task<string?> GetCachedResponseAsync(string cacheKey)
  {
    var cachedResponse = await _database.StringGetAsync(cacheKey);
    if (cachedResponse.IsNullOrEmpty) return null;
    return cachedResponse;
  }

  public async Task RemoveCacheByPattern(string pattern)
  {
    var server = redis.GetServer(redis.GetEndPoints().First());
    var keys = server.Keys(database: 1, pattern: $"*{pattern}*").ToArray();
    if (keys.Length != 0)
    {
      await _database.KeyDeleteAsync(keys);
    }
  }
}
