using AirportDisplayApp.Models;
using AirportDisplayApp.Services;
using AirportDisplayApp.UI;
using System;
using System.Windows;
using System.Windows.Input;

namespace AirportDisplayApp
{
    public partial class MainWindow : Window
    {
        // Veri servisi
        private readonly DataService _dataService;
        
        // UI Yöneticisi
        private readonly UIManager _uiManager;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Fullscreen borderless modu ayarla
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;
            
            // UI Manager oluştur
            _uiManager = new UIManager(this);
            
            // Veri servisini oluştur
            _dataService = new DataService();
            _dataService.ConnectionStatusChanged += DataService_ConnectionStatusChanged;
            _dataService.DataUpdated += DataService_DataUpdated;
            
            // Ana UI oluştur
            _uiManager.BuildUI();
            
            // UI Manager event'lerini bağla
            _uiManager.OnRefreshRequested += (s, e) => RefreshData();
            _uiManager.OnSettingsRequested += (s, e) => ShowSettings();
            
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
        
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Veri servisini durdur
            _dataService.Stop();
        }
        
        private void DataService_ConnectionStatusChanged(object sender, string status)
        {
            // UI thread'inde bağlantı durumunu güncelle
            Dispatcher.Invoke(() => {
                _uiManager.UpdateConnectionStatus(status);
            });
        }

        private void DataService_DataUpdated(object sender, AirportDataModel data)
        {
            // UI thread'inde verileri güncelle
            Dispatcher.Invoke(() =>
            {
                // Genel bilgileri güncelle
                _uiManager.UpdateDisplayElement("time", data.Time);
                _uiManager.UpdateDisplayElement("runwayInUse", data.RunwayInUse);
                _uiManager.UpdateDisplayElement("rwyInUseInfo", data.RwyInUseInfo);

                // Merkez panel verilerini güncelle
                _uiManager.UpdateDisplayElement("qnh", data.QNH);
                _uiManager.UpdateDisplayElement("qnhInHg", data.QNHInHg);
                _uiManager.UpdateDisplayElement("qfe", data.QFE);
                _uiManager.UpdateDisplayElement("qfeSynop", data.QFESynop);
                _uiManager.UpdateDisplayElement("low", data.Low);
                _uiManager.UpdateDisplayElement("temp", data.Temperature);
                _uiManager.UpdateDisplayElement("dewPoint", data.DewPoint);
                _uiManager.UpdateDisplayElement("relativeHumidity", data.RelativeHumidity);
                _uiManager.UpdateDisplayElement("tempMax", data.TempMax);
                _uiManager.UpdateDisplayElement("tempMin", data.TempMin);
                _uiManager.UpdateDisplayElement("runwayTemp", data.RunwayTemp);

                // METAR bilgisini güncelle
                _uiManager.UpdateDisplayElement("metar", data.Metar);

                // Rüzgar oklarını ve yön göstergelerini güncelle - tam RunwayDataModel ile
                // Bu metot içinde zaten tüm runway verileri güncelleniyor
                _uiManager.UpdateWindDirection("RWY 35", data.Runway35);
                _uiManager.UpdateWindDirection("RWY 17", data.Runway17);
                
                // NOT: Aşağıdaki tekrarlı güncellemeler kaldırıldı, çünkü UpdateWindDirection metodu 
                // içinde zaten tüm bu değerler güncelleniyor.
                // 
                // _uiManager.UpdateDisplayElement("leftWindSpeed", data.Runway35.WindSpeed);
                // _uiManager.UpdateDisplayElement("left2MinDir", data.Runway35.Min2Direction);
                // ...ve diğer alanlar
            });
        }

        private void RefreshData()
        {
            // Verileri yenile
            _uiManager.UpdateConnectionStatus("Veriler yenileniyor...");
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