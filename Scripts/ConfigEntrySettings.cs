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

        public Type type => typeof(T);
        public T Value(bool vanilla = false)
        {
            return !vanilla ? Entry.Value : VanillaValue;
        }

        public T DefaultValue { get; private set;}
        public string ConfigName { get; private set; }
        
        public string ConfigDesc => $"{ExtraDescription}\nVanilla Default: {VanillaValue}";
        public ConfigEntry<T> Entry { get; set; }
    
        private readonly T VanillaValue;
        private readonly string ExtraDescription;


    }
}
