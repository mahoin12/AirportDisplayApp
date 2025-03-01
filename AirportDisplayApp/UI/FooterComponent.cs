using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Alt bilgi çubuğu bileşeni - Bağlantı durumu ve kontrol butonları
    /// </summary>
    public class FooterComponent : BaseUIComponent
    {
        // Button click olayları
        public event EventHandler RefreshClicked;
        public event EventHandler SettingsClicked;

        public FooterComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                             ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            Border footerBorder = new Border();
            footerBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            footerBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            footerBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            footerBorder.Height = 30;

            Grid footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Sol taraf - Bağlantı durumu
            TextBlock connectionStatus = new TextBlock();
            connectionStatus.Text = "Bağlantı Durumu: Bağlanıyor...";
            connectionStatus.FontSize = 12;
            connectionStatus.VerticalAlignment = VerticalAlignment.Center;
            connectionStatus.Margin = new Thickness(10, 0, 0, 0);
            
            RegisterTextElement(connectionStatus, "connectionStatus");

            // Sağ taraf - Butonlar
            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonPanel.Margin = new Thickness(0, 0, 10, 0);

            Button refreshButton = new Button();
            refreshButton.Content = "Verileri Yenile";
            refreshButton.Padding = new Thickness(10, 3, 10, 3);
            refreshButton.Margin = new Thickness(5, 0, 5, 0);
            refreshButton.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
            refreshButton.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
            refreshButton.BorderThickness = new Thickness(1);
            refreshButton.Click += (s, e) => RefreshClicked?.Invoke(this, EventArgs.Empty);

            Button settingsButton = new Button();
            settingsButton.Content = "Ayarlar";
            settingsButton.Padding = new Thickness(10, 3, 10, 3);
            settingsButton.Margin = new Thickness(5, 0, 0, 0);
            settingsButton.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
            settingsButton.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
            settingsButton.BorderThickness = new Thickness(1);
            settingsButton.Click += (s, e) => SettingsClicked?.Invoke(this, EventArgs.Empty);

            buttonPanel.Children.Add(refreshButton);
            buttonPanel.Children.Add(settingsButton);

            Grid.SetColumn(connectionStatus, 0);
            Grid.SetColumn(buttonPanel, 1);

            footerGrid.Children.Add(connectionStatus);
            footerGrid.Children.Add(buttonPanel);

            footerBorder.Child = footerGrid;

            Grid.SetRow(footerBorder, row);
            Grid.SetColumnSpan(footerBorder, 3); // Tüm sütunlara yayılır
            parent.Children.Add(footerBorder);

            return footerBorder;
        }

        /// <summary>
        /// Bağlantı durumunu güncelle
        /// </summary>
        public void UpdateConnectionStatus(string status)
        {
            UpdateTextValue("connectionStatus", $"Bağlantı Durumu: {status}");
        }
    }
}