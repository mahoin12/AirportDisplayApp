using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Uygulama başlık bileşeni - Logo, havaalanı kodu ve saat göstergesi içerir
    /// </summary>
    public class HeaderComponent : BaseUIComponent
    {
        public HeaderComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                             ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            Border headerBorder = new Border();
            headerBorder.Background = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // VAISALA mavi
            headerBorder.Height = 60;

            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Sol taraf - VAISALA logosu/yazısı
            TextBlock vaisalaText = new TextBlock();
            vaisalaText.Text = "VAISALA";
            vaisalaText.FontSize = 20;
            vaisalaText.FontWeight = FontWeights.Bold;
            vaisalaText.Foreground = Brushes.White;
            vaisalaText.VerticalAlignment = VerticalAlignment.Center;
            vaisalaText.HorizontalAlignment = HorizontalAlignment.Left;
            vaisalaText.Margin = new Thickness(20, 0, 0, 0);

            // Orta kısım - Havaalanı kodu
            TextBlock airportText = new TextBlock();
            airportText.Text = "LTBL";
            airportText.FontSize = 20;
            airportText.FontWeight = FontWeights.Bold;
            airportText.Foreground = Brushes.White;
            airportText.VerticalAlignment = VerticalAlignment.Center;
            airportText.HorizontalAlignment = HorizontalAlignment.Center;

            // Sağ taraf - Saat göstergesi
            TextBlock timeText = new TextBlock();
            timeText.Text = DateTime.Now.ToString("HH:mm:ss");
            timeText.FontSize = 20;
            timeText.FontWeight = FontWeights.Bold;
            timeText.Foreground = Brushes.White;
            timeText.VerticalAlignment = VerticalAlignment.Center;
            timeText.HorizontalAlignment = HorizontalAlignment.Right;
            timeText.Margin = new Thickness(0, 0, 20, 0);
            
            RegisterTextElement(timeText, "time");

            Grid.SetColumn(vaisalaText, 0);
            Grid.SetColumn(airportText, 1);
            Grid.SetColumn(timeText, 2);

            headerGrid.Children.Add(vaisalaText);
            headerGrid.Children.Add(airportText);
            headerGrid.Children.Add(timeText);

            headerBorder.Child = headerGrid;
            Grid.SetRow(headerBorder, row);
            Grid.SetColumnSpan(headerBorder, 3); // Tüm sütunlara yayılır
            parent.Children.Add(headerBorder);

            return headerBorder;
        }

        /// <summary>
        /// Saati günceller
        /// </summary>
        public void UpdateTime(string time)
        {
            UpdateTextValue("time", time);
        }
    }
}