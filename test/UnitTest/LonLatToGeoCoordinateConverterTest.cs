//-----------------------------------------------------------------------
// <copyright file="LonLatToGeoCoordinateConverterTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using Mapbox;
    using Mapbox.Json;
    using NUnit.Framework;

    [TestFixture]
    internal class LonLatToGeoCoordinateConverterTest
    {
        private string lonLatStr = "[-77.0295,38.9165]";
        private GeoCoordinate lonLatObj = new GeoCoordinate(longitude: -77.0295, latitude: 38.9165);

        [Test]
        public void Deserialize()
        {
            GeoCoordinate deserializedLonLat = JsonConvert.DeserializeObject<GeoCoordinate>(this.lonLatStr, JsonConverters.Converters);
            Assert.AreEqual(this.lonLatObj.ToString(), deserializedLonLat.ToString());
        }

        [Test]
        public void Serialize()
        {
            string serializedLonLat = JsonConvert.SerializeObject(this.lonLatObj, JsonConverters.Converters);
            Assert.AreEqual(this.lonLatStr, serializedLonLat);
        }
    }
}