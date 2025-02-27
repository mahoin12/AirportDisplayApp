using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AirportDisplayApp.UI
{
    public class RunwayPanelBuilder
    {
        private readonly Window parentWindow;
        private readonly Dictionary<string, TextBlock> displayElements;
        
        public RunwayPanelBuilder(Window parentWindow, Dictionary<string, TextBlock> displayElements)
        {
            this.parentWindow = parentWindow;
            this.displayElements = displayElements;
        }
        
        public Border CreateRunwayPanel(string runwayName, Grid parentGrid, int column)
        {
            Border runwayBorder = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };
            
            Grid runwayGrid = new Grid();
            runwayBorder.Child = runwayGrid;
            
            runwayGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            runwayGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(300) });
            runwayGrid.RowDefinitions.Add(new RowDefinition());
            
            // Başlık
            Border runwayHeaderBorder = CreateRunwayHeader(runwayName);
            runwayGrid.Children.Add(runwayHeaderBorder);
            Grid.SetRow(runwayHeaderBorder, 0);
            
            // Rüzgar yönü göstergesi
            Grid windGrid = CreateWindIndicator(runwayName);
            runwayGrid.Children.Add(windGrid);
            Grid.SetRow(windGrid, 1);
            
            // Veri Grid'i
            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(10);
            
            // Satırlar ekle
            for (int i = 0; i < 9; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            }
            
            // Veri satırlarını oluştur
            CreateRunwayDataRows(runwayName, dataGrid);
            
            runwayGrid.Children.Add(dataGrid);
            Grid.SetRow(dataGrid, 2);
            
            parentGrid.Children.Add(runwayBorder);
            Grid.SetColumn(runwayBorder, column);
            
            return runwayBorder;
        }
        
        private Border CreateRunwayHeader(string runwayName)
        {
            Border headerBorder = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(240, 180, 180))
            };
            
            TextBlock headerText = new TextBlock
            {
                Text = runwayName,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            headerBorder.Child = headerText;
            
            return headerBorder;
        }
        
        private Grid CreateWindIndicator(string runwayName)
        {
            Grid windGrid = new Grid();
            windGrid.Margin = new Thickness(10);
            
            // Dairesel ölçek
            Ellipse outerCircle = new Ellipse
            {
                Width = 250,
                Height = 250,
                Stroke = Brushes.LightGray,
                StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            windGrid.Children.Add(outerCircle);
            
            // Yön işaretleri
            for (int i = 0; i < 12; i++)
            {
                double angle = i * 30;
                double radians = angle * Math.PI / 180;
                
                // Pozisyon hesapla
                double x = 125 + Math.Sin(radians) * 120;
                double y = 125 - Math.Cos(radians) * 120;
                
                // Etiket metni
                string mark = ((i * 3) % 36).ToString("D2");
                
                TextBlock directionMark = new TextBlock
                {
                    Text = mark,
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                
                // Metni yerleştir
                Canvas.SetLeft(directionMark, x - 10);
                Canvas.SetTop(directionMark, y - 10);
                
                // Panele ekle
                Canvas markCanvas = new Canvas();
                markCanvas.Children.Add(directionMark);
                windGrid.Children.Add(markCanvas);
            }
            
            // 30° aralıklarla rüzgar yönü çizgileri
            for (int i = 0; i < 12; i++)
            {
                double angle = i * 30;
                double radians = angle * Math.PI / 180;
                
                double startX = 125 + Math.Sin(radians) * 110;
                double startY = 125 - Math.Cos(radians) * 110;
                double endX = 125 + Math.Sin(radians) * 125;
                double endY = 125 - Math.Cos(radians) * 125;
                
                Line directionLine = new Line
                {
                    X1 = startX,
                    Y1 = startY,
                    X2 = endX,
                    Y2 = endY,
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 2
                };
                
                Canvas lineCanvas = new Canvas();
                lineCanvas.Children.Add(directionLine);
                windGrid.Children.Add(lineCanvas);
            }
            
            // Rüzgar yönü oku
            Rectangle windArrow = new Rectangle
            {
                Width = 20,
                Height = 100,
                Fill = Brushes.DarkGray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 1.0)
            };
            
            // Döndürme için transform grubu
            TransformGroup transformGroup = new TransformGroup();
            RotateTransform rotateTransform = new RotateTransform(0);
            transformGroup.Children.Add(rotateTransform);
            windArrow.RenderTransform = transformGroup;
            
            string arrowName = runwayName == "RWY 35" ? "LeftWindArrow" : "RightWindArrow";
            parentWindow.RegisterName(arrowName, windArrow);
            
            // Rüzgar hızı göstergesi
            Border windSpeedBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 180, 180)),
                Width = 40,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 40)
            };
            
            TextBlock windSpeedText = new TextBlock
            {
                Text = runwayName == "RWY 35" ? "7" : "5",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = runwayName == "RWY 35" ? "LeftWindSpeed" : "RightWindSpeed"
            };
            parentWindow.RegisterName(windSpeedText.Name, windSpeedText);
            
            string speedKey = runwayName == "RWY 35" ? "leftWindSpeed" : "rightWindSpeed";
            displayElements[speedKey] = windSpeedText;
            
            windSpeedBorder.Child = windSpeedText;
            
            // Pist göstergesi
            Rectangle runwayIndicator = new Rectangle
            {
                Width = 15,
                Height = 40,
                Fill = Brushes.DarkGray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            
            // Ekle
            windGrid.Children.Add(windArrow);
            windGrid.Children.Add(windSpeedBorder);
            windGrid.Children.Add(runwayIndicator);
            
            return windGrid;
        }
        
        private void CreateRunwayDataRows(string runwayName, Grid dataGrid)
        {
            string prefix = runwayName == "RWY 35" ? "Left" : "Right";
            string prefixLower = runwayName == "RWY 35" ? "left" : "right";
            
            // 2" Mnm verileri
            CreateRunwayDataRow(dataGrid, 0, "2\" Mnm", "250", "3", 
                prefix + "2MinDir", prefix + "2MinSpeed");
            
            displayElements[prefixLower + "2MinDir"] = (TextBlock)parentWindow.FindName(prefix + "2MinDir");
            displayElements[prefixLower + "2MinSpeed"] = (TextBlock)parentWindow.FindName(prefix + "2MinSpeed");
            
            // 2" Avg verileri
            string avgDir = runwayName == "RWY 35" ? "290" : "270";
            CreateRunwayDataRow(dataGrid, 1, "2\" Avg", avgDir, "5", 
                prefix + "2AvgDir", prefix + "2AvgSpeed");
            
            displayElements[prefixLower + "2AvgDir"] = (TextBlock)parentWindow.FindName(prefix + "2AvgDir");
            displayElements[prefixLower + "2AvgSpeed"] = (TextBlock)parentWindow.FindName(prefix + "2AvgSpeed");
            
            // 2" Max verileri
            string maxSpeed = runwayName == "RWY 35" ? "8" : "6";
            CreateRunwayDataRow(dataGrid, 2, "2\" Max", "320", maxSpeed, 
                prefix + "2MaxDir", prefix + "2MaxSpeed");
            
            displayElements[prefixLower + "2MaxDir"] = (TextBlock)parentWindow.FindName(prefix + "2MaxDir");
            displayElements[prefixLower + "2MaxSpeed"] = (TextBlock)parentWindow.FindName(prefix + "2MaxSpeed");
            
            // 2" Hw/Cw verileri
            string hwCwValue = runwayName == "RWY 35" ? "H02   L05" : "T01   R05";
            CreateRunwayDataRow(dataGrid, 3, "2\" Hw/Cw", hwCwValue, "", prefix + "HwCw", "");
            
            displayElements[prefixLower + "HwCw"] = (TextBlock)parentWindow.FindName(prefix + "HwCw");
            
            // Sol pist için Base verileri
            if (runwayName == "RWY 35")
            {
                CreateBaseDataRow(dataGrid, 4, "Base", "NCD");
                displayElements["baseValue"] = (TextBlock)parentWindow.FindName("BaseValue");
            }
            
            // 10" Mnm verileri
            string minValue = runwayName == "RWY 35" ? "* CALM" : "220";
            string minSpeed = runwayName == "RWY 35" ? "" : "2";
            CreateRunwayDataRow(dataGrid, 5, "10\" Mnm", minValue, minSpeed, 
                prefix + "10MinDir", prefix + "10MinSpeed");
            
            displayElements[prefixLower + "10MinDir"] = (TextBlock)parentWindow.FindName(prefix + "10MinDir");
            displayElements[prefixLower + "10MinSpeed"] = (TextBlock)parentWindow.FindName(prefix + "10MinSpeed");
            
            // 10" Avg verileri
            string avg10Dir = runwayName == "RWY 35" ? "280" : "290";
            CreateRunwayDataRow(dataGrid, 6, "10\" Avg", avg10Dir, "5", 
                prefix + "10AvgDir", prefix + "10AvgSpeed");
            
            displayElements[prefixLower + "10AvgDir"] = (TextBlock)parentWindow.FindName(prefix + "10AvgDir");
            displayElements[prefixLower + "10AvgSpeed"] = (TextBlock)parentWindow.FindName(prefix + "10AvgSpeed");
            
            // 10" Max verileri
            string max10Speed = runwayName == "RWY 35" ? "9" : "8";
            CreateRunwayDataRow(dataGrid, 7, "10\" Max", "320", max10Speed, 
                prefix + "10MaxDir", prefix + "10MaxSpeed");
            
            displayElements[prefixLower + "10MaxDir"] = (TextBlock)parentWindow.FindName(prefix + "10MaxDir");
            displayElements[prefixLower + "10MaxSpeed"] = (TextBlock)parentWindow.FindName(prefix + "10MaxSpeed");
            
            // QFE verileri
            CreateQfeDataRow(dataGrid, 8, "QFE", 
                runwayName == "RWY 35" ? "1012.8" : "1012.7", 
                runwayName == "RWY 35" ? "29.91" : "29.90", 
                prefix);
            
            displayElements[prefixLower + "QfeValue"] = (TextBlock)parentWindow.FindName(prefix + "QfeValue");
            displayElements[prefixLower + "QfeMm"] = (TextBlock)parentWindow.FindName(prefix + "QfeMmValue");
        }
        
        public void UpdateWindArrow(string arrowName, double direction)
        {
            try
            {
                // Ana rüzgar oku
                object arrowObj = parentWindow.FindName(arrowName);
                if (arrowObj is Rectangle windArrow)
                {
                    // Transform grubunu al
                    TransformGroup group = windArrow.RenderTransform as TransformGroup;
                    if (group != null)
                    {
                        // Döndürme transform'ını al
                        RotateTransform rotate = group.Children[0] as RotateTransform;
                        if (rotate != null)
                        {
                            // Açıyı güncelle
                            rotate.Angle = direction;
                        }
                    }
                }
                
                // Rüzgar oku yanındaki gösterge çizgileri, işaretler ve diğer elemanları da güncelle
                string panelName = arrowName.Replace("WindArrow", "");
                UpdateWindPanelColors(panelName, direction);
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir, ama UI'ı etkilememesi için sessizce geçiliyor
                System.Diagnostics.Debug.WriteLine($"Rüzgar oku güncellenirken hata: {ex.Message}");
            }
        }
        
        private void UpdateWindPanelColors(string panelName, double windDirection)
        {
            // Rüzgar yönüne göre paneldeki renk kodlamaları burada yapılabilir
            try
            {
                // Örnek: Pist yönüne dik bir rüzgar olduğunda crosswind uyarısı eklemek için
                double runwayDirection = panelName.Contains("Left") ? 350 : 170; // RWY 35 ve RWY 17
                double crosswindAngle = Math.Abs(((windDirection - runwayDirection + 180) % 360) - 180);
                
                // Crosswind açısı 45 dereceden fazlaysa uyarı rengini değiştirebiliriz
                if (crosswindAngle > 45)
                {
                    // İlgili UI elemanlarının rengini değiştir
                    string hwCwElementName = panelName + "HwCw";
                    object hwCwObj = parentWindow.FindName(hwCwElementName);
                    if (hwCwObj is TextBlock hwCwText)
                    {
                        hwCwText.Foreground = Brushes.DarkRed;
                    }
                }
            }
            catch
            {
                // Hata durumunda renk güncellemesi için kritik olmadığından sessizce geç
            }
        }
        
        private void CreateRunwayDataRow(Grid grid, int row, string label, string direction, string speed, string directionName, string speedName)
        {
            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 0)
            };
            
            TextBlock directionText = new TextBlock
            {
                Text = direction,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            if (!string.IsNullOrEmpty(directionName))
            {
                directionText.Name = directionName;
                parentWindow.RegisterName(directionName, directionText);
            }
            
            TextBlock unitText = new TextBlock
            {
                Text = "°",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0)
            };
            
            TextBlock speedText = new TextBlock
            {
                Text = speed,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20, 0, 0, 0)
            };
            
            if (!string.IsNullOrEmpty(speedName))
            {
                speedText.Name = speedName;
                parentWindow.RegisterName(speedName, speedText);
            }
            
            TextBlock ktText = new TextBlock
            {
                Text = "kt",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0)
            };
            
            // Yatay yönlü panel oluştur
            StackPanel directionPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            directionPanel.Children.Add(directionText);
            directionPanel.Children.Add(unitText);
            
            StackPanel speedPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            speedPanel.Children.Add(speedText);
            speedPanel.Children.Add(ktText);
            
            // Satır için grid oluştur
            Grid rowGrid = new Grid();
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
            
            rowGrid.Children.Add(labelText);
            Grid.SetColumn(labelText, 0);
            
            rowGrid.Children.Add(directionPanel);
            Grid.SetColumn(directionPanel, 1);
            
            if (!string.IsNullOrEmpty(speed))
            {
                rowGrid.Children.Add(speedPanel);
                Grid.SetColumn(speedPanel, 2);
            }
            
            grid.Children.Add(rowGrid);
            Grid.SetRow(rowGrid, row);
        }
        
        private void CreateBaseDataRow(Grid grid, int row, string label, string value)
        {
            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 0)
            };
            
            TextBlock valueText = new TextBlock
            {
                Text = value,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = "BaseValue"
            };
            parentWindow.RegisterName("BaseValue", valueText);
            
            TextBlock unitText = new TextBlock
            {
                Text = "ft",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0)
            };
            
            // Yatay yönlü panel oluştur
            StackPanel valuePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            valuePanel.Children.Add(valueText);
            valuePanel.Children.Add(unitText);
            
            // Satır için grid oluştur
            Grid rowGrid = new Grid();
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
            
            rowGrid.Children.Add(labelText);
            Grid.SetColumn(labelText, 0);
            
            rowGrid.Children.Add(valuePanel);
            Grid.SetColumn(valuePanel, 1);
            
            grid.Children.Add(rowGrid);
            Grid.SetRow(rowGrid, row);
        }
        
        private void CreateQfeDataRow(Grid grid, int row, string label, string value, string mmValue, string prefix)
        {
            // Benzersiz isimler oluştur
            string valueName = prefix + "QfeValue";
            string mmValueName = prefix + "QfeMmValue";
            
            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 0)
            };
            
            TextBlock valueText = new TextBlock
            {
                Text = value,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 0),
                Name = valueName
            };
            parentWindow.RegisterName(valueName, valueText);
            
            TextBlock unitText = new TextBlock
            {
                Text = "hPa",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 0, 0)
            };
            
            TextBlock mmValueText = new TextBlock
            {
                Text = mmValue,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20, 0, 5, 0),
                Name = mmValueName
            };
            parentWindow.RegisterName(mmValueName, mmValueText);
            
            TextBlock mmUnitText = new TextBlock
            {
                Text = "inHg",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 0, 0)
            };
            
            // Satır için grid oluştur
            Grid rowGrid = new Grid();
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
            
            // Değerler ve birimler için paneller oluştur
            StackPanel valuePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            valuePanel.Children.Add(valueText);
            valuePanel.Children.Add(unitText);
            
            StackPanel mmValuePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            mmValuePanel.Children.Add(mmValueText);
            mmValuePanel.Children.Add(mmUnitText);
            
            rowGrid.Children.Add(labelText);
            Grid.SetColumn(labelText, 0);
            
            rowGrid.Children.Add(valuePanel);
            Grid.SetColumn(valuePanel, 1);
            
            rowGrid.Children.Add(mmValuePanel);
            Grid.SetColumn(mmValuePanel, 2);
            
            grid.Children.Add(rowGrid);
            Grid.SetRow(rowGrid, row);
        }
    }
}