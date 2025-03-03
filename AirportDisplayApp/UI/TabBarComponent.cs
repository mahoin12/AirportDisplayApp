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
            // Referans görüntüye göre daha açık mavi/turkuaz arka plan
            Border tabBarBorder = new Border();
            tabBarBorder.Background = new SolidColorBrush(Color.FromRgb(120, 209, 229)); // Açık mavi/turkuaz
            tabBarBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 220, 230));
            tabBarBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            // Ana grid
            Grid tabBarGrid = new Grid();
            tabBarGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Sekmeler
            tabBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // RWY IN USE
            
            // Yatay tab butonları için StackPanel
            StackPanel tabPanel = new StackPanel();
            tabPanel.Orientation = Orientation.Horizontal;
            tabPanel.HorizontalAlignment = HorizontalAlignment.Left;
            tabPanel.Margin = new Thickness(0, 2, 0, 2); // Daha az kenar boşluğu
            
            // İlk sol sekme bilgisi (RWY 17-35 - kt bilgisi)
            Border infoTab = new Border();
            infoTab.Background = new SolidColorBrush(Color.FromRgb(120, 190, 210)); // Daha koyu turkuaz
            infoTab.Padding = new Thickness(10, 0, 10, 0);
            infoTab.Margin = new Thickness(0, 0, 3, 0);
            
            TextBlock infoLabel = new TextBlock();
            infoLabel.Text = "RWY 17-35 - kt";
            infoLabel.Foreground = Brushes.White;
            infoLabel.FontWeight = FontWeights.Normal;
            infoLabel.VerticalAlignment = VerticalAlignment.Center;
            infoLabel.HorizontalAlignment = HorizontalAlignment.Center;
            infoLabel.Padding = new Thickness(5);
            
            infoTab.Child = infoLabel;
            
            // İlk sekme (aktif) - RWY 17-35 - mps
            Button activeTab = CreateTabButton("RWY 17-35 - mps", true);
            activeTab.Click += (s, e) => OnTabChanged("RWY 17-35 - mps");
            
            // İkinci sekme - Reports
            Button reportsTab = CreateTabButton("Reports", false);
            reportsTab.Click += (s, e) => OnTabChanged("Reports");
            
            tabPanel.Children.Add(infoTab);
            tabPanel.Children.Add(activeTab);
            tabPanel.Children.Add(reportsTab);
            
            // Sağ tarafta RWY IN USE göstergesi
            Border rwyInUseBorder = new Border();
            rwyInUseBorder.Background = Brushes.Transparent;
            rwyInUseBorder.Padding = new Thickness(5);
            rwyInUseBorder.HorizontalAlignment = HorizontalAlignment.Right;
            
            Grid rwyInUseGrid = new Grid();
            rwyInUseGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Etiket
            rwyInUseGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Değer
            
            TextBlock rwyInUseLabel = new TextBlock();
            rwyInUseLabel.Text = "RWY IN USE";
            rwyInUseLabel.Foreground = Brushes.White;
            rwyInUseLabel.FontWeight = FontWeights.Bold;
            rwyInUseLabel.VerticalAlignment = VerticalAlignment.Center;
            rwyInUseLabel.HorizontalAlignment = HorizontalAlignment.Right;
            rwyInUseLabel.Margin = new Thickness(0, 0, 5, 0);
            
            TextBlock rwyInUseValue = new TextBlock();
            rwyInUseValue.Text = "35";
            rwyInUseValue.Foreground = Brushes.White;
            rwyInUseValue.FontWeight = FontWeights.Bold;
            rwyInUseValue.VerticalAlignment = VerticalAlignment.Center;
            rwyInUseValue.HorizontalAlignment = HorizontalAlignment.Left;
            
            // RWY IN USE değerini kaydet
            RegisterTextElement(rwyInUseValue, "runwayInUse");
            
            Grid.SetColumn(rwyInUseLabel, 0);
            Grid.SetColumn(rwyInUseValue, 1);
            
            rwyInUseGrid.Children.Add(rwyInUseLabel);
            rwyInUseGrid.Children.Add(rwyInUseValue);
            
            rwyInUseBorder.Child = rwyInUseGrid;
            
            Grid.SetColumn(tabPanel, 0);
            Grid.SetColumn(rwyInUseBorder, 1);
            
            tabBarGrid.Children.Add(tabPanel);
            tabBarGrid.Children.Add(rwyInUseBorder);
            
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
            tabButton.Padding = new Thickness(10, 4, 10, 4);
            tabButton.Margin = new Thickness(3, 0, 0, 0);
            
            if (isActive)
            {
                // Aktif sekme - beyaz arka plan, koyu mavi kenarlık
                tabButton.Background = Brushes.White;
                tabButton.Foreground = Brushes.Black;
                tabButton.BorderBrush = new SolidColorBrush(Color.FromRgb(80, 140, 170));
                tabButton.FontWeight = FontWeights.Normal;
            }
            else
            {
                // Pasif sekme - daha koyu mavi/gri arka plan
                tabButton.Background = new SolidColorBrush(Color.FromRgb(180, 210, 220)); // Açık gri-mavi
                tabButton.Foreground = Brushes.Black;
                tabButton.BorderBrush = new SolidColorBrush(Color.FromRgb(150, 190, 210));
            }
            
            tabButton.BorderThickness = new Thickness(1);
            
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