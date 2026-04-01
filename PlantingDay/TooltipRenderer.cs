using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PlantingDay.Compatibility;
using PlantingDay.Helpers;
using PlantingDay.Models;
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


        private static List<TooltipElement> BuildTooltip(PlantInfo info)
        {
            var list = new List<TooltipElement>();

            list.AddRange(GetSeasonsTooltip(info));

            // Growth info (includes DaysToProduce + ready day + warnings + Multiharvest)
            list.AddRange(GetGrowthTooltip(info));




            if (info.Trellis)
                list.Add(new TooltipElement { Text = "Requires a trellis", TextColor = Color.Orange });


            //if (info.Paddy)
            //    list.Add(new TooltipElement
            //    {
            //        Icon = TooltipIcons.WaterSeeds,
            //        Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.MultiSprite),
            //            info.MultiSprite
            //            ),
            //        TextColor = TooltipColors.Paddy
            //    });

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

        // How many grow seasons after this one (stops after a full year)
        private static int CountAdditionalSeasons(Season current, List<Season> allowed)
        {
            if (allowed == null || allowed.Count == 0)
                return 0;

            int count = 0;
            Season s = NextSeason(current);

            // Maximum of 3 additional seasons in a year
            for (int i = 0; i < 3; i++)
            {
                if (!allowed.Contains(s))
                    break;

                count++;
                s = NextSeason(s);
            }

            return count;

        }


        //----------------
        // How long to grow and when is it ready
        //----------------

        private static GrowthContext BuildGrowthContext(PlantInfo info, List<Season> seasons)
        {
            int today = Game1.dayOfMonth;
            Season currentSeason = Game1.season;
            Season nextSeason = NextSeason(currentSeason);

            int days = info.DaysToProduce ?? 0;

            int readyDay = today + days;

            int paddyDays = (int)Math.Floor(days / 1.25f);
            int paddyReadyDay = today + paddyDays;

            int additionalSeasons = CountAdditionalSeasons(currentSeason, seasons);
            int regrowDaysAvailable = 0;
            int paddyRegrowDaysAvailable = 0;
            int regrowQty = 0;
            int paddyRegrowQty = 0;

            // Regrow days for current season and additonal seasons for multi-harvest crops.
            if (info.RegrowDays > 0 && seasons.Contains(currentSeason))
                regrowDaysAvailable =
                    ((1 + additionalSeasons) * 28) - readyDay;
            
            // Regrow days for out of season
            if (info.RegrowDays > 0 && !seasons.Contains(currentSeason))
                regrowDaysAvailable = 28 * seasons.Count - days;

            paddyRegrowDaysAvailable = (int)Math.Floor(regrowDaysAvailable / 1.25);

            double r = (double)regrowDaysAvailable / info.RegrowDays.Value;
            regrowQty = 1 + (int)Math.Floor(r);

            r = (double)paddyRegrowDaysAvailable / info.RegrowDays.Value;
            paddyRegrowQty = 1 + (int)Math.Floor(r);

            return new GrowthContext
                {
                    Today = today,
                    CurrentSeason = currentSeason,
                    NextSeason = nextSeason,

                    ProduceDay = days,
                    PaddyProduceDay = paddyDays,

                    ReadyDay = readyDay,
                    PaddyReadyDay = paddyReadyDay,

                    OverflowDay = readyDay - 28,
                    PaddyOverflowDay = paddyReadyDay - 28,

                    //RegrowDaysAvailable = 
          rew
                    //AdditionalSeasons = CountAdditionalSeasons(currentSeason, seasons)
                };
            }

        private static List<TooltipElement> GetGrowthTooltip(PlantInfo info)
        {
            var list = new List<TooltipElement>();
            if (info.DaysToProduce <= 0)
                return list;

            //if (info.PlantType == PlantType.Crop)
            //    list.AddRange(GetCropGrowthTooltip(info));

            // build season list for the seed
            List<Season> seasons = info.Seasons
                .Select(s => Enum.Parse<Season>(s, ignoreCase: true))
                .ToList();

            // build growth context (e.g. when is it ready)
            GrowthContext growth = BuildGrowthContext(info, seasons);


            //int today = Game1.dayOfMonth; // current day in the game
            //Season currentSeason = Game1.season; //current season in the game
            //Season nextSeason = NextSeason(currentSeason); //next season in the game

            //int days = info.DaysToProduce ?? 0; // how long for the crop to grow
            //int readyDay = today + (info.DaysToProduce ?? 0); // what day is the crop ready

            //int paddyDays = (int)Math.Floor((info.DaysToProduce ?? 0) / 1.25); // paddy crops grown faster
            //int paddyReadyDay = today + paddyDays; // what day is the crop ready

            //int overflowDay = readyDay - 28; //ready day if multiseason crop is ready next season
            //int paddyOverflowDay = paddyReadyDay - 28;

            //int? regrowDaysAvailable = null; // how many days are available for regrowth
            //int? regrowQty = 0; //how many times will it regrow, including the first harvest
            //int? additionalSeasons = CountAdditionalSeasons(currentSeason, seasons); // how many regrow seasons


            //----------
            // First / only harvest
            //----------
            ModEntry.Instance.Monitor.Log($"  ready day: {growth.Today}", LogLevel.Info);

            if (!seasons.Contains(growth.CurrentSeason)) // out of season
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.DaysToProduce),
                        growth.ProduceDay
                    ),
                    TextColor = TooltipColors.Normal
                });

                if (info.Paddy)
                    list.Add(new TooltipElement
                    {
                        Icon = TooltipIcons.WaterSeeds,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.PaddyDaysToProduce),
                            growth.PaddyProduceDay
                            ),
                        TextColor = TooltipColors.Paddy
                    });
            }

            else if (growth.ReadyDay <= 28) // ready in the current season
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.ReadyOn),
                        info.DaysToProduce,
                        growth.CurrentSeason,
                        growth.ReadyDay),
                    TextColor = TooltipColors.Normal
                });

                if (info.Paddy)
                    list.Add(new TooltipElement
                    {
                        Icon = TooltipIcons.WaterSeeds,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.PaddyReadyOn),
                            growth.PaddyProduceDay,
                            growth.CurrentSeason,
                            growth.PaddyReadyDay
                            ),
                        TextColor = TooltipColors.Paddy
                    });
            }
            else if (seasons.Contains(growth.NextSeason)) // Ready next season
            {
                list.Add(new TooltipElement
                {
                    Text = string.Format(ModEntry.ModHelper.Translation
                        .Get(TooltipKeys.ReadyOn),
                        growth.ReadyDay,
                        growth.NextSeason,
                        growth.OverflowDay
                    ),
                    TextColor = TooltipColors.Normal
                });

                if (info.Paddy)
                    list.Add(new TooltipElement
                    {
                        Icon = TooltipIcons.WaterSeeds,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.ReadyOn),
                            growth.PaddyReadyDay,
                            growth.NextSeason,
                            growth.PaddyOverflowDay
                    ),
                        TextColor = TooltipColors.Paddy
                    });
            }

            else // too late to plant
            {
                if (info.PlantType == PlantType.Crop)
                {
                    // x days
                    list.Add(new TooltipElement
                    {
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.DaysToProduce),
                            info.DaysToProduce
                        ),
                        TextColor = TooltipColors.Normal
                    });

                    if (info.Paddy)
                        list.Add(new TooltipElement
                        {
                            Icon = TooltipIcons.WaterSeeds,
                            Text = string.Format(ModEntry.ModHelper.Translation
                                .Get(TooltipKeys.PaddyDaysToProduce),
                                growth.PaddyReadyDay
                                ),
                            TextColor = TooltipColors.Paddy
                        });


                    //warning
                    list.Add(new TooltipElement
                    {
                        Icon = TooltipIcons.Warning,
                        Text = string.Format(ModEntry.ModHelper.Translation
                            .Get(TooltipKeys.TooLate)
                            ),
                        TextColor = TooltipColors.Warning

                    });

                    if (info.Paddy)
                        list.Add(new TooltipElement
                        {
                            Icon = TooltipIcons.Warning,
                            Text = string.Format(ModEntry.ModHelper.Translation
                                .Get(TooltipKeys.PaddyTooLate)
                                ),
                            TextColor = TooltipColors.Warning
                        });
                }

            }


            //----------
            // Multiharvest Crops
            //----------


            //ModEntry.Instance.Monitor.Log($"  add seasons: {additionalSeasons}", LogLevel.Info);

            if (info.PlantType == PlantType.Crop)
            {

                // Regrow amount left when planting now
                if (info.RegrowDays > 0 && seasons.Contains(growth.CurrentSeason))
                {
                    regrowDaysAvailable =
                        ((1 + growth.AdditionalSeasons) * 28) - growth.ReadyDay;

                    if (regrowDaysAvailable > 0)
                    {
                        regrowQty = 1 + regrowDaysAvailable / info.RegrowDays; //first harvest plus regrows

                        list.Add(new TooltipElement
                        {
                            Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                                growth.RegrowQty
                            ),
                            TextColor = TooltipColors.Normal
                        });
                    }
                }

                // Grow days left when planting a whole season(s)
                if (info.RegrowDays > 0 && !seasons.Contains(growth.CurrentSeason))
                {
                    regrowDaysAvailable = 28 * info.Seasons.Count - info.DaysToProduce;

                    if (regrowDaysAvailable >= 0)
                    {
                        regrowQty = 1 + regrowDaysAvailable / info.RegrowDays; //first harvest plus regrows
                    }

                    list.Add(new TooltipElement
                    {
                        Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                            regrowQty
                        ),
                        TextColor = TooltipColors.Normal
                    });
                }

            }

            //----------
            // Fruit Trees
            // Multi harvest, grow out of season
            //----------


            ModEntry.Instance.Monitor.Log($"  season: {string.Join(", ", seasons)}", LogLevel.Info);
            ModEntry.Instance.Monitor.Log($"  plant type: {info.PlantType}", LogLevel.Info);

            if (info.PlantType == PlantType.FruitTree)
            {
                //When is the tree ready. Same day next month.

                // If next month is the tree's season, it will produce next season on the same day
                if (seasons.Contains(growth.NextSeason))
                {
                    ModEntry.Instance.Monitor.Log($"  ready day: {growth.Today}", LogLevel.Info);

                    list.Add(new TooltipElement
                    {
                        Text = string.Format(ModEntry.ModHelper.Translation
                                            .Get(TooltipKeys.ReadyOn),
                                            growth.ProduceDay,
                                            growth.NextSeason,
                                            growth.Today
                                        ),
                        TextColor = TooltipColors.Normal
                    });
                }

                // If next month is NOT the tree's season, it will be fully ready the next in-season


                // Regrow amount left when planting now
                if (info.RegrowDays > 0 && seasons.Contains(growth.CurrentSeason))
                {
                    regrowDaysAvailable =
                        ((1 + growth.AdditionalSeasons) * 28) - growth.ReadyDay;

                    if (regrowDaysAvailable > 0)
                    {
                        regrowQty = 1 + regrowDaysAvailable / info.RegrowDays; //first harvest plus regrows

                        list.Add(new TooltipElement
                        {
                            Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                                regrowQty
                            ),
                            TextColor = TooltipColors.Normal
                        });
                    }
                }

                // Grow days left when planting a whole season(s)
                if (info.RegrowDays > 0 && !seasons.Contains(growth.CurrentSeason))
                {
                    regrowDaysAvailable = 28 * info.Seasons.Count - info.DaysToProduce;

                    if (regrowDaysAvailable >= 0)
                    {
                        regrowQty = 1 + regrowDaysAvailable / info.RegrowDays; //first harvest plus regrows
                    }

                    list.Add(new TooltipElement
                    {
                        Text = string.Format(ModEntry.ModHelper.Translation.Get(TooltipKeys.RegrowQty),
                            regrowQty
                        ),
                        TextColor = TooltipColors.Normal
                    });
                }

            }

            else
                list.Add(new TooltipElement
                {
                    Icon = null,
                    Text = ""
                });

            return list;



        }

        //private static List<TooltipElement> GetCropGrowthTooltip(PlantInfo info)
        //{


        //}





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

                    //Item? hovered = invPage?.hoveredItem;
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
            const int IconRenderSize = 32;


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
                    lineWidth += IconRenderSize + 4;

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

