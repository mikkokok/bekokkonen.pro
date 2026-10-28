using bekokkonen.pro.Global;
using bekokkonen.pro.Global.Config;
using bekokkonen.pro.Models.HeatHarmony;
using System.Collections.Concurrent;

namespace bekokkonen.pro.Providers
{
    public sealed class RestlessFalconProvider(ILogger<RestlessFalconProvider> logger, IRequestProvider requestProvider)
    {
        private readonly string _serviceName = nameof(RestlessFalconProvider);
        private readonly ILogger<RestlessFalconProvider> _logger = logger;
        private readonly IRequestProvider _requestProvider = requestProvider;
        private readonly ConcurrentDictionary<(DateOnly date, int days), (IReadOnlyList<ElectricityPrice> prices, DateTime expiresAt)> _cache = new();
        private static readonly TimeSpan TomorrowPricesExpectedAt = new(14, 15, 0);
        private static readonly TimeSpan RetryInterval = TimeSpan.FromMinutes(10);

        public async Task<IEnumerable<ElectricityPrice>> GetElectricityPrices(int days)
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var key = (today, days);

            if (_cache.TryGetValue(key, out var cached) && now < cached.expiresAt)
            {
                return cached.prices;
            }

            try
            {
                var response = await _requestProvider.GetAsync<List<ElectricityPrice>>(HttpClientConst.RestlessFalconClient, $"https://api.kokkonen.pro/api/ElectricityPrice?ago={days}");
                var prices = response ?? [];
                if (prices.Count > 0)
                {
                    _cache[key] = (prices, GetExpiration(now, days, prices));
                    RemoveOldEntries(today);
                }
                return prices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_serviceName}:: Error occurred while fetching electricity prices.");
                throw;
            }
        }

        private static DateTime GetExpiration(DateTime now, int days, List<ElectricityPrice> prices)
        {
            var midnight = now.Date.AddDays(1);
            if (days > 0 || HasTomorrowPrices(now, prices))
            {
                return midnight;
            }

            var expectedAt = now.Date + TomorrowPricesExpectedAt;
            if (now < expectedAt)
            {
                return expectedAt;
            }

            var retryAt = now + RetryInterval;
            return retryAt < midnight ? retryAt : midnight;
        }

        private static bool HasTomorrowPrices(DateTime now, List<ElectricityPrice> prices)
        {
            var tomorrow = now.Date.AddDays(1);
            return prices.Any(p => DateTime.TryParse(p.date, out var date) && date.Date == tomorrow);
        }

        private void RemoveOldEntries(DateOnly today)
        {
            foreach (var oldKey in _cache.Keys.Where(k => k.date < today))
            {
                _cache.TryRemove(oldKey, out _);
            }
        }
    }
}
