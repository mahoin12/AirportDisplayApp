using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Merkez paneli bileşeni - Havaalanı genel bilgileri ve meteorolojik verileri içerir
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
            centerBorder.Style = _styles["CenterPanelStyleV2"] as Style;

            Grid centerGrid = new Grid();
            centerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) }); // Başlık
            centerGrid.RowDefinitions.Add(new RowDefinition()); // Veri alanı

            // Başlık
            Border headerBorder = new Border();
            headerBorder.Style = _styles["CenterPanelHeaderStyleV2"] as Style;

            TextBlock headerText = new TextBlock();
            headerText.Text = "Çiğli Havaalanı";
            headerText.Style = _styles["RunwayHeaderLabelStyle"] as Style;
            headerBorder.Child = headerText;

            // Veri paneli
            ScrollViewer dataScrollViewer = new ScrollViewer();
            dataScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            dataScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            dataScrollViewer.Margin = new Thickness(0);
            dataScrollViewer.Padding = new Thickness(0);

            Grid dataGrid = CreateDataGrid();
            dataScrollViewer.Content = dataGrid;

            Grid.SetRow(headerBorder, 0);
            Grid.SetRow(dataScrollViewer, 1);

            centerGrid.Children.Add(headerBorder);
            centerGrid.Children.Add(dataScrollViewer);

            centerBorder.Child = centerGrid;

            Grid.SetRow(centerBorder, row);
            Grid.SetColumn(centerBorder, column);
            parent.Children.Add(centerBorder);

            return centerBorder;
        }

        /// <summary>
        /// Veri tablosunu oluşturur
        /// </summary>
        private Grid CreateDataGrid()
        {
            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(5);
            dataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            
            // Tek sütunlu layout
            dataGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Satır tanımları
            for (int i = 0; i < 15; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            int rowIndex = 0;

            // Kullanılan Pist
            CreateDataBlockRow(dataGrid, rowIndex++, "RWYINUSE", "35", "", "rwyInUseInfo");

            // QNH
            CreateDataBlockRow(dataGrid, rowIndex++, "QNH", "1013.2", "hPa", "qnh");

            // QNH inHg
            CreateDataBlockRow(dataGrid, rowIndex++, "QNH", "29.92", "inHg", "qnhInHg");

            // QFE
            CreateDataBlockRow(dataGrid, rowIndex++, "QFE", "1013.2", "hPa", "qfe");

            // QFE SYNOP
            CreateDataBlockRow(dataGrid, rowIndex++, "QFE SYNOP", "1012.7", "hPa", "qfeSynop");

            // LOW
            CreateDataBlockRow(dataGrid, rowIndex++, "LOW", "NCD", "", "low");

            // Temp
            CreateDataBlockRow(dataGrid, rowIndex++, "Temp", "30.4", "°C", "temp");

            // Td
            CreateDataBlockRow(dataGrid, rowIndex++, "Td", "19.7", "°C", "td");

            // RH
            CreateDataBlockRow(dataGrid, rowIndex++, "RH", "52", "%", "rh");

            // Tmax
            CreateDataBlockRow(dataGrid, rowIndex++, "Tmax", "30.9", "°C", "tmax");

            // Tmin
            CreateDataBlockRow(dataGrid, rowIndex++, "Tmin", "20.4", "°C", "tmin");

            // Trwy
            CreateDataBlockRow(dataGrid, rowIndex++, "Trwy", "49.9", "°C", "trwy");

            return dataGrid;
        }

        /// <summary>
        /// Veri satırı oluşturur (Etiket ve Değer)
        /// </summary>
        private void CreateDataBlockRow(Grid grid, int row, string label, string value, string unit, string valueKey)
        {
            // Ana grid - her satır için
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(0, 2, 0, 2);
            
            // İki sütunlu layout: Etiket ve Değer
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Değer
            
            // Etiket bloğu
            Border labelBorder = new Border();
            labelBorder.Style = _styles["DataBlockStyle"] as Style;
            labelBorder.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.Style = _styles["DataLabelStyle"] as Style;
            labelText.Margin = new Thickness(5, 0, 5, 0);
            
            labelBorder.Child = labelText;
            
            // Değer bloğu
            Border valueBorder = new Border();
            valueBorder.Style = _styles["DataBlockStyle"] as Style;
            
            // Değer ve birim için grid
            Grid valueGrid = new Grid();
            valueGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Değer
            valueGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Birim
            
            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.Style = _styles["DataValueStyle"] as Style;
            RegisterTextElement(valueText, valueKey);
            
            Grid.SetColumn(valueText, 0);
            valueGrid.Children.Add(valueText);
            
            // Birim varsa ekle
            if (!string.IsNullOrEmpty(unit))
            {
                TextBlock unitText = new TextBlock();
                unitText.Text = unit;
                unitText.Style = _styles["DataUnitStyle"] as Style;
                
                Grid.SetColumn(unitText, 1);
                valueGrid.Children.Add(unitText);
            }
            
            valueBorder.Child = valueGrid;
            
            // Ana grid'e ekle
            Grid.SetColumn(labelBorder, 0);
            Grid.SetColumn(valueBorder, 1);
            
            rowGrid.Children.Add(labelBorder);
            rowGrid.Children.Add(valueBorder);
            
            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }
    }
}