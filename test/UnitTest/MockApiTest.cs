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

		private struct _testUrl {
			public static string testMockServer1 = "/testmock1";
			public static string testMockServer2 = "/testmock2";
			public static string rateLimitHit = "/ratelimithit";
		}

		[OneTimeTearDown]
		public void Finished() {
			if (null == _mockApi) { return; }
			_mockApi.Dispose();
			_mockApi = null;
		}


		[OneTimeSetUp]
		public void SetupMockHttp() {

			var serverFactory = new HttpServerFactory();
			_mockApi = serverFactory.Get(new Uri(_mockBaseUrl)).WithNewContext();

			_mockApi.Start();

			_mockApi.Stub(r => r.Get(_testUrl.testMockServer1))
				.Return(@"{""name"":""first test""}")
				.AsContentType("application/json")
				.OK();

			// test status code ('Unavailable For Legal Reasons') not available in .NET HttpStatusCode enum
			_mockApi.Stub(r => r.Get(_testUrl.testMockServer2)).WithStatus((HttpStatusCode)451);

			_mockApi.Stub(r => r.Get(_testUrl.rateLimitHit))
				.Return(string.Empty)
				.WithStatus((HttpStatusCode)429);

		}


		[Test]
		public void TestMockServer() {

			Response resp = new Response();
			_fs.Request(
				_mockBaseUrl + _testUrl.testMockServer1
				, (Response r) => {
					// hm why doesn't nunit work in here? maybe blocking 'WaitForAllRequests'
					resp = r;
				}
			);

			_fs.WaitForAllRequests();

			Assert.IsTrue(resp.StatusCode.HasValue, "mock api did not set status code");
			Assert.AreEqual(200, resp.StatusCode, "mock api returned wrong status code");
			Assert.AreEqual("application/json", resp.ContentType, "mock api didn't set correct content type");
			Assert.AreEqual(@"{""name"":""first test""}", System.Text.Encoding.UTF8.GetString(resp.Data), "mock api returned wrong response");


			_fs.Request(
				_mockBaseUrl + _testUrl.testMockServer2
				, (Response r) => {
					// hm why doesn't nunit work in here? maybe blocking 'WaitForAllRequests'
					resp = r;
				}
			);

			_fs.WaitForAllRequests();

			Assert.IsTrue(resp.StatusCode.HasValue, "mock api did not set status code");
			Assert.AreEqual(451, resp.StatusCode, "mock api returned wrong status code");

		}


		[Test]
		public void RateLimitHit() {

			Response resp = new Response();
			_fs.Request(
				_mockBaseUrl + _testUrl.rateLimitHit
				, (Response r) => {
					// hm why doesn't nunit work in here? maybe blocking 'WaitForAllRequests'
					resp = r;
				}
			);

			_fs.WaitForAllRequests();

			Assert.IsTrue(resp.StatusCode.HasValue, "mock api did not set status code");
			Assert.AreEqual(429, resp.StatusCode, "rate limit status code not set");
		}


	}
}