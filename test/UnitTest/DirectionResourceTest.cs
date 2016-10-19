//-----------------------------------------------------------------------
// <copyright file="DirectionResourceTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using Mapbox;
    using NUnit.Framework;

    [TestFixture]
    internal class DirectionResourceTest
    {
        private const string Base = "https://api.mapbox.com/directions/v5/mapbox/";
        private LatLng[] coordinates = { new LatLng(10, 10), new LatLng(20, 20) };
        private Directions.RoutingProfile profile = Directions.RoutingProfile.Driving;
        private Directions.DirectionResource dr;

        [SetUp]
        public void SetUp()
        {
            this.dr = new Directions.DirectionResource(this.coordinates, this.profile);
        }

        public void MismatchedBearings()
        {
            this.dr.Bearings = new BearingFilter[] { new BearingFilter(10, 10) };
        }

        public void MismatchedRadiuses()
        {
            this.dr.Radiuses = new double[] { 10 };
        }

        public void TooSmallRadius()
        {
            this.dr.Radiuses = new double[] { 10, -1 };
        }

        [Test]
        public void SetInvalidBearings()
        {
            Assert.Throws<Exception>(this.MismatchedBearings);
        }

        [Test]
        public void SetInvalidRadiuses_Mismatched()
        {
            Assert.Throws<Exception>(this.MismatchedRadiuses);
        }

        [Test]
        public void SetInvalidRadiuses_TooSmall()
        {
            Assert.Throws<Exception>(this.TooSmallRadius);
        }

        [Test]
        public void GetUrl()
        {
            // With only constructor
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json");

            // With alternatives
            this.dr.Alternatives = false;
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false");

            // With bearings
            this.dr.Bearings = new BearingFilter[] { new BearingFilter(90, 45), new BearingFilter(90, 30) };
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false&bearings=90,45;90,30");

            // Bearings are nullable
            this.dr.Bearings = new BearingFilter[] { new BearingFilter(90, 45), new BearingFilter(null, null) };
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false&bearings=90,45;");

            // With continue straight
            this.dr.ContinueStraight = false;
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false&bearings=90,45;&continue_straight=false");

            // With geometries
            this.dr.Geometries = Directions.Geometries.Geojson;
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false&bearings=90,45;&continue_straight=false&geometries=geojson");

            // With overview
            this.dr.Overview = Directions.Overview.Full;
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false&bearings=90,45;&continue_straight=false&geometries=geojson&overview=full");

            // With steps
            this.dr.Radiuses = new double[] { 30, 30 };
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false&bearings=90,45;&continue_straight=false&geometries=geojson&overview=full&radiuses=30,30");

            // With steps
            this.dr.Steps = false;
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json?alternatives=false&bearings=90,45;&continue_straight=false&geometries=geojson&overview=full&radiuses=30,30&steps=false");

            // Set all to null
            this.dr.Alternatives = null;
            this.dr.Bearings = null;
            this.dr.ContinueStraight = null;
            this.dr.Geometries = null;
            this.dr.Overview = null;
            this.dr.Radiuses = null;
            this.dr.Steps = null;
            Assert.AreEqual(this.dr.GetUrl(), Base + "driving/10,10;20,20.json");
        }
    }
}