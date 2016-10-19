//-----------------------------------------------------------------------
// <copyright file="ForwardGeocodeResourceTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using Mapbox;
    using NUnit.Framework;

    [TestFixture]
    internal class ForwardGeocodeResourceTest
    {
        private const string Query = "Minneapolis, MN";
        private Geocoding.ForwardGeocodeResource fgr;

        [SetUp]
        public void SetUp()
        {
            this.fgr = new Geocoding.ForwardGeocodeResource(Query);
        }

        public void BadType()
        {
            this.fgr.Types = new string[] { "fake" };
        }

        public void BadTypeWithGoodType()
        {
            this.fgr.Types = new string[] { "place", "fake" };
        }

        public void BadCountry()
        {
            this.fgr.Types = new string[] { "zz" };
        }

        public void BadCountryWithGoodType()
        {
            this.fgr.Types = new string[] { "zz", "ar" };
        }

        [Test]
        public void SetInvalidTypes()
        {
            Assert.Throws<Exception>(this.BadType);
            Assert.Throws<Exception>(this.BadTypeWithGoodType);
        }

        [Test]
        public void SetInvalidCountries()
        {
            Assert.Throws<Exception>(this.BadCountry);
            Assert.Throws<Exception>(this.BadCountryWithGoodType);
        }

        [Test]
        public void GetUrl()
        {
            // With only constructor
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json", this.fgr.GetUrl());

            // With autocomplete
            this.fgr.Autocomplete = false;
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json?autocomplete=false", this.fgr.GetUrl());

            // With bbox
            this.fgr.Bbox = new LatLngBounds(new LatLng(10, 15), new LatLng(20, 25));
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json?autocomplete=false&bbox=10%2C15%2C20%2C25", this.fgr.GetUrl());

            // With one country
            this.fgr.Country = new string[] { "ar" };
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json?autocomplete=false&bbox=10%2C15%2C20%2C25&country=ar", this.fgr.GetUrl());

            // With multiple countries
            this.fgr.Country = new string[] { "ar", "fi" };
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json?autocomplete=false&bbox=10%2C15%2C20%2C25&country=ar%2Cfi", this.fgr.GetUrl());

            // With proximity
            this.fgr.Proximity = new LatLng(5, 10);
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json?autocomplete=false&bbox=10%2C15%2C20%2C25&country=ar%2Cfi&proximity=5%2C10", this.fgr.GetUrl());

            // With one types
            this.fgr.Types = new string[] { "country" };
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json?autocomplete=false&bbox=10%2C15%2C20%2C25&country=ar%2Cfi&proximity=5%2C10&types=country", this.fgr.GetUrl());

            // With multiple types
            this.fgr.Types = new string[] { "country", "region" };
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json?autocomplete=false&bbox=10%2C15%2C20%2C25&country=ar%2Cfi&proximity=5%2C10&types=country%2Cregion", this.fgr.GetUrl());

            // Set all to null
            this.fgr.Autocomplete = null;
            this.fgr.Bbox = null;
            this.fgr.Country = null;
            this.fgr.Proximity = null;
            this.fgr.Types = null;
            Assert.AreEqual("https://api.mapbox.com/geocoding/v5/mapbox.places/Minneapolis%2C%20MN.json", this.fgr.GetUrl());
        }
    }
}