using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Objects;
using System.Globalization;
using Object = StardewValley.Object;

namespace LightMod
{
    public partial class ModEntry
    {
        /// <summary>
        /// Added to accomodate 1.6 new id schema
        /// </summary>
        /// <param name="obj">Object to be identified</param>
        /// <returns>sharedLights Id</returns>
        public string GetObjectIdent(Object obj)
        {
            return obj.GenerateLightSourceId(obj.tileLocation.Value);
        }
        private void ChangeLightAlpha(NPC value, int delta)
        {
            if (!lightDataDict.TryGetValue(value.Name, out var data))
                return;
            suppressingScroll = true;
            int shiftAmount = Config.AlphaAmount;
            if (Helper.Input.IsDown(Config.ModButton1))
            {
                shiftAmount = Config.Alpha1Amount;
            }
            else if (Helper.Input.IsDown(Config.ModButton2))
            {
                shiftAmount = Config.Alpha2Amount;
            }
            shiftAmount *= (delta > 0 ? 1 : -1);
            if (value.modData.TryGetValue(alphaKey, out string alphaString) && int.TryParse(alphaString, out int oldAlpha))
            {
                shiftAmount += oldAlpha;
            }
            else
            {
                shiftAmount += data.color.A;
            }
            shiftAmount = Math.Max(0, Math.Min(255, shiftAmount));
            value.modData[alphaKey] = shiftAmount.ToString();
            SMonitor.Log($"Set alpha to {shiftAmount}");
        }
        private void ChangeLightRadius(NPC value, int delta)
        {
            if (!lightDataDict.TryGetValue(value.Name, out var data))
                return;
            float shiftAmount = Config.RadiusAmount;
            if (Helper.Input.IsDown(Config.ModButton1))
            {
                shiftAmount = Config.Radius1Amount;
            }
            else if (Helper.Input.IsDown(Config.ModButton2))
            {
                shiftAmount = Config.Radius2Amount;
            }
            shiftAmount *= (delta > 0 ? 1 : -1);

            if (value.modData.TryGetValue(radiusKey, out string radiusString) && int.TryParse(radiusString, out int oldRad))
            {
                shiftAmount += oldRad;
            }
            else
            {
                shiftAmount += data.radius;
            }
            shiftAmount = (float)Math.Max(0, shiftAmount);

            value.modData[radiusKey] = shiftAmount.ToString();
            SMonitor.Log($"Set radius to {shiftAmount}");
        }

