namespace Mapbox.Platform
{
	public interface IResponseCachingStrategy
	{
		bool ShouldCache(string key, Response response);
		void Cache(string key, Response response);
	}
}
