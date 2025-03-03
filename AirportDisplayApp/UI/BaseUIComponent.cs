using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Tüm UI bileşenlerinin türetildiği temel sınıf
    /// </summary>
    public abstract class BaseUIComponent
    {
        protected readonly Window _owner;
        protected Dictionary<string, TextBlock> _displayElements;
        protected ResourceDictionary _styles;

        protected BaseUIComponent(Window owner, Dictionary<string, TextBlock> displayElements, ResourceDictionary styles)
        {
            _owner = owner;
            _displayElements = displayElements;
            _styles = styles;
        }

        /// <summary>
        /// Bileşeni oluşturup döndüren ana metot
        /// </summary>
        public abstract FrameworkElement Create(Grid parent, int row, int column);

        /// <summary>
        /// TextBlock öğesine benzersiz bir isim vererek kaydet ve görüntüleme sözlüğüne ekle
        /// </summary>
        protected void RegisterTextElement(TextBlock element, string key)
        {
            if (element != null && !string.IsNullOrEmpty(key))
            {
                // Her bileşen sınıfı için benzersiz bir önek oluştur
                string componentPrefix = this.GetType().Name.Replace("Component", "");
                string uniqueName = $"{componentPrefix}_{key}Value";
                
                try
                {
                    element.Name = uniqueName;
                    _owner.RegisterName(uniqueName, element);
                    _displayElements[key] = element;
                }
                catch (ArgumentException)
                {
                    // Eğer isim zaten kullanımdaysa, rastgele bir sayı ekleyerek tekrar dene
                    string altName = $"{uniqueName}_{Guid.NewGuid().ToString().Substring(0, 8)}";
                    element.Name = altName;
                    _owner.RegisterName(altName, element);
                    _displayElements[key] = element;
                }
            }
        }

        /// <summary>
        /// TextBlock'un değerini güncelle
        /// </summary>
        public void UpdateTextValue(string key, string value)
        {
            if (_displayElements.TryGetValue(key, out TextBlock element))
            {
                element.Text = value;
            }
        }
    }
}