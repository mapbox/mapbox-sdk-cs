namespace Mapbox.Platform
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public class MemoryCachingAsyncRequestHandler : AbstractCachingAyncRequestHandler
	{

		protected struct CacheItem
		{
			public long Timestamp;
			public byte[] Data;
		}

		int _maxCacheSize;
		private object _lock = new object();
		protected Dictionary<string, CacheItem> _cachedResponses;

		// TODO: add support for disposal strategy (timestamp, distance, etc.)
		public MemoryCachingAsyncRequestHandler(int maxCacheSize)
		{
			_maxCacheSize = maxCacheSize;
			_cachedResponses = new Dictionary<string, CacheItem>();
		}

		protected override bool CanHandle(string url)
		{
			return _cachedResponses.ContainsKey(url);
		}

		protected override IAsyncRequest Handle(string uri, Action<Response> callback, int timeout = 10)
		{
			callback(Response.FromCache(_cachedResponses[uri].Data));
			return new MemoryCacheAsyncRequest();
		}

		public override bool ShouldCache(string key, Response response)
		{
			return !_cachedResponses.ContainsKey(key);
		}

		public override void Cache(string key, Response response)
		{
			lock (_lock)
			{
				if (_cachedResponses.Count >= _maxCacheSize)
				{
					_cachedResponses.Remove(_cachedResponses.OrderBy(c => c.Value.Timestamp).First().Key);
				}

				_cachedResponses.Add(key, new CacheItem() { Timestamp = DateTime.Now.Ticks, Data = response.Data });
			}
		}

		class MemoryCacheAsyncRequest : IAsyncRequest
		{
			public bool IsCompleted
			{
				get
				{
					return true;
				}
			}

			public void Cancel()
			{
				// Empty. We can't cancel an instantaneous response.
			}
		}
	}
}
