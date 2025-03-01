using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Sekme çubuğu bileşeni
    /// </summary>
    public class TabBarComponent : BaseUIComponent
    {
        public TabBarComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                             ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            Border tabBarBorder = new Border();
            tabBarBorder.Background = new SolidColorBrush(Color.FromRgb(200, 240, 248)); // Açık mavi
            tabBarBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 220, 230));
            tabBarBorder.BorderThickness = new Thickness(0, 0, 0, 1);

            // Tab butonları için yatay StackPanel
            StackPanel tabPanel = new StackPanel();
            tabPanel.Orientation = Orientation.Horizontal;
            tabPanel.HorizontalAlignment = HorizontalAlignment.Left;
            tabPanel.Margin = new Thickness(5, 0, 0, 0);

            // Aktif tab
            Button activeTab = new Button();
            activeTab.Content = "RWY 17-35 - mps";
            activeTab.Background = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // VAISALA mavi
            activeTab.Foreground = Brushes.White;
            activeTab.FontWeight = FontWeights.Bold;
            activeTab.Padding = new Thickness(15, 3, 15, 3);
            activeTab.BorderThickness = new Thickness(1, 1, 1, 0);
            activeTab.BorderBrush = new SolidColorBrush(Colors.DarkGray);

            // Diğer tablar
            Button reportsTab = new Button();
            reportsTab.Content = "Reports";
            reportsTab.Background = new SolidColorBrush(Color.FromRgb(230, 240, 245));
            reportsTab.Padding = new Thickness(15, 3, 15, 3);
            reportsTab.Margin = new Thickness(5, 0, 0, 0);
            reportsTab.BorderThickness = new Thickness(1, 1, 1, 0);
            reportsTab.BorderBrush = new SolidColorBrush(Colors.DarkGray);

            tabPanel.Children.Add(activeTab);
            tabPanel.Children.Add(reportsTab);
            tabBarBorder.Child = tabPanel;

            Grid.SetRow(tabBarBorder, row);
            Grid.SetColumnSpan(tabBarBorder, 3); // Tüm sütunlara yayılır
            parent.Children.Add(tabBarBorder);

            return tabBarBorder;
        }
    }
}