        private void ChangeTileAlpha(Object value, string ident, int delta, Vector2 lightPosition, bool appyShift = true)
        {
            if (Game1.currentLocation.sharedLights.TryGetValue(ident, out LightSource l))
            {
                suppressingScroll = true;
                int shiftAmount;
                if (appyShift)
                {
                    shiftAmount = Config.AlphaAmount;
                    if (Helper.Input.IsDown(Config.ModButton1))
                    {
                        shiftAmount = Config.Alpha1Amount;
                    }
                    else if (Helper.Input.IsDown(Config.ModButton2))
                    {
                        shiftAmount = Config.Alpha2Amount;
                    }
                    shiftAmount *= (delta > 0 ? 1 : -1);
                    if (value.modData.TryGetValue(alphaKey, out string alphaString) && int.TryParse(alphaString, out int oldAlpha))
                    {
                        shiftAmount += oldAlpha;
                    }
                    else
                    {
                        shiftAmount += l.color.A;
                    }
                    shiftAmount = Math.Max(0, Math.Min(255, shiftAmount));
                    value.modData[alphaKey] = shiftAmount.ToString();
                }
                else
                {
                    if (value.modData.TryGetValue(alphaKey, out string alphaString) && int.TryParse(alphaString, out int oldAlpha))
                    {
                        shiftAmount = oldAlpha;
                    }
                    else
                    {
                        shiftAmount = l.color.A;
                    }
                }
                SMonitor.Log($"Set alpha to {shiftAmount}");
                Game1.currentLocation.removeLightSource(ident);

                if (value is FishTankFurniture tank)
                {
                    value.lightSource = new LightSource(ident, 8, lightPosition, 2f, Color.Black, LightSource.LightContext.None, 0L);
                    //__instance.lightSource = new LightSource(identifier, light.textureIndex, new Vector2(tileLocation.X  + light.offset.X, tileLocation.Y  + light.offset.Y), light.radius, light.color, LightSource.LightContext.None, 0L);
                    //value.isLamp.Value = light.isLamp;
                    value.lightSource.color.A = (byte)shiftAmount;
                    if (value.modData.TryGetValue(radiusKey, out string rstr) && float.TryParse(rstr, NumberStyles.Float, CultureInfo.InvariantCulture, out float radius))
                    {
                        value.lightSource.radius.Value = radius;
                        //SMonitor.Log($"New light radius: {__instance.lightSource.radius.Value}");
                    }
                    value.IsOn = true;
                    Game1.currentLocation.sharedLights.Add(ident, value.lightSource.Clone());
                }
                else
                {
                    value.initializeLightSource(lightPosition);
                }
            }
        }
        private void ChangeLightAlpha(Object value, int delta)
        {
            string ident = GetObjectIdent(value);
            if (value is FishTankFurniture tank)
            {
                bool addShift = true;
                Vector2 lightPosition = new Vector2(value.TileLocation.X * 64f + 32f + 2f, value.TileLocation.Y * 64f + 12f);
                for (int i = 0; i < tank.getTilesWide(); i++)
                {
                    ChangeTileAlpha(value, $"{ident}_tile{i}", delta, lightPosition, addShift);
                    lightPosition.X += 64f;
                    addShift = false;
                }
            }
            else
            {
                ChangeTileAlpha(value, ident, delta, value.TileLocation);
            }
        }
        private void ChangeLightRadius(Object value, int delta)
        {
            string ident = GetObjectIdent(value);
            if (value is FishTankFurniture)
            {
                // add fish tank suffix
                ident += "_tile0";
            }
            if (Game1.currentLocation.sharedLights.TryGetValue(ident, out LightSource l))
            {
                suppressingScroll = true;

                float shiftAmount = Config.RadiusAmount;
                if (Helper.Input.IsDown(Config.ModButton1))
                {
                    shiftAmount = Config.Radius1Amount;
                }
                else if (Helper.Input.IsDown(Config.ModButton2))
                {
                    shiftAmount = Config.Radius2Amount;
                }
                shiftAmount *= (delta > 0 ? 1 : -1);

                if (value.modData.TryGetValue(radiusKey, out string radiusString) && int.TryParse(radiusString, out int oldRad))
                {
                    shiftAmount += oldRad;
                }
                else
                {
                    shiftAmount += l.radius.Value;
                }
                shiftAmount = (float)Math.Max(0, shiftAmount);

                value.modData[radiusKey] = shiftAmount.ToString();
                SMonitor.Log($"Set radius to {shiftAmount}");
                if (value is FishTankFurniture tank) 
                { 
                    AddFishTankLights(tank);
                }
                else
                {
                    Game1.currentLocation.removeLightSource(ident);
                    value.initializeLightSource(value.TileLocation);
                }
            }
        }

        private void ChangeLightGlowAlpha(Vector2 light, int delta)
        {
            suppressingScroll = true;
            string key = $"{alphaKey}_{light.X}_{light.Y}";
            int shiftAmount = Config.AlphaAmount;
            if (Helper.Input.IsDown(Config.ModButton1))
            {
                shiftAmount = Config.Alpha1Amount;
            }
            else if (Helper.Input.IsDown(Config.ModButton2))
            {
                shiftAmount = Config.Alpha2Amount;
            }
            shiftAmount *= (delta > 0 ? 1 : -1);
            if (Game1.currentLocation.modData.TryGetValue(key, out string alphaString) && int.TryParse(alphaString, out int oldAlpha))
            {
                shiftAmount += oldAlpha;
            }
            else
            {
                shiftAmount += 255;
            }
            shiftAmount = Math.Max(0, Math.Min(255, shiftAmount));
            Game1.currentLocation.modData[key] = shiftAmount + "";
            SMonitor.Log($"Set light glow {light} alpha to {shiftAmount}");
        }

        private static Color GetLightGlowAlpha(GameLocation l, Vector2 light)
        {
            if (!Config.ModEnabled || !l.modData.TryGetValue($"{alphaKey}_{light.X}_{light.Y}", out string alphaString) || !int.TryParse(alphaString, out int alpha))
                return Color.White;
            return Color.White * (alpha / 255f);
        }


