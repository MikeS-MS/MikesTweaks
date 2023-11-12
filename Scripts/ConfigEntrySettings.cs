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
        public ConfigEntrySettings(string name, T defaultValue, T vanillaValue, string extraDescription = "")
        {
            ConfigName = name;
            DefaultValue = defaultValue;
            VanillaValue = vanillaValue;
            ExtraDescription = extraDescription;
        }

        public T Value => Entry.Value;

        public T DefaultValue { get; private set;}
        public string ConfigName { get; private set; }
        
        public string ConfigDesc => $"Vanilla Default: {VanillaValue}\n{ExtraDescription}";
        public ConfigEntry<T> Entry { private get; set; }
    
        private readonly T VanillaValue;

        private readonly string ExtraDescription;

    }
}
