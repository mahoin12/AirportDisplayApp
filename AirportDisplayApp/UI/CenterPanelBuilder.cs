using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    public class CenterPanelBuilder
    {
        private readonly Window parentWindow;
        private readonly Dictionary<string, TextBlock> displayElements;
        
        public CenterPanelBuilder(Window parentWindow, Dictionary<string, TextBlock> displayElements)
        {
            this.parentWindow = parentWindow;
            this.displayElements = displayElements;
        }
        
        public Border CreateCenterPanel(Grid parentGrid, int column)
        {
            Border centerInfoBorder = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };
            
            Grid centerInfoGrid = new Grid();
            centerInfoBorder.Child = centerInfoGrid;
            
            centerInfoGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            centerInfoGrid.RowDefinitions.Add(new RowDefinition());
            
            // Başlık
            Border centerHeaderBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 180, 180))
            };
            
            TextBlock centerHeaderText = new TextBlock
            {
                Text = "Çiğli Havaalanı",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            centerHeaderBorder.Child = centerHeaderText;
            
            centerInfoGrid.Children.Add(centerHeaderBorder);
            Grid.SetRow(centerHeaderBorder, 0);
            
            // Havaalanı Veri Grid'i
            Grid airportDataGrid = new Grid();
            airportDataGrid.Margin = new Thickness(10);
            
            // Başlık satırı
            airportDataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            
            // Tüm veri alanları için satırlar ekle
            for (int i = 0; i < 15; i++)
            {
                airportDataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            }
            
            // Merkez panel veri satırlarını oluştur
            CreateCenterDataRows(airportDataGrid);
            
            centerInfoGrid.Children.Add(airportDataGrid);
            Grid.SetRow(airportDataGrid, 1);
            
            parentGrid.Children.Add(centerInfoBorder);
            Grid.SetColumn(centerInfoBorder, column);
            
            return centerInfoBorder;
        }
        
        private void CreateCenterDataRows(Grid grid)
        {
            // RWYINUSE
            TextBlock rwyInUseLabel = CreateInfoLabel("RWYINUSE", grid, 0);
            TextBlock rwyInUseVal = CreateInfoValue("35", grid, 0, "RunwayInUseInfo");
            displayElements["rwyInUseInfo"] = rwyInUseVal;
            
            // QNH
            TextBlock qnhLabel = CreateInfoLabel("QNH", grid, 1);
            TextBlock qnhVal = CreateInfoValue("1013.2", grid, 1, "QNHValue");
            displayElements["qnh"] = qnhVal;
            
            CreateUnitLabel("hPa", grid, 1);
            
            // QNH mmHg
            TextBlock qnhMmLabel = CreateInfoLabel("QNH", grid, 2);
            TextBlock qnhMmVal = CreateInfoValue("29.92", grid, 2, "QNHMmValue");
            displayElements["qnhMm"] = qnhMmVal;
            
            CreateUnitLabel("inHg", grid, 2);
            
            // QFE
            TextBlock qfeLabel = CreateInfoLabel("QFE", grid, 3);
            TextBlock qfeVal = CreateInfoValue("1013.2", grid, 3, "QFEValue");
            displayElements["qfe"] = qfeVal;
            
            CreateUnitLabel("hPa", grid, 3);
            
            // QFE SYNOP
            TextBlock qfeSynopLabel = CreateInfoLabel("QFE SYNOP", grid, 4);
            TextBlock qfeSynopVal = CreateInfoValue("1012.7", grid, 4, "QFESynopValue");
            displayElements["qfeSynop"] = qfeSynopVal;
            
            CreateUnitLabel("hPa", grid, 4);
            
            // Aerodrome sections
            TextBlock aerodromeLabel = CreateInfoLabel("QFE AERODROME", grid, 5);
            
            // HIGH
            TextBlock highLabel = CreateInfoLabel("HIGH", grid, 6);
            
            // MID
            TextBlock midLabel = CreateInfoLabel("MID", grid, 7);
            
            // LOW
            TextBlock lowLabel = CreateInfoLabel("LOW", grid, 8);
            TextBlock lowVal = CreateInfoValue("NCD", grid, 8, "LowValue");
            displayElements["low"] = lowVal;
            
            // Temperature
            TextBlock tempLabel = CreateInfoLabel("Temp", grid, 9);
            TextBlock tempVal = CreateInfoValue("30.4", grid, 9, "TempValue");
            displayElements["temp"] = tempVal;
            
            CreateUnitLabel("°C", grid, 9);
            
            // Td
            TextBlock tdLabel = CreateInfoLabel("Td", grid, 10);
            TextBlock tdVal = CreateInfoValue("19.7", grid, 10, "TdValue");
            displayElements["td"] = tdVal;
            
            CreateUnitLabel("°C", grid, 10);
            
            // RH
            TextBlock rhLabel = CreateInfoLabel("RH", grid, 11);
            TextBlock rhVal = CreateInfoValue("52", grid, 11, "RHValue");
            displayElements["rh"] = rhVal;
            
            CreateUnitLabel("%", grid, 11);
            
            // Tmax
            TextBlock tmaxLabel = CreateInfoLabel("Tmax", grid, 12);
            TextBlock tmaxVal = CreateInfoValue("30.9", grid, 12, "TmaxValue");
            displayElements["tmax"] = tmaxVal;
            
            CreateUnitLabel("°C", grid, 12);
            
            // Tmin
            TextBlock tminLabel = CreateInfoLabel("Tmin", grid, 13);
            TextBlock tminVal = CreateInfoValue("20.4", grid, 13, "TminValue");
            displayElements["tmin"] = tminVal;
            
            CreateUnitLabel("°C", grid, 13);
            
            // Trwy
            TextBlock trwyLabel = CreateInfoLabel("Trwy", grid, 14);
            TextBlock trwyVal = CreateInfoValue("49.9", grid, 14, "TrwyValue");
            displayElements["trwy"] = trwyVal;
            
            CreateUnitLabel("°C", grid, 14);
        }
        
        private TextBlock CreateInfoLabel(string text, Grid grid, int row)
        {
            TextBlock label = new TextBlock
            {
                Text = text,
                FontSize = 14,
                FontWeight = FontWeights.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 0)
            };
            
            grid.Children.Add(label);
            Grid.SetRow(label, row);
            
            return label;
        }
        
        private TextBlock CreateInfoValue(string text, Grid grid, int row, string name)
        {
            TextBlock value = new TextBlock
            {
                Text = text,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 50, 0),
                Name = name
            };
            
            parentWindow.RegisterName(name, value);
            
            grid.Children.Add(value);
            Grid.SetRow(value, row);
            
            return value;
        }
        
        private void CreateUnitLabel(string unit, Grid grid, int row)
        {
            TextBlock unitLabel = new TextBlock
            {
                Text = unit,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            
            grid.Children.Add(unitLabel);
            Grid.SetRow(unitLabel, row);
        }
    }
}