using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Rüzgar göstergesi bileşeni - Referans görüntüye uygun olarak tasarlanmış
    /// </summary>
    public class WindIndicatorComponent
    {
        private readonly Window _owner;
        private readonly string _runwayName;
        private Rectangle _windArrow;
        private TextBlock _speedText;
        
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
                
                // Kutu pozisyonu (çemberin orta kısmı)
                double boxRadius = 100; // Kutuların ortalama uzaklığı
                
                // Kutu oluştur - Referans görüntüye göre küçük kare kutular
                Rectangle dirMark = new Rectangle();
                dirMark.Width = 12;
                dirMark.Height = 12;
                
                // Her 30 derecede bir turkuaz, diğerleri gri
                if (angle % 30 == 0)
                {
                    dirMark.Fill = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
                }
                else
                {
                    dirMark.Fill = new SolidColorBrush(Color.FromRgb(220, 220, 220)); // Gri
                }
                
                double markX = centerX + Math.Sin(radians) * boxRadius - 6; // 6 = genişlik/2
                double markY = centerY - Math.Cos(radians) * boxRadius - 6; // 6 = yükseklik/2
                
                Canvas.SetLeft(dirMark, markX);
                Canvas.SetTop(dirMark, markY);
                
                directionMarksCanvas.Children.Add(dirMark);
            }
            
            // 10 derecelik aralıklarla değil, sadece 30 derecelik aralıklarla rakamlar göster
            for (int angle = 0; angle < 360; angle += 30)
            {
                double radians = angle * Math.PI / 180;
                double centerX = 120;
                double centerY = 120;
                double textRadius = 120; // Rakamlar biraz daha dışta
                
                // Açı etiketleri (03, 06, 09, ... şeklinde her 30 derecede bir)
                // 360=00, 30=03, 60=06, ...
                int dirValue = angle / 10;
                string dirLabel = (dirValue % 36).ToString("00");
                if (dirLabel == "00" && angle == 0)
                {
                    dirLabel = "00"; // Kuzeyi özel olarak belirt
                }
                
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
            
            // Pist göstergesi - İkinci referans görüntüye göre düzeltildi
            // Ekran görüntüsündeki pist işareti (gri dikdörtgen üzerinde 3 beyaz dikdörtgen)
            Grid runwayGrid = new Grid();
            runwayGrid.Width = 30;
            runwayGrid.Height = 70;
            runwayGrid.HorizontalAlignment = HorizontalAlignment.Center;
            runwayGrid.VerticalAlignment = VerticalAlignment.Center;
            
            // Ana pist dikdörtgeni (gri)
            Rectangle runwayBase = new Rectangle();
            runwayBase.Width = 30;
            runwayBase.Height = 70;
            runwayBase.Fill = Brushes.Gray;
            
            // Pist üzerindeki 3 beyaz dikdörtgen çizgisi (sağ kenarında)
            Rectangle runway1 = new Rectangle();
            runway1.Width = 12;
            runway1.Height = 6;
            runway1.Fill = Brushes.White;
            runway1.VerticalAlignment = VerticalAlignment.Top;
            runway1.HorizontalAlignment = HorizontalAlignment.Right;
            runway1.Margin = new Thickness(0, 13, 3, 0);
            
            Rectangle runway2 = new Rectangle();
            runway2.Width = 12;
            runway2.Height = 6;
            runway2.Fill = Brushes.White;
            runway2.VerticalAlignment = VerticalAlignment.Center;
            runway2.HorizontalAlignment = HorizontalAlignment.Right;
            runway2.Margin = new Thickness(0, 0, 3, 0);
            
            Rectangle runway3 = new Rectangle();
            runway3.Width = 12;
            runway3.Height = 6;
            runway3.Fill = Brushes.White;
            runway3.VerticalAlignment = VerticalAlignment.Bottom;
            runway3.HorizontalAlignment = HorizontalAlignment.Right;
            runway3.Margin = new Thickness(0, 0, 3, 13);
            
            runwayGrid.Children.Add(runwayBase);
            runwayGrid.Children.Add(runway1);
            runwayGrid.Children.Add(runway2);
            runwayGrid.Children.Add(runway3);
            
            // Pist yönüne göre döndür
            TransformGroup runwayTransform = new TransformGroup();
            double runwayDirection = _runwayName == "RWY 35" ? 0 : 180; // RWY 35 ve RWY 17 için farklı yönler
            runwayTransform.Children.Add(new RotateTransform(runwayDirection));
            runwayGrid.RenderTransform = runwayTransform;
            runwayGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            
            windGrid.Children.Add(runwayGrid);
            
            // Rüzgar oku - İkinci referans görüntüye göre daha doğru şekilde düzeltildi
            // Daha çok bir ok gibi oluşturuyoruz
            Grid arrowGrid = new Grid();
            arrowGrid.Width = 24;
            arrowGrid.Height = 70;
            arrowGrid.HorizontalAlignment = HorizontalAlignment.Center;
            arrowGrid.VerticalAlignment = VerticalAlignment.Center;
            
            // Ok başı (üçgen)
            Polygon arrowHead = new Polygon();
            arrowHead.Points = new PointCollection
            {
                new Point(12, 0),   // Üst orta
                new Point(20, 15),  // Sağ alt
                new Point(4, 15)    // Sol alt
            };
            arrowHead.Fill = Brushes.Black;
            arrowHead.VerticalAlignment = VerticalAlignment.Top;
            arrowHead.HorizontalAlignment = HorizontalAlignment.Center;
            
            // Ok gövdesi (dikdörtgen)
            _windArrow = new Rectangle();
            _windArrow.Width = 8;
            _windArrow.Height = 55;
            _windArrow.Fill = Brushes.Black;
            _windArrow.VerticalAlignment = VerticalAlignment.Bottom;
            _windArrow.HorizontalAlignment = HorizontalAlignment.Center;
            
            arrowGrid.Children.Add(_windArrow);
            arrowGrid.Children.Add(arrowHead);
            
            // Dönüş transformu
            TransformGroup transformGroup = new TransformGroup();
            RotateTransform rotateTransform = new RotateTransform(210);  // Rüzgar yönü - başlangıç değeri
            transformGroup.Children.Add(rotateTransform);
            arrowGrid.RenderTransform = transformGroup;
            arrowGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            
            // Ok adı (okun güncellenmesi için)
            string arrowName = _runwayName == "RWY 35" ? "LeftWindArrow" : "RightWindArrow";
            _owner.RegisterName(arrowName, arrowGrid);
            
            windGrid.Children.Add(arrowGrid);
            
            // Rüzgar hızı göstergesi - ortadaki sayı beyaz kutu içinde siyah kenarlıklı
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
            
            // Border ekle - Veri bölümünden ayırmak için alt kenar
            Border separatorBorder = new Border();
            separatorBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            separatorBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            separatorBorder.Height = 1;
            separatorBorder.VerticalAlignment = VerticalAlignment.Bottom;
            
            windGrid.Children.Add(separatorBorder);
            
            return windGrid;
        }
        
        /// <summary>
        /// Rüzgar yönünü günceller
        /// </summary>
        public void UpdateWindDirection(double direction)
        {
            // Ok adını temel alarak ok gridini bul
            string arrowName = _runwayName == "RWY 35" ? "LeftWindArrow" : "RightWindArrow";
            var arrowGrid = _owner.FindName(arrowName) as FrameworkElement;
            
            if (arrowGrid != null)
            {
                // Transform grubunu al
                TransformGroup group = arrowGrid.RenderTransform as TransformGroup;
                if (group != null && group.Children.Count > 0)
                {
                    // İlk transform (RotateTransform) dönme olmalı
                    RotateTransform rotate = group.Children[0] as RotateTransform;
                    if (rotate != null)
                    {
                        // Önemli: Ok, rüzgarın geldiği yönü göstermeli (FROM)
                        // Bu, meteorolojide standart gösterimdir
                        rotate.Angle = direction;
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
    }
}