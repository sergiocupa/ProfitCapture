

namespace ProfitCapture.Models
{
    public class CandlePeriod
    {
        public string   Name   { get; set; }
        public TimeSpan Period { get; set; }


        public static DateTime AddInterval(DateTime dt, TimeSpan duration)
        {
            var am = dt.Add(duration);
            var an = CandlePeriod.RoundToNearestInterval(am, duration);
            return an;
        }

        public static DateTime RoundToNearestInterval(DateTime dt, TimeSpan interval)
        {
            long ticks = interval.Ticks;
            long halfIntervalTicks = ticks / 2;

            // Arredonda para o múltiplo mais próximo do intervalo
            return new DateTime(((dt.Ticks + halfIntervalTicks) / ticks) * ticks, dt.Kind);
        }

        public static List<CandlePeriod> GetDefaultPeriods()
        {
            var result = new List<CandlePeriod>();
            result.Add(new CandlePeriod() { Name = "1MIM", Period = new TimeSpan(0, 1, 0) });
            result.Add(new CandlePeriod() { Name = "5MIM", Period = new TimeSpan(0, 5, 0) });
            result.Add(new CandlePeriod() { Name = "15MIM", Period = new TimeSpan(0, 15, 0) });
            result.Add(new CandlePeriod() { Name = "30MIM", Period = new TimeSpan(0, 30, 0) });
            result.Add(new CandlePeriod() { Name = "60MIM", Period = new TimeSpan(1, 0, 0) });
            result.Add(new CandlePeriod() { Name = "1D", Period = new TimeSpan(1, 0, 0, 0) });
            return result;
        }
    }
}
