//-----------------------------------------------------------------------
// <copyright file="BboxToGeoCoordinateBoundsConverterTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using Mapbox.Json;
    using Mapbox.Utils;
    using Mapbox.Utils.JsonConverters;
    using NUnit.Framework;

    [TestFixture]
    internal class BboxToGeoCoordinateBoundsConverterTest
    {
        private string geoCoordinateBoundsStr = "[38.9165,-77.0295,30.2211,-80.5521]";

        private GeoCoordinateBounds geoCoordinateBoundsObj = new GeoCoordinateBounds(
            sw: new GeoCoordinate(longitude: -77.0295, latitude: 38.9165),
            ne: new GeoCoordinate(longitude: -80.5521, latitude: 30.2211));

        [Test]
        public void Deserialize()
        {
            GeoCoordinateBounds deserializedGeoCoordinateBounds = JsonConvert.DeserializeObject<GeoCoordinateBounds>(this.geoCoordinateBoundsStr, JsonConverters.Converters);
            Assert.AreEqual(this.geoCoordinateBoundsObj.ToString(), deserializedGeoCoordinateBounds.ToString());
        }

        [Test]
        public void Serialize()
        {
            string serializedGeoCoordinateBounds = JsonConvert.SerializeObject(this.geoCoordinateBoundsObj, JsonConverters.Converters);
            Assert.AreEqual(this.geoCoordinateBoundsStr, serializedGeoCoordinateBounds);
        }
    }
}