using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// METAR bilgisi paneli bileşeni
    /// </summary>
    public class MetarPanelComponent : BaseUIComponent
    {
        public MetarPanelComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                                  ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            Border metarBorder = new Border();
            metarBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            metarBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            metarBorder.BorderThickness = new Thickness(1);
            metarBorder.Margin = new Thickness(5, 0, 5, 5);

            Grid metarGrid = new Grid();
            metarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            metarGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock metarLabel = new TextBlock();
            metarLabel.Text = "METAR";
            metarLabel.FontWeight = FontWeights.Bold;
            metarLabel.Margin = new Thickness(5);
            metarLabel.VerticalAlignment = VerticalAlignment.Center;

            TextBlock metarText = new TextBlock();
            metarText.Text = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320";
            metarText.Padding = new Thickness(5);
            metarText.TextWrapping = TextWrapping.Wrap;
            
            RegisterTextElement(metarText, "metar");

            Grid.SetColumn(metarLabel, 0);
            Grid.SetColumn(metarText, 1);

            metarGrid.Children.Add(metarLabel);
            metarGrid.Children.Add(metarText);

            metarBorder.Child = metarGrid;

            Grid.SetRow(metarBorder, row);
            Grid.SetColumnSpan(metarBorder, 3); // Tüm sütunlara yayılır
            parent.Children.Add(metarBorder);

            return metarBorder;
        }
    }
}