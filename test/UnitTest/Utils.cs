//-----------------------------------------------------------------------
// <copyright file="Utils.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest {


	using System;
	using System.Collections.Generic;


	internal static class Utils {


		internal class MockFileSource : IFileSource {


			private Dictionary<string, Response> responses = new Dictionary<string, Response>();
			private List<MockRequest> requests = new List<MockRequest>();


			public IAsyncRequest Request(string uri, Action<Response> callback) {
				var response = new Response();
				if(this.responses.ContainsKey(uri)) {
					response = this.responses[uri];
				}

				var request = new MockRequest(response, callback);
				this.requests.Add(request);

				return request;
			}


			public void SetReponse(string uri, Response response) {
				this.responses[uri] = response;
			}


			public class MockRequest : IAsyncRequest {
				private Action<Response> callback;

				public MockRequest(Response response, Action<Response> callback) {
					this.callback = callback;
					callback(response);
				}


				public void Cancel() {
					this.callback = null;
				}
			}
		}


	}
}
