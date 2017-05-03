//-----------------------------------------------------------------------
// <copyright file="Response.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mapbox.Platform {


	/// <summary> A response from a <see cref="IFileSource" /> request. </summary>
	public struct Response {

		public IAsyncRequest Request;

		public bool RateLimitHit {
			get { return StatusCode.HasValue ? 429 == StatusCode.Value : false; }
		}


		/// <summary>Flag to indicate if the request was successful</summary>
		public bool HasError {
			get {
				return _exceptions == null ? false : _exceptions.Count > 0;
			}
		}

		public int? StatusCode;


		public string ContentType;


		/// <summary>Length of rate-limiting interval in seconds. https://www.mapbox.com/api-documentation/#rate-limits </summary>
		public int? XRateLimitInterval;

		/// <summary>Maximum number of requests you may make in the current interval before reaching the limit. https://www.mapbox.com/api-documentation/#rate-limits </summary>
		public long? XRateLimitLimit;

		/// <summary>Timestamp of when the current interval will end and the ratelimit counter is reset. https://www.mapbox.com/api-documentation/#rate-limits </summary>
		public DateTime? XRateLimitReset;

		private List<Exception> _exceptions;
		/// <summary> Exceptions that might have occured during the request. </summary>
		public ReadOnlyCollection<Exception> Exceptions {
			get { return null == _exceptions ? null : _exceptions.AsReadOnly(); }
		}


		/// <summary> Headers of the response. </summary>
		public Dictionary<string, string> Headers;


		/// <summary> Raw data fetched from the request. </summary>
		public byte[] Data;

		public void AddException(Exception ex) {
			if (null == _exceptions) { _exceptions = new List<Exception>(); }
			_exceptions.Add(ex);
		}


	}
}