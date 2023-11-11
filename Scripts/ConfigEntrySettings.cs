using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using BepInEx.Configuration;
using JetBrains.Annotations;

namespace MikesTweaks.Scripts
{
    public class ConfigEntrySettings<T>
    {
        public ConfigEntrySettings(string name, T defaultValue, T vanillaValue)
        {
            ConfigName = name;
            DefaultValue = defaultValue;
            VanillaValue = vanillaValue;
        }

        public T Value => Entry.Value;

        public T DefaultValue { get; private set;}
        public string ConfigName { get; private set; }
        public string ConfigDesc => $"Vanilla Default: {VanillaValue}";
        public ConfigEntry<T> Entry { private get; set; }
    
        private readonly T VanillaValue;

    }
}
