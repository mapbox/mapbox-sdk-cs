//-----------------------------------------------------------------------
// <copyright file="Response.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Mapbox.Platform
{
	/// <summary> A response from a <see cref="IFileSource" /> request. </summary>
	public struct Response
	{
		/// <summary> Error description, set on error, empty otherwise. </summary>
		public string Error;

		/// <summary> Headers of the response. </summary>
		public Dictionary<string, string> Headers;

		/// <summary> Raw data fetched from the request. </summary>
		public byte[] Data;
	}
}