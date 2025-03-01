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
        private TextBlock _connectionStatus;
        
        // Buton olayları
        public event EventHandler RefreshClicked;
        public event EventHandler SettingsClicked;
        
        public FooterComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                             ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            // Alt bilgi çubuğu
            Border footerBorder = new Border();
            footerBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)); // Açık gri
            footerBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            footerBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            footerBorder.Height = 30;
            
            Grid footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Bağlantı durumu için
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Butonlar için
            
            // Sol taraf - Bağlantı durumu
            _connectionStatus = new TextBlock();
            _connectionStatus.Text = "Bağlantı Durumu: Bağlanıyor...";
            _connectionStatus.FontSize = 12;
            _connectionStatus.VerticalAlignment = VerticalAlignment.Center;
            _connectionStatus.Margin = new Thickness(10, 0, 0, 0);
            
            RegisterTextElement(_connectionStatus, "connectionStatus");
            
            // Sağ taraf - Butonlar
            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonPanel.Margin = new Thickness(0, 0, 10, 0);
            
            // Yenile butonu
            Button refreshButton = CreateButton("Verileri Yenile");
            refreshButton.Click += (s, e) => RefreshClicked?.Invoke(this, EventArgs.Empty);
            
            // Ayarlar butonu
            Button settingsButton = CreateButton("Ayarlar");
            settingsButton.Click += (s, e) => SettingsClicked?.Invoke(this, EventArgs.Empty);
            
            // Butonlar arası boşluk
            settingsButton.Margin = new Thickness(5, 0, 0, 0);
            
            buttonPanel.Children.Add(refreshButton);
            buttonPanel.Children.Add(settingsButton);
            
            Grid.SetColumn(_connectionStatus, 0);
            Grid.SetColumn(buttonPanel, 1);
            
            footerGrid.Children.Add(_connectionStatus);
            footerGrid.Children.Add(buttonPanel);
            
            footerBorder.Child = footerGrid;
            
            Grid.SetRow(footerBorder, row);
            Grid.SetColumn(footerBorder, column);
            Grid.SetColumnSpan(footerBorder, 3); // Tüm sütunlara yay
            
            parent.Children.Add(footerBorder);
            
            return footerBorder;
        }
        
        /// <summary>
        /// Buton oluşturur
        /// </summary>
        private Button CreateButton(string text)
        {
            Button button = new Button();
            button.Content = text;
            button.Padding = new Thickness(10, 3, 10, 3);
            button.Margin = new Thickness(5, 0, 0, 0);
            button.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238)); // Açık gri
            button.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
            button.BorderThickness = new Thickness(1);
            
            return button;
        }
        
        /// <summary>
        /// Bağlantı durumunu günceller
        /// </summary>
        public void UpdateConnectionStatus(string status)
        {
            if (_connectionStatus != null)
            {
                _connectionStatus.Text = $"Bağlantı Durumu: {status}";
            }
        }
    }
}