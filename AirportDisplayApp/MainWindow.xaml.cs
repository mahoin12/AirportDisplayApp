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

                // Eski stil güncelleme:
                // _uiManager.UpdateWindDirection("RWY 35", data.Runway35.AvgWindDirection);
                // _uiManager.UpdateWindDirection("RWY 17", data.Runway17.AvgWindDirection);

                // YENİ: Rüzgar oklarını ve yön göstergelerini güncelle - tam RunwayDataModel ile
                _uiManager.UpdateWindDirection("RWY 35", data.Runway35);
                _uiManager.UpdateWindDirection("RWY 17", data.Runway17);

                // Diğer metin güncellemelerini korumalıyız
                _uiManager.UpdateDisplayElement("leftWindSpeed", data.Runway35.WindSpeed);
                _uiManager.UpdateDisplayElement("left2MinDir", data.Runway35.Min2Direction);
                _uiManager.UpdateDisplayElement("left2MinSpeed", data.Runway35.Min2Speed);
                _uiManager.UpdateDisplayElement("left2AvgDir", data.Runway35.Avg2Direction);
                _uiManager.UpdateDisplayElement("left2AvgSpeed", data.Runway35.Avg2Speed);
                _uiManager.UpdateDisplayElement("left2MaxDir", data.Runway35.Max2Direction);
                _uiManager.UpdateDisplayElement("left2MaxSpeed", data.Runway35.Max2Speed);
                _uiManager.UpdateDisplayElement("leftHwCw", data.Runway35.HwCw);
                _uiManager.UpdateDisplayElement("baseValue", data.Runway35.Base);
                _uiManager.UpdateDisplayElement("left10MinDir", data.Runway35.Min10Direction);
                _uiManager.UpdateDisplayElement("left10MinSpeed", data.Runway35.Min10Speed);
                _uiManager.UpdateDisplayElement("left10AvgDir", data.Runway35.Avg10Direction);
                _uiManager.UpdateDisplayElement("left10AvgSpeed", data.Runway35.Avg10Speed);
                _uiManager.UpdateDisplayElement("left10MaxDir", data.Runway35.Max10Direction);
                _uiManager.UpdateDisplayElement("left10MaxSpeed", data.Runway35.Max10Speed);
                _uiManager.UpdateDisplayElement("leftQfeValue", data.Runway35.QFE);
                _uiManager.UpdateDisplayElement("leftQfeMm", data.Runway35.QFEInHg);

                _uiManager.UpdateDisplayElement("rightWindSpeed", data.Runway17.WindSpeed);
                _uiManager.UpdateDisplayElement("right2MinDir", data.Runway17.Min2Direction);
                _uiManager.UpdateDisplayElement("right2MinSpeed", data.Runway17.Min2Speed);
                _uiManager.UpdateDisplayElement("right2AvgDir", data.Runway17.Avg2Direction);
                _uiManager.UpdateDisplayElement("right2AvgSpeed", data.Runway17.Avg2Speed);
                _uiManager.UpdateDisplayElement("right2MaxDir", data.Runway17.Max2Direction);
                _uiManager.UpdateDisplayElement("right2MaxSpeed", data.Runway17.Max2Speed);
                _uiManager.UpdateDisplayElement("rightHwCw", data.Runway17.HwCw);
                _uiManager.UpdateDisplayElement("right10MinDir", data.Runway17.Min10Direction);
                _uiManager.UpdateDisplayElement("right10MinSpeed", data.Runway17.Min10Speed);
                _uiManager.UpdateDisplayElement("right10AvgDir", data.Runway17.Avg10Direction);
                _uiManager.UpdateDisplayElement("right10AvgSpeed", data.Runway17.Avg10Speed);
                _uiManager.UpdateDisplayElement("right10MaxDir", data.Runway17.Max10Direction);
                _uiManager.UpdateDisplayElement("right10MaxSpeed", data.Runway17.Max10Speed);
                _uiManager.UpdateDisplayElement("rightQfeValue", data.Runway17.QFE);
                _uiManager.UpdateDisplayElement("rightQfeMm", data.Runway17.QFEInHg);
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