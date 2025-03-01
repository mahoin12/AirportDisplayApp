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
        private Rectangle _windArrow;
        
        public RunwayPanelComponent(Window owner, Dictionary<string, TextBlock> displayElements, 
                                   ResourceDictionary styles, string runwayName) 
            : base(owner, displayElements, styles)
        {
            _runwayName = runwayName;
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
            
            // Rüzgar göstergesi
            Grid windGrid = CreateWindIndicator();
            runwayGrid.Children.Add(windGrid);
            Grid.SetRow(windGrid, 1);
            
            // Veri tablosu
            Grid dataGrid = CreateDataGrid();
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = dataGrid;
            
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
        /// Rüzgar göstergesini oluşturur - Referans görüntüye uygun olarak tasarlandı
        /// </summary>
        private Grid CreateWindIndicator()
        {
            Grid windGrid = new Grid();
            windGrid.Margin = new Thickness(10);
            
            // Dış dairesel ölçek (Gri daire)
            Ellipse outerCircle = new Ellipse();
            outerCircle.Width = 240;
            outerCircle.Height = 240;
            outerCircle.Stroke = Brushes.LightGray;
            outerCircle.StrokeThickness = 1;
            outerCircle.Fill = Brushes.White;
            outerCircle.HorizontalAlignment = HorizontalAlignment.Center;
            outerCircle.VerticalAlignment = VerticalAlignment.Center;
            
            windGrid.Children.Add(outerCircle);
            
            // Rüzgar yönü için tick marks (30 derece aralıklarla)
            Canvas directionMarksCanvas = new Canvas();
            directionMarksCanvas.Width = 240;
            directionMarksCanvas.Height = 240;
            directionMarksCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            directionMarksCanvas.VerticalAlignment = VerticalAlignment.Center;
            
            // 30 derecelik aralıklarla çizgiler ve rakamlar
            for (int angle = 0; angle < 360; angle += 30)
            {
                double radians = angle * Math.PI / 180;
                double centerX = 120;
                double centerY = 120;
                
                // İç çemberin ve dış çemberin yarıçapları
                double innerRadius = 90;
                double outerRadius = 110;
                double textRadius = 120;
                
                // Tick çizgisi
                Line tickLine = new Line();
                tickLine.X1 = centerX + Math.Sin(radians) * innerRadius;
                tickLine.Y1 = centerY - Math.Cos(radians) * innerRadius;
                tickLine.X2 = centerX + Math.Sin(radians) * outerRadius;
                tickLine.Y2 = centerY - Math.Cos(radians) * outerRadius;
                tickLine.Stroke = Brushes.LightGray;
                tickLine.StrokeThickness = 1;
                
                directionMarksCanvas.Children.Add(tickLine);
                
                // Açı etiketleri (03, 06, 09, ... şeklinde her 30 derecede bir)
                string dirLabel = ((angle / 10) % 36).ToString("00");
                TextBlock dirText = new TextBlock();
                dirText.Text = dirLabel;
                dirText.FontSize = 12;
                
                // Metin pozisyonu (dairenin dışında)
                double textX = centerX + Math.Sin(radians) * textRadius - 8;
                double textY = centerY - Math.Cos(radians) * textRadius - 8;
                
                Canvas.SetLeft(dirText, textX);
                Canvas.SetTop(dirText, textY);
                
                directionMarksCanvas.Children.Add(dirText);
                
                // Ana yönler (00, 09, 18, 27) için renkli semboller (referansta turkuaz)
                if (angle % 90 == 0)
                {
                    Ellipse dirMark = new Ellipse();
                    dirMark.Width = 10;
                    dirMark.Height = 10;
                    dirMark.Fill = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
                    
                    double markX = centerX + Math.Sin(radians) * 75 - 5;
                    double markY = centerY - Math.Cos(radians) * 75 - 5;
                    
                    Canvas.SetLeft(dirMark, markX);
                    Canvas.SetTop(dirMark, markY);
                    
                    directionMarksCanvas.Children.Add(dirMark);
                }
                // Ara yönler (03, 06, 12, 15, 21, 24, 30, 33) için turkuaz çizgi
                else
                {
                    Rectangle dirMark = new Rectangle();
                    dirMark.Width = 8;
                    dirMark.Height = 8;
                    dirMark.Fill = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
                    
                    double markX = centerX + Math.Sin(radians) * 75 - 4;
                    double markY = centerY - Math.Cos(radians) * 75 - 4;
                    
                    Canvas.SetLeft(dirMark, markX);
                    Canvas.SetTop(dirMark, markY);
                    
                    directionMarksCanvas.Children.Add(dirMark);
                }
            }
            
            windGrid.Children.Add(directionMarksCanvas);
            
            // Daire içindeki rüzgar oku (gri)
            _windArrow = new Rectangle();
            _windArrow.Width = 20;
            _windArrow.Height = 100;
            _windArrow.Fill = Brushes.DarkGray;
            _windArrow.HorizontalAlignment = HorizontalAlignment.Center;
            _windArrow.VerticalAlignment = VerticalAlignment.Center;
            _windArrow.RenderTransformOrigin = new Point(0.5, 1.0);  // Dönüş merkezi alt orta nokta
            
            // Dönüş transformu
            TransformGroup transformGroup = new TransformGroup();
            RotateTransform rotateTransform = new RotateTransform(0);  // Başlangıçta 0 derece
            transformGroup.Children.Add(rotateTransform);
            _windArrow.RenderTransform = transformGroup;
            
            // Ok adı (OK'un güncellenmesi için)
            string arrowName = _runwayName == "RWY 35" ? "LeftWindArrow" : "RightWindArrow";
            _owner.RegisterName(arrowName, _windArrow);
            
            windGrid.Children.Add(_windArrow);
            
            // Rüzgar hızı göstergesi (ortadaki sayı)
            Border speedBorder = new Border();
            speedBorder.Background = Brushes.White;
            speedBorder.BorderBrush = Brushes.LightGray;
            speedBorder.BorderThickness = new Thickness(1);
            speedBorder.Width = 40;
            speedBorder.Height = 30;
            speedBorder.HorizontalAlignment = HorizontalAlignment.Center;
            speedBorder.VerticalAlignment = VerticalAlignment.Center;
            
            TextBlock speedText = new TextBlock();
            speedText.Text = _runwayName == "RWY 35" ? "7" : "5";  // Varsayılan değerler
            speedText.FontSize = 18;
            speedText.FontWeight = FontWeights.Bold;
            speedText.VerticalAlignment = VerticalAlignment.Center;
            speedText.HorizontalAlignment = HorizontalAlignment.Center;
            
            string speedKey = _runwayName == "RWY 35" ? "leftWindSpeed" : "rightWindSpeed";
            RegisterTextElement(speedText, speedKey);
            
            speedBorder.Child = speedText;
            windGrid.Children.Add(speedBorder);
            
            return windGrid;
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
            
            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }
        
        /// <summary>
        /// Hw/Cw satırı oluşturur
        /// </summary>
        private void CreateHwCwRow(Grid grid, int row, string label, string value, string valueKey)
        {
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
            
            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }
        
        /// <summary>
        /// Base satırı oluşturur
        /// </summary>
        private void CreateBaseRow(Grid grid, int row, string label, string value, string valueKey)
        {
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
            
            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }
        
        /// <summary>
        /// QFE satırı oluşturur
        /// </summary>
        private void CreateQfeRow(Grid grid, int row, string label, string value, string inHgValue,
                                 string valueKey, string inHgKey)
        {
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
            
            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }
        
        /// <summary>
        /// Rüzgar yönünü günceller
        /// </summary>
        public void UpdateWindDirection(double direction)
        {
            if (_windArrow != null)
            {
                // Transform grubunu al
                TransformGroup group = _windArrow.RenderTransform as TransformGroup;
                if (group != null && group.Children.Count > 0)
                {
                    // İlk transform (RotateTransform) dönme olmalı
                    RotateTransform rotate = group.Children[0] as RotateTransform;
                    if (rotate != null)
                    {
                        // Açıyı güncelle
                        rotate.Angle = direction;
                    }
                }
            }
        }
    }
}