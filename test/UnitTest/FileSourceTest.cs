//-----------------------------------------------------------------------
// <copyright file="FileSourceTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest {


	using System;
	using Mapbox;
	using NUnit.Framework;


	[TestFixture]
	internal class FileSourceTest {


		private const string Uri = "https://api.mapbox.com/geocoding/v5/mapbox.places/helsinki.json";
		private Mono.FileSource fs;


		[SetUp]
		public void SetUp() {
			this.fs = new Mono.FileSource();
		}


		[Test]
		public void AccessTokenSet() {
			Assert.IsNotNull(
				Environment.GetEnvironmentVariable("MAPBOX_ACCESS_TOKEN"),
				"MAPBOX_ACCESS_TOKEN not set in the environment.");
		}


		[Test]
		public void Request() {
			bool requestFinished = false;
			this.fs.Request(
				Uri,
				(Response res) => {
					Assert.IsNotNull(res.Data, "No data received from the servers.");
					requestFinished = true;
				});
			while(!requestFinished) { System.Threading.Thread.Sleep(5); }
		}


		[Test]
		[Ignore("FileSource.Request.Cancel() is currently not implemented")]
		public void RequestCancel() {
			var request = this.fs.Request(
				Uri,
				(Response res) => {
					Assert.Fail("Should never happen.");
				});

			request.Cancel();
		}


		[Test]
		public void RequestDnsError() {
			bool requestFinished = false;
			this.fs.Request(
				"https://dnserror.shouldnotwork",
				(Response res) => {
					// Do no assume any error message. Mono != .NET.
					Assert.NotNull(res.Error);
					requestFinished = true;
				});
			while(!requestFinished) { System.Threading.Thread.Sleep(5); }
		}


		[Test]
		public void RequestForbidden() {
			// Mapbox servers will return a forbidden when attempting
			// to access a page outside the API space with a token
			// on the query. Let's hope the behaviour stay like this.
			bool requestFinished = false;
			this.fs.Request(
				"https://mapbox.com/forbidden",
				(Response res) => {
					Assert.AreEqual(res.Error, "Forbidden");
					requestFinished = true;
				});
			while(!requestFinished) { System.Threading.Thread.Sleep(5); }
		}


	}
}