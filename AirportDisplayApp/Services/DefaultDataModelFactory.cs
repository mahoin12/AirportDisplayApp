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
            
            // Zaman ve genel meteorolojik değerler
            model.Time = DateTime.Now.ToString("HH:mm:ss");
            double temp = 20 + rand.NextDouble() * 15;
            model.Temperature = temp.ToString("F1");
            double dewPoint = temp - (5 + rand.NextDouble() * 10);
            model.DewPoint = dewPoint.ToString("F1");
            
            // Rastgele Nem ve Basınç değerleri
            double relHumidity = 40 + rand.NextDouble() * 45;
            model.RelativeHumidity = relHumidity.ToString("F0");
            
            double qnh = 1010 + rand.NextDouble() * 15;
            model.QNH = qnh.ToString("F1");
            model.QNHInHg = (qnh / 33.8639).ToString("F2");
            
            double qfe = qnh - (1 + rand.NextDouble() * 2);
            model.QFE = qfe.ToString("F1");
            model.QFESynop = (qfe - rand.NextDouble()).ToString("F1");
            
            // Runway 35 (Sol) için veriler
            UpdateRunwayData(model.Runway35, rand, true);
            
            // Runway 17 (Sağ) için veriler
            UpdateRunwayData(model.Runway17, rand, false);
            
            // Kullanılan pist bilgisini rastgele değiştir
            if (rand.Next(0, 2) == 0) {
                model.RunwayInUse = "35";
                model.RwyInUseInfo = "35";
            } else {
                model.RunwayInUse = "17";
                model.RwyInUseInfo = "17";
            }
            
            // METAR bilgisini güncelle
            model.Metar = GenerateSimulatedMetar(rand, model);
        }
        
        /// <summary>
        /// Pist verilerini rastgele günceller
        /// </summary>
        private static void UpdateRunwayData(RunwayDataModel model, Random rand, bool isLeft)
        {
            if (model == null) return;
            
            // Ana rüzgar yön ve hızları
            double avgWindDir = rand.Next(0, 360);
            int windSpeed = rand.Next(2, 15);
            
            // Hız değişkenliğini belirle
            int minSpeedDiff = Math.Min(2, windSpeed - 1);
            int maxSpeedDiff = Math.Min(5, windSpeed);
            
            // Yön değişkenliğini belirle
            int minDirDiff = 10;
            int maxDirDiff = 70;
            
            // Ortalama değerler
            model.AvgWindDirection = avgWindDir;
            model.WindSpeed = windSpeed.ToString();
            model.Avg2Direction = ((int)avgWindDir).ToString();
            model.Avg2Speed = windSpeed.ToString();
            
            // 2 dakikalık min/max değerler
            double min2Dir = NormalizeAngle(avgWindDir - rand.Next(minDirDiff, maxDirDiff));
            double max2Dir = NormalizeAngle(avgWindDir + rand.Next(minDirDiff, maxDirDiff));
            
            model.Min2Direction = ((int)min2Dir).ToString();
            model.Min2Speed = Math.Max(1, windSpeed - rand.Next(1, minSpeedDiff)).ToString();
            
            model.Max2Direction = ((int)max2Dir).ToString();
            model.Max2Speed = (windSpeed + rand.Next(1, maxSpeedDiff)).ToString();
            
            // 10 dakikalık değerler - daha geniş aralıklar
            if (rand.Next(0, 10) == 0 && isLeft) {
                // Sol pist için bazen CALM durumu olsun
                model.Min10Direction = "* CALM";
                model.Min10Speed = "";
            } else {
                double min10Dir = NormalizeAngle(avgWindDir - rand.Next(minDirDiff*2, maxDirDiff*2));
                model.Min10Direction = ((int)min10Dir).ToString();
                model.Min10Speed = Math.Max(1, windSpeed - rand.Next(1, minSpeedDiff+1)).ToString();
            }
            
            double avg10Dir = NormalizeAngle(avgWindDir + rand.Next(-20, 20));
            model.Avg10Direction = ((int)avg10Dir).ToString();
            model.Avg10Speed = windSpeed.ToString();
            
            double max10Dir = NormalizeAngle(avgWindDir + rand.Next(minDirDiff*2, maxDirDiff*2));
            model.Max10Direction = ((int)max10Dir).ToString();
            model.Max10Speed = (windSpeed + rand.Next(1, maxSpeedDiff+2)).ToString();
            
            // QFE değerleri
            double qfeDiff = rand.NextDouble() * 0.5;
            model.QFE = (1012 + qfeDiff).ToString("F1");
            model.QFEInHg = ((1012 + qfeDiff) / 33.8639).ToString("F2");
            
            // Hw/Cw (Headwind/Crosswind) değerleri
            UpdateHeadwindCrosswind(model, avgWindDir, windSpeed, isLeft);
            
            // Base değeri (sadece Sol pist için)
            if (isLeft) {
                if (rand.Next(0, 5) == 0) {
                    // 20% olasılıkla NCD (No Cloud Detected)
                    model.Base = "NCD";
                } else {
                    // Rastgele bir taban yüksekliği (feet cinsinden)
                    int baseHeight = rand.Next(1, 10) * 500;
                    model.Base = baseHeight.ToString();
                }
            }
        }
        
        /// <summary>
        /// Açı değerini 0-360 arasında normalleştirir
        /// </summary>
        private static double NormalizeAngle(double angle)
        {
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }
        
        /// <summary>
        /// Başucu rüzgarı (Headwind) ve yanal rüzgar (Crosswind) değerlerini günceller
        /// </summary>
        private static void UpdateHeadwindCrosswind(RunwayDataModel model, double windDir, int windSpeed, bool isLeft)
        {
            // Pist yönü (derece cinsinden)
            double runwayHeading = isLeft ? 350 : 170;
            
            // Rüzgar yönü ile pist yönü arasındaki farkı hesapla
            double angleDiff = Math.Abs(NormalizeAngle(windDir - runwayHeading));
            
            // Başucu ve yanal rüzgar bileşenlerini hesapla
            double headwindComponent = Math.Cos(angleDiff * Math.PI / 180) * windSpeed;
            double crosswindComponent = Math.Sin(angleDiff * Math.PI / 180) * windSpeed;
            
            // Başucu ve yanal rüzgar yönünü belirle
            string headwindPrefix = headwindComponent >= 0 ? "H" : "T"; // Headwind veya Tailwind
            string crosswindPrefix = isLeft ? 
                (crosswindComponent >= 0 ? "R" : "L") : // Sol pist için R(sağ) veya L(sol)
                (crosswindComponent >= 0 ? "L" : "R");  // Sağ pist için L(sol) veya R(sağ)
            
            // HwCw değerini oluştur - format: "H05   R03" (Headwind 5kt, Rightward crosswind 3kt)
            model.HwCw = $"{headwindPrefix}{Math.Abs((int)headwindComponent):D2}   {crosswindPrefix}{Math.Abs((int)crosswindComponent):D2}";
        }
        
        /// <summary>
        /// Simüle edilmiş METAR mesajı oluşturur
        /// </summary>
        private static string GenerateSimulatedMetar(Random rand, AirportDataModel model)
        {
            // Basit bir METAR formatı oluştur
            string windInfo = $"{model.Runway35.Avg2Direction}0{int.Parse(model.Runway35.Avg2Speed):D2}KT";
            
            // Rassal bulut bilgileri
            string[] cloudTypes = { "FEW", "SCT", "BKN", "OVC" };
            string[] cloudLevels = { "010", "025", "035", "060", "080", "100", "150" };
            
            string clouds = "";
            int cloudCount = rand.Next(1, 3);
            for (int i = 0; i < cloudCount; i++) {
                clouds += $" {cloudTypes[rand.Next(cloudTypes.Length)]}{cloudLevels[rand.Next(cloudLevels.Length)]}";
            }
            
            // Görüş mesafesi - 9999 (>10km) veya daha düşük bir değer
            string visibility = rand.Next(0, 5) == 0 ? 
                ((rand.Next(1, 10) * 1000).ToString()) : 
                "9999";
            
            // METAR mesajı oluştur
            string metar = $"METAR LTBL {DateTime.Now:ddHHmmZ} {windInfo} {visibility}{clouds} {model.Temperature}/{model.DewPoint.Split('.')[0]} Q{model.QNH.Split('.')[0]} NOSIG";
            
            // Ek bilgiler
            if (rand.Next(0, 2) == 0) {
                metar += $" RMK RWY{model.RunwayInUse} {model.Runway35.Avg2Direction}0{int.Parse(model.Runway35.Avg2Speed):D2}KT";
            }
            
            return metar;
        }
    }
}