namespace Mapbox.Platform
{
	using System;
	using System.Collections.Generic;

	public class MemoryCachingAsyncRequestHandler : AbstractCachingAyncRequestHandler
	{
		int _maxCacheSize;
		protected Dictionary<string, byte[]> _cachedResponses;

		// TODO: add support for disposal strategy (timestamp, distance, etc.)
		public MemoryCachingAsyncRequestHandler(int maxCacheSize)
		{
			_maxCacheSize = maxCacheSize;
			_cachedResponses = new Dictionary<string, byte[]>();
		}

		protected override bool CanHandle(string url)
		{
			return _cachedResponses.ContainsKey(url);
		}

		protected override IAsyncRequest Handle(string uri, Action<Response> callback, int timeout = 10)
		{
			callback(Response.FromCache(_cachedResponses[uri]));
			return new MemoryCacheAsyncRequest();
		}

		public override bool ShouldCache(string key, Response response)
		{
			return !_cachedResponses.ContainsKey(key);
		}

		public override void Cache(string key, Response response)
		{
			if (_cachedResponses.Count >= _maxCacheSize)
			{
				// TODO: fully implement. If the cache is full, we need to dispose older items.
				return;
			}

			_cachedResponses.Add(key, response.Data);
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
