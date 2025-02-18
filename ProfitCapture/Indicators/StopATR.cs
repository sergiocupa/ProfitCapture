using ProfitCapture.Models;


namespace ProfitCapture.Indicators
{
    public class StopATR
    {

        public static double ComputeATR(int period, List<AssetQuoteTimelinePeriod> candles)
        {
            if (candles == null || candles.Count < 2)
                throw new ArgumentException("São necessários pelo menos 2 candles para calcular o ATR.");

            List<double> trueRanges = new List<double>();

            // Calcula o True Range para cada candle (exceto o primeiro, pois não há candle anterior)
            for (int i = 1; i < candles.Count; i++)
            {
                var currentHigh   = (double)candles[i].Max;
                var currentLow    = (double)candles[i].Min;
                var previousClose = (double)candles[i - 1].Close;

                // True Range: maior entre (High - Low), |High - previousClose| e |Low - previousClose|
                double tr = Math.Max(currentHigh - currentLow, Math.Max(Math.Abs(currentHigh - previousClose), Math.Abs(currentLow - previousClose)));
                trueRanges.Add(tr);
            }

            // Se a quantidade de true ranges for menor que o período, ajusta o período
            int count = trueRanges.Count;
            if (count < period)
                period = count;

            // Calcula a média simples dos últimos "period" valores de True Range
            double sum = 0;
            for (int i = count - period; i < count; i++)
            {
                sum += trueRanges[i];
            }
            return sum / period;
        }

      
        public static double ComputeStopATR(int period, double multiplier, bool isLong, List<AssetQuoteTimelinePeriod> candles)
        {
            // Considera que o preço de entrada é o Close do último candle
            AssetQuoteTimelinePeriod lastCandle = candles.Last();
            var entryPrice = (double)lastCandle.Close;

            double atr = ComputeATR(period, candles);

            if (isLong)
            {
                // Para posição longa, o stop fica abaixo do preço de entrada
                return entryPrice - multiplier * atr;
            }
            else
            {
                // Para posição curta, o stop fica acima do preço de entrada
                return entryPrice + multiplier * atr;
            }
        }



    }
}
