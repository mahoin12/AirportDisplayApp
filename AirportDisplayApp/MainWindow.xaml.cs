using AirportDisplayApp.Models;
using AirportDisplayApp.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AirportDisplayApp
{
    public partial class MainWindow : Window
    {
        // Veri servisi
        private readonly DataService dataService;

        // UI elementleri sözlüğü
        private Dictionary<string, TextBlock> displayElements = new Dictionary<string, TextBlock>();

        public MainWindow()
        {
            InitializeComponent();

            // Set window to fullscreen borderless mode
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;

            // Veri servisini oluştur
            dataService = new DataService();
            dataService.ConnectionStatusChanged += DataService_ConnectionStatusChanged;
            dataService.DataUpdated += DataService_DataUpdated;

            // UI oluştur
            CreateAirportDisplay();

            // Saat güncelleyicisini başlat
            StartClockTimer();

            // Veri almayı başlat
            dataService.StartAsync();

            // Pencere kapandığında kaynakları temizle
            Closed += MainWindow_Closed;

            // ESC tuşu ile çıkış için event handler ekle
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // ESC tuşu ile uygulamadan çıkış
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void StartClockTimer()
        {
            // Saat güncelleme zamanlayıcısı
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateClock();
            timer.Start();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Veri servisini durdur
            dataService.Stop();
        }

        private void DataService_ConnectionStatusChanged(object sender, string status)
        {
            // UI güncelleme işlemleri UI thread'inde yapılmalı
            Dispatcher.Invoke(() =>
            {
                TextBlock connectionStatus = (TextBlock)FindName("ConnectionStatus");
                if (connectionStatus != null)
                {
                    connectionStatus.Text = $"Bağlantı Durumu: {status}";
                }
            });
        }

        private void DataService_DataUpdated(object sender, AirportDataModel data)
        {
            // UI güncelleme işlemleri UI thread'inde yapılmalı
            Dispatcher.Invoke(() =>
            {
                // Genel bilgileri güncelle
                UpdateTextBlock("time", data.Time);
                UpdateTextBlock("runwayInUse", data.RunwayInUse);
                UpdateTextBlock("rwyInUseInfo", data.RwyInUseInfo);

                // Merkez panel verilerini güncelle
                UpdateTextBlock("qnh", data.QNH);
                UpdateTextBlock("qnhInHg", data.QNHInHg);
                UpdateTextBlock("qfe", data.QFE);
                UpdateTextBlock("qfeSynop", data.QFESynop);
                UpdateTextBlock("low", data.Low);
                UpdateTextBlock("temp", data.Temperature);
                UpdateTextBlock("td", data.DewPoint);
                UpdateTextBlock("rh", data.RelativeHumidity);
                UpdateTextBlock("tmax", data.TempMax);
                UpdateTextBlock("tmin", data.TempMin);
                UpdateTextBlock("trwy", data.RunwayTemp);

                // METAR bilgisini güncelle
                UpdateTextBlock("metar", data.Metar);

                // RWY 35 verilerini güncelle
                UpdateTextBlock("leftWindSpeed", data.Runway35.WindSpeed);
                UpdateTextBlock("left2MinDir", data.Runway35.Min2Direction);
                UpdateTextBlock("left2MinSpeed", data.Runway35.Min2Speed);
                UpdateTextBlock("left2AvgDir", data.Runway35.Avg2Direction);
                UpdateTextBlock("left2AvgSpeed", data.Runway35.Avg2Speed);
                UpdateTextBlock("left2MaxDir", data.Runway35.Max2Direction);
                UpdateTextBlock("left2MaxSpeed", data.Runway35.Max2Speed);
                UpdateTextBlock("leftHwCw", data.Runway35.HwCw);
                UpdateTextBlock("baseValue", data.Runway35.Base);
                UpdateTextBlock("left10MinDir", data.Runway35.Min10Direction);
                UpdateTextBlock("left10MinSpeed", data.Runway35.Min10Speed);
                UpdateTextBlock("left10AvgDir", data.Runway35.Avg10Direction);
                UpdateTextBlock("left10AvgSpeed", data.Runway35.Avg10Speed);
                UpdateTextBlock("left10MaxDir", data.Runway35.Max10Direction);
                UpdateTextBlock("left10MaxSpeed", data.Runway35.Max10Speed);
                UpdateTextBlock("leftQfeValue", data.Runway35.QFE);
                UpdateTextBlock("leftQfeInHg", data.Runway35.QFEInHg);

                // RWY 17 verilerini güncelle
                UpdateTextBlock("rightWindSpeed", data.Runway17.WindSpeed);
                UpdateTextBlock("right2MinDir", data.Runway17.Min2Direction);
                UpdateTextBlock("right2MinSpeed", data.Runway17.Min2Speed);
                UpdateTextBlock("right2AvgDir", data.Runway17.Avg2Direction);
                UpdateTextBlock("right2AvgSpeed", data.Runway17.Avg2Speed);
                UpdateTextBlock("right2MaxDir", data.Runway17.Max2Direction);
                UpdateTextBlock("right2MaxSpeed", data.Runway17.Max2Speed);
                UpdateTextBlock("rightHwCw", data.Runway17.HwCw);
                UpdateTextBlock("right10MinDir", data.Runway17.Min10Direction);
                UpdateTextBlock("right10MinSpeed", data.Runway17.Min10Speed);
                UpdateTextBlock("right10AvgDir", data.Runway17.Avg10Direction);
                UpdateTextBlock("right10AvgSpeed", data.Runway17.Avg10Speed);
                UpdateTextBlock("right10MaxDir", data.Runway17.Max10Direction);
                UpdateTextBlock("right10MaxSpeed", data.Runway17.Max10Speed);
                UpdateTextBlock("rightQfeValue", data.Runway17.QFE);
                UpdateTextBlock("rightQfeInHg", data.Runway17.QFEInHg);

                // Rüzgar oklarını güncelle
                UpdateWindArrow("LeftWindArrow", data.Runway35.AvgWindDirection);
                UpdateWindArrow("RightWindArrow", data.Runway17.AvgWindDirection);
            });
        }

        private void UpdateTextBlock(string key, string value)
        {
            if (displayElements.TryGetValue(key, out TextBlock textBlock))
            {
                textBlock.Text = value;
            }
        }

        private void UpdateWindArrow(string arrowName, double direction)
        {
            try
            {
                // Ana rüzgar oku
                object arrowObj = FindName(arrowName);
                if (arrowObj is Path windArrow)
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
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir, ama UI'ı etkilememesi için sessizce geçiliyor
                System.Diagnostics.Debug.WriteLine($"Rüzgar oku güncellenirken hata: {ex.Message}");
            }
        }

        private void CreateAirportDisplay()
        {
            // Stil ayarları
            // No need to set Background color as it will inherit from parent
            Title = "Havaalanı Hava Durumu Göstergesi";
            // No need to set Width and Height as it will be fullscreen
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Ana Grid
            Grid mainGrid = new Grid();
            Content = mainGrid;

            // Ana Grid tanımı - satırlar
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) }); // Başlık
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) }); // Durum çubuğu
            mainGrid.RowDefinitions.Add(new RowDefinition()); // İçerik
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) }); // Alt bilgi çubuğu

            // Başlık
            Border headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 156, 178))
            };

            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerBorder.Child = headerGrid;

            TextBlock titleText = new TextBlock
            {
                Text = "VAISALA",
                Foreground = Brushes.White,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(20, 0, 0, 0)
            };

            TextBlock airportText = new TextBlock
            {
                Text = "LTBL",
                Foreground = Brushes.White,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock timeText = new TextBlock
            {
                Text = "00:00:00",
                Foreground = Brushes.White,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 20, 0),
                Name = "TimeDisplay"
            };
            this.RegisterName("TimeDisplay", timeText);
            displayElements["time"] = timeText;

            Grid.SetColumn(titleText, 0);
            Grid.SetColumn(airportText, 1);
            Grid.SetColumn(timeText, 2);

            headerGrid.Children.Add(titleText);
            headerGrid.Children.Add(airportText);
            headerGrid.Children.Add(timeText);

            Grid.SetRow(headerBorder, 0);
            mainGrid.Children.Add(headerBorder);

            // Durum Çubuğu
            Border statusBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
            };

            Grid statusGrid = new Grid();
            statusBorder.Child = statusGrid;

            statusGrid.ColumnDefinitions.Add(new ColumnDefinition());
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock rwInUseText = new TextBlock
            {
                Text = "KULLANILAN PİST",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };

            TextBlock rwInUseValue = new TextBlock
            {
                Text = "35",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 20, 0),
                Foreground = Brushes.Red,
                Name = "RwyInUseValue"
            };
            this.RegisterName("RwyInUseValue", rwInUseValue);
            displayElements["runwayInUse"] = rwInUseValue;

            Grid.SetColumn(rwInUseText, 1);
            Grid.SetColumn(rwInUseValue, 2);

            statusGrid.Children.Add(rwInUseText);
            statusGrid.Children.Add(rwInUseValue);

            Grid.SetRow(statusBorder, 1);
            mainGrid.Children.Add(statusBorder);

            // Ana İçerik Grid'i (3 sütunlu)
            Grid contentGrid = new Grid();

            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Sol Pist Paneli (RWY 35)
            CreateRunwayPanel(contentGrid, 0, "RWY 35", "35");

            // Orta Bilgi Paneli
            CreateCenterInfoPanel(contentGrid, 1);

            // Sağ Pist Paneli (RWY 17)
            CreateRunwayPanel(contentGrid, 2, "RWY 17", "17");

            Grid.SetRow(contentGrid, 2);
            mainGrid.Children.Add(contentGrid);

            // Alt bilgi çubuğu
            Border footerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
            };

            StackPanel footerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock connectionStatus = new TextBlock
            {
                Text = "Bağlantı Durumu: Başlatılıyor...",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                Name = "ConnectionStatus"
            };
            this.RegisterName("ConnectionStatus", connectionStatus);

            // Butonlar için bir panel
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Button refreshButton = new Button
            {
                Content = "Verileri Yenile",
                Padding = new Thickness(10, 3, 10, 3),
                Margin = new Thickness(0, 0, 10, 0)
            };
            refreshButton.Click += (s, e) => RefreshData();

            Button settingsButton = new Button
            {
                Content = "Ayarlar",
                Padding = new Thickness(10, 3, 10, 3),
                Margin = new Thickness(0, 0, 10, 0)
            };
            settingsButton.Click += (s, e) => ShowSettings();

            buttonPanel.Children.Add(refreshButton);
            buttonPanel.Children.Add(settingsButton);

            // Footer grid oluştur
            Grid footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(connectionStatus, 0);
            Grid.SetColumn(buttonPanel, 1);

            footerGrid.Children.Add(connectionStatus);
            footerGrid.Children.Add(buttonPanel);

            footerBorder.Child = footerGrid;
            Grid.SetRow(footerBorder, 3);
            mainGrid.Children.Add(footerBorder);

            // METAR bilgisi
            TextBlock metarBlock = new TextBlock
            {
                Text = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320",
                TextWrapping = TextWrapping.Wrap,
                Name = "MetarText",
                Margin = new Thickness(10, 0, 10, 5),
                FontSize = 11
            };
            this.RegisterName("MetarText", metarBlock);
            displayElements["metar"] = metarBlock;

            Border metarBorder = new Border
            {
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                Padding = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10, 0, 10, 5)
            };
            metarBorder.Child = metarBlock;

            Grid metarGrid = new Grid();
            metarGrid.Children.Add(metarBorder);

            Grid.SetRow(metarGrid, 2);
            Grid.SetColumnSpan(metarGrid, 3);
            mainGrid.Children.Add(metarGrid);
        }

        private void CreateRunwayPanel(Grid parentGrid, int column, string runwayName, string runwayNumber)
        {
            Border runwayBorder = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };

            Grid runwayGrid = new Grid();
            runwayGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Başlık
            runwayGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) }); // Rüzgar göstergesi
            runwayGrid.RowDefinitions.Add(new RowDefinition()); // Veri alanı
            runwayBorder.Child = runwayGrid;

            // Başlık
            Border headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 180, 180))
            };

            TextBlock headerText = new TextBlock
            {
                Text = runwayName,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            headerBorder.Child = headerText;

            Grid.SetRow(headerBorder, 0);
            runwayGrid.Children.Add(headerBorder);

            // Rüzgar göstergesi paneli
            Grid windIndicatorGrid = new Grid();
            CreateWindIndicator(windIndicatorGrid, runwayName == "RWY 35" ? "LeftWindArrow" : "RightWindArrow");
            Grid.SetRow(windIndicatorGrid, 1);
            runwayGrid.Children.Add(windIndicatorGrid);

            // Veri paneli
            ScrollViewer dataScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(5);

            // Veri satırları için RowDefinition'ları ekle
            for (int i = 0; i < 9; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            string prefix = runwayName == "RWY 35" ? "left" : "right";

            // 2" Mnm verileri
            CreateDataRow(dataGrid, 0, "2\" Mnm", prefix + "2MinDir", prefix + "2MinSpeed", "250", "3");

            // 2" Avg verileri
            CreateDataRow(dataGrid, 1, "2\" Avg", prefix + "2AvgDir", prefix + "2AvgSpeed",
                runwayName == "RWY 35" ? "290" : "270", "5");

            // 2" Max verileri
            CreateDataRow(dataGrid, 2, "2\" Max", prefix + "2MaxDir", prefix + "2MaxSpeed",
                "320", runwayName == "RWY 35" ? "8" : "6");

            // 2" Hw/Cw verileri
            CreateHwCwRow(dataGrid, 3, "Hw/Cw", prefix + "HwCw",
                runwayName == "RWY 35" ? "H02  L05" : "T01  R05");

            // Base verisi (sadece RWY 35 için)
            if (runwayName == "RWY 35")
            {
                CreateBaseRow(dataGrid, 4, "Base", "baseValue", "NCD");
            }

            // 10" Mnm verileri
            CreateDataRow(dataGrid, 5, "10\" Mnm", prefix + "10MinDir", prefix + "10MinSpeed",
                runwayName == "RWY 35" ? "* CALM" : "220",
                runwayName == "RWY 35" ? "" : "2");

            // 10" Avg verileri
            CreateDataRow(dataGrid, 6, "10\" Avg", prefix + "10AvgDir", prefix + "10AvgSpeed",
                runwayName == "RWY 35" ? "280" : "290", "5");

            // 10" Max verileri
            CreateDataRow(dataGrid, 7, "10\" Max", prefix + "10MaxDir", prefix + "10MaxSpeed",
                "320", runwayName == "RWY 35" ? "9" : "8");

            // QFE verileri
            CreateQfeRow(dataGrid, 8, "QFE", prefix + "QfeValue", prefix + "QfeInHg",
                runwayName == "RWY 35" ? "1012.8" : "1012.7",
                runwayName == "RWY 35" ? "29.91" : "29.90");

            dataScrollViewer.Content = dataGrid;
            Grid.SetRow(dataScrollViewer, 2);
            runwayGrid.Children.Add(dataScrollViewer);

            Grid.SetColumn(runwayBorder, column);
            parentGrid.Children.Add(runwayBorder);
        }

        private void CreateWindIndicator(Grid parent, string arrowName)
        {
            // Ana konteyner (canvas)
            Canvas windCanvas = new Canvas
            {
                Width = 300,
                Height = 300
            };

            // Dış daire (wind rose)
            Ellipse outerCircle = new Ellipse
            {
                Width = 260,
                Height = 260,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(outerCircle, 20);
            Canvas.SetTop(outerCircle, 20);
            windCanvas.Children.Add(outerCircle);

            // İç daire (merkez belirteci)
            Ellipse innerCircle = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Gray
            };
            Canvas.SetLeft(innerCircle, 140);
            Canvas.SetTop(innerCircle, 140);
            windCanvas.Children.Add(innerCircle);

            // Yön işaretleri (her 30 derecede bir)
            for (int i = 0; i < 360; i += 30)
            {
                double radians = i * Math.PI / 180;
                double centerX = 150;
                double centerY = 150;
                double radius = 130;

                // Dış nokta
                double x1 = centerX + Math.Sin(radians) * radius;
                double y1 = centerY - Math.Cos(radians) * radius;

                // İç nokta
                double x2 = centerX + Math.Sin(radians) * (radius - 15);
                double y2 = centerY - Math.Cos(radians) * (radius - 15);

                // Çizgiyi oluştur
                Line tickLine = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                windCanvas.Children.Add(tickLine);

                // Etiket metni
                string mark = i == 0 ? "00" : (i / 10).ToString("00");
                TextBlock directionMark = new TextBlock
                {
                    Text = mark,
                    FontSize = 12,
                    Foreground = Brushes.Black
                };

                // Etiketi çemberden biraz daha uzağa yerleştir
                double textRadius = radius + 15;
                double textX = centerX + Math.Sin(radians) * textRadius - 10;
                double textY = centerY - Math.Cos(radians) * textRadius - 8;

                Canvas.SetLeft(directionMark, textX);
                Canvas.SetTop(directionMark, textY);
                windCanvas.Children.Add(directionMark);
            }

            // Rüzgar oku (path element)
            Path windArrow = new Path
            {
                Data = Geometry.Parse("M 150,150 L 150,60 L 160,80 L 150,70 L 140,80 Z"),
                Fill = Brushes.DarkGray,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Name = arrowName,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            // Transform için döndürme
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform(0));
            windArrow.RenderTransform = transformGroup;

            this.RegisterName(arrowName, windArrow);
            windCanvas.Children.Add(windArrow);

            // Rüzgar hızı göstergesi
            Border speedBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 200, 200)),
                Width = 40,
                Height = 30,
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4)
            };

            TextBlock speedText = new TextBlock
            {
                Text = arrowName.Contains("Left") ? "7" : "5",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Name = arrowName.Contains("Left") ? "leftWindSpeed" : "rightWindSpeed"
            };

            this.RegisterName(speedText.Name, speedText);
            displayElements[speedText.Name] = speedText;

            speedBorder.Child = speedText;
            Canvas.SetLeft(speedBorder, 130);
            Canvas.SetTop(speedBorder, 175);
            windCanvas.Children.Add(speedBorder);

            // Ana gridi ekle
            parent.Children.Add(windCanvas);
        }

        private void CreateCenterInfoPanel(Grid parentGrid, int column)
        {
            Border centerBorder = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };

            Grid centerGrid = new Grid();
            centerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Başlık
            centerGrid.RowDefinitions.Add(new RowDefinition()); // Veri alanı
            centerBorder.Child = centerGrid;

            // Başlık
            Border headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 180, 180))
            };

            TextBlock headerText = new TextBlock
            {
                Text = "Çiğli Havaalanı",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            headerBorder.Child = headerText;

            Grid.SetRow(headerBorder, 0);
            centerGrid.Children.Add(headerBorder);

            // Veri alanı
            ScrollViewer dataScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(10);

            // Veri satırları
            for (int i = 0; i < 15; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Kullanılan Pist
            CreateCenterDataRow(dataGrid, 0, "RWYINUSE", "rwyInUseInfo", "35");

            // QNH
            CreateCenterDataRow(dataGrid, 1, "QNH", "qnh", "1013.2", "hPa");

            // QNH (inHg)
            CreateCenterDataRow(dataGrid, 2, "QNH", "qnhInHg", "29.92", "inHg");

            // QFE
            CreateCenterDataRow(dataGrid, 3, "QFE", "qfe", "1013.2", "hPa");

            // QFE SYNOP
            CreateCenterDataRow(dataGrid, 4, "QFE SYNOP", "qfeSynop", "1012.7", "hPa");

            // LOW
            CreateCenterDataRow(dataGrid, 5, "LOW", "low", "NCD", "");

            // Temp
            CreateCenterDataRow(dataGrid, 6, "Temp", "temp", "30.4", "°C");

            // Dew Point
            CreateCenterDataRow(dataGrid, 7, "Td", "td", "19.7", "°C");

            // Relative Humidity
            CreateCenterDataRow(dataGrid, 8, "RH", "rh", "52", "%");

            // Max Temperature
            CreateCenterDataRow(dataGrid, 9, "Tmax", "tmax", "30.9", "°C");

            // Min Temperature
            CreateCenterDataRow(dataGrid, 10, "Tmin", "tmin", "20.4", "°C");

            // Runway Temperature
            CreateCenterDataRow(dataGrid, 11, "Trwy", "trwy", "49.9", "°C");

            dataScrollViewer.Content = dataGrid;
            Grid.SetRow(dataScrollViewer, 1);
            centerGrid.Children.Add(dataScrollViewer);

            Grid.SetColumn(centerBorder, column);
            parentGrid.Children.Add(centerBorder);
        }

        private void CreateCenterDataRow(Grid grid, int row, string label, string valueKey, string defaultValue, string unit = "")
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(0, 5, 0, 5);
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                Foreground = Brushes.DarkBlue,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MinWidth = 80
            };

            TextBlock valueText = new TextBlock
            {
                Text = defaultValue,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Name = valueKey + "Value"
            };
            this.RegisterName(valueKey + "Value", valueText);
            displayElements[valueKey] = valueText;

            TextBlock unitText = new TextBlock
            {
                Text = unit,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 0, 0)
            };

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);
            Grid.SetColumn(unitText, 2);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);

            if (!string.IsNullOrEmpty(unit))
            {
                rowGrid.Children.Add(unitText);
            }

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        private void CreateDataRow(Grid grid, int row, string label, string directionKey, string speedKey,
            string defaultDirection, string defaultSpeed)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(0, 5, 0, 5);
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 10, 0),
                MinWidth = 60
            };

            TextBlock directionText = new TextBlock
            {
                Text = defaultDirection,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 5, 0),
                MinWidth = 50,
                Name = directionKey + "Value"
            };
            this.RegisterName(directionKey + "Value", directionText);
            displayElements[directionKey] = directionText;

            TextBlock dirUnitText = new TextBlock
            {
                Text = "°",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 15, 0)
            };

            TextBlock speedText = null;
            TextBlock speedUnitText = null;

            if (!string.IsNullOrEmpty(defaultSpeed))
            {
                speedText = new TextBlock
                {
                    Text = defaultSpeed,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Red,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    MinWidth = 30,
                    Margin = new Thickness(0, 0, 5, 0),
                    Name = speedKey + "Value"
                };
                this.RegisterName(speedKey + "Value", speedText);
                displayElements[speedKey] = speedText;

                speedUnitText = new TextBlock
                {
                    Text = "kt",
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(directionText, 1);
            Grid.SetColumn(dirUnitText, 2);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(directionText);
            rowGrid.Children.Add(dirUnitText);

            if (speedText != null && speedUnitText != null)
            {
                Grid.SetColumn(speedText, 3);
                Grid.SetColumn(speedUnitText, 4);
                rowGrid.Children.Add(speedText);
                rowGrid.Children.Add(speedUnitText);
            }

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        private void CreateHwCwRow(Grid grid, int row, string label, string valueKey, string defaultValue)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(0, 5, 0, 5);
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 10, 0),
                MinWidth = 60
            };

            TextBlock valueText = new TextBlock
            {
                Text = defaultValue,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                Name = valueKey + "Value"
            };
            this.RegisterName(valueKey + "Value", valueText);
            displayElements[valueKey] = valueText;

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        private void CreateBaseRow(Grid grid, int row, string label, string valueKey, string defaultValue)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(0, 5, 0, 5);
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 10, 0),
                MinWidth = 60
            };

            TextBlock valueText = new TextBlock
            {
                Text = defaultValue,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Name = valueKey + "Value"
            };
            this.RegisterName(valueKey + "Value", valueText);
            displayElements[valueKey] = valueText;

            TextBlock unitText = new TextBlock
            {
                Text = "ft",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            };

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);
            Grid.SetColumn(unitText, 2);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);
            rowGrid.Children.Add(unitText);

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        private void CreateQfeRow(Grid grid, int row, string label, string valueKey, string inHgKey,
            string defaultValue, string defaultInHg)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(0, 5, 0, 5);
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock labelText = new TextBlock
            {
                Text = label,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 10, 0),
                MinWidth = 60
            };

            TextBlock valueText = new TextBlock
            {
                Text = defaultValue,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 5, 0),
                MinWidth = 60,
                Name = valueKey + "Value"
            };
            this.RegisterName(valueKey + "Value", valueText);
            displayElements[valueKey] = valueText;

            TextBlock unitText = new TextBlock
            {
                Text = "hPa",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 15, 0)
            };

            TextBlock inHgText = new TextBlock
            {
                Text = defaultInHg,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 5, 0),
                MinWidth = 40,
                Name = inHgKey + "Value"
            };
            this.RegisterName(inHgKey + "Value", inHgText);
            displayElements[inHgKey] = inHgText;

            TextBlock inHgUnitText = new TextBlock
            {
                Text = "inHg",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center
            };

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

        private void UpdateClock()
        {
            TextBlock timeDisplay = (TextBlock)FindName("TimeDisplay");
            if (timeDisplay != null)
            {
                timeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            }
        }

        private void RefreshData()
        {
            // Verileri yenile
            TextBlock connectionStatus = (TextBlock)FindName("ConnectionStatus");
            if (connectionStatus != null)
            {
                connectionStatus.Text = "Bağlantı Durumu: Veriler yenileniyor...";
            }

            // Gelecekteki uygulamada dataService.RefreshData() çağrılacak
        }

        private void ShowSettings()
        {
            MessageBox.Show(
                "Havaalanı Göstergesi Ayarları\n\n" +
                "Bu pencerede bağlantı ayarları, görsel tercihler ve alarm limitleri yapılandırılabilir.",
                "Ayarlar",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}