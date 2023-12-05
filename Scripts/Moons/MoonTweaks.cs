using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MikesTweaks.Scripts.Configs;
using MikesTweaks.Scripts.Networking;
using MikesTweaks.Scripts.World;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

namespace MikesTweaks.Scripts.Moons
{
    public class MoonTweaks
    {
        public static class Configs
        {
            public static string MoonPricesHeader => "MoonPrices";

            public static readonly Dictionary<string, int> DefaultMoonCosts = new Dictionary<string, int>()
            {
                {"Experimentation", 0},
                {"Assurance", 0},
                {"Vow", 0},
                {"Offense", 0},
                {"March", 0},
                {"Rend", 550},
                {"Dine", 600},
                {"Titan", 700}
            };

            public static ConfigEntrySettings<string> MoonPrices = new ConfigEntrySettings<string>("MoonPrices", JsonConvert.SerializeObject(DefaultMoonCosts), JsonConvert.SerializeObject(DefaultMoonCosts), "To change the cost to go to a planet you can change the amount corresponding to the moon you want to modify.\nYou can also modify the cost of moons from different mods here, by just adding another entry anywhere in the dictionary with the planet's name and the cost you want it to be\nThe value is a json string which is why you see \\ everywhere before \".\nTo add another moon, just add , after Titan's value and write it like so \\\"MoonName\\\":Value");

            public static Dictionary<string, int> MoonPricesDeserialized;

            public static int MoonPricesSize => FastBufferWriter.GetWriteSize(ConfigsSynchronizer.ToBytes(JsonConvert.SerializeObject(MoonPricesDeserialized)));
        }

        public static void RegisterConfigs()
        {
            MikesTweaks.Instance.BindConfig(ref Configs.MoonPrices, Configs.MoonPricesHeader);
            MikesTweaks.Instance.Config.ConfigReloaded += new EventHandler((object sender, EventArgs a) =>
            {
                ReadMoonPrices();
                ApplyVanillaMoonCosts(ref Configs.MoonPricesDeserialized, WorldTweaks.Configs.UseVanillaMoonCosts.Value());
            });

            ReadMoonPrices();
            ApplyVanillaMoonCosts(ref Configs.MoonPricesDeserialized, WorldTweaks.Configs.UseVanillaMoonCosts.Value());

            ConfigsSynchronizer.OnConfigsChangedDelegate += () => ReapplyConfigs(WorldTweaks.TerminalInstance);
            ConfigsSynchronizer.Instance.AddConfigSizeGetter(() => Configs.MoonPricesSize);
            ConfigsSynchronizer.Instance.AddConfigGetter(SendConfigs);
            ConfigsSynchronizer.Instance.AddConfigSetter(OnConfigsReceived);
        }

        public static FastBufferWriter SendConfigs(FastBufferWriter writer)
        {
            byte[] MoonPrices = ConfigsSynchronizer.ToBytes(JsonConvert.SerializeObject(Configs.MoonPricesDeserialized));
            writer.WriteValueSafe(MoonPrices);

            return writer;
        }

        public static FastBufferReader OnConfigsReceived(FastBufferReader payload)
        {
            payload.ReadValueSafe(out byte[] moonPricesBytes);
            Configs.MoonPricesDeserialized = JsonConvert.DeserializeObject<Dictionary<string, int>>((string)ConfigsSynchronizer.ToObject(moonPricesBytes));

            return payload;
        }

        public static void ReapplyConfigs(Terminal terminal)
        {
            if (!terminal)
                return;

            TerminalKeyword Route = Array.Find(terminal.terminalNodes.allKeywords, (TerminalKeyword keyword) => keyword.name == "Route");

            if (Route == null)
                return;

            Dictionary<string, int> MoonCosts = new Dictionary<string, int>(Configs.MoonPricesDeserialized);


            foreach (var moon in Route.compatibleNouns)
            {
                foreach (var configMoon in MoonCosts)
                {
                    if (!moon.noun.name.Contains(configMoon.Key))
                        continue;

                    moon.result.itemCost = configMoon.Value;
                    foreach (var confirmAction in moon.result.terminalOptions)
                    {
                        if (confirmAction.noun.name.ToLower().Contains("deny"))
                            continue;

                        confirmAction.result.itemCost = configMoon.Value;
                        break;
                    }

                    MoonCosts.Remove(configMoon.Key);
                    break;
                }
            }
        }

        private static void ApplyVanillaMoonCosts(ref Dictionary<string, int> MoonCosts, bool vanilla)
        {
            if (!vanilla)
                return;

            foreach (var defaultMoon in Configs.DefaultMoonCosts)
                MoonCosts[defaultMoon.Key] = defaultMoon.Value;
        }

        private static void ReadMoonPrices()
        {
            Configs.MoonPricesDeserialized = JsonConvert.DeserializeObject<Dictionary<string, int>>(Configs.MoonPrices.Value());
        }
    }
}
