using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Durum çubuğu bileşeni - Kullanılan pist bilgisi
    /// </summary>
    public class StatusBarComponent : BaseUIComponent
    {
        public StatusBarComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                                ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            Border statusBorder = new Border();
            statusBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            statusBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            statusBorder.BorderThickness = new Thickness(0, 1, 0, 1);

            Grid statusGrid = new Grid();
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition());
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // RWY IN USE metni
            TextBlock rwyInUseLabel = new TextBlock();
            rwyInUseLabel.Text = "RWY IN USE";
            rwyInUseLabel.FontSize = 14;
            rwyInUseLabel.HorizontalAlignment = HorizontalAlignment.Right;
            rwyInUseLabel.VerticalAlignment = VerticalAlignment.Center;
            rwyInUseLabel.Margin = new Thickness(0, 0, 10, 0);
            rwyInUseLabel.FontWeight = FontWeights.Bold;

            TextBlock rwyInUseValue = new TextBlock();
            rwyInUseValue.Text = "35";
            rwyInUseValue.FontSize = 14;
            rwyInUseValue.FontWeight = FontWeights.Bold;
            rwyInUseValue.Foreground = Brushes.DarkRed;
            rwyInUseValue.HorizontalAlignment = HorizontalAlignment.Right;
            rwyInUseValue.VerticalAlignment = VerticalAlignment.Center;
            rwyInUseValue.Margin = new Thickness(0, 0, 20, 0);
            
            RegisterTextElement(rwyInUseValue, "runwayInUse");

            Grid.SetColumn(rwyInUseLabel, 1);
            Grid.SetColumn(rwyInUseValue, 2);

            statusGrid.Children.Add(rwyInUseLabel);
            statusGrid.Children.Add(rwyInUseValue);

            statusBorder.Child = statusGrid;

            Grid.SetRow(statusBorder, row);
            Grid.SetColumnSpan(statusBorder, 3); // Tüm sütunlara yayılır
            parent.Children.Add(statusBorder);

            return statusBorder;
        }
    }
}