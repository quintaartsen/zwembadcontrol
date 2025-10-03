using Tibber.Sdk;

namespace ZwembadControl.Connectors
{
    public class TibberConnector : ITibberConnector
    {

        public async Task<PriceInfo> GetPriceLevel()
        {
            var client = new TibberApiClient("06E350858D6C6DAD6D8D7058689EE05E760A6FC3077535A232CB3C4B7C40AE87-1");
            var basicData = await client.GetBasicData();
            var homeId = basicData.Data.Viewer.Homes.First().Id.Value;
            var consumption = await client.GetHomeConsumption(homeId, EnergyResolution.Daily);

            var customQueryBuilder =
                new TibberQueryBuilder()
                    .WithViewer(
                        new ViewerQueryBuilder()
                            .WithHome(
                                new HomeQueryBuilder()
                                    .WithCurrentSubscription(
                                        new SubscriptionQueryBuilder()
                                            .WithAllScalarFields()
                                            .WithSubscriber(new LegalEntityQueryBuilder().WithAllFields())
                                            .WithPriceInfo(new PriceInfoQueryBuilder().WithToday(new PriceQueryBuilder().WithAllFields()).WithCurrent(new PriceQueryBuilder().WithAllFields()))
                                    ),
                                homeId
                            )
                    );

            var customQuery = customQueryBuilder.Build();
            var result = await client.Query(customQuery);
            var priceInfo = result.Data.Viewer.Home.CurrentSubscription.PriceInfo;
            return priceInfo;
        }
    }
}
