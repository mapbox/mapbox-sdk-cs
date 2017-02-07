//-----------------------------------------------------------------------
// <copyright file="FileSource.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Mono {
	using System;
	using System.Collections.Generic;
	using System.Threading;

	/// <summary>
	///     Mono implementation of the FileSource class. It will use Mono's
	///     <see href="http://www.mono-project.com/docs/advanced/runtime/">runtime</see> to
	///     asynchronously fetch data from the network via HTTP or HTTPS requests.
	/// </summary>
	/// <remarks>
	///     This implementation requires .NET 4.5 and later. The access token is expected to
	///     be exported to the environment as MAPBOX_ACCESS_TOKEN.
	/// </remarks>
	public sealed class FileSource : IFileSource {


		private readonly string accessToken = Environment.GetEnvironmentVariable("MAPBOX_ACCESS_TOKEN");


		/// <summary> Performs a request asynchronously. </summary>
		/// <param name="url"> The HTTP/HTTPS url. </param>
		/// <param name="callback"> Callback to be called after the request is completed. </param>
		/// <returns>
		///     Returns a <see cref="IAsyncRequest" /> that can be used for canceling a pending
		///     request. This handle can be completely ignored if there is no intention of ever
		///     canceling the request.
		/// </returns>
		public IAsyncRequest Request(string url, Action<Response> callback) {

			if(this.accessToken != null) {
				url += "?access_token=" + this.accessToken;
			}

			var request = new HTTPRequest(url, callback);
			return request;
		}


	}
}
