using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.Models;

namespace ZwembadControl.flows;

public abstract class Flow(ZwembadServiceActies zwembadServiceActies)
{
    public abstract Task ExecuteAsync(PriceInfo priceInfo, decimal totalPrice, PriceLevel priceLevel, AirWellData airWellData, HyconData hyconData);
}
