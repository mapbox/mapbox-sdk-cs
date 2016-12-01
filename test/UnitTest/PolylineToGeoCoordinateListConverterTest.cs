//-----------------------------------------------------------------------
// <copyright file="PolylineToGeoCoordinateListConverterTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using System.Collections.Generic;
    using Mapbox;
    using Mapbox.Json;
    using NUnit.Framework;

    [TestFixture]
    internal class PolylineToGeoCoordinateListConverterTest
    {
        // (38.5, -120.2), (40.7, -120.95), (43.252, -126.453)
        private readonly List<GeoCoordinate> polyLineObj = new List<GeoCoordinate>()
        {
            new GeoCoordinate(38.5, -120.2),
            new GeoCoordinate(40.7, -120.95),
            new GeoCoordinate(43.252, -126.453)
        };

        private string polyLineString = "\"_p~iF~ps|U_ulLnnqC_mqNvxq`@\"";

        [Test]
        public void Deserialize()
        {
            List<GeoCoordinate> deserializedLine = JsonConvert.DeserializeObject<List<GeoCoordinate>>(this.polyLineString, JsonConverters.Converters);
            Assert.AreEqual(this.polyLineObj, deserializedLine);
        }

        [Test]
        public void Serialize()
        {
            string serializedLine = JsonConvert.SerializeObject(this.polyLineObj, JsonConverters.Converters);
            Assert.AreEqual(this.polyLineString, serializedLine);
        }
    }
}