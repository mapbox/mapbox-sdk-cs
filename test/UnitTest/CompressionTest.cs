//-----------------------------------------------------------------------
// <copyright file="CompressionTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest {
	using System.Text;
	using Mapbox.Utils;
	using NUnit.Framework;

	[TestFixture]
	internal class CompressionTest {
		[Test]
		public void Empty() {
			var buffer = new byte[] { };
			Assert.AreEqual(buffer, Compression.Decompress(buffer));
		}

		[Test]
		public void NotCompressed() {
			var buffer = Encoding.ASCII.GetBytes("foobar");
			Assert.AreEqual(buffer, Compression.Decompress(buffer));
		}

		[Test]
		public void Corrupt() {
			var fs = new Mono.FileSource();
			var buffer = new byte[] { };
			bool finished = false;
			// Vector tiles are compressed.
			fs.Request(
				"https://api.mapbox.com/v4/mapbox.mapbox-streets-v7/0/0/0.vector.pbf",
				(Response res) => {
					buffer = res.Data;
					Assert.NotNull(buffer, "tile data not null");
					Assert.Greater(buffer.Length, 30);

					buffer[10] = 0;
					buffer[20] = 0;
					buffer[30] = 0;

					Assert.AreEqual(buffer, Compression.Decompress(buffer));
					finished = true;
				});

			while(!finished) {
				System.Threading.Thread.Sleep(5);
			}
		}

		[Test]
		public void Decompress() {
			var fs = new Mono.FileSource();
			var buffer = new byte[] { };
			bool finished = false;

			// Vector tiles are compressed.
			fs.Request(
				"https://api.mapbox.com/v4/mapbox.mapbox-streets-v7/0/0/0.vector.pbf",
				(Response res) => {
					buffer = res.Data;
					Assert.Less(buffer.Length, Compression.Decompress(buffer).Length);
					finished = true;
				});

			while(!finished) { System.Threading.Thread.Sleep(5); }
		}
	}
}
