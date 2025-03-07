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
    /// Rüzgar göstergesi bileşeni - İşaret kutuları düzeltilmiş versiyon
    /// </summary>
    public class WindIndicatorComponent
    {
        private readonly Window _owner;
        private readonly string _runwayName;
        private TextBlock _speedText;
        private Polygon _windTriangle;
        
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
            
            // Pist yönüne göre döndür - sabittir, güncellenmez
            double runwayAngle = _runwayName == "RWY 35" ? 0 : 180; // RWY 35 ve RWY 17 için ters yönler
            
            // Pist göstergesi - İkinci ve üçüncü referans görüntüye göre düzeltildi
            Canvas runwayCanvas = new Canvas();
            runwayCanvas.Width = 240;
            runwayCanvas.Height = 240;
            runwayCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            runwayCanvas.VerticalAlignment = VerticalAlignment.Center;
            
            // Ana pist dikdörtgeni (koyu gri)
            Rectangle runwayBase = new Rectangle();
            runwayBase.Width = 32;
            runwayBase.Height = 150;
            runwayBase.Fill = new SolidColorBrush(Color.FromRgb(90, 90, 90)); // Koyu gri
            
            // Pist şeklini canvas'e yerleştir
            Canvas.SetLeft(runwayBase, 120 - runwayBase.Width/2); // Tam merkezde
            Canvas.SetTop(runwayBase, 120 - runwayBase.Height/2); // Tam merkezde
            
            // Ana pist için transform grubu
            TransformGroup baseTransform = new TransformGroup();
            baseTransform.Children.Add(new RotateTransform(runwayAngle));
            runwayBase.RenderTransform = baseTransform;
            runwayBase.RenderTransformOrigin = new Point(0.5, 0.5);
            
            // Runwayı ekle
            runwayCanvas.Children.Add(runwayBase);
            
            // Üst işaretleri oluştur - dikdörtgenler piste dik olacak (90 derece)
            // Orta işaret (pist eksenine hizalı)
            Rectangle topMarkingCenter = new Rectangle();
            topMarkingCenter.Width = 24;
            topMarkingCenter.Height = 6;
            topMarkingCenter.Fill = Brushes.LightGray;
            
            // Sol işaret
            Rectangle topMarkingLeft = new Rectangle();
            topMarkingLeft.Width = 24;
            topMarkingLeft.Height = 6;
            topMarkingLeft.Fill = Brushes.LightGray;
            
            // Sağ işaret
            Rectangle topMarkingRight = new Rectangle();
            topMarkingRight.Width = 24;
            topMarkingRight.Height = 6;
            topMarkingRight.Fill = Brushes.LightGray;
            
            // İşaretler için kök Canvas (bu tüm işaretleri içerecek ve pist yönüne göre döndürülecek)
            Canvas markingsCanvas = new Canvas();
            markingsCanvas.Width = 80; // Yeterince geniş olsun
            markingsCanvas.Height = 30; // Yeterince yüksek olsun
            
            // Merkez işareti Canvas içinde yerleştir
            Canvas.SetLeft(topMarkingCenter, 40 - topMarkingCenter.Width/2); // Merkezde
            Canvas.SetTop(topMarkingCenter, 12); // Ortada 
            
            // Sol işareti Canvas içinde yerleştir
            Canvas.SetLeft(topMarkingLeft, 10);  // Sol tarafta
            Canvas.SetTop(topMarkingLeft, 12);  // Ortada
            
            // Sağ işareti Canvas içinde yerleştir
            Canvas.SetLeft(topMarkingRight, 70 - topMarkingRight.Width); // Sağ tarafta
            Canvas.SetTop(topMarkingRight, 12); // Ortada
            
            // İşaretleri ekle
            markingsCanvas.Children.Add(topMarkingCenter);
            markingsCanvas.Children.Add(topMarkingLeft);
            markingsCanvas.Children.Add(topMarkingRight);
            
            // İşaretleri içeren Canvas'ı yerleştir - pistin üstünde olacak şekilde
            Canvas.SetLeft(markingsCanvas, 120 - markingsCanvas.Width/2); // Ana canvas'ın ortasında
            Canvas.SetTop(markingsCanvas, 120 - runwayBase.Height/2 - markingsCanvas.Height); // Pistin üstünde
            
            // İşaretler canvas'ını pist açısına göre döndür
            TransformGroup markingsTransform = new TransformGroup();
            markingsTransform.Children.Add(new RotateTransform(runwayAngle));
            markingsCanvas.RenderTransform = markingsTransform;
            markingsCanvas.RenderTransformOrigin = new Point(0.5, 1.0); // Alt merkez noktası etrafında döndür
            
            // İşaretleri canvas'a ekle
            runwayCanvas.Children.Add(markingsCanvas);
            
            // Pist canvas'ını ekle
            windGrid.Children.Add(runwayCanvas);
            
            // Rüzgar göstergesi (üçgen) - Ok yerine üçgen kullanıldı
            Canvas indicatorCanvas = new Canvas();
            indicatorCanvas.Width = 240;
            indicatorCanvas.Height = 240;
            indicatorCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            indicatorCanvas.VerticalAlignment = VerticalAlignment.Center;
            
            // Üçgen şeklinde rüzgar göstergesi (rüzgarın geldiği yönü gösterir)
            _windTriangle = new Polygon();
            _windTriangle.Points = new PointCollection
            {
                new Point(0, 0),     // Alt
                new Point(-6, -20),  // Sol üst
                new Point(6, -20)    // Sağ üst
            };
            _windTriangle.Fill = Brushes.Black;
            
            // Üçgeni canvas'e ekle
            Canvas.SetLeft(_windTriangle, 120);
            Canvas.SetTop(_windTriangle, 120);
            
            // Dönüş transformu
            TransformGroup triangleTransform = new TransformGroup();
            
            // 1. Transform: Üçgeni yerleştir (dairenin dışına)
            TranslateTransform translateTransform = new TranslateTransform(0, -100);
            triangleTransform.Children.Add(translateTransform);
            
            // 2. Transform: Başlangıç açısı 0 - Rüzgarın geldiği yönü gösterir
            RotateTransform rotateTransform = new RotateTransform(0);
            triangleTransform.Children.Add(rotateTransform);
            
            _windTriangle.RenderTransform = triangleTransform;
            _windTriangle.RenderTransformOrigin = new Point(0, 0);
            
            indicatorCanvas.Children.Add(_windTriangle);
            windGrid.Children.Add(indicatorCanvas);
            
            // Rüzgar göstergesi için bir isim kaydedelim
            string windIndicatorName = _runwayName == "RWY 35" ? "LeftWindIndicator" : "RightWindIndicator";
            _owner.RegisterName(windIndicatorName, _windTriangle);
            
            // Rüzgar hızı göstergesi - tam ortada beyaz kutu içinde siyah rakam
            Border speedBorder = new Border();
            speedBorder.Background = Brushes.White;
            speedBorder.BorderBrush = Brushes.Gray;
            speedBorder.BorderThickness = new Thickness(1);
            speedBorder.Width = 40;
            speedBorder.Height = 30;
            speedBorder.HorizontalAlignment = HorizontalAlignment.Center;
            speedBorder.VerticalAlignment = VerticalAlignment.Center;
            
            _speedText = new TextBlock();
            _speedText.Text = _runwayName == "RWY 35" ? "7" : "5";  // Ekran görüntüsündeki değerlere uygun
            _speedText.FontSize = 20;
            _speedText.FontWeight = FontWeights.Bold;
            _speedText.Foreground = Brushes.Black;
            _speedText.VerticalAlignment = VerticalAlignment.Center;
            _speedText.HorizontalAlignment = HorizontalAlignment.Center;
            
            // Text elemanı için ID atama
            string speedTextName = _runwayName == "RWY 35" ? "LeftWindSpeedText" : "RightWindSpeedText";
            _owner.RegisterName(speedTextName, _speedText);
            
            speedBorder.Child = _speedText;
            windGrid.Children.Add(speedBorder);
            
            // Başlangıçta seçili rüzgar kutularını renklendir (örnek gösterim için)
            // Soldaki pist için 280-340 dereceler arası 
            // Sağdaki pist için 240-300 dereceler arası
            if (_runwayName == "RWY 35")
            {
                ColorDirectionRange(280, 340, Color.FromRgb(0, 150, 150)); // Turkuaz/Yeşil
            }
            else
            {
                ColorDirectionRange(240, 300, Color.FromRgb(0, 150, 150)); // Turkuaz/Yeşil
            }
            
            return windGrid;
        }
        
        /// <summary>
        /// Rüzgar yönünü günceller
        /// </summary>
        public void UpdateWindDirection(double direction)
        {
            try
            {
                // Rüzgar üçgeni direkt olarak referans tutuluyor, bu daha güvenilir
                if (_windTriangle != null)
                {
                    // Transform grubunu al
                    TransformGroup transformGroup = _windTriangle.RenderTransform as TransformGroup;
                    if (transformGroup != null && transformGroup.Children.Count > 1)
                    {
                        // İkinci transform döndürme olmalı (ilki konumlandırma)
                        RotateTransform rotateTransform = transformGroup.Children[1] as RotateTransform;
                        if (rotateTransform != null)
                        {
                            // Rüzgar yönünü ayarla (rüzgarın nereden geldiğini gösterir)
                            rotateTransform.Angle = direction;
                            
                            // Aynı zamanda kutu renklerini de güncelle
                            UpdateDirectionColors(direction - 40, direction + 40, 0, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunu logla ama UI'ı engellemeden devam et
                System.Diagnostics.Debug.WriteLine($"Rüzgar yönü güncellenirken hata: {ex.Message}");
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
            
            // Önce tüm kutuları gri yap
            foreach (var mark in _directionMarks.Values)
            {
                mark.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200)); // Açık gri
            }
            
            // Kutuların renklerini güncelle
            UpdateDirectionColors(min2Direction, max2Direction, min10Direction, max10Direction);
        }
        
        /// <summary>
        /// Rüzgar yön kutularının renklerini günceller
        /// </summary>
        private void UpdateDirectionColors(double min2Direction, double max2Direction, 
                                         double min10Direction, double max10Direction)
        {
            // 2 dakikalık ölçümler için turkuaz renk (koyu mavi)
            Color color2Min = Color.FromRgb(0, 150, 150); // Turkuaz/Yeşil
            
            // Min ve max arasındaki bölgeyi renklendir (2-dakika)
            if (min2Direction > 0) // CALM değilse
            {
                ColorDirectionRange((int)min2Direction, (int)max2Direction, color2Min);
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
            
            if (double.TryParse(cleanedStr, out double direction))
            {
                return direction;
            }
            
            return 0; // Dönüştürme başarısız olursa 0 değeri
        }
    }
}