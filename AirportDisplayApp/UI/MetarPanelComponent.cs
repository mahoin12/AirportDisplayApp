using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// METAR bilgisi paneli bileşeni - Havaalanı METAR verisini görüntüler
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
            // Referans ekran görüntüsüne göre gri arka plan üzerinde text
            Border metarBorder = new Border();
            metarBorder.Background = Brushes.White;
            metarBorder.BorderBrush = Brushes.LightGray;
            metarBorder.BorderThickness = new Thickness(1);
            metarBorder.Margin = new Thickness(5, 0, 5, 5);
            
            Grid metarGrid = new Grid();
            metarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            metarGrid.ColumnDefinitions.Add(new ColumnDefinition());
            
            // METAR etiketi
            TextBlock metarLabel = new TextBlock();
            metarLabel.Text = "METAR";
            metarLabel.FontWeight = FontWeights.Bold;
            metarLabel.Margin = new Thickness(10, 5, 10, 5);
            metarLabel.VerticalAlignment = VerticalAlignment.Center;
            
            // METAR değeri
            TextBlock metarText = new TextBlock();
            // Örnek METAR değeri - referans ekran görüntüsündeki değer
            metarText.Text = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320";
            metarText.Padding = new Thickness(5);
            metarText.TextWrapping = TextWrapping.Wrap;
            metarText.VerticalAlignment = VerticalAlignment.Center;
            
            // METAR değerini kaydet
            RegisterTextElement(metarText, "metar");
            
            Grid.SetColumn(metarLabel, 0);
            Grid.SetColumn(metarText, 1);
            
            metarGrid.Children.Add(metarLabel);
            metarGrid.Children.Add(metarText);
            
            metarBorder.Child = metarGrid;
            
            Grid.SetRow(metarBorder, row);
            Grid.SetColumn(metarBorder, column);
            
            // Tüm sütunlara yayılacak şekilde ayarla (ColSpan=3)
            Grid.SetColumnSpan(metarBorder, 3);
            
            parent.Children.Add(metarBorder);
            
            return metarBorder;
        }
    }
}