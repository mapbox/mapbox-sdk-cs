//-----------------------------------------------------------------------
// <copyright file="LonLatToVector2dConverterTest.cs" company="Mapbox">
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
    internal class LonLatToVector2dConverterTest
    {
        private string lonLatStr = "[-77.0295,38.9165]";
        private Vector2d lonLatObj = new Vector2d(x: -77.0295, y: 38.9165);

        [Test]
        public void Deserialize()
        {
            Vector2d deserializedLonLat = JsonConvert.DeserializeObject<Vector2d>(this.lonLatStr, JsonConverters.Converters);
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