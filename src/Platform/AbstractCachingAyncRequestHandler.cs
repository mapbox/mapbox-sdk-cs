namespace Mapbox.Platform
{
	using System;

	public abstract class AbstractCachingAyncRequestHandler : IAsyncRequestHandler, IResponseCachingStrategy
	{
		IAsyncRequestHandler _nextHandler;
		public virtual IAsyncRequestHandler NextHandler
		{
			set
			{
				_nextHandler = value;
			}
		}

		public IAsyncRequest Request(string url, Action<Response> callback, int timeout = 10)
		{
			if (CanHandle(url))
			{
				return Handle(url, callback, timeout);
			}

			return _nextHandler.Request(url, (response) =>
			 {
				 if (ShouldCache(url))
				 {
					 Cache(url, response);
				 }
				 callback(response);
			 }, timeout);
		}

		protected abstract bool CanHandle(string url);

		protected abstract IAsyncRequest Handle(string uri, Action<Response> callback, int timeout = 10);

		public abstract bool ShouldCache(string key);

		public abstract void Cache(string key, Response response);
	}
}
