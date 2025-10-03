using System.Collections.Generic;
using Moq;
using System.Threading.Tasks;
using Tibber.Sdk;
using Xunit;
using ZwembadControl.Controllers;
using ZwembadControl.Connectors;
using ZwembadControl.Models;
using ZwembadControl.Rules;

namespace ZwembadControl.Tests
{
    public class KlimaatFlowTests
    {
        [Fact]
        public async Task ExecuteAsync_AutoMode_Ons_WhenWithinCheapestWindow()
        {
            CurrentState.Reset(new DateModel { klimaatMode = "auto" });

            var relay = new Mock<IRelayConnector>(MockBehavior.Strict);
            relay.Setup(r => r.CloseRelay(It.IsAny<int>()));
            relay.Setup(r => r.OpenRelay(It.IsAny<int>()));

            var acties = new ZwembadServiceActies(relay.Object, Mock.Of<IAirWellConnector>(), Mock.Of<ITibberConnector>(), Mock.Of<IHyconConnector>(), Mock.Of<IAcquaNetConnector>());
            var flow = new KlimaatFlow(acties);
            var today = new List<Price>
            {
                new Price { StartsAt = "2024-01-01T23:00:00Z", Total = 1m },
                new Price { StartsAt = "2024-01-01T01:00:00Z", Total = 2m },
            };
            var current = new Price { StartsAt = "2024-01-01T23:00:00Z", Total = 1m };
            var priceInfo = new PriceInfo { Today = today, Current = current };

            await flow.ExecuteAsync(priceInfo, 0, PriceLevel.Cheap, new AirWellData(), new Connectors.HyconData());

            Assert.True(CurrentState.Instance.KlimaatSysteem);
        }
    }
}


