using AirportDisplayApp.Models;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirportDisplayApp.Services
{
    /// <summary>
    /// AVOS verileri ile iletişim kuran servis sınıfı
    /// </summary>
    public class DataService
    {
        // Veri değişim eventi
        public event EventHandler<string> ConnectionStatusChanged;
        public event EventHandler<AirportDataModel> DataUpdated;

        // Named pipe ismi
        private const string PipeNameFromServer = "UnrealToWpfPipe";
        
        // Named pipe bağlantı değişkenleri
        private NamedPipeClientStream pipeClient;
        private StreamReader pipeReader;
        private bool isPipeConnected = false;
        
        // Veri modeli
        private AirportDataModel airportData;
        
        public DataService()
        {
            airportData = new AirportDataModel();
        }
        
        /// <summary>
        /// Veri alışını başlat
        /// </summary>
        public async Task StartAsync()
        {
            UpdateConnectionStatus("Veri kaynağına bağlanıyor...");
            await InitializePipeAsync();
        }
        
        /// <summary>
        /// Veri alışını durdur
        /// </summary>
        public void Stop()
        {
            isPipeConnected = false;
            CleanupResources();
            UpdateConnectionStatus("Veri alışı durduruldu");
        }
        
        /// <summary>
        /// Named pipe bağlantısını başlat
        /// </summary>
        private async Task InitializePipeAsync()
        {
            try
            {
                UpdateConnectionStatus("Veri kaynağına bağlanıyor...");
                
                // Named pipe istemcisi oluştur
                pipeClient = new NamedPipeClientStream(".", PipeNameFromServer, PipeDirection.In);
                
                // Bağlantı için timeout süresi (10 saniye)
                const int connectionTimeout = 10000;
                
                try
                {
                    // Pipe'a asenkron bağlan
                    await Task.Run(() => {
                        try {
                            pipeClient.Connect(connectionTimeout);
                            return true;
                        }
                        catch {
                            return false;
                        }
                    });
                    
                    if (pipeClient.IsConnected)
                    {
                        UpdateConnectionStatus("Veri kaynağına bağlandı");
                        pipeReader = new StreamReader(pipeClient);
                        isPipeConnected = true;
                        
                        // Veri okumayı başlat
                        _ = ReadPipeDataAsync();
                    }
                    else
                    {
                        UpdateConnectionStatus("Bağlantı zaman aşımına uğradı");
                        // Yeniden bağlanma zamanlayıcısını başlat
                        StartRetryTimer();
                    }
                }
                catch (Exception ex)
                {
                    UpdateConnectionStatus($"Bağlantı hatası: {ex.Message}");
                    // Yeniden bağlanma zamanlayıcısını başlat
                    StartRetryTimer();
                }
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus($"Kurulum hatası: {ex.Message}");
                // Yeniden bağlanma zamanlayıcısını başlat
                StartRetryTimer();
            }
        }
        
        /// <summary>
        /// Yeniden bağlanma zamanlayıcısını başlat
        /// </summary>
        private void StartRetryTimer()
        {
            Task.Delay(5000).ContinueWith(_ => InitializePipeAsync());
        }
        
        /// <summary>
        /// Named pipe'tan veri oku
        /// </summary>
        private async Task ReadPipeDataAsync()
        {
            try
            {
                while (isPipeConnected && pipeClient != null && pipeClient.IsConnected)
                {
                    string data = await pipeReader.ReadLineAsync();
                    
                    if (string.IsNullOrEmpty(data))
                    {
                        // Boş satır veya pipe kapandı
                        UpdateConnectionStatus("Bağlantı kapandı veya boş veri alındı");
                        isPipeConnected = false;
                        break;
                    }
                    
                    try
                    {
                        // JSON verisini işle
                        ProcessJsonData(data);
                        UpdateConnectionStatus($"Veri alındı: {DateTime.Now:HH:mm:ss}");
                    }
                    catch (Exception ex)
                    {
                        UpdateConnectionStatus($"Veri işleme hatası: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus($"Okuma hatası: {ex.Message}");
                isPipeConnected = false;
            }
            
            // Temizlik
            CleanupResources();
            
            // Yeniden bağlanma zamanlayıcısını başlat
            StartRetryTimer();
        }
        
        /// <summary>
        /// Kaynakları temizle
        /// </summary>
        private void CleanupResources()
        {
            if (pipeReader != null)
            {
                pipeReader.Dispose();
                pipeReader = null;
            }
            
            if (pipeClient != null)
            {
                pipeClient.Dispose();
                pipeClient = null;
            }
        }
        
        /// <summary>
        /// JSON verisini işle
        /// </summary>
        private void ProcessJsonData(string jsonData)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;
                    
                    // AVOS formatını kontrol et
                    if (root.TryGetProperty("AVOS", out JsonElement avosElement) && 
                        avosElement.TryGetProperty("Komutlar", out JsonElement komutlarElement) &&
                        komutlarElement.ValueKind == JsonValueKind.Array)
                    {
                        // AVOS komutlarını işle
                        foreach (JsonElement komut in komutlarElement.EnumerateArray())
                        {
                            if (komut.TryGetProperty("Komut", out JsonElement komutNoElement) &&
                                komut.TryGetProperty("Deger", out JsonElement degerElement))
                            {
                                int komutNo = komutNoElement.GetInt32();
                                string deger = degerElement.GetString();
                                
                                ProcessAvosCommand(komutNo, deger);
                            }
                        }
                        
                        // Veri değişim eventini tetikle
                        DataUpdated?.Invoke(this, airportData);
                    }
                    else
                    {
                        UpdateConnectionStatus("Tanınmayan veri formatı");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"JSON ayrıştırma hatası: {ex.Message}");
            }
        }
        
        /// <summary>
        /// AVOS komutunu işle
        /// </summary>
        private void ProcessAvosCommand(int commandNum, string commandValue)
        {
            switch (commandNum)
            {
                case 1: // Saat
                    airportData.Time = commandValue;
                    break;
                    
                case 2: // Kullanılan pist
                    airportData.RunwayInUse = commandValue;
                    airportData.RwyInUseInfo = commandValue;
                    break;
                    
                case 3: // QNH değeri
                    airportData.QNH = commandValue;
                    break;
                    
                case 4: // QNH inHg değeri
                    airportData.QNHInHg = commandValue;
                    break;
                    
                case 5: // QFE değeri
                    airportData.QFE = commandValue;
                    break;
                    
                case 6: // QFE SYNOP değeri
                    airportData.QFESynop = commandValue;
                    break;
                    
                case 7: // Bulut Tabanı (LOW)
                    airportData.Low = commandValue;
                    break;
                    
                case 8: // Sıcaklık
                    airportData.Temperature = commandValue;
                    break;
                    
                case 9: // Çiy noktası sıcaklığı
                    airportData.DewPoint = commandValue;
                    break;
                    
                case 10: // Bağıl nem
                    airportData.RelativeHumidity = commandValue;
                    break;
                    
                case 11: // Maksimum sıcaklık
                    airportData.TempMax = commandValue;
                    break;
                    
                case 12: // Minimum sıcaklık
                    airportData.TempMin = commandValue;
                    break;
                    
                case 13: // Pist sıcaklığı
                    airportData.RunwayTemp = commandValue;
                    break;
                    
                case 14: // METAR bilgisi
                    airportData.Metar = commandValue;
                    break;
                    
                // RWY 35 Verileri
                case 21: // Rüzgar hızı
                    airportData.Runway35.WindSpeed = commandValue;
                    break;
                    
                case 22: // 2 dk min rüzgar yönü
                    airportData.Runway35.Min2Direction = commandValue;
                    break;
                    
                case 23: // 2 dk min rüzgar hızı
                    airportData.Runway35.Min2Speed = commandValue;
                    break;
                    
                case 24: // 2 dk avg rüzgar yönü
                    airportData.Runway35.Avg2Direction = commandValue;
                    break;
                    
                case 25: // 2 dk avg rüzgar hızı
                    airportData.Runway35.Avg2Speed = commandValue;
                    break;
                    
                case 26: // 2 dk max rüzgar yönü
                    airportData.Runway35.Max2Direction = commandValue;
                    break;
                    
                case 27: // 2 dk max rüzgar hızı
                    airportData.Runway35.Max2Speed = commandValue;
                    break;
                    
                case 28: // Hw/Cw değeri
                    airportData.Runway35.HwCw = commandValue;
                    break;
                    
                case 29: // Base değeri
                    airportData.Runway35.Base = commandValue;
                    break;
                    
                case 30: // 10 dk min rüzgar yönü
                    airportData.Runway35.Min10Direction = commandValue;
                    break;
                    
                case 31: // 10 dk min rüzgar hızı
                    airportData.Runway35.Min10Speed = commandValue;
                    break;
                    
                case 32: // 10 dk avg rüzgar yönü
                    airportData.Runway35.Avg10Direction = commandValue;
                    break;
                    
                case 33: // 10 dk avg rüzgar hızı
                    airportData.Runway35.Avg10Speed = commandValue;
                    break;
                    
                case 34: // 10 dk max rüzgar yönü
                    airportData.Runway35.Max10Direction = commandValue;
                    break;
                    
                case 35: // 10 dk max rüzgar hızı
                    airportData.Runway35.Max10Speed = commandValue;
                    break;
                    
                case 36: // QFE değeri
                    airportData.Runway35.QFE = commandValue;
                    break;
                    
                case 37: // QFE inHg değeri
                    airportData.Runway35.QFEInHg = commandValue;
                    break;
                    
                case 38: // Ortalama rüzgar yönü (ok için)
                    if (double.TryParse(commandValue, out double windDir))
                    {
                        airportData.Runway35.AvgWindDirection = windDir;
                    }
                    break;
                    
                // RWY 17 Verileri
                case 41: // Rüzgar hızı
                    airportData.Runway17.WindSpeed = commandValue;
                    break;
                    
                case 42: // 2 dk min rüzgar yönü
                    airportData.Runway17.Min2Direction = commandValue;
                    break;
                    
                case 43: // 2 dk min rüzgar hızı
                    airportData.Runway17.Min2Speed = commandValue;
                    break;
                    
                case 44: // 2 dk avg rüzgar yönü
                    airportData.Runway17.Avg2Direction = commandValue;
                    break;
                    
                case 45: // 2 dk avg rüzgar hızı
                    airportData.Runway17.Avg2Speed = commandValue;
                    break;
                    
                case 46: // 2 dk max rüzgar yönü
                    airportData.Runway17.Max2Direction = commandValue;
                    break;
                    
                case 47: // 2 dk max rüzgar hızı
                    airportData.Runway17.Max2Speed = commandValue;
                    break;
                    
                case 48: // Hw/Cw değeri
                    airportData.Runway17.HwCw = commandValue;
                    break;
                    
                case 50: // 10 dk min rüzgar yönü
                    airportData.Runway17.Min10Direction = commandValue;
                    break;
                    
                case 51: // 10 dk min rüzgar hızı
                    airportData.Runway17.Min10Speed = commandValue;
                    break;
                    
                case 52: // 10 dk avg rüzgar yönü
                    airportData.Runway17.Avg10Direction = commandValue;
                    break;
                    
                case 53: // 10 dk avg rüzgar hızı
                    airportData.Runway17.Avg10Speed = commandValue;
                    break;
                    
                case 54: // 10 dk max rüzgar yönü
                    airportData.Runway17.Max10Direction = commandValue;
                    break;
                    
                case 55: // 10 dk max rüzgar hızı
                    airportData.Runway17.Max10Speed = commandValue;
                    break;
                    
                case 56: // QFE değeri
                    airportData.Runway17.QFE = commandValue;
                    break;
                    
                case 57: // QFE inHg değeri
                    airportData.Runway17.QFEInHg = commandValue;
                    break;
                    
                case 58: // Ortalama rüzgar yönü (ok için)
                    if (double.TryParse(commandValue, out double windDirRight))
                    {
                        airportData.Runway17.AvgWindDirection = windDirRight;
                    }
                    break;
                    
                default:
                    UpdateConnectionStatus($"Bilinmeyen komut numarası: {commandNum}");
                    break;
            }
        }
        
        /// <summary>
        /// Bağlantı durum güncelleme
        /// </summary>
        private void UpdateConnectionStatus(string status)
        {
            ConnectionStatusChanged?.Invoke(this, status);
        }
    }
}