        private void ToggleLight(Object value)
        {
            if (value.modData.TryGetValue(switchKey, out string status))
            {
                if (status == "off")
                {
                    TurnOnLight(value);
                }
                else
                {
                    TurnOffLight(value);
                }
            }
            else
            {
                if (value.lightSource is null)
                {
                    //value.initializeLightSource(Game1.currentCursorTile);
                    value.initializeLightSource(value.TileLocation);
                    if (value.lightSource is not null)
                    {
                        Monitor.Log($"turning on {value.Name}");
                        value.modData[switchKey] = "on";
                        if (value is Furniture f)
                        {
                            f.addLights();
                        }
                        else
                        {
                            string ident = GetObjectIdent(value);// (value.TileLocation.X * 2000f + value.TileLocation.Y).ToString();
                            if (value.lightSource is not null && !Game1.currentLocation.hasLightSource(ident))
                                Game1.currentLocation.sharedLights[ident] = value.lightSource.Clone();
                        }
                    }
                }
                else
                {
                    TurnOffLight(value);
                }
            }
        }

        private void TurnOffLight(Object value)
        {
            Monitor.Log($"turning off {value.Name}");
            value.modData[switchKey] = "off";
            if (value.lightSource != null)
            {
                value.lightSource = null;
            }

            if (value is Furniture f)
            {
                f.removeLights();
            }
            else
            {
                Game1.currentLocation.removeLightSource(GetObjectIdent(value));
            }
        }

        private void TurnOnLight(Object value)
        {
            Monitor.Log($"turning on {value.Name}");
            value.modData[switchKey] = "on";

            if (lightDataDict.TryGetValue(value.Name, out var ldata))
            {
                value.initializeLightSource(new Vector2(value.TileLocation.X * 64 + ldata.offset.X, value.TileLocation.Y * 64 + ldata.offset.Y));
                string ident = GetObjectIdent(value);
                if (value.lightSource is not null && !Game1.currentLocation.hasLightSource(ident))
                    Game1.currentLocation.sharedLights[ident] = value.lightSource.Clone();
            }
            else if (value is FishTankFurniture tank)
            {
                AddFishTankLights(tank);
            }
            else if(value is Furniture f) 
            {
                f.addLights();
            }
            else
            {
                value.IsOn = true;
                value.initializeLightSource(value.TileLocation);
                string ident = GetObjectIdent(value);
                if (value.lightSource is not null && !Game1.currentLocation.hasLightSource(ident))
                    Game1.currentLocation.sharedLights[ident] = value.lightSource.Clone();
            }
        }
        /// <summary>
        /// Custom addLights for fish tanks to apply alpha
        /// and radius values at creation
        /// </summary>
        /// <param name="tank">FishTankFurniture to add lights to</param>
        private void AddFishTankLights(FishTankFurniture tank) 
        {
            string value = tank.GenerateLightSourceId(tank.TileLocation);
            Vector2 position = new Vector2(tank.tileLocation.X * 64f + 32f + 2f, tank.tileLocation.Y * 64f + 12f);
            for (int i = 0; i < tank.getTilesWide(); i++)
            {
                tank.lightSource = new LightSource($"{value}_tile{i}", 8, position, 2f, Color.Black, LightSource.LightContext.None, 0L, tank.Location.NameOrUniqueName);
                if (tank.modData.TryGetValue(alphaKey, out string astr) && int.TryParse(astr, out int alpha))
                {
                    tank.lightSource.color.A = (byte)alpha;
                    //SMonitor.Log($"New light alpha: {__instance.lightSource.color.A}");
                }
                if (tank.modData.TryGetValue(radiusKey, out string rstr) && float.TryParse(rstr, NumberStyles.Float, CultureInfo.InvariantCulture, out float radius))
                {
                    tank.lightSource.radius.Value = radius;
                    //SMonitor.Log($"New light radius: {__instance.lightSource.radius.Value}");
                }

                tank.Location.sharedLights.AddLight( tank.lightSource.Clone());
                position.X += 64f;
            }
        }
        private static int GetMorningLightTime()
        {
            /*
                switch (Game1.currentSeason)
                {
                    case "spring":
                        return Config.SpringMorningLightTime;
                    case "summer":
                        return Config.SummerMorningLightTime;
                    case "fall":
                        return Config.SummerMorningLightTime;
                    case "winter":
                        return Config.SummerMorningLightTime;
                }
            */
            return 0;
        }
        private static int GetNightDarkTime()
        {
            /*
            switch (Game1.currentSeason)
            {
                case "spring":
                    return Config.SpringDarkTime;
                case "summer":
                    return Config.SummerDarkTime;
                case "fall":
                    return Config.FallDarkTime;
                case "winter":
                    return Config.WinterDarkTime;
            }
        */
            return 1600;
        }
    }
}