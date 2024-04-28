using System;
using HeadWindCSS.Domains.Serialization;
using UnityEngine;

namespace HeadWindCSS.Domains.Settings.Theme
{
    public enum ThemeSettingType
    {
        Value,
        Object
    }
    
    /// <summary>
    /// Define your setting here
    /// for example
    /// 'white': '#ffffff',
    /// 'tahiti': {
    ///    100: '#cffafe',
    ///    200: '#a5f3fc',
    ///    300: '#67e8f9',
    ///    400: '#22d3ee',
    ///    500: '#06b6d4',
    ///    600: '#0891b2',
    ///    700: '#0e7490',
    ///    800: '#155e75',
    ///    900: '#164e63',
    /// </summary>
    [Serializable]
    public class ThemeSetting
    {
        public ThemeSettingType Type => type;
        public string Key => key;
        public Color Value => value;
        
        [SerializeField] private ThemeSettingType type;
        [SerializeField] private string key;
        [SerializeField] private Color value;
        
        public ThemeSetting(ThemeSettingType type, string key, Color value)
        {
            this.type = type;
            this.key = key;
            this.value = value;
        }
    }
    
    [Serializable]
    public class ThemeSettingObject : ThemeSetting
    {
        public new SerializableDictionary<string, Color> Value => value;
        
        [SerializeField]
        private SerializableDictionary<string, Color> value;
        
        public ThemeSettingObject(
            ThemeSettingType type, string key, SerializableDictionary<string, Color> value) : base(type, key, Color.clear)
        {
            this.value = value;
        }
    }
}