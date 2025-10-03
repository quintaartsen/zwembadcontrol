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
    public class ZwembadFlowTests
    {
        [Fact]
        public async Task ExecuteAsync_StartsHeating_WhenBelowTargetAndBoilerHotEnough()
        {
            CurrentState.Reset(new DateModel { CurrentBoilerWaterTemp = 50 });

            var relay = new Mock<IRelayConnector>(MockBehavior.Strict);
            relay.Setup(r => r.CloseRelay(It.IsAny<int>()));
            relay.Setup(r => r.OpenRelay(It.IsAny<int>()));

            var acties = new ZwembadServiceActies(relay.Object, Mock.Of<IAirWellConnector>(), Mock.Of<ITibberConnector>(), Mock.Of<IHyconConnector>(), Mock.Of<IAcquaNetConnector>());
            var flow = new ZwembadFlow(acties);

            var hycon = new HyconData { TargetTemperature = 28, CurrentTemperature = 27 };
            var priceInfo = new PriceInfo { Current = new Price { Level = PriceLevel.Cheap, Total = 0 } };

            await flow.ExecuteAsync(priceInfo, 0, PriceLevel.Cheap, new AirWellData(), hycon);

            relay.Verify(r => r.CloseRelay(It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}


