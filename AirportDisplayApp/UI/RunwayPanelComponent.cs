using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Pist paneli bileşeni - Rüzgar göstergesi ve veri tablosu içerir
    /// </summary>
    public class RunwayPanelComponent : BaseUIComponent
    {
        private string _runwayName;
        private WindIndicatorComponent _windIndicator;
        
        public RunwayPanelComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                                   ResourceDictionary styles, string runwayName) 
            : base(owner, displayElements, styles)
        {
            _runwayName = runwayName;
            _windIndicator = new WindIndicatorComponent(owner, runwayName);
        }

        public override FrameworkElement Create(Grid parent, int row, int column)
        {
            Border runwayBorder = new Border();
            runwayBorder.BorderBrush = Brushes.LightGray;
            runwayBorder.BorderThickness = new Thickness(1);
            runwayBorder.Margin = new Thickness(5);
            runwayBorder.Background = Brushes.White;
            
            Grid runwayGrid = new Grid();
            runwayBorder.Child = runwayGrid;
            
            runwayGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });  // Başlık
            runwayGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(300) }); // Rüzgar göstergesi
            runwayGrid.RowDefinitions.Add(new RowDefinition());                                 // Veri tablosu
            
            // Başlık
            Border headerBorder = CreateRunwayHeader();
            runwayGrid.Children.Add(headerBorder);
            Grid.SetRow(headerBorder, 0);
            
            // Rüzgar göstergesi - artık WindIndicatorComponent kullanıyor
            Grid windGrid = _windIndicator.CreateWindIndicator();
            runwayGrid.Children.Add(windGrid);
            Grid.SetRow(windGrid, 1);
            
            // Veri tablosu
            Grid dataGrid = CreateDataGrid();
            
            // Veri tablosu için bir Border ekle
            Border dataBorder = new Border();
            dataBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            dataBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            dataBorder.Child = dataGrid;
            
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = dataBorder;
            
            runwayGrid.Children.Add(scrollViewer);
            Grid.SetRow(scrollViewer, 2);
            
            Grid.SetRow(runwayBorder, row);
            Grid.SetColumn(runwayBorder, column);
            parent.Children.Add(runwayBorder);
            
            return runwayBorder;
        }
        
        /// <summary>
        /// Pist başlığını oluşturur
        /// </summary>
        private Border CreateRunwayHeader()
        {
            Border headerBorder = new Border();
            headerBorder.Background = Brushes.LightGray;
            headerBorder.BorderBrush = Brushes.LightGray;
            headerBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            TextBlock headerText = new TextBlock();
            headerText.Text = _runwayName;
            headerText.FontSize = 16;
            headerText.FontWeight = FontWeights.Bold;
            headerText.VerticalAlignment = VerticalAlignment.Center;
            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            
            headerBorder.Child = headerText;
            
            return headerBorder;
        }
        
        /// <summary>
        /// Veri tablosunu oluşturur
        /// </summary>
        private Grid CreateDataGrid()
        {
            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(10);
            
            // Satırlar
            for (int i = 0; i < 9; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            }
            
            string prefix = _runwayName == "RWY 35" ? "left" : "right";
            
            // 2" Mnm
            CreateDataRow(dataGrid, 0, "2\" Mnm", "250", "3", prefix + "2MinDir", prefix + "2MinSpeed");
            
            // 2" Avg
            string avgDir = _runwayName == "RWY 35" ? "290" : "270";
            CreateDataRow(dataGrid, 1, "2\" Avg", avgDir, "5", prefix + "2AvgDir", prefix + "2AvgSpeed");
            
            // 2" Max
            string maxSpeed = _runwayName == "RWY 35" ? "8" : "6";
            CreateDataRow(dataGrid, 2, "2\" Max", "320", maxSpeed, prefix + "2MaxDir", prefix + "2MaxSpeed");
            
            // 2" Hw/Cw
            string hwCwValue = _runwayName == "RWY 35" ? "H02   L05" : "T01   R05";
            CreateHwCwRow(dataGrid, 3, "2\" Hw/Cw", hwCwValue, prefix + "HwCw");
            
            // Base (sadece sol pist için)
            if (_runwayName == "RWY 35")
            {
                CreateBaseRow(dataGrid, 4, "Base", "NCD", "baseValue");
            }
            
            // 10" Mnm
            string min10Dir = _runwayName == "RWY 35" ? "* CALM" : "220";
            string min10Speed = _runwayName == "RWY 35" ? "" : "2";
            CreateDataRow(dataGrid, 5, "10\" Mnm", min10Dir, min10Speed, prefix + "10MinDir", prefix + "10MinSpeed");
            
            // 10" Avg
            string avg10Dir = _runwayName == "RWY 35" ? "280" : "290";
            CreateDataRow(dataGrid, 6, "10\" Avg", avg10Dir, "5", prefix + "10AvgDir", prefix + "10AvgSpeed");
            
            // 10" Max
            string max10Speed = _runwayName == "RWY 35" ? "9" : "8";
            CreateDataRow(dataGrid, 7, "10\" Max", "320", max10Speed, prefix + "10MaxDir", prefix + "10MaxSpeed");
            
            // QFE
            string qfeValue = _runwayName == "RWY 35" ? "1012.8" : "1012.7";
            string qfeInHg = _runwayName == "RWY 35" ? "29.91" : "29.90";
            CreateQfeRow(dataGrid, 8, "QFE", qfeValue, qfeInHg, prefix + "QfeValue", prefix + "QfeInHg");
            
            return dataGrid;
        }
        
        /// <summary>
        /// Veri satırı oluşturur (yön ve hız)
        /// </summary>
        private void CreateDataRow(Grid grid, int row, string label, string direction, string speed,
                                  string directionKey, string speedKey)
        {
            // Her satır için bir border ekle - ilk resimde olan çizgi efektini vermek için
            Border rowBorder = new Border();
            rowBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            Grid rowGrid = new Grid();
            
            // Üç sütunlu düzen
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });  // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) }); // Yön
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });  // Hız
            
            // Etiket
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.FontSize = 14;
            labelText.VerticalAlignment = VerticalAlignment.Center;
            labelText.HorizontalAlignment = HorizontalAlignment.Left;
            labelText.Margin = new Thickness(5, 0, 0, 0);
            
            // Yön değeri
            TextBlock directionText = new TextBlock();
            directionText.Text = direction;
            directionText.FontSize = 14;
            directionText.FontWeight = FontWeights.Bold;
            directionText.Foreground = Brushes.Black;
            directionText.VerticalAlignment = VerticalAlignment.Center;
            directionText.HorizontalAlignment = HorizontalAlignment.Right;
            directionText.Margin = new Thickness(0, 0, 5, 0);
            
            RegisterTextElement(directionText, directionKey);
            
            // Yön birimi
            TextBlock dirUnitText = new TextBlock();
            dirUnitText.Text = "°";
            dirUnitText.FontSize = 14;
            dirUnitText.VerticalAlignment = VerticalAlignment.Center;
            dirUnitText.Margin = new Thickness(0, 0, 10, 0);
            
            // Yön Panel
            StackPanel dirPanel = new StackPanel();
            dirPanel.Orientation = Orientation.Horizontal;
            dirPanel.HorizontalAlignment = HorizontalAlignment.Right;
            dirPanel.Children.Add(directionText);
            dirPanel.Children.Add(dirUnitText);
            
            // Hız değeri (varsa)
            if (!string.IsNullOrEmpty(speed))
            {
                TextBlock speedText = new TextBlock();
                speedText.Text = speed;
                speedText.FontSize = 14;
                speedText.FontWeight = FontWeights.Bold;
                speedText.Foreground = Brushes.Black;
                speedText.VerticalAlignment = VerticalAlignment.Center;
                speedText.HorizontalAlignment = HorizontalAlignment.Right;
                speedText.Margin = new Thickness(0, 0, 5, 0);
                
                RegisterTextElement(speedText, speedKey);
                
                TextBlock speedUnitText = new TextBlock();
                speedUnitText.Text = "kt";
                speedUnitText.FontSize = 14;
                speedUnitText.VerticalAlignment = VerticalAlignment.Center;
                
                StackPanel speedPanel = new StackPanel();
                speedPanel.Orientation = Orientation.Horizontal;
                speedPanel.HorizontalAlignment = HorizontalAlignment.Right;
                speedPanel.Children.Add(speedText);
                speedPanel.Children.Add(speedUnitText);
                
                Grid.SetColumn(speedPanel, 2);
                rowGrid.Children.Add(speedPanel);
            }
            
            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(dirPanel, 1);
            
            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(dirPanel);
            
            rowBorder.Child = rowGrid;
            
            Grid.SetRow(rowBorder, row);
            grid.Children.Add(rowBorder);
        }
        
        /// <summary>
        /// Hw/Cw satırı oluşturur
        /// </summary>
        private void CreateHwCwRow(Grid grid, int row, string label, string value, string valueKey)
        {
            // Her satır için bir border ekle - ilk resimde olan çizgi efektini vermek için
            Border rowBorder = new Border();
            rowBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            Grid rowGrid = new Grid();
            
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });  // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());                                // Değer
            
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.FontSize = 14;
            labelText.VerticalAlignment = VerticalAlignment.Center;
            labelText.HorizontalAlignment = HorizontalAlignment.Left;
            labelText.Margin = new Thickness(5, 0, 0, 0);
            
            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.FontSize = 14;
            valueText.FontWeight = FontWeights.Bold;
            valueText.Foreground = Brushes.Black;
            valueText.VerticalAlignment = VerticalAlignment.Center;
            valueText.HorizontalAlignment = HorizontalAlignment.Center;
            
            RegisterTextElement(valueText, valueKey);
            
            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);
            
            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);
            
            rowBorder.Child = rowGrid;
            
            Grid.SetRow(rowBorder, row);
            grid.Children.Add(rowBorder);
        }
        
        /// <summary>
        /// Base satırı oluşturur
        /// </summary>
        private void CreateBaseRow(Grid grid, int row, string label, string value, string valueKey)
        {
            // Her satır için bir border ekle - ilk resimde olan çizgi efektini vermek için
            Border rowBorder = new Border();
            rowBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            Grid rowGrid = new Grid();
            
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });  // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) }); // Değer
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });  // Birim
            
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.FontSize = 14;
            labelText.VerticalAlignment = VerticalAlignment.Center;
            labelText.HorizontalAlignment = HorizontalAlignment.Left;
            labelText.Margin = new Thickness(5, 0, 0, 0);
            
            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.FontSize = 14;
            valueText.FontWeight = FontWeights.Bold;
            valueText.Foreground = Brushes.Black;
            valueText.VerticalAlignment = VerticalAlignment.Center;
            valueText.HorizontalAlignment = HorizontalAlignment.Right;
            valueText.Margin = new Thickness(0, 0, 5, 0);
            
            RegisterTextElement(valueText, valueKey);
            
            TextBlock unitText = new TextBlock();
            unitText.Text = "ft";
            unitText.FontSize = 14;
            unitText.VerticalAlignment = VerticalAlignment.Center;
            
            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);
            Grid.SetColumn(unitText, 2);
            
            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);
            rowGrid.Children.Add(unitText);
            
            rowBorder.Child = rowGrid;
            
            Grid.SetRow(rowBorder, row);
            grid.Children.Add(rowBorder);
        }
        
        /// <summary>
        /// QFE satırı oluşturur
        /// </summary>
        private void CreateQfeRow(Grid grid, int row, string label, string value, string inHgValue,
                                 string valueKey, string inHgKey)
        {
            // Her satır için bir border ekle - ilk resimde olan çizgi efektini vermek için
            Border rowBorder = new Border();
            rowBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            
            Grid rowGrid = new Grid();
            
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });   // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });   // hPa değeri
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });   // hPa birimi
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });   // inHg değeri
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });   // inHg birimi
            
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.FontSize = 14;
            labelText.VerticalAlignment = VerticalAlignment.Center;
            labelText.HorizontalAlignment = HorizontalAlignment.Left;
            labelText.Margin = new Thickness(5, 0, 0, 0);
            
            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.FontSize = 14;
            valueText.FontWeight = FontWeights.Bold;
            valueText.Foreground = Brushes.Black;
            valueText.VerticalAlignment = VerticalAlignment.Center;
            valueText.HorizontalAlignment = HorizontalAlignment.Right;
            valueText.Margin = new Thickness(0, 0, 5, 0);
            
            RegisterTextElement(valueText, valueKey);
            
            TextBlock unitText = new TextBlock();
            unitText.Text = "hPa";
            unitText.FontSize = 14;
            unitText.VerticalAlignment = VerticalAlignment.Center;
            
            TextBlock inHgText = new TextBlock();
            inHgText.Text = inHgValue;
            inHgText.FontSize = 14;
            inHgText.FontWeight = FontWeights.Bold;
            inHgText.Foreground = Brushes.Black;
            inHgText.VerticalAlignment = VerticalAlignment.Center;
            inHgText.HorizontalAlignment = HorizontalAlignment.Right;
            inHgText.Margin = new Thickness(0, 0, 5, 0);
            
            RegisterTextElement(inHgText, inHgKey);
            
            TextBlock inHgUnitText = new TextBlock();
            inHgUnitText.Text = "inHg";
            inHgUnitText.FontSize = 14;
            inHgUnitText.VerticalAlignment = VerticalAlignment.Center;
            
            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);
            Grid.SetColumn(unitText, 2);
            Grid.SetColumn(inHgText, 3);
            Grid.SetColumn(inHgUnitText, 4);
            
            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);
            rowGrid.Children.Add(unitText);
            rowGrid.Children.Add(inHgText);
            rowGrid.Children.Add(inHgUnitText);
            
            rowBorder.Child = rowGrid;
            
            Grid.SetRow(rowBorder, row);
            grid.Children.Add(rowBorder);
        }
        
        /// <summary>
        /// Rüzgar yönünü WindIndicatorComponent üzerinden günceller
        /// </summary>
        public void UpdateWindDirection(double direction)
        {
            _windIndicator.UpdateWindDirection(direction);
        }
        
        /// <summary>
        /// Rüzgar hızını WindIndicatorComponent üzerinden günceller
        /// </summary>
        public void UpdateWindSpeed(string speed)
        {
            _windIndicator.UpdateWindSpeed(speed);
        }
    }
}