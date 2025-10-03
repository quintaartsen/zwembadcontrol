using System.Collections.Generic;
using Moq;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.flows;

namespace ZwembadControl.Tests
{
    internal static class TestHelpers
    {
        public static ZwembadService CreateService()
        {
            var airwell = new Mock<IAirWellConnector>(MockBehavior.Strict);
            var tibber = new Mock<ITibberConnector>(MockBehavior.Strict);
            var hycon = new Mock<IHyconConnector>(MockBehavior.Strict);
            var flows = new List<Flow>();
            return new ZwembadService(airwell.Object, tibber.Object, hycon.Object, flows);
        }
    }
}


