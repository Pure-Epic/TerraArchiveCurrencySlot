using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace TerraArchiveCurrencySlot
{
    public class PyroxeneCurrency : CustomCurrencySingleCoin
    {
        public PyroxeneCurrency(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = CurrencyTextKey;
            CurrencyTextColor = Color.LightCyan;
        }
    }
}