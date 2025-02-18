using ProfitCapture.Models;


namespace ProfitCapture.Indicators
{
    public class Average
    {

        public static decimal Moving(int periodos, ulong index, List<AssetQuoteTimelinePeriod> candles)
        {
            if (periodos <= 0)
            {
                periodos = 1;
            }

            if (index <= 0)
            {
                return (candles.Count > 0) ? candles[0].Close : 0;
            }

            var mi = 0.0m;
            int sm = 0;
            int ix = (int)index;
            while (ix > 0 && sm < periodos)
            {
                var cand = candles[ix];
                var me = cand.Close;
                mi += me;
                ix--;
                sm++;
            }
            var average = mi / (decimal)sm;
            return average;
        }

    }
}
