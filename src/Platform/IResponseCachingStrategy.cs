namespace Mapbox.Platform
{
	public interface IResponseCachingStrategy
	{
		bool ShouldCache(string key);
		void Cache(string key, Response response);
	}
}
