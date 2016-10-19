//-----------------------------------------------------------------------
// <copyright file="ReverseGeocodeResourceTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using Mapbox;
    using NUnit.Framework;

    [TestFixture]
    internal class ReverseGeocodeResourceTest
    {
        private const string Base = "https://api.mapbox.com/geocoding/v5/mapbox.places/";
        private LatLng query = new LatLng(10, 10);
        private string expectedQueryString = "10,10";
        private Geocoding.ReverseGeocodeResource rgr;

        [SetUp]
        public void SetUp()
        {
            this.rgr = new Geocoding.ReverseGeocodeResource(this.query);
        }

        public void BadType()
        {
            this.rgr.Types = new string[] { "fake" };
        }

        public void BadTypeWithGoodType()
        {
            this.rgr.Types = new string[] { "place", "fake" };
        }

        [Test]
        public void SetInvalidTypes()
        {
            Assert.Throws<Exception>(this.BadType);
            Assert.Throws<Exception>(this.BadTypeWithGoodType);
        }

        [Test]
        public void GetUrl()
        {
            // With only constructor
            Assert.AreEqual(this.rgr.GetUrl(), Base + this.expectedQueryString + ".json");

            // With one types
            this.rgr.Types = new string[] { "country" };
            Assert.AreEqual(this.rgr.GetUrl(), Base + this.expectedQueryString + ".json?types=country");

            // With multiple types
            this.rgr.Types = new string[] { "country", "region" };
            Assert.AreEqual(this.rgr.GetUrl(), Base + this.expectedQueryString + ".json?types=country,region");

            // Set all to null
            this.rgr.Types = null;
            Assert.AreEqual(this.rgr.GetUrl(), Base + this.expectedQueryString + ".json");
        }
    }
}