namespace Core.Interfaces;

public interface IResponseCacheService
{
  // This method will cache the response for a certain time period (timeToLive)
  Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
  Task<string?> GetCachedResponseAsync(string cacheKey);

  // This will allow us to remove items from the cache according to a certain pattern
  Task RemoveCacheByPattern(string pattern);
}
