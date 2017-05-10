namespace Mapbox.Platform
{
	using System;
	using System.Collections.Generic;

	public abstract class AbstractAyncRequestHandler : IAsyncRequestHandler
	{
		protected Dictionary<string, byte[]> _cachedResponses;

		IAsyncRequestHandler _nextHandler;
		public virtual IAsyncRequestHandler NextHandler
		{
			set
			{
				_nextHandler = value;
			}
		}

		public virtual void CacheResponse(string request, Response response)
		{
			if (_cachedResponses.ContainsKey(request))
			{
				return;
			}

			_cachedResponses.Add(request, response.Data);
		}

		public virtual IAsyncRequest Request(string uri, Action<Response> callback, int timeout = 10)
		{
			if (_cachedResponses.ContainsKey(uri))
			{
				return HandleHere(uri, callback, timeout);
			}

			return _nextHandler.Request(uri, callback, timeout);
		}

		protected internal abstract IAsyncRequest HandleHere(string uri, Action<Response> callback, int timeout = 10);
	}
}
