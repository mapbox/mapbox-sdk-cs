namespace Mapbox.Platform
{
	using System;

	public class MemoryCacheAsyncRequestHandler : AbstractAyncRequestHandler
	{
		protected internal override IAsyncRequest HandleHere(string uri, Action<Response> callback, int timeout = 10)
		{
			callback(Response.FromCache(_cachedResponses[uri]));
			return new MemoryCacheAsyncRequest();
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
				// HACK: This implementation doesn't need to cancel as it's instantaneous?
			}
		}
	}
}
