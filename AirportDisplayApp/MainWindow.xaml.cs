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
            
            // Veri servisini oluştur
            dataService = new DataService();
            dataService.ConnectionStatusChanged += DataService_ConnectionStatusChanged;
            dataService.DataUpdated += DataService_DataUpdated;
            
            // UI oluştur
            CreateAirportDisplay();
            
            // Veri almayı başlat
            dataService.StartAsync();
            
            // Pencere kapandığında kaynakları temizle
            Closed += MainWindow_Closed;
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
                UpdateTextBlock("qnhMm", data.QNHInHg);
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
                UpdateTextBlock("leftQfeMm", data.Runway35.QFEInHg);
                
                // RWY 17 verilerini güncelle
                UpdateTextBlock("rightWindSpeed", data.Runway17.WindSpeed);
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
                UpdateTextBlock("rightQfeMm", data.Runway17.QFEInHg);
                
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
                object arrowObj = FindName(arrowName);
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
            }
            catch
            {
                // Sessizce hatayı geç
            }
        }
        
        /// <summary>
        /// Havaalanı görüntüleme panelini oluşturur
        /// </summary>
        private void CreateAirportDisplay()
        {
            // Stil ayarları
            Background = new SolidColorBrush(Colors.LightGray);
            Title = "Havaalanı Hava Durumu Göstergesi";
            Width = 1024;
            Height = 768;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Ana Grid
            Grid mainGrid = new Grid();
            Content = mainGrid;

            // Başlık
            Border headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 156, 178)),
                Height = 60
            };
            
            Grid headerGrid = new Grid();
            headerBorder.Child = headerGrid;
            
            TextBlock titleText = new TextBlock
            {
                Text = "VAISALA",
                Foreground = Brushes.White,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(20, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
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
                Margin = new Thickness(0, 0, 20, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Name = "TimeDisplay"
            };
            this.RegisterName("TimeDisplay", timeText);
            displayElements["time"] = timeText;

            headerGrid.Children.Add(titleText);
            headerGrid.Children.Add(airportText);
            headerGrid.Children.Add(timeText);
            
            mainGrid.Children.Add(headerBorder);
            Grid.SetRow(headerBorder, 0);

            // Durum Çubuğu
            Border statusBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Height = 30,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 60, 0, 0)
            };
            
            Grid statusGrid = new Grid();
            statusBorder.Child = statusGrid;
            
            TextBlock rwInUseText = new TextBlock
            {
                Text = "KULLANILAN PİST",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 20, 0)
            };
            
            TextBlock rwInUseValue = new TextBlock
            {
                Text = "35",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 100, 0),
                Name = "RunwayInUse"
            };
            this.RegisterName("RunwayInUse", rwInUseValue);
            displayElements["runwayInUse"] = rwInUseValue;
            
            statusGrid.Children.Add(rwInUseText);
            statusGrid.Children.Add(rwInUseValue);
            
            mainGrid.Children.Add(statusBorder);
            
            // Ana İçerik Grid'i (3 sütunlu)
            Grid contentGrid = new Grid();
            contentGrid.Margin = new Thickness(0, 90, 0, 30);
            
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            
            mainGrid.Children.Add(contentGrid);

            // Sol Pist Paneli
            Border leftRunwayBorder = CreateRunwayPanel("RWY 35", contentGrid, 0);
            
            // Merkez Havaalanı Bilgi Paneli
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
            airportDataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            
            // Tüm veri alanları için satırlar ekle
            for (int i = 0; i < 15; i++)
            {
                airportDataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            }
            
            // RWYINUSE
            TextBlock rwyInUseLabel = CreateInfoLabel("RWYINUSE", airportDataGrid, 0);
            TextBlock rwyInUseVal = CreateInfoValue("35", airportDataGrid, 0, "RunwayInUseInfo");
            displayElements["rwyInUseInfo"] = rwyInUseVal;
            
            // QNH
            TextBlock qnhLabel = CreateInfoLabel("QNH", airportDataGrid, 1);
            TextBlock qnhVal = CreateInfoValue("1013.2", airportDataGrid, 1, "QNHValue");
            displayElements["qnh"] = qnhVal;
            
            TextBlock qnhUnitLabel = new TextBlock
            {
                Text = "hPa",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(qnhUnitLabel);
            Grid.SetRow(qnhUnitLabel, 1);
            
            // QNH mmHg
            TextBlock qnhMmLabel = CreateInfoLabel("QNH", airportDataGrid, 2);
            TextBlock qnhMmVal = CreateInfoValue("29.92", airportDataGrid, 2, "QNHMmValue");
            displayElements["qnhMm"] = qnhMmVal;
            
            TextBlock qnhMmUnitLabel = new TextBlock
            {
                Text = "inHg",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(qnhMmUnitLabel);
            Grid.SetRow(qnhMmUnitLabel, 2);
            
            // QFE
            TextBlock qfeLabel = CreateInfoLabel("QFE", airportDataGrid, 3);
            TextBlock qfeVal = CreateInfoValue("1013.2", airportDataGrid, 3, "QFEValue");
            displayElements["qfe"] = qfeVal;
            
            TextBlock qfeUnitLabel = new TextBlock
            {
                Text = "hPa",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(qfeUnitLabel);
            Grid.SetRow(qfeUnitLabel, 3);
            
            // QFE SYNOP
            TextBlock qfeSynopLabel = CreateInfoLabel("QFE SYNOP", airportDataGrid, 4);
            TextBlock qfeSynopVal = CreateInfoValue("1012.7", airportDataGrid, 4, "QFESynopValue");
            displayElements["qfeSynop"] = qfeSynopVal;
            
            TextBlock qfeSynopUnitLabel = new TextBlock
            {
                Text = "hPa",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(qfeSynopUnitLabel);
            Grid.SetRow(qfeSynopUnitLabel, 4);
            
            // Aerodrome sections
            TextBlock aerodromeLabel = CreateInfoLabel("QFE AERODROME", airportDataGrid, 5);
            
            // HIGH
            TextBlock highLabel = CreateInfoLabel("HIGH", airportDataGrid, 6);
            
            // MID
            TextBlock midLabel = CreateInfoLabel("MID", airportDataGrid, 7);
            
            // LOW
            TextBlock lowLabel = CreateInfoLabel("LOW", airportDataGrid, 8);
            TextBlock lowVal = CreateInfoValue("NCD", airportDataGrid, 8, "LowValue");
            displayElements["low"] = lowVal;
            
            // Temperature
            TextBlock tempLabel = CreateInfoLabel("Temp", airportDataGrid, 9);
            TextBlock tempVal = CreateInfoValue("30.4", airportDataGrid, 9, "TempValue");
            displayElements["temp"] = tempVal;
            
            TextBlock tempUnitLabel = new TextBlock
            {
                Text = "°C",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(tempUnitLabel);
            Grid.SetRow(tempUnitLabel, 9);
            
            // Td
            TextBlock tdLabel = CreateInfoLabel("Td", airportDataGrid, 10);
            TextBlock tdVal = CreateInfoValue("19.7", airportDataGrid, 10, "TdValue");
            displayElements["td"] = tdVal;
            
            TextBlock tdUnitLabel = new TextBlock
            {
                Text = "°C",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(tdUnitLabel);
            Grid.SetRow(tdUnitLabel, 10);
            
            // RH
            TextBlock rhLabel = CreateInfoLabel("RH", airportDataGrid, 11);
            TextBlock rhVal = CreateInfoValue("52", airportDataGrid, 11, "RHValue");
            displayElements["rh"] = rhVal;
            
            TextBlock rhUnitLabel = new TextBlock
            {
                Text = "%",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(rhUnitLabel);
            Grid.SetRow(rhUnitLabel, 11);
            
            // Tmax
            TextBlock tmaxLabel = CreateInfoLabel("Tmax", airportDataGrid, 12);
            TextBlock tmaxVal = CreateInfoValue("30.9", airportDataGrid, 12, "TmaxValue");
            displayElements["tmax"] = tmaxVal;
            
            TextBlock tmaxUnitLabel = new TextBlock
            {
                Text = "°C",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(tmaxUnitLabel);
            Grid.SetRow(tmaxUnitLabel, 12);
            
            // Tmin
            TextBlock tminLabel = CreateInfoLabel("Tmin", airportDataGrid, 13);
            TextBlock tminVal = CreateInfoValue("20.4", airportDataGrid, 13, "TminValue");
            displayElements["tmin"] = tminVal;
            
            TextBlock tminUnitLabel = new TextBlock
            {
                Text = "°C",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(tminUnitLabel);
            Grid.SetRow(tminUnitLabel, 13);
            
            // Trwy
            TextBlock trwyLabel = CreateInfoLabel("Trwy", airportDataGrid, 14);
            TextBlock trwyVal = CreateInfoValue("49.9", airportDataGrid, 14, "TrwyValue");
            displayElements["trwy"] = trwyVal;
            
            TextBlock trwyUnitLabel = new TextBlock
            {
                Text = "°C",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            airportDataGrid.Children.Add(trwyUnitLabel);
            Grid.SetRow(trwyUnitLabel, 14);
            
            centerInfoGrid.Children.Add(airportDataGrid);
            Grid.SetRow(airportDataGrid, 1);
            
            contentGrid.Children.Add(centerInfoBorder);
            Grid.SetColumn(centerInfoBorder, 1);
            
            // Sağ Pist Paneli
            Border rightRunwayBorder = CreateRunwayPanel("RWY 17", contentGrid, 2);
            
            // METAR bilgisi ile alt bilgi çubuğu
            Border footerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Height = 30,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            
            TextBlock metarLabel = new TextBlock
            {
                Text = "METAR",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            
            TextBlock metarText = new TextBlock
            {
                Text = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(60, 0, 0, 0),
                Name = "MetarInfo"
            };
            this.RegisterName("MetarInfo", metarText);
            displayElements["metar"] = metarText;
            
            Grid footerGrid = new Grid();
            footerBorder.Child = footerGrid;
            footerGrid.Children.Add(metarLabel);
            footerGrid.Children.Add(metarText);
            
            mainGrid.Children.Add(footerBorder);

            // Bağlantı durumu göstergesi
            TextBlock connectionStatus = new TextBlock
            {
                Text = "Bağlantı Durumu: Veri bekleniyor...",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 35),
                Name = "ConnectionStatus"
            };
            this.RegisterName("ConnectionStatus", connectionStatus);
            mainGrid.Children.Add(connectionStatus);

            // Saat güncelleme zamanlayıcısını başlat
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (s, e) => UpdateClock();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private Border CreateRunwayPanel(string runwayName, Grid parentGrid, int column)
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
            Border runwayHeaderBorder = new Border
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
            runwayHeaderBorder.Child = headerText;
            
            runwayGrid.Children.Add(runwayHeaderBorder);
            Grid.SetRow(runwayHeaderBorder, 0);
            
            // Rüzgar yönü göstergesi (daire)
            Grid windGrid = new Grid();
            windGrid.Margin = new Thickness(10);
            
            // Dairesel ölçek oluştur
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
            
            // Yön işaretleri ekle
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
            
            // Rüzgar yönü oku (veriye göre döndürülecek)
            Rectangle windArrow = new Rectangle
            {
                Width = 20,
                Height = 100,
                Fill = Brushes.DarkGray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 1.0)
            };
            
            // Döndürme için transform grubu oluştur
            TransformGroup transformGroup = new TransformGroup();
            RotateTransform rotateTransform = new RotateTransform(0);
            transformGroup.Children.Add(rotateTransform);
            windArrow.RenderTransform = transformGroup;
            
            if (runwayName == "RWY 35")
            {
                // Sol pist için kaydet
                this.RegisterName("LeftWindArrow", windArrow);
            }
            else
            {
                // Sağ pist için kaydet
                this.RegisterName("RightWindArrow", windArrow);
            }
            
            // Rüzgar hızı göstergesi (merkez)
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
            this.RegisterName(windSpeedText.Name, windSpeedText);
            if (runwayName == "RWY 35")
            {
                displayElements["baseValue"] = (TextBlock)this.FindName("BaseValue");
            }
            
            // 10" Mnm verileri
            string minValue = runwayName == "RWY 35" ? "* CALM" : "220";
            string minSpeed = runwayName == "RWY 35" ? "" : "2";
            CreateRunwayDataRow(dataGrid, 5, "10\" Mnm", minValue, minSpeed, runwayName == "RWY 35" ? "Left10MinDir" : "Right10MinDir", runwayName == "RWY 35" ? "Left10MinSpeed" : "Right10MinSpeed");
            if (runwayName == "RWY 35")
            {
                displayElements["left10MinDir"] = (TextBlock)this.FindName("Left10MinDir");
                displayElements["left10MinSpeed"] = (TextBlock)this.FindName("Left10MinSpeed");
            }
            else
            {
                displayElements["right10MinDir"] = (TextBlock)this.FindName("Right10MinDir");
                displayElements["right10MinSpeed"] = (TextBlock)this.FindName("Right10MinSpeed");
            }
            
            // 10" Avg verileri
            string avg10Dir = runwayName == "RWY 35" ? "280" : "290";
            CreateRunwayDataRow(dataGrid, 6, "10\" Avg", avg10Dir, "5", runwayName == "RWY 35" ? "Left10AvgDir" : "Right10AvgDir", runwayName == "RWY 35" ? "Left10AvgSpeed" : "Right10AvgSpeed");
            if (runwayName == "RWY 35")
            {
                displayElements["left10AvgDir"] = (TextBlock)this.FindName("Left10AvgDir");
                displayElements["left10AvgSpeed"] = (TextBlock)this.FindName("Left10AvgSpeed");
            }
            else
            {
                displayElements["right10AvgDir"] = (TextBlock)this.FindName("Right10AvgDir");
                displayElements["right10AvgSpeed"] = (TextBlock)this.FindName("Right10AvgSpeed");
            }
            
            // 10" Max verileri
            string max10Speed = runwayName == "RWY 35" ? "9" : "8";
            CreateRunwayDataRow(dataGrid, 7, "10\" Max", "320", max10Speed, runwayName == "RWY 35" ? "Left10MaxDir" : "Right10MaxDir", runwayName == "RWY 35" ? "Left10MaxSpeed" : "Right10MaxSpeed");
            if (runwayName == "RWY 35")
            {
                displayElements["left10MaxDir"] = (TextBlock)this.FindName("Left10MaxDir");
                displayElements["left10MaxSpeed"] = (TextBlock)this.FindName("Left10MaxSpeed");
            }
            else
            {
                displayElements["right10MaxDir"] = (TextBlock)this.FindName("Right10MaxDir");
                displayElements["right10MaxSpeed"] = (TextBlock)this.FindName("Right10MaxSpeed");
            }
            
            // Her iki pist için QFE verileri
            if (runwayName == "RWY 35")
            {
                CreateQfeDataRow(dataGrid, 8, "QFE", "1012.8", "29.91", "Left");
                displayElements["leftQfeValue"] = (TextBlock)this.FindName("LeftQfeValue");
                displayElements["leftQfeMm"] = (TextBlock)this.FindName("LeftQfeMmValue");
            }
            else
            {
                CreateQfeDataRow(dataGrid, 8, "QFE", "1012.7", "29.90", "Right");
                displayElements["rightQfeValue"] = (TextBlock)this.FindName("RightQfeValue");
                displayElements["rightQfeMm"] = (TextBlock)this.FindName("RightQfeMmValue");
            }
            
            runwayGrid.Children.Add(dataGrid);
            Grid.SetRow(dataGrid, 2);
            
            parentGrid.Children.Add(runwayBorder);
            Grid.SetColumn(runwayBorder, column);
            
            return runwayBorder;
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
                this.RegisterName(directionName, directionText);
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
                this.RegisterName(speedName, speedText);
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
            this.RegisterName("BaseValue", valueText);
            
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
            this.RegisterName(valueName, valueText);
            
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
            this.RegisterName(mmValueName, mmValueText);
            
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
            
            this.RegisterName(name, value);
            
            grid.Children.Add(value);
            Grid.SetRow(value, row);
            
            return value;
        }
        
        private void UpdateClock()
        {
            TextBlock timeDisplay = (TextBlock)FindName("TimeDisplay");
            timeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}["leftWindSpeed"] = windSpeedText;
            }
            else
            {
                displayElements["rightWindSpeed"] = windSpeedText;
            }
            
            windSpeedBorder.Child = windSpeedText;
            
            // Rüzgar yönü göstergesi (pist görselleştirme)
            Rectangle runwayIndicator = new Rectangle
            {
                Width = 15,
                Height = 40,
                Fill = Brushes.DarkGray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            
            // Rüzgar grid'ine ekle
            windGrid.Children.Add(windArrow);
            windGrid.Children.Add(windSpeedBorder);
            windGrid.Children.Add(runwayIndicator);
            
            runwayGrid.Children.Add(windGrid);
            Grid.SetRow(windGrid, 1);
            
            // Veri Grid'i
            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(10);
            
            // Ölçüm verileri için satırlar ekle
            for (int i = 0; i < 9; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            }
            
            // 2" Mnm verileri
            CreateRunwayDataRow(dataGrid, 0, "2\" Mnm", "250", "3", runwayName == "RWY 35" ? "Left2MinDir" : "Right2MinDir", runwayName == "RWY 35" ? "Left2MinSpeed" : "Right2MinSpeed");
            if (runwayName == "RWY 35")
            {
                displayElements["left2MinDir"] = (TextBlock)this.FindName("Left2MinDir");
                displayElements["left2MinSpeed"] = (TextBlock)this.FindName("Left2MinSpeed");
            }
            else
            {
                displayElements["right2MinDir"] = (TextBlock)this.FindName("Right2MinDir");
                displayElements["right2MinSpeed"] = (TextBlock)this.FindName("Right2MinSpeed");
            }
            
            // 2" Avg verileri
            string avgDir = runwayName == "RWY 35" ? "290" : "270";
            CreateRunwayDataRow(dataGrid, 1, "2\" Avg", avgDir, "5", runwayName == "RWY 35" ? "Left2AvgDir" : "Right2AvgDir", runwayName == "RWY 35" ? "Left2AvgSpeed" : "Right2AvgSpeed");
            if (runwayName == "RWY 35")
            {
                displayElements["left2AvgDir"] = (TextBlock)this.FindName("Left2AvgDir");
                displayElements["left2AvgSpeed"] = (TextBlock)this.FindName("Left2AvgSpeed");
            }
            else
            {
                displayElements["right2AvgDir"] = (TextBlock)this.FindName("Right2AvgDir");
                displayElements["right2AvgSpeed"] = (TextBlock)this.FindName("Right2AvgSpeed");
            }
            
            // 2" Max verileri
            string maxSpeed = runwayName == "RWY 35" ? "8" : "6";
            CreateRunwayDataRow(dataGrid, 2, "2\" Max", "320", maxSpeed, runwayName == "RWY 35" ? "Left2MaxDir" : "Right2MaxDir", runwayName == "RWY 35" ? "Left2MaxSpeed" : "Right2MaxSpeed");
            if (runwayName == "RWY 35")
            {
                displayElements["left2MaxDir"] = (TextBlock)this.FindName("Left2MaxDir");
                displayElements["left2MaxSpeed"] = (TextBlock)this.FindName("Left2MaxSpeed");
            }
            else
            {
                displayElements["right2MaxDir"] = (TextBlock)this.FindName("Right2MaxDir");
                displayElements["right2MaxSpeed"] = (TextBlock)this.FindName("Right2MaxSpeed");
            }
            
            // 2" Hw/Cw verileri
            string hwCwValue = runwayName == "RWY 35" ? "H02   L05" : "T01   R05";
            CreateRunwayDataRow(dataGrid, 3, "2\" Hw/Cw", hwCwValue, "", runwayName == "RWY 35" ? "LeftHwCw" : "RightHwCw", "");
            if (runwayName == "RWY 35")
            {
                displayElements["leftHwCw"] = (TextBlock)this.FindName("LeftHwCw");
            }
            else
            {
                displayElements["rightHwCw"] = (TextBlock)this.FindName("RightHwCw");
            }
            
            // Sol pist için Base verileri
            if (runwayName == "RWY 35")
            {
                CreateBaseDataRow(dataGrid, 4, "Base", "NCD");
                displayElementsMinDir", data.Runway17.Min2Direction);
                UpdateTextBlock("right2MinSpeed", data.Runway17.Min2Speed);
                UpdateTextBlock("right2AvgDir", data.Runway17.Avg2Direction);
                UpdateTextBlock("right2