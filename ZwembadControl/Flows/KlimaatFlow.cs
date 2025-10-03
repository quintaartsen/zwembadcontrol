using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.flows;
using ZwembadControl.Models;

namespace ZwembadControl.Rules;

public class KlimaatFlow(ZwembadServiceActies zwembadServiceActies) : Flow(zwembadServiceActies)
{
    public override async Task ExecuteAsync(PriceInfo priceInfo, decimal totalPrice, PriceLevel priceLevel, AirWellData airWellData, HyconData hyconData)
    {
        if (CurrentState.Instance.klimaatMode == "auto")
        {
            if (KlimaatSysteemMoetAan(priceInfo.Today, priceInfo.Current))
            {
                await zwembadServiceActies.StartKlimaatSysteemasync();
            }
            else
            {
                await zwembadServiceActies.StopKlimaatSysteemasync();
            }
        }
    }

    private bool KlimaatSysteemMoetAan(ICollection<Price> prices, Price currentPrice)
    {
        var startFrame = new TimeOnly(22, 0, 0);
        var endFrame = new TimeOnly(5, 0, 0);
        var priceLow = decimal.MaxValue;
        var time = string.Empty;

        foreach (var price in prices)
        {
            var start = DateTime.Parse(price.StartsAt);
            if (start.Hour >= startFrame.Hour || start.Hour <= endFrame.Hour)
            {
                if (price.Total < priceLow)
                {
                    priceLow = (decimal)price.Total;
                    time = price.StartsAt;
                }
            }
        }

        var currentTime = DateTime.Parse(currentPrice.StartsAt);
        return currentPrice.StartsAt == time || (currentTime.Hour < startFrame.Hour && currentTime.Hour > endFrame.Hour);
    }
}
