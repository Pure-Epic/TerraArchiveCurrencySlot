using Humanizer;
using Microsoft.Xna.Framework;
using MonoMod.Core.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace TerraArchiveShopSlot
{
    public class TerraArchiveShopSlot : Mod
    {
        public static readonly int Pyroxene = CustomCurrencyManager.RegisterCurrency(new PyroxeneCurrency(ModContent.ItemType<Pyroxene>(), 999L, "Pyroxene"));

        public override void Load()
        {
            On_Main.DrawInventory += Hook_DrawInventory;
            On_Player.BuyItem += Hook_BuyItem;
        }

        public override void Unload()
        {
            On_Main.DrawInventory -= Hook_DrawInventory;
            On_Player.BuyItem -= Hook_BuyItem;
        }

        private void Hook_DrawInventory(On_Main.orig_DrawInventory orig, Main self)
        {
            Player player = Main.LocalPlayer;
            Main.inventoryScale = 0.85f;
            Point cursorPosition = new Point(Main.mouseX, Main.mouseY);
            Rectangle r = new Rectangle(0, 0, (int)(TextureAssets.InventoryBack.Width() * Main.inventoryScale), (int)(TextureAssets.InventoryBack.Height() * Main.inventoryScale));
            Item[] inv = player.GetModPlayer<SlotPlayer>().money;
            int positionX = 20 + (int)(56 * Main.inventoryScale);
            int positionY = 20 + (int)(280 * Main.inventoryScale);
            r.X = positionX - 47;
            r.Y = positionY;
            // TODO: Replace the item type check when more currencies are added
            if (r.Contains(cursorPosition) && !PlayerInput.IgnoreMouseInterface && (Main.mouseItem.type == ModContent.ItemType<Pyroxene>() || Main.mouseItem.type == ItemID.None))
            {
                player.mouseInterface = true;
                Main.armorHide = true;
                ItemSlot.Handle(inv, 0, 0);
            }
            ItemSlot.Draw(Main.spriteBatch, inv, 1, 0, r.TopLeft());
            orig(self);
        }

        private bool Hook_BuyItem(On_Player.orig_BuyItem orig, Player self, long price, int customCurrency)
        {
            long priceRemaining = price;
            if (customCurrency == Pyroxene)
            {
                if (CanAfford(self, price, customCurrency))
                {
                    Item[][] inventories =
                    [
                        self.GetModPlayer<SlotPlayer>().money,
                        self.inventory,
                        self.bank.item,
                        self.bank2.item,
                        self.bank3.item,
                        self.bank4.item
                    ];
                    foreach (Item[] inventory in inventories)
                    {
                        GetCurrency(customCurrency, inventory, price, ref priceRemaining);
                        if (priceRemaining <= 0) break;
                    }
                    return priceRemaining <= 0;
                }
                return false;
            }
            else return orig(self, price, customCurrency);
        }

        public static bool CanAfford(Player player, long price, int customCurrency)
        {
            CustomCurrencyManager.TryGetCurrencySystem(customCurrency, out CustomCurrencySystem customCurrencySystem);
            long num = customCurrencySystem.CountCurrency(out _, player.GetModPlayer<SlotPlayer>().money);
            long num2 = customCurrencySystem.CountCurrency(out _, player.inventory, 58, 57, 56, 55, 54);
            long num3 = customCurrencySystem.CountCurrency(out _, player.bank.item);
            long num4 = customCurrencySystem.CountCurrency(out _, player.bank2.item);
            long num5 = customCurrencySystem.CountCurrency(out _, player.bank3.item);
            long num6 = customCurrencySystem.CountCurrency(out _, player.bank4.item);
            if (customCurrencySystem.CombineStacks(out _, num, num2, num3, num4, num5, num6) < price)
                return false;
            return true;
        }

        public static void GetCurrency(int customCurrency, Item[] inv, long price, ref long remainder)
        {
            CustomCurrencyManager.TryGetCurrencySystem(customCurrency, out CustomCurrencySystem system);
            foreach (Item item in inv)
            {
                if (system.Accepts(item))
                {
                    if (item.stack >= price)
                    {
                        item.stack -= (int)price;
                        remainder = 0;
                    }
                    else
                    {
                        remainder -= item.stack;
                        item.stack = 0;
                    }
                }
                if (remainder <= 0) break;
            }
        }
    }

    public class SlotPlayer : ModPlayer
    {
        public Item[] money = new Item[3];

        public override void OnEnterWorld()
        {
            for (int i = 0; i < 2; i++)
            {
                if (money[i] == null)
                    money[i] = new Item(ItemID.None);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            money[0] = tag.Get<Item>("moneySlot1");
            money[1] = tag.Get<Item>("moneySlot2");
            money[2] = tag.Get<Item>("moneySlot3");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["moneySlot1"] = money[0];
            tag["moneySlot2"] = money[1];
            tag["moneySlot3"] = money[2];
        }
    }

    class ExampleNPCShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            shop.Add(new Item(ItemID.Zenith)
            {
                shopCustomPrice = 2,
                shopSpecialCurrency = TerraArchiveShopSlot.Pyroxene
            });
            shop.Add(new Item(ItemID.PoopBlock)
            {
                shopCustomPrice = 2,
                shopSpecialCurrency = TerraArchiveShopSlot.Pyroxene
            });
        }
    }
}
