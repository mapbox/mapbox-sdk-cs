//-----------------------------------------------------------------------
// <copyright file="BboxToVector2dBoundsConverterTest.cs" company="Mapbox">
//     Copyright (c) 2017 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Mapbox.UnitTest {
	using Mapbox.Json;
	using Mapbox.Utils;
	using Mapbox.Utils.JsonConverters;
	using NUnit.Framework;
	using Platform;
	using RichardSzalay.MockHttp;
	using System.Net;


	[TestFixture]
	internal class MockApiTest {


		private static readonly string _mockBaseUrl="http://localhost:2345";
		private MockHttpMessageHandler _mockHttp = new MockHttpMessageHandler();
		private FileSource _fs = new FileSource();


		[SetUp]
		public void SetupMockHttp() {
			_mockHttp
				.When(_mockBaseUrl + "/*")
				.Respond(HttpStatusCode.OK, "application/json", "{}");
		}


		[Test]
		public void First() {
			_fs.Request(
				_mockBaseUrl + "/bla/bla"
				, (Response r)=> {
					System.Diagnostics.Debug.WriteLine(r.StatusCode);
				} 
			);

			_fs.WaitForAllRequests();
		}



	}
}