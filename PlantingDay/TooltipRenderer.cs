using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PlantingDay;
using PlantingDay.Compatibility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PlantingDay
{

    public static class TooltipRenderer
    {
        public static void DrawMenu(SpriteBatch spriteBatch)
        {
            if (!Context.IsWorldReady)
                return;

            Item? hovered = GetHoveredItemFromAnyMenu();
            if (hovered is not StardewValley.Object obj)
                return;

            // Use the correct key format O:#### to see if the item is in the plant library
            string lookupKey = "O:" + obj.ItemId;

            var info = PlantDatabase.LookupFromKey(lookupKey);
            if (info is null)
                return;

            var elements = BuildTooltip(info);

            PlaceTooltip(spriteBatch, elements);



            //DrawTooltip(spriteBatch, info);
        }



        private static void DrawTooltip(SpriteBatch spriteBatch, PlantInfo info)
        {
            var elements = BuildTooltip(info);

            PlaceTooltip(spriteBatch, elements);

        }

        private static List<TooltipElement> BuildTooltip(PlantInfo info)
        {
            var list = new List<TooltipElement>();

            list.AddRange(GetSeasonsTooltip(info));


            if (info.Trellis)
                list.Add(new TooltipElement { Text = "Requires a trellis", TextColor = Color.Orange });



            if (info.Paddy)
                list.Add(new TooltipElement { Text = "Grows faster near water", TextColor = Color.CornflowerBlue });

            // Growth info (includes DaysToProduce + ready day + warnings)
            list.AddRange(GetGrowthTooltip(info));

            //Too late to plant
            list.Add(new TooltipElement
            {
                Icon = TooltipIcons.Warning,
                Text = ModEntry.ModHelper.Translation.Get(TooltipKeys.TooLate),
                TextColor = TooltipColors.Warning
            });

            //Multicolor sprites
            if (info.MultiSprite > 0)
            {
                list.Add(new TooltipElement
                {
                    Icon = TooltipIcons.Rainbow,
                    Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.MultiSprite),
                        info.MultiSprite
                        ),
                    TextColor = TooltipColors.Normal
                });
            }








            return list;
        }


        //--------------
        // Season display
        //--------------

        private static List<TooltipElement> GetSeasonsTooltip(PlantInfo info)
        {
            var list = new List<TooltipElement>();
            if (info.Seasons == null || info.Seasons.Count == 0)
                return list;

            // Display the relevant seasons and highlight the current season
            if (info.Seasons.Count > 0)
            {
                var segments = BuildInlineSegments(
                    info.Seasons,
                    season =>
                    {
                        string translated = TranslateSeason(season);
                        var (color, bold) = GetSeasonStyle(season);

                        return new InlineSegment
                        {
                            Text = translated,
                            Color = color,
                            Bold = bold
                        };
                    }
                );

                list.Add(new TooltipElement
                {
                    InlineSegments = segments
                });
            }


            return list;

        }


        private static string TranslateSeason(string season)
        {
            string key = season.ToLower() switch
            {
                "spring" => "season.spring",
                "summer" => "season.summer",
                "fall" => "season.fall",
                "winter" => "season.winter",
                _ => season // fallback: return raw string
            };

            return ModEntry.ModHelper.Translation.Get(key);
        }

        private static (Color color, bool bold) GetSeasonStyle(string season)
        {
            bool isCurrent = season.Equals(Game1.currentSeason, StringComparison.OrdinalIgnoreCase);

            Color color = season.ToLower() switch
            {
                "spring" => TooltipColors.SpringColor,
                "summer" => TooltipColors.SummerColor,
                "fall" => TooltipColors.FallColor,
                "winter" => TooltipColors.WinterColor,
                _ => TooltipColors.Normal
            };

            // If it's the current season → bold + colored
            if (isCurrent)
                return (color, true);

            // Otherwise → normal color + not bold
            return (TooltipColors.Normal, false);
        }

        //----------------
        // Days until harvest display
        //----------------

        private static List<TooltipElement> GetGrowthTooltip(PlantInfo info)
        {
            var list = new List<TooltipElement>();
            if (info.DaysToProduce <= 0)
                return list;

            // Line: How many days + when is it ready
            // Line: Planting warning
            int today = Game1.dayOfMonth; // current day in the game
            int days = info.DaysToProduce ?? 0; // how long for the crop to grow
            int readyDay = today + days; // what day is the crop ready

            Season currentSeason = Game1.season; //current season in the game

            List<Season> seasons = info.Seasons //list of seasons of the seed
                .Select(s => Enum.Parse<Season>(s, ignoreCase: true))
                .ToList();



            // PRINT How many days until ready
            list.Add(new TooltipElement
            {
                Text = string.Format(ModEntry.ModHelper.Translation
                    .Get(TooltipKeys.DaysToProduce),
                    info.DaysToProduce
                ),
                TextColor = TooltipColors.Normal
            });


            // Single-season crop
            if (seasons.Count == 1)
            {
                if (readyDay <= 28)
                {
                    list.Add(new TooltipElement
                    {
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.ReadyOn),
                            Ordinal(readyDay),
                            currentSeason
                        ),

                        //Text = $"Ready on: {Ordinal(readyDay)} {currentSeason}",
                        TextColor = TooltipColors.Normal
                    });
                }
                else
                {
                    list.Add(new TooltipElement
                    {
                        Text = "⚠ Too late to plant this season",
                        TextColor = Color.DarkRed
                    });
                }

                return list;
            }

            // Multi-season crop (e.g., corn)
            Season nextSeason = NextSeason(currentSeason);

            if (readyDay <= 28)
            {
                // Ready this season
                list.Add(new TooltipElement
                {
                    Text = $"Ready on: {Ordinal(readyDay)} {currentSeason}",
                    TextColor = Color.Black
                });
            }
            else
            {
                int overflow = readyDay - 28;

                if (seasons.Contains(nextSeason))
                {
                    // Ready next season
                    list.Add(new TooltipElement
                    {
                        Text = $"Ready on: {Ordinal(overflow)} {nextSeason}",
                        TextColor = Color.Black
                    });
                }
                else
                {
                    // Too late even with multi-season support
                    list.Add(new TooltipElement
                    {
                        Text = "⚠ Will not mature in time",
                        TextColor = Color.DarkRed
                    });
                }
            }

            return list;
        }

        private static Season NextSeason(Season s)
        {
            return s switch
            {
                Season.Spring => Season.Summer,
                Season.Summer => Season.Fall,
                Season.Fall => Season.Winter,
                Season.Winter => Season.Spring,
                _ => Season.Spring
            };
        }

        private static string Ordinal(int n)
        {
            if (n % 100 is 11 or 12 or 13)
                return $"{n}th";

            return (n % 10) switch
            {
                1 => $"{n}st",
                2 => $"{n}nd",
                3 => $"{n}rd",
                _ => $"{n}th"
            };
        }



        //--------------
        // Get the item to draw the tooltip about
        //--------------

        private static Item? GetHoveredItemFromAnyMenu()
        {
            IClickableMenu menu = Game1.activeClickableMenu;
            if (menu == null)
                return null;

            switch (menu)
            {
                // Inventory (GameMenu → InventoryPage)
                case GameMenu gm:
                    var invPage = gm.pages
                        .OfType<StardewValley.Menus.InventoryPage>()
                        .FirstOrDefault();

                    Item? hovered = invPage?.hoveredItem;
                    //if (hovered is not null)
                    //{
                    //    this.Monitor.Log($"Hover item QID: {hovered.QualifiedItemId}", LogLevel.Info);

                    //}

                    return invPage?.hoveredItem;

                // Chests, Fridges, Dressers, Junimo Chests, etc.
                case StardewValley.Menus.ItemGrabMenu chest:
                    return chest.hoveredItem;

                // Pierre’s shop, Traveling Cart, Krobus, Qi, etc.
                case StardewValley.Menus.ShopMenu shop:
                    return shop.hoveredItem as Item;

                default:
                    return null;

            }
        }

        //--------------
        // Creating the display
        //--------------


        private static void PlaceTooltip(SpriteBatch b, List<TooltipElement> elements)
        {
            SpriteFont font = Game1.smallFont;

            // Measure total height + max width
            int width = 0;
            int height = 0;

            foreach (var el in elements)
            {
                int lineHeight = (int)font.LineSpacing;

                height += el.PaddingTop + lineHeight + el.PaddingBottom;

                int lineWidth = 0;

                // Icon width
                if (el.Icon.HasValue)
                    lineWidth += el.Icon.Value.Size + 4;

                // Inline segments width
                if (el.InlineSegments != null)
                {
                    foreach (var seg in el.InlineSegments)
                        lineWidth += (int)font.MeasureString(seg.Text).X;

                    width = Math.Max(width, lineWidth);
                    continue;
                }

                // Normal text width
                if (!string.IsNullOrEmpty(el.Text))
                    lineWidth += (int)font.MeasureString(el.Text).X;

                width = Math.Max(width, lineWidth);
            }

            width += 32;  // vanilla padding
            height += 32;

            // Position to the left of the mouse
            int x = Game1.getMouseX() - width + 32;
            int y = Game1.getMouseY() + 32;

            if (x < 0)
                x = 0;

            if (y + height > Game1.uiViewport.Height)
                y = Game1.uiViewport.Height - height;

            // Draw background
            IClickableMenu.drawTextureBox(
                b,
                x,
                y,
                width,
                height,
                Color.White
            );

            // Draw content
            int drawY = y + 16;

            foreach (var el in elements)
            {
                drawY += el.PaddingTop;
                int drawX = x + 16;

                // Icon
                const int IconRenderSize = 32;

                if (el.Icon.HasValue)
                {
                    var icon = el.Icon.Value;

                    b.Draw(
                        icon.Texture,
                        new Rectangle(drawX, drawY, IconRenderSize, IconRenderSize),
                        icon.Source,
                        Color.White
                    );

                    drawX += IconRenderSize + 4;
                }

                // Inline segments
                if (el.InlineSegments != null)
                {
                    int xCursor = drawX;

                    foreach (var seg in el.InlineSegments)
                    {
                        if (seg.Bold)
                        {
                            b.DrawString(font, seg.Text, new Vector2(xCursor + 1, drawY), seg.Color);
                            b.DrawString(font, seg.Text, new Vector2(xCursor, drawY), seg.Color);
                        }
                        else
                        {
                            b.DrawString(font, seg.Text, new Vector2(xCursor, drawY), seg.Color);
                        }

                        xCursor += (int)font.MeasureString(seg.Text).X;
                    }

                    drawY += (int)font.LineSpacing + el.PaddingBottom;
                    continue;
                }

                // Text
                if (!string.IsNullOrEmpty(el.Text))
                {
                    if (el.Bold)
                    {
                        b.DrawString(font, el.Text, new Vector2(drawX + 1, drawY), el.TextColor);
                        b.DrawString(font, el.Text, new Vector2(drawX, drawY), el.TextColor);
                    }
                    else
                    {
                        b.DrawString(font, el.Text, new Vector2(drawX, drawY), el.TextColor);
                    }
                }

                drawY += (int)font.LineSpacing + el.PaddingBottom;
            }
        }
        // Put multiple segments on the same line with separators (e.g., seasons)
        public static List<InlineSegment> BuildInlineSegments<T>(
            IEnumerable<T> items,
            Func<T, InlineSegment> buildSegment,
            string separator = " • ")
        {
            var result = new List<InlineSegment>();
            var list = items.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                // Add the main segment
                result.Add(buildSegment(list[i]));

                // Add separator if not last
                if (i < list.Count - 1)
                {
                    result.Add(new InlineSegment
                    {
                        Text = separator,
                        Color = TooltipColors.Muted,
                        Bold = false
                    });
                }
            }

            return result;
        }

    }

    public struct InlineSegment
    {
        public string Text;
        public Color Color;
        public bool Bold;
    }



    /* HUD may not work. Nothing below this.
           //public static void DrawHud(SpriteBatch spriteBatch)
           //{
           //    if (!Context.IsWorldReady)
           //        return;
           //    //ModEntry.Instance.Monitor.Log("HERE1", LogLevel.Info);
           //    Item? hovered = GetHudHoveredItem();
           //    if (hovered is not StardewValley.Object obj)
           //        return;

           //    ModEntry.Instance.Monitor.Log("HERE2", LogLevel.Info);


           //    var info = PlantDatabase.FromItem(obj);
           //    if (info is null)
           //        return;

           //    DrawTooltip(spriteBatch, info);
           //}

           //private static Item? GetHudHoveredItem()
           //{
           //    // 1. Toolbar hover (most common)
           //    if (Game1.player.CurrentItem != null)
           //    return Game1.player.CurrentItem;

           //    // 2. On-screen menus (buffs, etc.)
           //    foreach (var menu in Game1.onScreenMenus)
           //    {
           //        switch (menu)
           //        {
           //            case ItemGrabMenu grab when grab.hoveredItem is Item item1:
           //                //ModEntry.Instance.Monitor.Log("HERE5", LogLevel.Info);

           //                return item1;

           //            case InventoryPage inv when inv.hoveredItem is Item item2:
           //                ModEntry.Instance.Monitor.Log("HERE6", LogLevel.Info);

           //                return item2;
           //        }
           //    }

           //    return null;
           //}
           */





}

//// Set the season color and bold if it matches the season
//private static (Color color, bool bold) GetSeasonStyle(string season)
//{
//    bool isCurrent = season.Equals(Game1.currentSeason, StringComparison.OrdinalIgnoreCase);

//    Color color = season switch
//    {
//        "Spring" => TooltipColors.SpringColor,
//        "Summer" => TooltipColors.SummerColor,
//        "Fall" => TooltipColors.FallColor,
//        "Winter" => TooltipColors.WinterColor,
//        _ => TooltipColors.Normal
//    };

//    // If it's not the current season, force black + not bold
//    if (!isCurrent)
//        return (TooltipColors.Normal, false);

//    // If it IS the current season, return the season color + bold
//    return (color, true);
//}

