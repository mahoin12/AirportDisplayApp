using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Merkez panel bileşeni - Havaalanı bilgileri ve meteorolojik verileri
    /// </summary>
    public class CenterPanelComponent : BaseUIComponent
    {
        public CenterPanelComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                                   ResourceDictionary styles) 
            : base(owner, displayElements, styles)
        {
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            Border centerBorder = new Border();
            centerBorder.BorderBrush = Brushes.LightGray;
            centerBorder.BorderThickness = new Thickness(1);
            centerBorder.Margin = new Thickness(5);
            centerBorder.Background = Brushes.White;
            
            Grid centerGrid = new Grid();
            centerBorder.Child = centerGrid;
            
            centerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });  // Başlık
            centerGrid.RowDefinitions.Add(new RowDefinition());                                 // İçerik
            
            // Başlık
            Border headerBorder = CreateCenterHeader();
            centerGrid.Children.Add(headerBorder);
            Grid.SetRow(headerBorder, 0);
            
            // İçerik
            ScrollViewer contentScrollViewer = new ScrollViewer();
            contentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            
            Grid contentGrid = CreateContentGrid();
            contentScrollViewer.Content = contentGrid;
            
            centerGrid.Children.Add(contentScrollViewer);
            Grid.SetRow(contentScrollViewer, 1);
            
            Grid.SetRow(centerBorder, row);
            Grid.SetColumn(centerBorder, column);
            parent.Children.Add(centerBorder);
            
            return centerBorder;
        }
        
        /// <summary>
        /// Merkez panel başlığını oluşturur
        /// </summary>
        private Border CreateCenterHeader()
        {
            Border headerBorder = new Border();
            headerBorder.Background = Brushes.LightGray;
            headerBorder.BorderBrush = Brushes.LightGray;
            headerBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            TextBlock headerText = new TextBlock();
            headerText.Text = "Çiğli Airport";  // Referans ekran görüntüsüne göre
            headerText.FontSize = 16;
            headerText.FontWeight = FontWeights.Bold;
            headerText.VerticalAlignment = VerticalAlignment.Center;
            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            
            headerBorder.Child = headerText;
            
            return headerBorder;
        }
        
        /// <summary>
        /// Merkez panel içeriğini oluşturur
        /// </summary>
        private Grid CreateContentGrid()
        {
            Grid contentGrid = new Grid();
            contentGrid.Margin = new Thickness(10);
            
            // Satırlar - Referans ekran görüntüsündeki merkez panelin satır sayısı
            for (int i = 0; i < 14; i++)
            {
                contentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            }
            
            int rowIndex = 0;
            
            // RWYINUSE
            CreateDataRow(contentGrid, rowIndex++, "RWYINUSE", "35", "", "rwyInUseInfo");
            
            // QNH (hPa)
            CreateDataRow(contentGrid, rowIndex++, "QNH", "1013.2", "hPa", "qnh");
            
            // QNH (inHg)
            CreateDataRow(contentGrid, rowIndex++, "QNH", "29.92", "inHg", "qnhInHg");
            
            // QFE
            CreateDataRow(contentGrid, rowIndex++, "QFE", "1013.2", "hPa", "qfe");
            
            // QFE SYNOP
            CreateDataRow(contentGrid, rowIndex++, "QFE SYNOP", "1012.7", "hPa", "qfeSynop");
            
            // SKY AERODROME header
            CreateHeaderRow(contentGrid, rowIndex++, "SKY AERODROME");
            
            // HIGH
            CreateDataRow(contentGrid, rowIndex++, "HIGH", "", "ft", "high");
            
            // MID
            CreateDataRow(contentGrid, rowIndex++, "MID", "", "ft", "mid");
            
            // LOW
            CreateDataRow(contentGrid, rowIndex++, "LOW", "NCD", "ft", "low");
            
            // Temp
            CreateDataRow(contentGrid, rowIndex++, "Temp", "30.4", "°C", "temp");
            
            // Td
            CreateDataRow(contentGrid, rowIndex++, "Td", "19.7", "°C", "dewPoint");
            
            // RH
            CreateDataRow(contentGrid, rowIndex++, "RH", "52", "%", "relativeHumidity");
            
            // Tmax
            CreateDataRow(contentGrid, rowIndex++, "Tmax", "30.9", "°C", "tempMax");
            
            // Tmin
            CreateDataRow(contentGrid, rowIndex++, "Tmin", "20.4", "°C", "tempMin");
            
            // Trwy
            CreateDataRow(contentGrid, rowIndex++, "Trwy", "49.9", "°C", "runwayTemp");
            
            return contentGrid;
        }
        
        /// <summary>
        /// Başlık satırı oluşturur
        /// </summary>
        private void CreateHeaderRow(Grid grid, int row, string headerText)
        {
            TextBlock headerLabel = new TextBlock();
            headerLabel.Text = headerText;
            headerLabel.FontSize = 14;
            headerLabel.FontWeight = FontWeights.Bold;
            headerLabel.VerticalAlignment = VerticalAlignment.Center;
            headerLabel.HorizontalAlignment = HorizontalAlignment.Left;
            headerLabel.Margin = new Thickness(5, 0, 0, 0);
            
            Grid.SetRow(headerLabel, row);
            grid.Children.Add(headerLabel);
        }
        
        /// <summary>
        /// Veri satırı oluşturur
        /// </summary>
        private void CreateDataRow(Grid grid, int row, string label, string value, string unit, string valueKey)
        {
            Grid rowGrid = new Grid();
            
            // İki sütunlu düzen
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());                               // Değer ve birim
            
            // Etiket
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.FontSize = 14;
            labelText.VerticalAlignment = VerticalAlignment.Center;
            labelText.HorizontalAlignment = HorizontalAlignment.Left;
            labelText.Margin = new Thickness(5, 0, 0, 0);
            
            // Değer ve birim için panel
            StackPanel valuePanel = new StackPanel();
            valuePanel.Orientation = Orientation.Horizontal;
            valuePanel.HorizontalAlignment = HorizontalAlignment.Left;
            
            // Değer
            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.FontSize = 14;
            valueText.FontWeight = FontWeights.Bold;
            valueText.Foreground = Brushes.Black;
            valueText.VerticalAlignment = VerticalAlignment.Center;
            valueText.Margin = new Thickness(0, 0, 5, 0);
            
            RegisterTextElement(valueText, valueKey);
            
            valuePanel.Children.Add(valueText);
            
            // Birim (varsa)
            if (!string.IsNullOrEmpty(unit))
            {
                TextBlock unitText = new TextBlock();
                unitText.Text = unit;
                unitText.FontSize = 14;
                unitText.VerticalAlignment = VerticalAlignment.Center;
                
                valuePanel.Children.Add(unitText);
            }
            
            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valuePanel, 1);
            
            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valuePanel);
            
            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }
    }
}