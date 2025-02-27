using AirportDisplayApp.Models;
using AirportDisplayApp.Services;
using AirportDisplayApp.UI;
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
        
        // Panel oluşturucular
        private RunwayPanelBuilder runwayBuilder;
        private CenterPanelBuilder centerBuilder;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Yardımcı sınıfları oluştur
            runwayBuilder = new RunwayPanelBuilder(this, displayElements);
            centerBuilder = new CenterPanelBuilder(this, displayElements);
            
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
                UpdateRunwayData(data.Runway35, "left");
                
                // RWY 17 verilerini güncelle
                UpdateRunwayData(data.Runway17, "right");
                
                // Rüzgar oklarını güncelle
                runwayBuilder.UpdateWindArrow("LeftWindArrow", data.Runway35.AvgWindDirection);
                runwayBuilder.UpdateWindArrow("RightWindArrow", data.Runway17.AvgWindDirection);
            });
        }
        
        private void UpdateRunwayData(RunwayDataModel runway, string prefix)
        {
            string windSpeedKey = prefix + "WindSpeed";
            UpdateTextBlock(windSpeedKey, runway.WindSpeed);
            
            // 2 dakikalık veriler
            UpdateTextBlock(prefix + "2MinDir", runway.Min2Direction);
            UpdateTextBlock(prefix + "2MinSpeed", runway.Min2Speed);
            UpdateTextBlock(prefix + "2AvgDir", runway.Avg2Direction);
            UpdateTextBlock(prefix + "2AvgSpeed", runway.Avg2Speed);
            UpdateTextBlock(prefix + "2MaxDir", runway.Max2Direction);
            UpdateTextBlock(prefix + "2MaxSpeed", runway.Max2Speed);
            UpdateTextBlock(prefix + "HwCw", runway.HwCw);
            
            // Base değeri (sadece sol pist için)
            if (prefix == "left")
            {
                UpdateTextBlock("baseValue", runway.Base);
            }
            
            // 10 dakikalık veriler
            UpdateTextBlock(prefix + "10MinDir", runway.Min10Direction);
            UpdateTextBlock(prefix + "10MinSpeed", runway.Min10Speed);
            UpdateTextBlock(prefix + "10AvgDir", runway.Avg10Direction);
            UpdateTextBlock(prefix + "10AvgSpeed", runway.Avg10Speed);
            UpdateTextBlock(prefix + "10MaxDir", runway.Max10Direction);
            UpdateTextBlock(prefix + "10MaxSpeed", runway.Max10Speed);
            
            // QFE değerleri
            UpdateTextBlock(prefix + "QfeValue", runway.QFE);
            UpdateTextBlock(prefix + "QfeMm", runway.QFEInHg);
        }
        
        private void UpdateTextBlock(string key, string value)
        {
            if (displayElements.TryGetValue(key, out TextBlock textBlock))
            {
                textBlock.Text = value;
            }
        }
        
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
            Border headerBorder = CreateHeaderPanel();
            mainGrid.Children.Add(headerBorder);
            Grid.SetRow(headerBorder, 0);

            // Durum Çubuğu
            Border statusBorder = CreateStatusBar();
            mainGrid.Children.Add(statusBorder);
            
            // Ana İçerik Grid'i (3 sütunlu)
            Grid contentGrid = new Grid();
            contentGrid.Margin = new Thickness(0, 90, 0, 30);
            
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300) });
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            
            mainGrid.Children.Add(contentGrid);

            // Sol Pist Paneli (RWY 35)
            Border leftRunwayBorder = runwayBuilder.CreateRunwayPanel("RWY 35", contentGrid, 0);
            
            // Merkez Havaalanı Bilgi Paneli
            Border centerInfoBorder = centerBuilder.CreateCenterPanel(contentGrid, 1);
            
            // Sağ Pist Paneli (RWY 17)
            Border rightRunwayBorder = runwayBuilder.CreateRunwayPanel("RWY 17", contentGrid, 2);
            
            // METAR bilgisi ile alt bilgi çubuğu
            Border footerBorder = CreateFooter();
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

            // Kontrol paneli
            StackPanel controlPanel = CreateControlPanel();
            mainGrid.Children.Add(controlPanel);

            // Saat güncelleme zamanlayıcısını başlat
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (s, e) => UpdateClock();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }
        
        private Border CreateHeaderPanel()
        {
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
            
            return headerBorder;
        }
        
        private Border CreateStatusBar()
        {
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
            
            return statusBorder;
        }
        
        private Border CreateFooter()
        {
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
            
            return footerBorder;
        }
        
        private StackPanel CreateControlPanel()
        {
            StackPanel controlPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 10, 35)
            };
            
            Button refreshButton = new Button
            {
                Content = "Verileri Yenile",
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5),
                Name = "RefreshButton"
            };
            refreshButton.Click += (s, e) => RefreshData();
            this.RegisterName("RefreshButton", refreshButton);
            
            Button settingsButton = new Button
            {
                Content = "Ayarlar",
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5),
                Name = "SettingsButton"
            };
            settingsButton.Click += (s, e) => ShowSettings();
            this.RegisterName("SettingsButton", settingsButton);
            
            controlPanel.Children.Add(refreshButton);
            controlPanel.Children.Add(settingsButton);
            
            return controlPanel;
        }
        
        private void UpdateClock()
        {
            TextBlock timeDisplay = (TextBlock)FindName("TimeDisplay");
            timeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        
        private void RefreshData()
        {
            // Manuel veri yenileme
            dataService.RefreshData();
            
            // Kullanıcıya bilgi ver
            TextBlock connectionStatus = (TextBlock)FindName("ConnectionStatus");
            connectionStatus.Text = "Bağlantı Durumu: Manuel veri yenileme talep edildi...";
        }
        
        private void ShowSettings()
        {
            // Ayarlar penceresi gösterimi
            MessageBox.Show(
                "Bu bölümde bağlantı ayarları, görüntüleme seçenekleri ve diğer tercihler yapılandırılabilir.",
                "Ayarlar",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}