﻿using ArcGIS.ServiceModel;
using ArcGIS.ServiceModel.Common;
using ArcGIS.ServiceModel.Operation;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ArcGIS.Test
{
    public class GeocodeTests : TestsFixture
    {
        [Theory]
        [InlineData("http://geocode.arcgis.com/arcgis", "/World/GeocodeServer/", "100 Willis Street, Wellington", "NZL")]
        public async Task CanGeocode(string rootUrl, string relativeUrl, string text, string sourceCountry = "")
        {
            var gateway = new PortalGateway(rootUrl);
            var geocode = new SingleInputGeocode(relativeUrl.AsEndpoint())
            {
                Text = text,
                SourceCountry = sourceCountry
            };
            var response = await gateway.Geocode(geocode);

            Assert.Null(response.Error);
            Assert.NotNull(response.SpatialReference);
            Assert.NotNull(response.Results);
            Assert.True(response.Results.Any());
            var result = response.Results.First();
            Assert.NotNull(result.Feature);
            Assert.NotNull(result.Feature.Geometry);
        }

        [Theory]
        [InlineData("http://geocode.arcgis.com/arcgis", "/World/GeocodeServer/", "100 Willis Street, Wellington")]
        public async Task CanSuggest(string rootUrl, string relativeUrl, string text)
        {
            var gateway = new PortalGateway(rootUrl);
            var suggest = new SuggestGeocode(relativeUrl.AsEndpoint())
            {
                Text = text
            };
            var response = await gateway.Suggest(suggest);

            Assert.Null(response.Error);
            Assert.NotNull(response.Suggestions);
            Assert.True(response.Suggestions.Any());
            var result = response.Suggestions.First();
            Assert.True(!string.IsNullOrWhiteSpace(result.Text));
        }

        [Theory]
        [InlineData("http://geocode.arcgis.com/arcgis", "/World/GeocodeServer/", 174.775505, -41.290893, 4326)]
        public async Task CanReverseGeocodePoint(string rootUrl, string relativeUrl, double x, double y, int wkid)
        {
            var gateway = new PortalGateway(rootUrl);
            var reverseGeocode = new ReverseGeocode(relativeUrl.AsEndpoint())
            {
                Location = new Point
                {
                    X = x,
                    Y = y,
                    SpatialReference = new SpatialReference { Wkid = wkid }
                }
            };
            var response = await gateway.ReverseGeocode(reverseGeocode);

            Assert.Null(response.Error);
            Assert.NotNull(response.Address);
            Assert.NotNull(response.Location);
        }
    }
}
