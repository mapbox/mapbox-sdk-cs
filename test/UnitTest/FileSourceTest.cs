//-----------------------------------------------------------------------
// <copyright file="FileSourceTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest {
	using System;
	using Mapbox.Platform;
	using NUnit.Framework;

	[TestFixture]
	internal class FileSourceTest {
		private const string Uri = "https://api.mapbox.com/geocoding/v5/mapbox.places/helsinki.json";
		private FileSource fs;

		[SetUp]
		public void SetUp() {
			this.fs = new FileSource();
		}

		[Test]
		public void AccessTokenSet() {
			Assert.IsNotNull(
				Environment.GetEnvironmentVariable("MAPBOX_ACCESS_TOKEN"),
				"MAPBOX_ACCESS_TOKEN not set in the environment.");
		}

		[Test]
		public void Request() {
			this.fs.Request(
				Uri,
				(Response res) => {
					Assert.IsNotNull(res.Data, "No data received from the servers.");
				});

			this.fs.WaitForAllRequests();
		}

		[Test]
		public void MultipleRequests() {
			int count = 0;

			this.fs.Request(Uri, (Response res) => ++count);
			this.fs.Request(Uri, (Response res) => ++count);
			this.fs.Request(Uri, (Response res) => ++count);

			this.fs.WaitForAllRequests();

			Assert.AreEqual(count, 3, "Should have received 3 replies.");
		}

		[Test]
		public void RequestCancel() {
			var request = this.fs.Request(
				Uri,
				(Response res) => {
					Assert.Fail("Should never happen.");
				});

			request.Cancel();

			this.fs.WaitForAllRequests();
		}

		[Test]
		public void RequestDnsError() {
			this.fs.Request(
				"https://dnserror.shouldnotwork",
				(Response res) => {
					Assert.IsTrue(res.HasError);
				});

			this.fs.WaitForAllRequests();
		}

		[Test]
		public void RequestForbidden() {
			// Mapbox servers will return a forbidden when attempting
			// to access a page outside the API space with a token
			// on the query. Let's hope the behaviour stay like this.
			this.fs.Request(
				"https://mapbox.com/forbidden",
				(Response res) => {
					Assert.IsTrue(res.HasError);
				});

			this.fs.WaitForAllRequests();
		}

		[Test]
		public void WaitWithNoRequests() {
			// This should simply not block.
			this.fs.WaitForAllRequests();
		}
	}
}