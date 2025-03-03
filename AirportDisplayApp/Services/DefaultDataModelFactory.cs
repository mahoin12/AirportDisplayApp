using AirportDisplayApp.Models;
using System;

namespace AirportDisplayApp.Services
{
    /// <summary>
    /// Varsayılan değerlere sahip veri modelleri oluşturan fabrika sınıfı
    /// </summary>
    public static class DefaultDataModelFactory
    {
        /// <summary>
        /// Varsayılan değerlere sahip AirportDataModel oluşturur
        /// </summary>
        public static AirportDataModel CreateDefaultDataModel()
        {
            AirportDataModel model = new AirportDataModel();
            
            // Merkez panel verileri
            model.Time = "00:00:00";
            model.RunwayInUse = "35";
            model.RwyInUseInfo = "35";
            model.QNH = "1013.2";
            model.QNHInHg = "29.92";
            model.QFE = "1013.2";
            model.QFESynop = "1012.7";
            model.Low = "NCD";
            model.Temperature = "30.4";
            model.DewPoint = "19.7";
            model.RelativeHumidity = "52";
            model.TempMax = "30.9";
            model.TempMin = "20.4";
            model.RunwayTemp = "49.9";
            model.Metar = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320";
            
            // Sol pist (35) verilerini başlat
            InitializeRunway35(model.Runway35);
            
            // Sağ pist (17) verilerini başlat
            InitializeRunway17(model.Runway17);
            
            return model;
        }
        
        /// <summary>
        /// Sol Pist (RWY 35) modeline varsayılan değerleri atar
        /// </summary>
        private static void InitializeRunway35(RunwayDataModel model)
        {
            if (model == null) return;
            
            model.WindSpeed = "7";
            model.AvgWindDirection = 270;
            model.HwCw = "H02   L05";
            model.Base = "NCD";
            model.Min2Direction = "250";
            model.Min2Speed = "3";
            model.Avg2Direction = "270";
            model.Avg2Speed = "5";
            model.Max2Direction = "320";
            model.Max2Speed = "8";
            model.Min10Direction = "* CALM";
            model.Min10Speed = "";
            model.Avg10Direction = "280";
            model.Avg10Speed = "5";
            model.Max10Direction = "320";
            model.Max10Speed = "9";
            model.QFE = "1012.8";
            model.QFEInHg = "29.91";
        }
        
        /// <summary>
        /// Sağ Pist (RWY 17) modeline varsayılan değerleri atar
        /// </summary>
        private static void InitializeRunway17(RunwayDataModel model)
        {
            if (model == null) return;
            
            model.WindSpeed = "5";
            model.AvgWindDirection = 90;
            model.HwCw = "T01   R05";
            model.Min2Direction = "250";
            model.Min2Speed = "3";
            model.Avg2Direction = "270";
            model.Avg2Speed = "5";
            model.Max2Direction = "320";
            model.Max2Speed = "6";
            model.Min10Direction = "220";
            model.Min10Speed = "2";
            model.Avg10Direction = "290";
            model.Avg10Speed = "5";
            model.Max10Direction = "320";
            model.Max10Speed = "8";
            model.QFE = "1012.7";
            model.QFEInHg = "29.90";
        }
        
        /// <summary>
        /// Mevcut bir modeli rastgele verilerle günceller
        /// </summary>
        public static void UpdateModelWithRandomData(AirportDataModel model)
        {
            if (model == null) return;
            
            Random rand = new Random();
            
            // Rüzgar yönlerini rastgele güncelle
            double leftWindDir = rand.Next(0, 360);
            double rightWindDir = rand.Next(0, 360);
            
            // Rüzgar hızlarını rastgele güncelle
            int leftWindSpeed = rand.Next(2, 15);
            int rightWindSpeed = rand.Next(2, 15);
            
            // Verileri doğrudan güncelle
            // Zaman ve sıcaklık
            model.Time = DateTime.Now.ToString("HH:mm:ss");
            double temp = 20 + rand.NextDouble() * 15;
            model.Temperature = temp.ToString("F1");
            double dewPoint = temp - (5 + rand.NextDouble() * 10);
            model.DewPoint = dewPoint.ToString("F1");
            
            // Runway35 verilerini güncelle (property'nin içindeki değerleri güncelliyoruz, property'nin kendisini değil)
            model.Runway35.WindSpeed = leftWindSpeed.ToString();
            model.Runway35.AvgWindDirection = leftWindDir;
            
            // Runway17 verilerini güncelle (property'nin içindeki değerleri güncelliyoruz, property'nin kendisini değil)
            model.Runway17.WindSpeed = rightWindSpeed.ToString();
            model.Runway17.AvgWindDirection = rightWindDir;
        }
    }
}