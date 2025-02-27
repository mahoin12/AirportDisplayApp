using System;
using System.Collections.Generic;

namespace AirportDisplayApp.Models
{
    /// <summary>
    /// Havaalanı verilerini tutan model sınıfı
    /// </summary>
    public class AirportDataModel
    {
        // Genel bilgiler
        public string Time { get; set; } = "00:00:00";
        public string RunwayInUse { get; set; } = "35";
        public string RwyInUseInfo { get; set; } = "35";
        
        // Merkez panel verileri
        public string QNH { get; set; } = "1013.2";
        public string QNHInHg { get; set; } = "29.92";
        public string QFE { get; set; } = "1013.2";
        public string QFESynop { get; set; } = "1012.7";
        public string Low { get; set; } = "NCD";
        public string Temperature { get; set; } = "30.4";
        public string DewPoint { get; set; } = "19.7";
        public string RelativeHumidity { get; set; } = "52";
        public string TempMax { get; set; } = "30.9";
        public string TempMin { get; set; } = "20.4";
        public string RunwayTemp { get; set; } = "49.9";
        
        // METAR bilgisi
        public string Metar { get; set; } = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320";
        
        // Sol pist (RWY 35) verileri
        public RunwayDataModel Runway35 { get; set; } = new RunwayDataModel();
        
        // Sağ pist (RWY 17) verileri
        public RunwayDataModel Runway17 { get; set; } = new RunwayDataModel();
        
        public AirportDataModel()
        {
            // Varsayılan değerleri ayarla
            Runway35.RunwayName = "RWY 35";
            Runway35.WindSpeed = "7";
            Runway35.AvgWindDirection = 270;
            Runway35.HwCw = "H02   L05";
            Runway35.Base = "NCD";
            Runway35.Min10Direction = "* CALM";
            Runway35.Min10Speed = "";
            Runway35.QFE = "1012.8";
            Runway35.QFEInHg = "29.91";
            
            Runway17.RunwayName = "RWY 17";
            Runway17.WindSpeed = "5";
            Runway17.AvgWindDirection = 90;
            Runway17.HwCw = "T01   R05";
            Runway17.Min10Direction = "220";
            Runway17.Min10Speed = "2";
            Runway17.QFE = "1012.7";
            Runway17.QFEInHg = "29.90";
        }
    }
    
    /// <summary>
    /// Pist verilerini tutan model sınıfı
    /// </summary>
    public class RunwayDataModel
    {
        public string RunwayName { get; set; } = "";
        public string WindSpeed { get; set; } = "0";
        public double AvgWindDirection { get; set; } = 0;
        
        // 2 dakikalık veriler
        public string Min2Direction { get; set; } = "250";
        public string Min2Speed { get; set; } = "3";
        public string Avg2Direction { get; set; } = "270";
        public string Avg2Speed { get; set; } = "5";
        public string Max2Direction { get; set; } = "320";
        public string Max2Speed { get; set; } = "8";
        
        // Hw/Cw değeri
        public string HwCw { get; set; } = "";
        
        // Base değeri (sadece sol pist için)
        public string Base { get; set; } = "";
        
        // 10 dakikalık veriler
        public string Min10Direction { get; set; } = "";
        public string Min10Speed { get; set; } = "";
        public string Avg10Direction { get; set; } = "280";
        public string Avg10Speed { get; set; } = "5";
        public string Max10Direction { get; set; } = "320";
        public string Max10Speed { get; set; } = "9";
        
        // QFE değerleri
        public string QFE { get; set; } = "";
        public string QFEInHg { get; set; } = "";
    }
}