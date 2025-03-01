using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Başlık bileşeni - Uygulamanın üst kısmında yer alan başlık çubuğu, logo, havaalanı kodu ve saat
    /// </summary>
    public class HeaderComponent : BaseUIComponent
    {
        private TextBlock _timeDisplay;
        
        public HeaderComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                             ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            // Referans görüntüye göre turkuaz/mavi arka plan
            Border headerBorder = new Border();
            headerBorder.Background = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
            headerBorder.Height = 60;
            
            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Sol
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Orta
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Sağ
            
            // Sol taraf - VAISALA logo/isim
            TextBlock vaisalaText = new TextBlock();
            vaisalaText.Text = "VAISALA";
            vaisalaText.FontSize = 20;
            vaisalaText.FontWeight = FontWeights.Bold;
            vaisalaText.Foreground = Brushes.White;
            vaisalaText.VerticalAlignment = VerticalAlignment.Center;
            vaisalaText.HorizontalAlignment = HorizontalAlignment.Left;
            vaisalaText.Margin = new Thickness(20, 0, 0, 0);
            
            // Orta kısım - Havaalanı kodu (LTBL)
            TextBlock airportText = new TextBlock();
            airportText.Text = "LTBL"; // Referans ekran görüntüsüne göre
            airportText.FontSize = 20;
            airportText.FontWeight = FontWeights.Bold;
            airportText.Foreground = Brushes.White;
            airportText.VerticalAlignment = VerticalAlignment.Center;
            airportText.HorizontalAlignment = HorizontalAlignment.Center;
            
            // Sağ taraf - Saat göstergesi (HH:MM:SS)
            _timeDisplay = new TextBlock();
            _timeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            _timeDisplay.FontSize = 20;
            _timeDisplay.FontWeight = FontWeights.Bold;
            _timeDisplay.Foreground = Brushes.White;
            _timeDisplay.VerticalAlignment = VerticalAlignment.Center;
            _timeDisplay.HorizontalAlignment = HorizontalAlignment.Right;
            _timeDisplay.Margin = new Thickness(0, 0, 20, 0);
            
            // Saat göstergesini kaydet
            RegisterTextElement(_timeDisplay, "time");
            
            // Zamanlayıcı ekle - saat güncelleme
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateClock();
            timer.Start();
            
            Grid.SetColumn(vaisalaText, 0);
            Grid.SetColumn(airportText, 1);
            Grid.SetColumn(_timeDisplay, 2);
            
            headerGrid.Children.Add(vaisalaText);
            headerGrid.Children.Add(airportText);
            headerGrid.Children.Add(_timeDisplay);
            
            headerBorder.Child = headerGrid;
            
            Grid.SetRow(headerBorder, row);
            Grid.SetColumn(headerBorder, column);
            Grid.SetColumnSpan(headerBorder, 3); // Tüm sütunlara yay
            
            parent.Children.Add(headerBorder);
            
            return headerBorder;
        }
        
        /// <summary>
        /// Saat göstergesini günceller
        /// </summary>
        private void UpdateClock()
        {
            if (_timeDisplay != null)
            {
                _timeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            }
        }
        
        /// <summary>
        /// Saat değerini manuel olarak günceller
        /// </summary>
        public void UpdateTime(string time)
        {
            if (_timeDisplay != null)
            {
                _timeDisplay.Text = time;
            }
        }
    }
}