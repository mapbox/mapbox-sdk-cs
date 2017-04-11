//-----------------------------------------------------------------------
// <copyright file="HTTPRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Mono {
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Mapbox.Platform;

	internal sealed class HTTPRequest : IAsyncRequest {
		private static readonly HttpClient Client = new HttpClient();

		private Task<Response> _task;
		private Action<Response> _callback;

		public HTTPRequest(string url, Action<Response> callback) {
			_callback = callback;
			_task = DoRequestAsync(url);
		}

		public void Cancel() {
			// FIXME: CancellationTokenSource not available on Mono?
			// We should use it when it gets available.
			_callback = null;
		}

		public bool Wait() {
			if (_task.IsCompleted) {
				if (_callback != null) {
					_callback(_task.Result);
					_callback = null;
				}
			}

			return _callback == null;
		}

		private async Task<Response> DoRequestAsync(string url) {
			var response = new Response();

			try {
				var message = await Client.GetAsync(url);

				if (message.IsSuccessStatusCode) {
					response.Data = await message.Content.ReadAsByteArrayAsync();
				} else {
					response.Error = message.StatusCode.ToString();
				}
			}
			catch (Exception exception) {
				response.Error = exception.Message;
			}

			return response;
		}
	}
}
