//-----------------------------------------------------------------------
// <copyright file="BboxToVector2dBoundsConverterTest.cs" company="Mapbox">
//     Copyright (c) 2017 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Mapbox.UnitTest {
	using HttpMock;
	using Mapbox.Json;
	using Mapbox.Utils;
	using Mapbox.Utils.JsonConverters;
	using NUnit.Framework;
	using Platform;
	using System;
	using System.Net;


	[TestFixture]
	internal class MockApiTest {


		private static readonly string _mockBaseUrl = "http://localhost:2345";
		private FileSource _fs = new FileSource();
		IHttpServer _mockApi;


		[TearDown]
		public void Finished() {
			if (null == _mockApi) { return; }
			_mockApi.Dispose();
			_mockApi = null;
		}


		[SetUp]
		public void SetupMockHttp() {

			//_mockApi = new HttpServer(new Uri(_mockBaseUrl));

			var serverFactory = new HttpServerFactory();
			_mockApi = serverFactory.Get(new Uri(_mockBaseUrl)).WithNewContext();

			_mockApi.Start();

			_mockApi.Stub(r => r.Get("/*"))
				.Return("{}")
				.AsContentType("application/json")
				.OK();

		}


		[Test]
		public void First() {

			Response resp = new Response();
			_fs.Request(
				_mockBaseUrl + "/bla/bla"
				, (Response r) => {
					// hm why doesn't nunit work in here? maybe blocking 'WaitForAllRequests'
					resp = r;
				}
			);

			_fs.WaitForAllRequests();

			System.Diagnostics.Debug.WriteLine(resp.StatusCode);
			Assert.AreEqual(200, resp.StatusCode, "wrong status code");
		}



	}
}