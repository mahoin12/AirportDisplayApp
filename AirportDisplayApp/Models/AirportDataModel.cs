using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AirportDisplayApp.Models
{
    /// <summary>
    /// Havaalanı verilerini tutan model sınıfı - Geliştirilmiş sürüm
    /// INotifyPropertyChanged implementasyonu ile UI güncellemesini destekler
    /// </summary>
    public class AirportDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Özellik değişikliğini bildir
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Zaman bilgisi
        private string _time = "00:00:00";
        public string Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged();
                }
            }
        }

        // Kullanılan pist bilgisi
        private string _runwayInUse = "35";
        public string RunwayInUse
        {
            get => _runwayInUse;
            set
            {
                if (_runwayInUse != value)
                {
                    _runwayInUse = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _rwyInUseInfo = "35";
        public string RwyInUseInfo
        {
            get => _rwyInUseInfo;
            set
            {
                if (_rwyInUseInfo != value)
                {
                    _rwyInUseInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        // Merkez panel verileri
        private string _qnh = "1013.2";
        public string QNH
        {
            get => _qnh;
            set
            {
                if (_qnh != value)
                {
                    _qnh = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _qnhInHg = "29.92";
        public string QNHInHg
        {
            get => _qnhInHg;
            set
            {
                if (_qnhInHg != value)
                {
                    _qnhInHg = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _qfe = "1013.2";
        public string QFE
        {
            get => _qfe;
            set
            {
                if (_qfe != value)
                {
                    _qfe = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _qfeSynop = "1012.7";
        public string QFESynop
        {
            get => _qfeSynop;
            set
            {
                if (_qfeSynop != value)
                {
                    _qfeSynop = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _low = "NCD";
        public string Low
        {
            get => _low;
            set
            {
                if (_low != value)
                {
                    _low = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _temperature = "30.4";
        public string Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _dewPoint = "19.7";
        public string DewPoint
        {
            get => _dewPoint;
            set
            {
                if (_dewPoint != value)
                {
                    _dewPoint = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _relativeHumidity = "52";
        public string RelativeHumidity
        {
            get => _relativeHumidity;
            set
            {
                if (_relativeHumidity != value)
                {
                    _relativeHumidity = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _tempMax = "30.9";
        public string TempMax
        {
            get => _tempMax;
            set
            {
                if (_tempMax != value)
                {
                    _tempMax = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _tempMin = "20.4";
        public string TempMin
        {
            get => _tempMin;
            set
            {
                if (_tempMin != value)
                {
                    _tempMin = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _runwayTemp = "49.9";
        public string RunwayTemp
        {
            get => _runwayTemp;
            set
            {
                if (_runwayTemp != value)
                {
                    _runwayTemp = value;
                    OnPropertyChanged();
                }
            }
        }

        // METAR bilgisi
        private string _metar = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320";
        public string Metar
        {
            get => _metar;
            set
            {
                if (_metar != value)
                {
                    _metar = value;
                    OnPropertyChanged();
                }
            }
        }

        // Pist verileri
        public RunwayDataModel Runway35 { get; private set; }
        public RunwayDataModel Runway17 { get; private set; }

        public AirportDataModel()
        {
            // Varsayılan değerlerle pist nesnelerini oluştur
            Runway35 = new RunwayDataModel("RWY 35");
            Runway17 = new RunwayDataModel("RWY 17");

            // Sol pist (35) için varsayılan değerler
            Runway35.WindSpeed = "7";
            Runway35.AvgWindDirection = 270;
            Runway35.HwCw = "H02   L05";
            Runway35.Base = "NCD";
            Runway35.Min10Direction = "* CALM";
            Runway35.Min10Speed = "";
            Runway35.QFE = "1012.8";
            Runway35.QFEInHg = "29.91";

            // Sağ pist (17) için varsayılan değerler
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
    /// Pist verilerini tutan model sınıfı - Geliştirilmiş sürüm
    /// INotifyPropertyChanged implementasyonu ile UI güncellemesini destekler
    /// </summary>
    public class RunwayDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Özellik değişikliğini bildir
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Pist adı
        private string _runwayName;
        public string RunwayName
        {
            get => _runwayName;
            set
            {
                if (_runwayName != value)
                {
                    _runwayName = value;
                    OnPropertyChanged();
                }
            }
        }

        // Rüzgar hızı
        private string _windSpeed = "0";
        public string WindSpeed
        {
            get => _windSpeed;
            set
            {
                if (_windSpeed != value)
                {
                    _windSpeed = value;
                    OnPropertyChanged();
                }
            }
        }

        // Rüzgar yönü (ok için)
        private double _avgWindDirection = 0;
        public double AvgWindDirection
        {
            get => _avgWindDirection;
            set
            {
                if (_avgWindDirection != value)
                {
                    _avgWindDirection = value;
                    OnPropertyChanged();
                }
            }
        }

        // 2 dakikalık veriler
        private string _min2Direction = "250";
        public string Min2Direction
        {
            get => _min2Direction;
            set
            {
                if (_min2Direction != value)
                {
                    _min2Direction = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _min2Speed = "3";
        public string Min2Speed
        {
            get => _min2Speed;
            set
            {
                if (_min2Speed != value)
                {
                    _min2Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _avg2Direction = "270";
        public string Avg2Direction
        {
            get => _avg2Direction;
            set
            {
                if (_avg2Direction != value)
                {
                    _avg2Direction = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _avg2Speed = "5";
        public string Avg2Speed
        {
            get => _avg2Speed;
            set
            {
                if (_avg2Speed != value)
                {
                    _avg2Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _max2Direction = "320";
        public string Max2Direction
        {
            get => _max2Direction;
            set
            {
                if (_max2Direction != value)
                {
                    _max2Direction = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _max2Speed = "8";
        public string Max2Speed
        {
            get => _max2Speed;
            set
            {
                if (_max2Speed != value)
                {
                    _max2Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        // Hw/Cw değeri (Headwind/Crosswind)
        private string _hwCw = "";
        public string HwCw
        {
            get => _hwCw;
            set
            {
                if (_hwCw != value)
                {
                    _hwCw = value;
                    OnPropertyChanged();
                }
            }
        }

        // Base değeri (sadece sol pist için)
        private string _base = "";
        public string Base
        {
            get => _base;
            set
            {
                if (_base != value)
                {
                    _base = value;
                    OnPropertyChanged();
                }
            }
        }

        // 10 dakikalık veriler
        private string _min10Direction = "";
        public string Min10Direction
        {
            get => _min10Direction;
            set
            {
                if (_min10Direction != value)
                {
                    _min10Direction = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _min10Speed = "";
        public string Min10Speed
        {
            get => _min10Speed;
            set
            {
                if (_min10Speed != value)
                {
                    _min10Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _avg10Direction = "280";
        public string Avg10Direction
        {
            get => _avg10Direction;
            set
            {
                if (_avg10Direction != value)
                {
                    _avg10Direction = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _avg10Speed = "5";
        public string Avg10Speed
        {
            get => _avg10Speed;
            set
            {
                if (_avg10Speed != value)
                {
                    _avg10Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _max10Direction = "320";
        public string Max10Direction
        {
            get => _max10Direction;
            set
            {
                if (_max10Direction != value)
                {
                    _max10Direction = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _max10Speed = "9";
        public string Max10Speed
        {
            get => _max10Speed;
            set
            {
                if (_max10Speed != value)
                {
                    _max10Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        // QFE değerleri
        private string _qfe = "";
        public string QFE
        {
            get => _qfe;
            set
            {
                if (_qfe != value)
                {
                    _qfe = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _qfeInHg = "";
        public string QFEInHg
        {
            get => _qfeInHg;
            set
            {
                if (_qfeInHg != value)
                {
                    _qfeInHg = value;
                    OnPropertyChanged();
                }
            }
        }

        // Constructor
        public RunwayDataModel(string runwayName)
        {
            RunwayName = runwayName;
        }
    }
}