using System.Collections.Generic;
using Tibber.Sdk;
using Xunit;
using ZwembadControl.Controllers;

namespace ZwembadControl.Tests
{
    public class PriceLevelTests
    {
        [Fact]
        public void CalculateCurrentPriceLevel_ReturnsCurrent_WhenPricesNull()
        {
            var service = TestHelpers.CreateService();
            var current = new Price { Level = PriceLevel.Cheap };

            var result = service.CalculateCurrentPriceLevel(null, current);

            Assert.Equal(PriceLevel.Cheap, result);
        }

        [Fact]
        public void CalculateCurrentPriceLevel_StaysNormal_WhenUpcomingIsCheaperThanCurrentNormal()
        {
            var service = TestHelpers.CreateService();
            var current = new Price { StartsAt = "2024-01-01T10:00:00Z", Level = PriceLevel.Normal };
            var prices = new List<Price>
            {
                new Price { StartsAt = current.StartsAt, Level = PriceLevel.Normal },
                new Price { StartsAt = "2024-01-01T11:00:00Z", Level = PriceLevel.Cheap }
            };

            var result = service.CalculateCurrentPriceLevel(prices, current);

            Assert.Equal(PriceLevel.Normal, result);
        }
    }
}


