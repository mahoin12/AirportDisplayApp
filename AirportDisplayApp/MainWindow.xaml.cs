using AirportDisplayApp.Models;
using AirportDisplayApp.Services;
using AirportDisplayApp.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace AirportDisplayApp
{
    public partial class MainWindow : Window
    {
        // Veri servisi
        private readonly DataService _dataService;

        // UI bileşenleri yöneticisi
        private UIComponents _uiComponents;

        // UI elementleri sözlüğü
        private Dictionary<string, TextBlock> _displayElements = new Dictionary<string, TextBlock>();

        public MainWindow()
        {
            InitializeComponent();

            // Fullscreen borderless modu ayarla
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;
            Background = new SolidColorBrush(Colors.WhiteSmoke);

            // Veri servisini oluştur
            _dataService = new DataService();
            _dataService.ConnectionStatusChanged += DataService_ConnectionStatusChanged;
            _dataService.DataUpdated += DataService_DataUpdated;

            // UI bileşenlerini hazırla
            _uiComponents = new UIComponents(this, _displayElements);
            
            // Ana UI'ı oluştur
            CreateUI();

            // Saat güncelleyicisini başlat
            StartClockTimer();

            // Veri almayı başlat
            _dataService.StartAsync();

            // Pencere kapandığında kaynakları temizle
            Closed += MainWindow_Closed;

            // ESC tuşu ile çıkış için event handler ekle
            KeyDown += MainWindow_KeyDown;
            
            // İlk açılışta simüle edilmiş veri göster
            _dataService.GenerateSimulatedData();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // ESC tuşu ile uygulamadan çıkış
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void StartClockTimer()
        {
            // Saat güncelleme zamanlayıcısı
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateClock();
            timer.Start();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Veri servisini durdur
            _dataService.Stop();
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
                _uiComponents.UpdateTextElement("time", data.Time);
                _uiComponents.UpdateTextElement("runwayInUse", data.RunwayInUse);
                _uiComponents.UpdateTextElement("rwyInUseInfo", data.RwyInUseInfo);

                // Merkez panel verilerini güncelle
                _uiComponents.UpdateTextElement("qnh", data.QNH);
                _uiComponents.UpdateTextElement("qnhInHg", data.QNHInHg);
                _uiComponents.UpdateTextElement("qfe", data.QFE);
                _uiComponents.UpdateTextElement("qfeSynop", data.QFESynop);
                _uiComponents.UpdateTextElement("low", data.Low);
                _uiComponents.UpdateTextElement("temp", data.Temperature);
                _uiComponents.UpdateTextElement("td", data.DewPoint);
                _uiComponents.UpdateTextElement("rh", data.RelativeHumidity);
                _uiComponents.UpdateTextElement("tmax", data.TempMax);
                _uiComponents.UpdateTextElement("tmin", data.TempMin);
                _uiComponents.UpdateTextElement("trwy", data.RunwayTemp);

                // METAR bilgisini güncelle
                _uiComponents.UpdateTextElement("metar", data.Metar);

                // RWY 35 verilerini güncelle
                _uiComponents.UpdateTextElement("leftWindSpeed", data.Runway35.WindSpeed);
                _uiComponents.UpdateTextElement("left2MinDir", data.Runway35.Min2Direction);
                _uiComponents.UpdateTextElement("left2MinSpeed", data.Runway35.Min2Speed);
                _uiComponents.UpdateTextElement("left2AvgDir", data.Runway35.Avg2Direction);
                _uiComponents.UpdateTextElement("left2AvgSpeed", data.Runway35.Avg2Speed);
                _uiComponents.UpdateTextElement("left2MaxDir", data.Runway35.Max2Direction);
                _uiComponents.UpdateTextElement("left2MaxSpeed", data.Runway35.Max2Speed);
                _uiComponents.UpdateTextElement("leftHwCw", data.Runway35.HwCw);
                _uiComponents.UpdateTextElement("baseValue", data.Runway35.Base);
                _uiComponents.UpdateTextElement("left10MinDir", data.Runway35.Min10Direction);
                _uiComponents.UpdateTextElement("left10MinSpeed", data.Runway35.Min10Speed);
                _uiComponents.UpdateTextElement("left10AvgDir", data.Runway35.Avg10Direction);
                _uiComponents.UpdateTextElement("left10AvgSpeed", data.Runway35.Avg10Speed);
                _uiComponents.UpdateTextElement("left10MaxDir", data.Runway35.Max10Direction);
                _uiComponents.UpdateTextElement("left10MaxSpeed", data.Runway35.Max10Speed);
                _uiComponents.UpdateTextElement("leftQfeValue", data.Runway35.QFE);
                _uiComponents.UpdateTextElement("leftQfeInHg", data.Runway35.QFEInHg);

                // RWY 17 verilerini güncelle
                _uiComponents.UpdateTextElement("rightWindSpeed", data.Runway17.WindSpeed);
                _uiComponents.UpdateTextElement("right2MinDir", data.Runway17.Min2Direction);
                _uiComponents.UpdateTextElement("right2MinSpeed", data.Runway17.Min2Speed);
                _uiComponents.UpdateTextElement("right2AvgDir", data.Runway17.Avg2Direction);
                _uiComponents.UpdateTextElement("right2AvgSpeed", data.Runway17.Avg2Speed);
                _uiComponents.UpdateTextElement("right2MaxDir", data.Runway17.Max2Direction);
                _uiComponents.UpdateTextElement("right2MaxSpeed", data.Runway17.Max2Speed);
                _uiComponents.UpdateTextElement("rightHwCw", data.Runway17.HwCw);
                _uiComponents.UpdateTextElement("right10MinDir", data.Runway17.Min10Direction);
                _uiComponents.UpdateTextElement("right10MinSpeed", data.Runway17.Min10Speed);
                _uiComponents.UpdateTextElement("right10AvgDir", data.Runway17.Avg10Direction);
                _uiComponents.UpdateTextElement("right10AvgSpeed", data.Runway17.Avg10Speed);
                _uiComponents.UpdateTextElement("right10MaxDir", data.Runway17.Max10Direction);
                _uiComponents.UpdateTextElement("right10MaxSpeed", data.Runway17.Max10Speed);
                _uiComponents.UpdateTextElement("rightQfeValue", data.Runway17.QFE);
                _uiComponents.UpdateTextElement("rightQfeInHg", data.Runway17.QFEInHg);

                // Rüzgar oklarını güncelle
                _uiComponents.UpdateWindDirection("LeftWindArrow", data.Runway35.AvgWindDirection);
                _uiComponents.UpdateWindDirection("RightWindArrow", data.Runway17.AvgWindDirection);
            });
        }

        private void CreateUI()
        {
            // Ana Layout'u oluştur
            Grid mainGrid = _uiComponents.CreateMainLayout();
            Content = mainGrid;

            // Başlık paneli
            _uiComponents.CreateHeader(mainGrid);

            // Tab Bar
            _uiComponents.CreateTabBar(mainGrid);

            // Durum çubuğu
            _uiComponents.CreateStatusBar(mainGrid);

            // Ana içerik paneli
            Grid contentGrid = _uiComponents.CreateContentPanel(mainGrid);

            // Sol pist paneli (RWY 35)
            _uiComponents.CreateRunwayPanel(contentGrid, 0, "RWY 35");

            // Orta bilgi paneli
            _uiComponents.CreateCenterInfoPanel(contentGrid, 1);

            // Sağ pist paneli (RWY 17)
            _uiComponents.CreateRunwayPanel(contentGrid, 2, "RWY 17");

            // METAR paneli
            _uiComponents.CreateMetarPanel(mainGrid);

            // Alt bilgi çubuğu
            _uiComponents.CreateFooter(mainGrid);
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

            _dataService.RefreshAsync();
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