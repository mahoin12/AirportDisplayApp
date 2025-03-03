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
            
            // İçerik için Border ve kenar çizgileri
            Border contentBorder = new Border();
            contentBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            contentBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            
            Grid contentGrid = CreateContentGrid();
            contentBorder.Child = contentGrid;
            
            contentScrollViewer.Content = contentBorder;
            
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
            for (int i = 0; i < 15; i++)
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
            
            // SKY AERODROME header - Özel stil ile başlık olarak oluştur
            CreateSectionHeader(contentGrid, rowIndex++, "SKY AERODROME");
            
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
        /// Bölüm başlık satırı oluşturur - SKY AERODROME başlığı için özel stil
        /// </summary>
        private void CreateSectionHeader(Grid grid, int row, string headerText)
        {
            // Başlık satırı için özel border - Koyu renkli arka plan ve belirgin kenar çizgisi
            Border sectionBorder = new Border();
            sectionBorder.Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)); // Daha koyu gri arka plan
            sectionBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            sectionBorder.BorderThickness = new Thickness(0, 1, 0, 1); // Üst ve alt kenarlarda belirgin çizgi
            sectionBorder.Padding = new Thickness(5, 2, 5, 2);
            
            // İç Grid
            Grid innerGrid = new Grid();
            
            TextBlock headerLabel = new TextBlock();
            headerLabel.Text = headerText;
            headerLabel.FontSize = 14;
            headerLabel.FontWeight = FontWeights.Bold; // Kalın font ağırlığı
            headerLabel.VerticalAlignment = VerticalAlignment.Center;
            headerLabel.HorizontalAlignment = HorizontalAlignment.Left;
            headerLabel.Margin = new Thickness(5, 0, 0, 0);
            
            innerGrid.Children.Add(headerLabel);
            sectionBorder.Child = innerGrid;
            
            Grid.SetRow(sectionBorder, row);
            grid.Children.Add(sectionBorder);
        }
        
        /// <summary>
        /// Veri satırı oluşturur - Geliştirilmiş hizalama ile
        /// </summary>
        private void CreateDataRow(Grid grid, int row, string label, string value, string unit, string valueKey)
        {
            // Her satır için border oluştur - kenar çizgisi için
            Border rowBorder = new Border();
            rowBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            Grid rowGrid = new Grid();
            
            // İki sütunlu düzen
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());                              // Değer ve birim
            
            // Etiket
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.FontSize = 14;
            labelText.VerticalAlignment = VerticalAlignment.Center;
            labelText.HorizontalAlignment = HorizontalAlignment.Left;
            labelText.Margin = new Thickness(5, 0, 0, 0);
            
            // Değer ve birim için panel - Hizalama düzeltildi
            StackPanel valuePanel = new StackPanel();
            valuePanel.Orientation = Orientation.Horizontal;
            // Ekran görüntüsüne göre değerler solda olmalı, bu nedenle panel hizalamasını değiştir
            valuePanel.HorizontalAlignment = HorizontalAlignment.Left;
            
            // Değer
            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.FontSize = 14;
            valueText.FontWeight = FontWeights.Bold;
            valueText.Foreground = Brushes.Black;
            valueText.VerticalAlignment = VerticalAlignment.Center;
            // Sağa hizalama gerekmiyor - referans görüntüye göre solda
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
            
            rowBorder.Child = rowGrid;
            
            Grid.SetRow(rowBorder, row);
            grid.Children.Add(rowBorder);
        }
    }
}