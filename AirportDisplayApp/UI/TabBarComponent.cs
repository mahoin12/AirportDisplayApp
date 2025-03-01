using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Sekme çubuğu bileşeni - Farklı görünümler arasında geçiş yapmak için sekmeler
    /// </summary>
    public class TabBarComponent : BaseUIComponent
    {
        // Sekme değişim olayı
        public event EventHandler<string> TabChanged;
        
        public TabBarComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                             ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            // Referans görüntüye göre açık turkuaz/mavi arka plan
            Border tabBarBorder = new Border();
            tabBarBorder.Background = new SolidColorBrush(Color.FromRgb(200, 240, 248)); // Açık turkuaz
            tabBarBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 220, 230));
            tabBarBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            // Yatay tab butonları için StackPanel
            StackPanel tabPanel = new StackPanel();
            tabPanel.Orientation = Orientation.Horizontal;
            tabPanel.HorizontalAlignment = HorizontalAlignment.Left;
            tabPanel.Margin = new Thickness(10, 5, 0, 5);
            
            // İlk sekme (aktif) - RWY 17-35 - mps
            Button activeTab = CreateTabButton("RWY 17-35 - mps", true);
            activeTab.Click += (s, e) => OnTabChanged("RWY 17-35 - mps");
            
            // İkinci sekme - Reports
            Button reportsTab = CreateTabButton("Reports", false);
            reportsTab.Click += (s, e) => OnTabChanged("Reports");
            
            // Sekmeler arası boşluk
            reportsTab.Margin = new Thickness(5, 0, 0, 0);
            
            tabPanel.Children.Add(activeTab);
            tabPanel.Children.Add(reportsTab);
            
            // Sağ tarafta RWY IN USE göstergesi (referans görüntüye göre)
            Grid tabBarGrid = new Grid();
            tabBarGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Tab butonları için
            tabBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // RWY IN USE için
            
            TextBlock rwyInUseText = new TextBlock();
            rwyInUseText.Text = "RWY IN USE";
            rwyInUseText.FontSize = 12;
            rwyInUseText.FontWeight = FontWeights.Bold;
            rwyInUseText.VerticalAlignment = VerticalAlignment.Center;
            rwyInUseText.HorizontalAlignment = HorizontalAlignment.Right;
            rwyInUseText.Margin = new Thickness(0, 0, 10, 0);
            
            Grid.SetColumn(tabPanel, 0);
            Grid.SetColumn(rwyInUseText, 1);
            
            tabBarGrid.Children.Add(tabPanel);
            tabBarGrid.Children.Add(rwyInUseText);
            
            tabBarBorder.Child = tabBarGrid;
            
            Grid.SetRow(tabBarBorder, row);
            Grid.SetColumn(tabBarBorder, column);
            Grid.SetColumnSpan(tabBarBorder, 3); // Tüm sütunlara yay
            
            parent.Children.Add(tabBarBorder);
            
            return tabBarBorder;
        }
        
        /// <summary>
        /// Sekme butonu oluşturur
        /// </summary>
        private Button CreateTabButton(string text, bool isActive)
        {
            Button tabButton = new Button();
            tabButton.Content = text;
            tabButton.Padding = new Thickness(15, 3, 15, 3);
            
            if (isActive)
            {
                // Aktif sekme - koyu mavi arka plan, beyaz yazı
                tabButton.Background = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
                tabButton.Foreground = Brushes.White;
                tabButton.FontWeight = FontWeights.Bold;
            }
            else
            {
                // Pasif sekme - açık gri arka plan
                tabButton.Background = new SolidColorBrush(Color.FromRgb(230, 240, 245)); // Açık gri
                tabButton.Foreground = Brushes.Black;
            }
            
            tabButton.BorderThickness = new Thickness(1, 1, 1, 0);
            tabButton.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            
            return tabButton;
        }
        
        /// <summary>
        /// Sekme değişim olayını tetikler
        /// </summary>
        private void OnTabChanged(string tabName)
        {
            TabChanged?.Invoke(this, tabName);
        }
    }
}