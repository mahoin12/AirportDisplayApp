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
            
            // Rüzgar yönü için tick marks (30 derece aralıklarla)
            Canvas directionMarksCanvas = new Canvas();
            directionMarksCanvas.Width = 240;
            directionMarksCanvas.Height = 240;
            directionMarksCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            directionMarksCanvas.VerticalAlignment = VerticalAlignment.Center;
            
            // 30 derecelik aralıklarla çizgiler ve rakamlar
            for (int angle = 0; angle < 360; angle += 30)
            {
                double radians = angle * Math.PI / 180;
                double centerX = 120;
                double centerY = 120;
                
                // İç çemberin ve dış çemberin yarıçapları
                double innerRadius = 90;
                double outerRadius = 110;
                double textRadius = 120;
                
                // Tick çizgisi
                Line tickLine = new Line();
                tickLine.X1 = centerX + Math.Sin(radians) * innerRadius;
                tickLine.Y1 = centerY - Math.Cos(radians) * innerRadius;
                tickLine.X2 = centerX + Math.Sin(radians) * outerRadius;
                tickLine.Y2 = centerY - Math.Cos(radians) * outerRadius;
                tickLine.Stroke = Brushes.LightGray;
                tickLine.StrokeThickness = 1;
                
                directionMarksCanvas.Children.Add(tickLine);
                
                // Açı etiketleri (03, 06, 09, ... şeklinde her 30 derecede bir)
                string dirLabel = ((angle / 10) % 36).ToString("00");
                TextBlock dirText = new TextBlock();
                dirText.Text = dirLabel;
                dirText.FontSize = 12;
                
                // Metin pozisyonu (dairenin dışında)
                double textX = centerX + Math.Sin(radians) * textRadius - 8;
                double textY = centerY - Math.Cos(radians) * textRadius - 8;
                
                Canvas.SetLeft(dirText, textX);
                Canvas.SetTop(dirText, textY);
                
                directionMarksCanvas.Children.Add(dirText);
                
                // Turkuaz renkli işaretler - resme göre düzeltildi
                if (angle % 90 == 0 || angle % 30 == 0)
                {
                    // Ana yön ve ara yönlerin farklı gösterimle ayırt edilmesi
                    if (angle % 90 == 0)
                    {
                        // Ana yönler (00, 09, 18, 27) için turkuaz kare
                        Rectangle dirMark = new Rectangle();
                        dirMark.Width = 8;
                        dirMark.Height = 8;
                        dirMark.Fill = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
                        
                        double markX = centerX + Math.Sin(radians) * 80 - 4;
                        double markY = centerY - Math.Cos(radians) * 80 - 4;
                        
                        Canvas.SetLeft(dirMark, markX);
                        Canvas.SetTop(dirMark, markY);
                        
                        directionMarksCanvas.Children.Add(dirMark);
                    }
                    else
                    {
                        // Ara yönler (03, 06, 12, 15, 21, 24, 30, 33) için turkuaz daire
                        Ellipse dirMark = new Ellipse();
                        dirMark.Width = 8;
                        dirMark.Height = 8;
                        dirMark.Fill = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
                        
                        double markX = centerX + Math.Sin(radians) * 80 - 4;
                        double markY = centerY - Math.Cos(radians) * 80 - 4;
                        
                        Canvas.SetLeft(dirMark, markX);
                        Canvas.SetTop(dirMark, markY);
                        
                        directionMarksCanvas.Children.Add(dirMark);
                    }
                }
            }
            
            windGrid.Children.Add(directionMarksCanvas);
            
            // Pist göstergesi - resme göre düzeltildi (uzun, gri çubuk)
            Rectangle runwayIndicator = new Rectangle();
            runwayIndicator.Width = 20;
            runwayIndicator.Height = 70;
            runwayIndicator.Fill = Brushes.Gray;
            runwayIndicator.HorizontalAlignment = HorizontalAlignment.Center;
            runwayIndicator.VerticalAlignment = VerticalAlignment.Center;
            
            // Pist yönüne göre döndür
            TransformGroup runwayTransform = new TransformGroup();
            double runwayDirection = _runwayName == "RWY 35" ? 0 : 180; // RWY 35 ve RWY 17 için farklı yönler
            runwayTransform.Children.Add(new RotateTransform(runwayDirection));
            runwayIndicator.RenderTransform = runwayTransform;
            runwayIndicator.RenderTransformOrigin = new Point(0.5, 0.5);
            
            windGrid.Children.Add(runwayIndicator);
            
            // Rüzgar oku (gri renkli ok) - Güncel ekran görüntüsüne göre düzeltildi
            _windArrow = new Rectangle();
            _windArrow.Width = 10;  // Daha ince ok
            _windArrow.Height = 80; // Daha uzun ok
            _windArrow.Fill = Brushes.DarkGray;
            _windArrow.HorizontalAlignment = HorizontalAlignment.Center;
            _windArrow.VerticalAlignment = VerticalAlignment.Center;
            _windArrow.RenderTransformOrigin = new Point(0.5, 0.5);  // Dönüş merkezi merkez noktası
            
            // Dönüş transformu
            TransformGroup transformGroup = new TransformGroup();
            RotateTransform rotateTransform = new RotateTransform(210);  // Rüzgar yönü - başlangıç değeri
            transformGroup.Children.Add(rotateTransform);
            _windArrow.RenderTransform = transformGroup;
            
            // Ok adı (okun güncellenmesi için)
            string arrowName = _runwayName == "RWY 35" ? "LeftWindArrow" : "RightWindArrow";
            _owner.RegisterName(arrowName, _windArrow);
            
            windGrid.Children.Add(_windArrow);
            
            // Rüzgar hızı göstergesi - ortadaki sayı siyah kenarlıklı kutu içinde
            Border speedBorder = new Border();
            speedBorder.Background = Brushes.White;
            speedBorder.BorderBrush = Brushes.Black;
            speedBorder.BorderThickness = new Thickness(1);
            speedBorder.Width = 40;
            speedBorder.Height = 30;
            speedBorder.HorizontalAlignment = HorizontalAlignment.Center;
            speedBorder.VerticalAlignment = VerticalAlignment.Center;
            
            _speedText = new TextBlock();
            _speedText.Text = _runwayName == "RWY 35" ? "8" : "8";  // Ekran görüntüsündeki değerlere uygun
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
            if (_windArrow != null)
            {
                // Transform grubunu al
                TransformGroup group = _windArrow.RenderTransform as TransformGroup;
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