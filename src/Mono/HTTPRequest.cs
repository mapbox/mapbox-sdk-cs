//-----------------------------------------------------------------------
// <copyright file="HTTPRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Mono {
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;

	internal sealed class HTTPRequest : IAsyncRequest {
		private static readonly HttpClient Client = new HttpClient();

		//private Task<Response> task;
		private Action<Response> callback;

		public HTTPRequest(string url, Action<Response> callback) {
			this.callback = callback;
			//this.task = this.DoRequestAsync(url);
			DoRequest(url);
		}

		public void Cancel() {
			// FIXME: CancellationTokenSource not available on Mono?
			// We should use it when it gets available.
			this.callback = null;
		}

		private async void DoRequest(string url) {
			var response = await DoRequestAsync(url);
			this.callback(response);
		}

		private async Task<Response> DoRequestAsync(string url) {
			var response = new Response();

			try {
				var message = await Client.GetAsync(url);

				if(message.IsSuccessStatusCode) {
					response.Data = await message.Content.ReadAsByteArrayAsync();
				} else {
					response.Error = message.StatusCode.ToString();
				}
			}
			catch(Exception exception) {
				response.Error = exception.Message;
			}

			return response;
		}
	}
}
