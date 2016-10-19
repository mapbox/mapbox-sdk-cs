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
        private const string Query = "Minneapolis";
        private const string Base = "https://api.mapbox.com/geocoding/v5/mapbox.places/";
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
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json");

            // With autocomplete
            this.fgr.Autocomplete = false;
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json?autocomplete=false");

            // With bbox
            this.fgr.Bbox = new LatLngBounds(new LatLng(10, 15), new LatLng(20, 25));
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json?autocomplete=false&bbox=10,15,20,25");

            // With one country
            this.fgr.Country = new string[] { "ar" };
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json?autocomplete=false&bbox=10,15,20,25&country=ar");

            // With multiple countries
            this.fgr.Country = new string[] { "ar", "fi" };
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json?autocomplete=false&bbox=10,15,20,25&country=ar,fi");

            // With proximity
            this.fgr.Proximity = new LatLng(5, 10);
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json?autocomplete=false&bbox=10,15,20,25&country=ar,fi&proximity=5,10");

            // With one types
            this.fgr.Types = new string[] { "country" };
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json?autocomplete=false&bbox=10,15,20,25&country=ar,fi&proximity=5,10&types=country");

            // With multiple types
            this.fgr.Types = new string[] { "country", "region" };
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json?autocomplete=false&bbox=10,15,20,25&country=ar,fi&proximity=5,10&types=country,region");

            // Set all to null
            this.fgr.Autocomplete = null;
            this.fgr.Bbox = null;
            this.fgr.Country = null;
            this.fgr.Proximity = null;
            this.fgr.Types = null;
            Assert.AreEqual(this.fgr.GetUrl(), Base + Query + ".json");
        }
    }
}