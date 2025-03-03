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
            headerBorder.Background = new SolidColorBrush(Color.FromRgb(0, 160, 198)); // Türkiz/mavi (referans görüntüye göre ayarlandı)
            headerBorder.Height = 60;
            
            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // Sol
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // Orta
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // Sağ
            
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
            StackPanel timePanel = new StackPanel();
            timePanel.Orientation = Orientation.Horizontal;
            timePanel.HorizontalAlignment = HorizontalAlignment.Right;
            timePanel.VerticalAlignment = VerticalAlignment.Center;
            timePanel.Margin = new Thickness(0, 0, 20, 0);
            
            _timeDisplay = new TextBlock();
            _timeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            _timeDisplay.FontSize = 20;
            _timeDisplay.FontWeight = FontWeights.Bold;
            _timeDisplay.Foreground = Brushes.White;
            _timeDisplay.VerticalAlignment = VerticalAlignment.Center;
            
            // UTC etiketi - ilk resimde olduğu gibi
            TextBlock utcLabel = new TextBlock();
            utcLabel.Text = "UTC";
            utcLabel.FontSize = 14;
            utcLabel.Foreground = Brushes.White;
            utcLabel.VerticalAlignment = VerticalAlignment.Center;
            utcLabel.Margin = new Thickness(5, 0, 0, 0);
            
            timePanel.Children.Add(_timeDisplay);
            timePanel.Children.Add(utcLabel);
            
            // Saat göstergesini kaydet
            RegisterTextElement(_timeDisplay, "time");
            
            // Zamanlayıcı ekle - saat güncelleme
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateClock();
            timer.Start();
            
            // Sağ taraftaki ek göstergeler (varsa) - referans görüntüye göre
            StackPanel rightPanel = new StackPanel();
            rightPanel.Orientation = Orientation.Horizontal;
            rightPanel.HorizontalAlignment = HorizontalAlignment.Right;
            rightPanel.VerticalAlignment = VerticalAlignment.Center;
            rightPanel.Margin = new Thickness(0, 0, 20, 0);
            
            // İcon göstergeleri (referans görüntüdeki gibi)
            StackPanel iconsPanel = new StackPanel();
            iconsPanel.Orientation = Orientation.Horizontal;
            iconsPanel.Margin = new Thickness(0, 0, 15, 0);
            
            // Daylight-Day/Night göstergesi
            TextBlock daylightLabel = new TextBlock();
            daylightLabel.Text = "Daylight";
            daylightLabel.FontSize = 12;
            daylightLabel.Foreground = Brushes.White;
            daylightLabel.VerticalAlignment = VerticalAlignment.Center;
            daylightLabel.Margin = new Thickness(0, 0, 10, 0);
            
            // Burada ikon göstergeleri oluşturulabilir (NEC logosu vs. referans resimde)
            
            // Elemanları Grid'e ekle
            Grid.SetColumn(vaisalaText, 0);
            Grid.SetColumn(airportText, 1);
            Grid.SetColumn(timePanel, 2);
            
            headerGrid.Children.Add(vaisalaText);
            headerGrid.Children.Add(airportText);
            headerGrid.Children.Add(timePanel);
            
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