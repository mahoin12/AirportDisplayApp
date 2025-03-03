using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Rüzgar göstergesi bileşeni - Referans görüntülere (2. ve 3. resim) uygun olarak düzeltilmiş
    /// Herhangi bir değeri saklamadan, dışarıdan gelen değerlerle çalışır
    /// </summary>
    public class WindIndicatorComponent
    {
        private readonly Window _owner;
        private readonly string _runwayName;
        private TextBlock _speedText;
        private FrameworkElement _windIndicator;
        
        // Kutuları saklamak için koleksiyon
        private Dictionary<int, Rectangle> _directionMarks = new Dictionary<int, Rectangle>();

        public WindIndicatorComponent(Window owner, string runwayName)
        {
            _owner = owner;
            _runwayName = runwayName;
        }
        
        /// <summary>
        /// Rüzgar göstergesini oluşturur ve döndürür
        /// </summary>
        public Grid CreateWindIndicator()
        {
            Grid windGrid = new Grid();
            windGrid.Margin = new Thickness(10);
            
            // Dış dairesel ölçek (beyaz daire)
            Ellipse outerCircle = new Ellipse();
            outerCircle.Width = 240;
            outerCircle.Height = 240;
            outerCircle.Stroke = Brushes.LightGray;
            outerCircle.StrokeThickness = 1;
            outerCircle.Fill = Brushes.White;
            outerCircle.HorizontalAlignment = HorizontalAlignment.Center;
            outerCircle.VerticalAlignment = VerticalAlignment.Center;
            
            windGrid.Children.Add(outerCircle);
            
            // Rüzgar yönü için işaretler ve daire üzerindeki kutular (10 derecelik aralıklarla, toplam 36 tane)
            Canvas directionMarksCanvas = new Canvas();
            directionMarksCanvas.Width = 240;
            directionMarksCanvas.Height = 240;
            directionMarksCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            directionMarksCanvas.VerticalAlignment = VerticalAlignment.Center;
            
            // 10 derecelik aralıklarla kutular (toplam 36 tane)
            for (int angle = 0; angle < 360; angle += 10)
            {
                double radians = angle * Math.PI / 180;
                double centerX = 120;
                double centerY = 120;
                
                // Kutu pozisyonu (çemberin kenarına yakın)
                double boxRadius = 105; // Kutuların dairenin dışına doğru uzaklığı
                
                // Kutu oluştur - Referans görüntüye göre küçük kare kutular
                Rectangle dirMark = new Rectangle();
                dirMark.Width = 10;
                dirMark.Height = 10;
                
                // Başlangıçta tüm kutular gri
                dirMark.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200)); // Açık gri
                
                // Kutuların tag'ine açı değerini saklayalım (dinamik renklendirme için)
                dirMark.Tag = angle;
                
                // Kutuları sözlüğe ekleyelim (sonradan renklendirmek için)
                _directionMarks[angle] = dirMark;
                
                // Kutuların merkeze bakacak şekilde konumlandırılması
                double markX = centerX + Math.Sin(radians) * boxRadius - dirMark.Width / 2;
                double markY = centerY - Math.Cos(radians) * boxRadius - dirMark.Height / 2;
                
                Canvas.SetLeft(dirMark, markX);
                Canvas.SetTop(dirMark, markY);
                
                directionMarksCanvas.Children.Add(dirMark);
            }
            
            // 30 derecelik aralıklarla rakamlar
            for (int angle = 0; angle < 360; angle += 30)
            {
                double radians = angle * Math.PI / 180;
                double centerX = 120;
                double centerY = 120;
                double textRadius = 125; // Rakamlar biraz daha dışta
                
                // Açı etiketleri (03, 06, 09, ... şeklinde her 30 derecede bir)
                // 360=00, 30=03, 60=06, ...
                int dirValue = angle / 10;
                string dirLabel = (dirValue % 36).ToString("00");
                
                TextBlock dirText = new TextBlock();
                dirText.Text = dirLabel;
                dirText.FontSize = 12;
                dirText.Foreground = Brushes.Black;
                
                // Metin pozisyonu (dairenin dışında)
                double textX = centerX + Math.Sin(radians) * textRadius - 8;
                double textY = centerY - Math.Cos(radians) * textRadius - 8;
                
                Canvas.SetLeft(dirText, textX);
                Canvas.SetTop(dirText, textY);
                
                directionMarksCanvas.Children.Add(dirText);
            }
            
            windGrid.Children.Add(directionMarksCanvas);
            
            // Pist göstergesi - İkinci ve üçüncü referans görüntüye göre düzeltildi
            Grid runwayGrid = new Grid();
            runwayGrid.HorizontalAlignment = HorizontalAlignment.Center;
            runwayGrid.VerticalAlignment = VerticalAlignment.Center;
            
            // Ana pist dikdörtgeni (gri)
            Rectangle runwayBase = new Rectangle();
            runwayBase.Width = 20;
            runwayBase.Height = 60;
            runwayBase.Fill = Brushes.DarkGray;
            
            // Pist üzerindeki 3 beyaz çizgi (altında)
            StackPanel runwayStripes = new StackPanel();
            runwayStripes.Width = 20;
            runwayStripes.Height = 20;
            runwayStripes.VerticalAlignment = VerticalAlignment.Bottom;
            runwayStripes.HorizontalAlignment = HorizontalAlignment.Center;
            
            for (int i = 0; i < 3; i++)
            {
                Rectangle stripe = new Rectangle();
                stripe.Width = 12;
                stripe.Height = 3;
                stripe.Margin = new Thickness(0, 1, 0, 1);
                stripe.Fill = Brushes.White;
                stripe.HorizontalAlignment = HorizontalAlignment.Center;
                
                runwayStripes.Children.Add(stripe);
            }
            
            runwayGrid.Children.Add(runwayBase);
            runwayGrid.Children.Add(runwayStripes);
            
            // Pist yönüne göre döndür
            TransformGroup runwayTransform = new TransformGroup();
            double runwayDirection = _runwayName == "RWY 35" ? 0 : 180; // RWY 35 ve RWY 17 için farklı yönler
            runwayTransform.Children.Add(new RotateTransform(runwayDirection));
            runwayGrid.RenderTransform = runwayTransform;
            runwayGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            
            windGrid.Children.Add(runwayGrid);
            
            // Rüzgar göstergesi (üçgen) - Ok yerine üçgen kullanıldı
            Canvas indicatorCanvas = new Canvas();
            indicatorCanvas.Width = 240;
            indicatorCanvas.Height = 240;
            indicatorCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            indicatorCanvas.VerticalAlignment = VerticalAlignment.Center;
            
            // Üçgen şeklinde rüzgar göstergesi (daha ince ve uzun)
            Polygon windTriangle = new Polygon();
            windTriangle.Points = new PointCollection
            {
                new Point(0, -15),   // Üst
                new Point(-5, 0),    // Sol alt
                new Point(5, 0)      // Sağ alt
            };
            windTriangle.Fill = Brushes.Black;
            
            // Üçgeni canvas'e ekle
            Canvas.SetLeft(windTriangle, 120);
            Canvas.SetTop(windTriangle, 120);
            
            // Dönüş transformu
            TransformGroup triangleTransform = new TransformGroup();
            
            // 1. Transform: Üçgeni yerleştir (dairenin dışına)
            TranslateTransform translateTransform = new TranslateTransform(0, -115);
            triangleTransform.Children.Add(translateTransform);
            
            // 2. Transform: Başlangıç açısı 0
            RotateTransform rotateTransform = new RotateTransform(0);
            triangleTransform.Children.Add(rotateTransform);
            
            windTriangle.RenderTransform = triangleTransform;
            windTriangle.RenderTransformOrigin = new Point(0, 0);
            
            indicatorCanvas.Children.Add(windTriangle);
            windGrid.Children.Add(indicatorCanvas);
            
            // Rüzgar göstergesini kaydet (referans için)
            _windIndicator = windTriangle;
            
            // Rüzgar göstergesi için bir isim kaydedelim
            string windIndicatorName = _runwayName == "RWY 35" ? "LeftWindIndicator" : "RightWindIndicator";
            _owner.RegisterName(windIndicatorName, windTriangle);
            
            // Rüzgar hızı göstergesi - tam ortada beyaz kutu içinde siyah rakam
            Border speedBorder = new Border();
            speedBorder.Background = Brushes.White;
            speedBorder.BorderBrush = Brushes.Black;
            speedBorder.BorderThickness = new Thickness(1);
            speedBorder.Width = 40;
            speedBorder.Height = 30;
            speedBorder.HorizontalAlignment = HorizontalAlignment.Center;
            speedBorder.VerticalAlignment = VerticalAlignment.Center;
            
            _speedText = new TextBlock();
            _speedText.Text = _runwayName == "RWY 35" ? "7" : "5";  // Ekran görüntüsündeki değerlere uygun
            _speedText.FontSize = 20;
            _speedText.FontWeight = FontWeights.Bold;
            _speedText.VerticalAlignment = VerticalAlignment.Center;
            _speedText.HorizontalAlignment = HorizontalAlignment.Center;
            
            // Text elemanı için ID atama
            string speedTextName = _runwayName == "RWY 35" ? "LeftWindSpeedText" : "RightWindSpeedText";
            _owner.RegisterName(speedTextName, _speedText);
            
            speedBorder.Child = _speedText;
            windGrid.Children.Add(speedBorder);
            
            return windGrid;
        }
        
        /// <summary>
        /// Rüzgar yönünü günceller
        /// </summary>
        public void UpdateWindDirection(double direction)
        {
            // Rüzgar göstergesini bul
            string indicatorName = _runwayName == "RWY 35" ? "LeftWindIndicator" : "RightWindIndicator";
            var windTriangle = _owner.FindName(indicatorName) as Polygon;
            
            if (windTriangle != null)
            {
                // Transform grubunu al
                TransformGroup transformGroup = windTriangle.RenderTransform as TransformGroup;
                if (transformGroup != null && transformGroup.Children.Count > 1)
                {
                    // İkinci transform döndürme olmalı (ilki konumlandırma)
                    RotateTransform rotateTransform = transformGroup.Children[1] as RotateTransform;
                    if (rotateTransform != null)
                    {
                        // Rüzgar yönünü ayarla (rüzgarın nereden geldiğini gösterir)
                        rotateTransform.Angle = direction;
                    }
                }
            }
        }
        
        /// <summary>
        /// Rüzgar hızını günceller
        /// </summary>
        public void UpdateWindSpeed(string speed)
        {
            if (_speedText != null)
            {
                _speedText.Text = speed;
            }
        }
        
        /// <summary>
        /// Tüm rüzgar ölçüm yönlerini günceller
        /// </summary>
        public void UpdateWindDirections(string min2Dir, string avg2Dir, string max2Dir, 
                                        string min10Dir, string avg10Dir, string max10Dir)
        {
            // Önce tüm değerleri dönüştür
            double min2Direction = TryParseDirection(min2Dir);
            double avg2Direction = TryParseDirection(avg2Dir);
            double max2Direction = TryParseDirection(max2Dir);
            double min10Direction = TryParseDirection(min10Dir);
            double avg10Direction = TryParseDirection(avg10Dir);
            double max10Direction = TryParseDirection(max10Dir);
            
            // Üçgeni avg2Dir yönüne çevir
            UpdateWindDirection(avg2Direction);
            
            // Kutuların renklerini güncelle
            UpdateDirectionColors(min2Direction, max2Direction, min10Direction, max10Direction);
        }
        
        /// <summary>
        /// Rüzgar yön kutularının renklerini günceller
        /// </summary>
        private void UpdateDirectionColors(double min2Direction, double max2Direction, 
                                         double min10Direction, double max10Direction)
        {
            // Önce tüm kutuları gri yap
            foreach (var mark in _directionMarks.Values)
            {
                mark.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200)); // Açık gri
            }
            
            // 2 dakikalık ölçümler için turkuaz renk (koyu mavi)
            Color color2Min = Color.FromRgb(0, 156, 178); // Turkuaz
            
            // Min ve max arasındaki bölgeyi renklendir (2-dakika)
            if (min2Direction > 0) // CALM değilse
            {
                ColorDirectionRange((int)min2Direction, (int)max2Direction, color2Min);
            }
            
            // 10 dakikalık ölçümler için açık mavi renk
            Color color10Min = Color.FromRgb(86, 180, 233); // Açık mavi
            
            // Min ve max arasındaki bölgeyi renklendir (10-dakika)
            if (min10Direction > 0) // CALM değilse
            {
                ColorDirectionRange((int)min10Direction, (int)max10Direction, color10Min);
            }
        }
        
        /// <summary>
        /// Belirli aralıktaki kutuları renklendirir
        /// </summary>
        private void ColorDirectionRange(int startAngle, int endAngle, Color color)
        {
            // Açıları 10'un katlarına yuvarla
            startAngle = Math.Max(0, (startAngle / 10) * 10);
            endAngle = Math.Max(0, (endAngle / 10) * 10);
            
            // Eğer başlangıç açısı bitiş açısından büyükse, 360 ekleyerek tam tur tamamla
            if (startAngle > endAngle)
            {
                for (int angle = startAngle; angle < 360; angle += 10)
                {
                    if (_directionMarks.ContainsKey(angle))
                    {
                        _directionMarks[angle].Fill = new SolidColorBrush(color);
                    }
                }
                
                for (int angle = 0; angle <= endAngle; angle += 10)
                {
                    if (_directionMarks.ContainsKey(angle))
                    {
                        _directionMarks[angle].Fill = new SolidColorBrush(color);
                    }
                }
            }
            else
            {
                // Normal aralık
                for (int angle = startAngle; angle <= endAngle; angle += 10)
                {
                    if (_directionMarks.ContainsKey(angle))
                    {
                        _directionMarks[angle].Fill = new SolidColorBrush(color);
                    }
                }
            }
        }
        
        /// <summary>
        /// Güvenli bir şekilde yön değerini dönüştürür (CALM vs. özel durumlar için)
        /// </summary>
        private double TryParseDirection(string directionStr)
        {
            // CALM kontrolü
            if (string.IsNullOrEmpty(directionStr) || directionStr.Contains("CALM"))
            {
                return 0; // CALM durumu için 0 değeri
            }
            
            // Sayısal kısmı çıkart (özel karakterleri temizle)
            string cleanedStr = new string(directionStr.Where(c => char.IsDigit(c) || c == '.').ToArray());
            
            double direction;
            if (double.TryParse(cleanedStr, out direction))
            {
                return direction;
            }
            
            return 0; // Dönüştürme başarısız olursa 0 değeri
        }
    }
}