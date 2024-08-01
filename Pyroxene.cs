using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraArchiveShopSlot
{
    public class Pyroxene : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.buyPrice(platinum: 100);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override bool OnPickup(Player player)
        {
            ref Item currency = ref player.GetModPlayer<SlotPlayer>().money[0];
            if (currency.type == Item.type)
            {
                if (currency.stack + Item.stack <= Item.maxStack)
                {
                    currency.stack += Item.stack;
                    SoundEngine.PlaySound(SoundID.Grab);
                    return false;
                }
                else
                {
                    int remainder = Item.maxStack - currency.stack;
                    currency.stack = Item.maxStack;
                    Item.stack -= remainder;
                    return true;
                }
            }
            else if (currency.type == ItemID.None)
            {
                currency.SetDefaults(Item.type);
                currency.stack = Item.stack;
                SoundEngine.PlaySound(SoundID.Grab);
                return false;
            }
            return base.OnPickup(player);
        }
    }
}