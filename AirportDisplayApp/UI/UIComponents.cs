// UIComponents.cs - Modüler UI Bileşenleri
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// Yeniden kullanılabilir UI bileşenleri için ana sınıf
    /// </summary>
    public class UIComponents
    {
        private readonly Window _owner;
        private Dictionary<string, TextBlock> _displayElements;
        private ResourceDictionary _styles;

        public UIComponents(Window owner, Dictionary<string, TextBlock> displayElements)
        {
            _owner = owner;
            _displayElements = displayElements;
            LoadStyles();
        }

        // UIComponents.cs içindeki LoadStyles metodu düzeltildi

        private void LoadStyles()
        {
            try
            {
                // Styles.xaml'ı pack URI formatında yükle
                _styles = new ResourceDictionary();
                _styles.Source = new Uri("/AirportDisplayApp;component/Styles/Styles.xaml", UriKind.RelativeOrAbsolute);
                _owner.Resources.MergedDictionaries.Add(_styles);
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılan stilleri kullan
                MessageBox.Show($"Stil dosyası yüklenemedi: {ex.Message}\nVarsayılan stiller kullanılacak.",
                    "Stil Yükleme Hatası",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                // Varsayılan stil sözlüğü oluştur
                _styles = new ResourceDictionary();

                // Temel renk tanımları
                _styles["HeaderBackgroundBrush"] = new SolidColorBrush(Colors.DodgerBlue);
                _styles["DefaultBackgroundBrush"] = new SolidColorBrush(Colors.WhiteSmoke);
                _styles["BorderBrush"] = new SolidColorBrush(Colors.LightGray);
                _styles["ValueTextBrush"] = new SolidColorBrush(Colors.Black);
                _styles["LabelTextBrush"] = new SolidColorBrush(Color.FromRgb(68, 68, 68));
                _styles["HeaderTextBrush"] = new SolidColorBrush(Colors.White);
                _styles["RunwayHighlightBrush"] = new SolidColorBrush(Colors.DodgerBlue);
                _styles["WindSpeedBrush"] = new SolidColorBrush(Color.FromRgb(255, 236, 236));

                // Temel stil tanımları
                Style headerTextStyle = new Style(typeof(TextBlock));
                headerTextStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 20.0));
                headerTextStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
                headerTextStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, _styles["HeaderTextBrush"]));
                headerTextStyle.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
                headerTextStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
                _styles["HeaderTextStyle"] = headerTextStyle;

                // Diğer stiller de benzer şekilde buraya eklenebilir...
            }
        }

        #region Ana UI Bileşenleri

        public Grid CreateMainLayout()
        {
            // Ana Grid - tüm uygulamanın container'ı
            Grid mainGrid = new Grid();
            mainGrid.Style = _styles["MainGridStyle"] as Style;

            // Ana satır tanımları
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) }); // Başlık
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) }); // Tab Bar
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) }); // Durum Çubuğu
            mainGrid.RowDefinitions.Add(new RowDefinition()); // Ana İçerik
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) }); // METAR
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) }); // Alt Bilgi

            return mainGrid;
        }

        public Border CreateHeader(Grid parent)
        {
            Border headerBorder = new Border();
            headerBorder.Style = _styles["HeaderBorderStyle"] as Style;

            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Sol taraf - VAISALA logosu/yazısı
            TextBlock vaisalaText = new TextBlock();
            vaisalaText.Text = "VAISALA";
            vaisalaText.Style = _styles["HeaderTextStyle"] as Style;
            vaisalaText.HorizontalAlignment = HorizontalAlignment.Left;
            vaisalaText.Margin = new Thickness(20, 0, 0, 0);

            // Orta kısım - Havaalanı kodu
            TextBlock airportText = new TextBlock();
            airportText.Text = "LTBL";
            airportText.Style = _styles["HeaderTextStyle"] as Style;

            // Sağ taraf - Saat göstergesi
            TextBlock timeText = new TextBlock();
            timeText.Text = DateTime.Now.ToString("HH:mm:ss");
            timeText.Style = _styles["TimeDisplayStyle"] as Style;
            timeText.Name = "TimeDisplay";
            _owner.RegisterName("TimeDisplay", timeText);
            _displayElements["time"] = timeText;

            Grid.SetColumn(vaisalaText, 0);
            Grid.SetColumn(airportText, 1);
            Grid.SetColumn(timeText, 2);

            headerGrid.Children.Add(vaisalaText);
            headerGrid.Children.Add(airportText);
            headerGrid.Children.Add(timeText);

            headerBorder.Child = headerGrid;
            Grid.SetRow(headerBorder, 0);
            parent.Children.Add(headerBorder);

            return headerBorder;
        }

        public Border CreateTabBar(Grid parent)
        {
            Border tabBarBorder = new Border();
            tabBarBorder.Background = new SolidColorBrush(Colors.WhiteSmoke);
            tabBarBorder.BorderBrush = new SolidColorBrush(Colors.LightGray);
            tabBarBorder.BorderThickness = new Thickness(0, 0, 0, 1);

            // Tab butonları için yatay StackPanel
            StackPanel tabPanel = new StackPanel();
            tabPanel.Orientation = Orientation.Horizontal;
            tabPanel.HorizontalAlignment = HorizontalAlignment.Left;
            tabPanel.Margin = new Thickness(10, 0, 0, 0);

            // Aktif tab
            Button activeTab = new Button();
            activeTab.Content = "RWY 17-35 - mps";
            activeTab.Style = _styles["ActiveTabButtonStyle"] as Style;

            // Diğer tablar
            Button reportsTab = new Button();
            reportsTab.Content = "Reports";
            reportsTab.Style = _styles["TabButtonStyle"] as Style;
            reportsTab.Margin = new Thickness(5, 0, 0, 0);

            tabPanel.Children.Add(activeTab);
            tabPanel.Children.Add(reportsTab);
            tabBarBorder.Child = tabPanel;

            Grid.SetRow(tabBarBorder, 1);
            parent.Children.Add(tabBarBorder);

            return tabBarBorder;
        }

        public Border CreateStatusBar(Grid parent)
        {
            Border statusBorder = new Border();
            statusBorder.Style = _styles["StatusBarBorderStyle"] as Style;

            Grid statusGrid = new Grid();
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition());
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // RWY IN USE metni
            TextBlock rwyInUseLabel = new TextBlock();
            rwyInUseLabel.Text = "RWY IN USE";
            rwyInUseLabel.Style = _styles["LabelTextStyle"] as Style;
            rwyInUseLabel.HorizontalAlignment = HorizontalAlignment.Right;
            rwyInUseLabel.Margin = new Thickness(0, 0, 10, 0);
            rwyInUseLabel.FontWeight = FontWeights.Bold;

            TextBlock rwyInUseValue = new TextBlock();
            rwyInUseValue.Text = "35";
            rwyInUseValue.Style = _styles["ValueTextStyle"] as Style;
            rwyInUseValue.Foreground = Brushes.DarkRed;
            rwyInUseValue.FontWeight = FontWeights.Bold;
            rwyInUseValue.Name = "RwyInUseValue";
            _owner.RegisterName("RwyInUseValue", rwyInUseValue);
            _displayElements["runwayInUse"] = rwyInUseValue;

            Grid.SetColumn(rwyInUseLabel, 1);
            Grid.SetColumn(rwyInUseValue, 2);

            statusGrid.Children.Add(rwyInUseLabel);
            statusGrid.Children.Add(rwyInUseValue);

            statusBorder.Child = statusGrid;

            Grid.SetRow(statusBorder, 2);
            parent.Children.Add(statusBorder);

            return statusBorder;
        }

        public Grid CreateContentPanel(Grid parent)
        {
            Grid contentGrid = new Grid();
            contentGrid.Margin = new Thickness(0, 5, 0, 5);

            // 3 Sütunlu düzen: Sol Pist, Orta Panel, Sağ Pist
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Grid.SetRow(contentGrid, 3);
            parent.Children.Add(contentGrid);

            return contentGrid;
        }

        public Border CreateMetarPanel(Grid parent)
        {
            Border metarBorder = new Border();
            metarBorder.Style = _styles["MetarBorderStyle"] as Style;

            Grid metarGrid = new Grid();
            metarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            metarGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock metarLabel = new TextBlock();
            metarLabel.Text = "METAR";
            metarLabel.FontWeight = FontWeights.Bold;
            metarLabel.Margin = new Thickness(5);
            metarLabel.VerticalAlignment = VerticalAlignment.Center;

            TextBlock metarText = new TextBlock();
            metarText.Style = _styles["MetarTextStyle"] as Style;
            metarText.Text = "METAR LTBL 060950Z 29004KT 240V340 9999 FEW035 SCT100 31/20 Q1013 NOSIG RMK RWY17 28006KT 230V320";
            metarText.Name = "MetarText";
            _owner.RegisterName("MetarText", metarText);
            _displayElements["metar"] = metarText;

            Grid.SetColumn(metarLabel, 0);
            Grid.SetColumn(metarText, 1);

            metarGrid.Children.Add(metarLabel);
            metarGrid.Children.Add(metarText);

            metarBorder.Child = metarGrid;

            Grid.SetRow(metarBorder, 4);
            parent.Children.Add(metarBorder);

            return metarBorder;
        }

        public Border CreateFooter(Grid parent)
        {
            Border footerBorder = new Border();
            footerBorder.Style = _styles["FooterBorderStyle"] as Style;

            Grid footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Sol taraf - Bağlantı durumu
            TextBlock connectionStatus = new TextBlock();
            connectionStatus.Text = "Bağlantı Durumu: Bağlanıyor...";
            connectionStatus.Style = _styles["StatusTextStyle"] as Style;
            connectionStatus.Name = "ConnectionStatus";
            _owner.RegisterName("ConnectionStatus", connectionStatus);

            // Sağ taraf - Butonlar
            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Right;
            buttonPanel.Margin = new Thickness(0, 0, 10, 0);

            Button refreshButton = new Button();
            refreshButton.Content = "Verileri Yenile";
            refreshButton.Style = _styles["StandardButtonStyle"] as Style;

            Button settingsButton = new Button();
            settingsButton.Content = "Ayarlar";
            settingsButton.Style = _styles["StandardButtonStyle"] as Style;

            buttonPanel.Children.Add(refreshButton);
            buttonPanel.Children.Add(settingsButton);

            Grid.SetColumn(connectionStatus, 0);
            Grid.SetColumn(buttonPanel, 1);

            footerGrid.Children.Add(connectionStatus);
            footerGrid.Children.Add(buttonPanel);

            footerBorder.Child = footerGrid;

            Grid.SetRow(footerBorder, 5);
            parent.Children.Add(footerBorder);

            return footerBorder;
        }

        #endregion

        #region Pist Panelleri

        public Border CreateRunwayPanel(Grid parent, int column, string runwayName)
        {
            Border runwayBorder = new Border();
            runwayBorder.Style = _styles["RunwayPanelBorderStyle"] as Style;

            Grid runwayGrid = new Grid();
            runwayGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Başlık
            runwayGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) }); // Rüzgar göstergesi
            runwayGrid.RowDefinitions.Add(new RowDefinition()); // Veri tablosu

            // Pist başlığı
            Border headerBorder = new Border();
            headerBorder.Style = _styles["RunwayHeaderStyle"] as Style;

            TextBlock headerText = new TextBlock();
            headerText.Text = runwayName;
            headerText.Style = _styles["SubHeaderTextStyle"] as Style;
            headerText.Foreground = Brushes.Black;
            headerBorder.Child = headerText;

            // Rüzgar göstergesi paneli
            Canvas windCanvas = CreateWindIndicator(runwayName);

            // Veri tablosu
            ScrollViewer dataScrollViewer = new ScrollViewer();
            dataScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            Grid dataGrid = CreateRunwayDataGrid(runwayName);
            dataScrollViewer.Content = dataGrid;

            Grid.SetRow(headerBorder, 0);
            Grid.SetRow(windCanvas, 1);
            Grid.SetRow(dataScrollViewer, 2);

            runwayGrid.Children.Add(headerBorder);
            runwayGrid.Children.Add(windCanvas);
            runwayGrid.Children.Add(dataScrollViewer);

            runwayBorder.Child = runwayGrid;

            Grid.SetColumn(runwayBorder, column);
            parent.Children.Add(runwayBorder);

            return runwayBorder;
        }

        private Canvas CreateWindIndicator(string runwayName)
        {
            // Ana konteyner (canvas)
            Canvas windCanvas = new Canvas();
            windCanvas.Width = 290;
            windCanvas.Height = 290;
            windCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            windCanvas.VerticalAlignment = VerticalAlignment.Center;

            // Dış daire (wind rose)
            Ellipse outerCircle = new Ellipse();
            outerCircle.Style = _styles["WindIndicatorCircleStyle"] as Style;
            Canvas.SetLeft(outerCircle, 15);
            Canvas.SetTop(outerCircle, 15);
            windCanvas.Children.Add(outerCircle);

            // İç nokta (merkez belirteci)
            Ellipse innerCircle = new Ellipse();
            innerCircle.Width = 10;
            innerCircle.Height = 10;
            innerCircle.Fill = Brushes.Gray;
            Canvas.SetLeft(innerCircle, 145);
            Canvas.SetTop(innerCircle, 145);
            windCanvas.Children.Add(innerCircle);

            // Yön işaretleri (her 30 derecede bir)
            for (int i = 0; i < 360; i += 30)
            {
                double radians = i * Math.PI / 180;
                double centerX = 145;
                double centerY = 145;
                double radius = 130;

                // Açıyı ve markı hesapla (havacılık için 360=00, 30=03, vs.)
                string directionMark = ((i / 10) % 36).ToString("00");
                double labelRadius = radius + 15;

                // Etiket metni
                TextBlock markText = new TextBlock();
                markText.Text = directionMark;
                markText.FontSize = 12;
                markText.Foreground = Brushes.Black;

                // Çizgi
                Line tickLine = new Line();
                tickLine.X1 = centerX + Math.Sin(radians) * radius;
                tickLine.Y1 = centerY - Math.Cos(radians) * radius;
                tickLine.X2 = centerX + Math.Sin(radians) * (radius - 15);
                tickLine.Y2 = centerY - Math.Cos(radians) * (radius - 15);
                tickLine.Stroke = Brushes.LightGray;
                tickLine.StrokeThickness = 1;
                windCanvas.Children.Add(tickLine);

                // Etiketi yerleştir
                double textX = centerX + Math.Sin(radians) * labelRadius - 10;
                double textY = centerY - Math.Cos(radians) * labelRadius - 8;
                Canvas.SetLeft(markText, textX);
                Canvas.SetTop(markText, textY);
                windCanvas.Children.Add(markText);

                // Her 90 derecede kalın gösterge ekle (N, E, S, W)
                if (i % 90 == 0)
                {
                    Line mainLine = new Line();
                    mainLine.X1 = centerX + Math.Sin(radians) * radius;
                    mainLine.Y1 = centerY - Math.Cos(radians) * radius;
                    mainLine.X2 = centerX + Math.Sin(radians) * (radius - 25);
                    mainLine.Y2 = centerY - Math.Cos(radians) * (radius - 25);
                    mainLine.Stroke = Brushes.DarkGray;
                    mainLine.StrokeThickness = 2;
                    windCanvas.Children.Add(mainLine);
                }
            }

            // Rüzgar oku
            Rectangle windArrow = new Rectangle();
            windArrow.Style = _styles["WindArrowStyle"] as Style;

            // Transform grubu
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform(0));
            windArrow.RenderTransform = transformGroup;

            // Pist göstergesi (pist yönünü gösteren çizgi)
            Rectangle runwayIndicator = new Rectangle();
            runwayIndicator.Style = _styles["RunwayIndicatorStyle"] as Style;
            runwayIndicator.RenderTransformOrigin = new Point(0.5, 0.5);

            // Pist yönünü ayarla
            TransformGroup runwayTransform = new TransformGroup();
            double runwayDirection = runwayName == "RWY 35" ? 0 : 180; // RWY 35 ve RWY 17
            runwayTransform.Children.Add(new RotateTransform(runwayDirection));
            runwayIndicator.RenderTransform = runwayTransform;

            // Rüzgar hızı göstergesi
            Border speedBorder = new Border();
            speedBorder.Style = _styles["WindValueBoxStyle"] as Style;

            TextBlock speedText = new TextBlock();
            speedText.Text = runwayName == "RWY 35" ? "7" : "5";
            speedText.FontSize = 16;
            speedText.FontWeight = FontWeights.Bold;
            speedText.Foreground = Brushes.DarkRed;
            speedText.HorizontalAlignment = HorizontalAlignment.Center;
            speedText.VerticalAlignment = VerticalAlignment.Center;

            string speedKey = runwayName == "RWY 35" ? "leftWindSpeed" : "rightWindSpeed";
            string windArrowName = runwayName == "RWY 35" ? "LeftWindArrow" : "RightWindArrow";

            speedText.Name = speedKey + "Value";
            _owner.RegisterName(speedText.Name, speedText);
            _displayElements[speedKey] = speedText;

            _owner.RegisterName(windArrowName, windArrow);

            speedBorder.Child = speedText;

            // Elemanları canvas'a ekle
            Canvas.SetLeft(windArrow, 139);
            Canvas.SetTop(windArrow, 65);

            Canvas.SetLeft(runwayIndicator, 139);
            Canvas.SetTop(runwayIndicator, 145);

            Canvas.SetLeft(speedBorder, 125);
            Canvas.SetTop(speedBorder, 175);

            windCanvas.Children.Add(windArrow);
            windCanvas.Children.Add(runwayIndicator);
            windCanvas.Children.Add(speedBorder);

            return windCanvas;
        }

        private Grid CreateRunwayDataGrid(string runwayName)
        {
            // Veriler
            string prefix = runwayName == "RWY 35" ? "left" : "right";

            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(10);

            // Satır tanımları
            for (int i = 0; i < 9; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            }

            // 2" Mnm
            CreateDataRow(dataGrid, 0, "2\" Mnm", "250", "3", prefix + "2MinDir", prefix + "2MinSpeed");

            // 2" Avg
            string avgDir = runwayName == "RWY 35" ? "290" : "270";
            CreateDataRow(dataGrid, 1, "2\" Avg", avgDir, "5", prefix + "2AvgDir", prefix + "2AvgSpeed");

            // 2" Max
            string maxSpeed = runwayName == "RWY 35" ? "8" : "6";
            CreateDataRow(dataGrid, 2, "2\" Max", "320", maxSpeed, prefix + "2MaxDir", prefix + "2MaxSpeed");

            // 2" Hw/Cw
            string hwCwValue = runwayName == "RWY 35" ? "H02   L05" : "T01   R05";
            CreateHwCwRow(dataGrid, 3, "Hw/Cw", hwCwValue, prefix + "HwCw");

            // Base sadece sol pist için
            if (runwayName == "RWY 35")
            {
                CreateBaseRow(dataGrid, 4, "Base", "NCD", "baseValue");
            }

            // 10" Mnm
            string minDir = runwayName == "RWY 35" ? "* CALM" : "220";
            string minSpeed = runwayName == "RWY 35" ? "" : "2";
            CreateDataRow(dataGrid, 5, "10\" Mnm", minDir, minSpeed, prefix + "10MinDir", prefix + "10MinSpeed");

            // 10" Avg
            string avg10Dir = runwayName == "RWY 35" ? "280" : "290";
            CreateDataRow(dataGrid, 6, "10\" Avg", avg10Dir, "5", prefix + "10AvgDir", prefix + "10AvgSpeed");

            // 10" Max
            string max10Speed = runwayName == "RWY 35" ? "9" : "8";
            CreateDataRow(dataGrid, 7, "10\" Max", "320", max10Speed, prefix + "10MaxDir", prefix + "10MaxSpeed");

            // QFE
            string qfeValue = runwayName == "RWY 35" ? "1012.8" : "1012.7";
            string qfeInHg = runwayName == "RWY 35" ? "29.91" : "29.90";
            CreateQfeRow(dataGrid, 8, "QFE", qfeValue, qfeInHg, prefix + "QfeValue", prefix + "QfeInHg");

            return dataGrid;
        }

        private void CreateDataRow(Grid grid, int row, string label, string direction, string speed,
            string directionKey, string speedKey)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(5, 0, 5, 0);

            // Üç sütunlu düzen
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Yön değeri ve birimi
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Hız değeri ve birimi

            // Etiket
            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.Style = _styles["LabelTextStyle"] as Style;

            // Yön değeri
            TextBlock dirText = new TextBlock();
            dirText.Text = direction;
            dirText.Style = _styles["ValueTextStyle"] as Style;
            dirText.Margin = new Thickness(0, 0, 5, 0);
            dirText.Name = directionKey + "Value";
            _owner.RegisterName(directionKey + "Value", dirText);
            _displayElements[directionKey] = dirText;

            // Yön birimi
            TextBlock dirUnitText = new TextBlock();
            dirUnitText.Text = "°";
            dirUnitText.Style = _styles["UnitTextStyle"] as Style;

            // Hız değeri ve birimi
            TextBlock speedText = null;
            TextBlock speedUnitText = null;

            if (!string.IsNullOrEmpty(speed))
            {
                speedText = new TextBlock();
                speedText.Text = speed;
                speedText.Style = _styles["ValueTextStyle"] as Style;
                speedText.Margin = new Thickness(0, 0, 5, 0);
                speedText.Name = speedKey + "Value";
                _owner.RegisterName(speedKey + "Value", speedText);
                _displayElements[speedKey] = speedText;

                speedUnitText = new TextBlock();
                speedUnitText.Text = "kt";
                speedUnitText.Style = _styles["UnitTextStyle"] as Style;
            }

            // Yön için stack panel
            StackPanel dirPanel = new StackPanel();
            dirPanel.Orientation = Orientation.Horizontal;
            dirPanel.HorizontalAlignment = HorizontalAlignment.Right;
            dirPanel.Children.Add(dirText);
            dirPanel.Children.Add(dirUnitText);

            // Grid'e ekle
            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(dirPanel, 1);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(dirPanel);

            if (speedText != null && speedUnitText != null)
            {
                StackPanel speedPanel = new StackPanel();
                speedPanel.Orientation = Orientation.Horizontal;
                speedPanel.HorizontalAlignment = HorizontalAlignment.Right;
                speedPanel.Children.Add(speedText);
                speedPanel.Children.Add(speedUnitText);

                Grid.SetColumn(speedPanel, 2);
                rowGrid.Children.Add(speedPanel);
            }

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        private void CreateHwCwRow(Grid grid, int row, string label, string value, string valueKey)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(5, 0, 5, 0);

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Değer

            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.Style = _styles["LabelTextStyle"] as Style;

            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.Style = _styles["ValueTextStyle"] as Style;
            valueText.HorizontalAlignment = HorizontalAlignment.Left;
            valueText.Margin = new Thickness(0, 0, 0, 0);
            valueText.Name = valueKey + "Value";
            _owner.RegisterName(valueKey + "Value", valueText);
            _displayElements[valueKey] = valueText;

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        private void CreateBaseRow(Grid grid, int row, string label, string value, string valueKey)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(5, 0, 5, 0);

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Değer
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) }); // Birim

            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.Style = _styles["LabelTextStyle"] as Style;

            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.Style = _styles["ValueTextStyle"] as Style;
            valueText.Name = valueKey + "Value";
            _owner.RegisterName(valueKey + "Value", valueText);
            _displayElements[valueKey] = valueText;

            TextBlock unitText = new TextBlock();
            unitText.Text = "ft";
            unitText.Style = _styles["UnitTextStyle"] as Style;

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);
            Grid.SetColumn(unitText, 2);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);
            rowGrid.Children.Add(unitText);

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        private void CreateQfeRow(Grid grid, int row, string label, string value, string inHgValue,
            string valueKey, string inHgKey)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(5, 0, 5, 0);

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // hPa değeri
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) }); // hPa birimi
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // inHg değeri
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) }); // inHg birimi

            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.Style = _styles["LabelTextStyle"] as Style;

            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.Style = _styles["ValueTextStyle"] as Style;
            valueText.Name = valueKey + "Value";
            _owner.RegisterName(valueKey + "Value", valueText);
            _displayElements[valueKey] = valueText;

            TextBlock unitText = new TextBlock();
            unitText.Text = "hPa";
            unitText.Style = _styles["UnitTextStyle"] as Style;

            TextBlock inHgText = new TextBlock();
            inHgText.Text = inHgValue;
            inHgText.Style = _styles["ValueTextStyle"] as Style;
            inHgText.Name = inHgKey + "Value";
            _owner.RegisterName(inHgKey + "Value", inHgText);
            _displayElements[inHgKey] = inHgText;

            TextBlock inHgUnitText = new TextBlock();
            inHgUnitText.Text = "inHg";
            inHgUnitText.Style = _styles["UnitTextStyle"] as Style;

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);
            Grid.SetColumn(unitText, 2);
            Grid.SetColumn(inHgText, 3);
            Grid.SetColumn(inHgUnitText, 4);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);
            rowGrid.Children.Add(unitText);
            rowGrid.Children.Add(inHgText);
            rowGrid.Children.Add(inHgUnitText);

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        #endregion

        #region Merkez Panel Bileşenleri

        public Border CreateCenterInfoPanel(Grid parent, int column)
        {
            Border centerBorder = new Border();
            centerBorder.Style = _styles["RunwayPanelBorderStyle"] as Style;

            Grid centerGrid = new Grid();
            centerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Başlık
            centerGrid.RowDefinitions.Add(new RowDefinition()); // Veri alanı

            // Başlık
            Border headerBorder = new Border();
            headerBorder.Style = _styles["RunwayHeaderStyle"] as Style;

            TextBlock headerText = new TextBlock();
            headerText.Text = "Çiğli Havaalanı";
            headerText.Style = _styles["SubHeaderTextStyle"] as Style;
            headerText.Foreground = Brushes.Black;
            headerBorder.Child = headerText;

            // Veri paneli
            ScrollViewer dataScrollViewer = new ScrollViewer();
            dataScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            Grid dataGrid = CreateCenterDataGrid();
            dataScrollViewer.Content = dataGrid;

            Grid.SetRow(headerBorder, 0);
            Grid.SetRow(dataScrollViewer, 1);

            centerGrid.Children.Add(headerBorder);
            centerGrid.Children.Add(dataScrollViewer);

            centerBorder.Child = centerGrid;

            Grid.SetColumn(centerBorder, column);
            parent.Children.Add(centerBorder);

            return centerBorder;
        }

        private Grid CreateCenterDataGrid()
        {
            Grid dataGrid = new Grid();
            dataGrid.Margin = new Thickness(10);

            // Satır tanımları
            for (int i = 0; i < 15; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            }

            // Kullanılan Pist
            CreateCenterRow(dataGrid, 0, "RWYINUSE", "35", "", "rwyInUseInfo");

            // QNH
            CreateCenterRow(dataGrid, 1, "QNH", "1013.2", "hPa", "qnh");

            // QNH inHg
            CreateCenterRow(dataGrid, 2, "QNH", "29.92", "inHg", "qnhInHg");

            // QFE
            CreateCenterRow(dataGrid, 3, "QFE", "1013.2", "hPa", "qfe");

            // QFE SYNOP
            CreateCenterRow(dataGrid, 4, "QFE SYNOP", "1012.7", "hPa", "qfeSynop");

            // LOW
            CreateCenterRow(dataGrid, 5, "LOW", "NCD", "", "low");

            // Temp
            CreateCenterRow(dataGrid, 6, "Temp", "30.4", "°C", "temp");

            // Td
            CreateCenterRow(dataGrid, 7, "Td", "19.7", "°C", "td");

            // RH
            CreateCenterRow(dataGrid, 8, "RH", "52", "%", "rh");

            // Tmax
            CreateCenterRow(dataGrid, 9, "Tmax", "30.9", "°C", "tmax");

            // Tmin
            CreateCenterRow(dataGrid, 10, "Tmin", "20.4", "°C", "tmin");

            // Trwy
            CreateCenterRow(dataGrid, 11, "Trwy", "49.9", "°C", "trwy");

            return dataGrid;
        }

        private void CreateCenterRow(Grid grid, int row, string label, string value, string unit, string valueKey)
        {
            Grid rowGrid = new Grid();
            rowGrid.Margin = new Thickness(5, 0, 5, 0);

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Etiket
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Değer
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) }); // Birim

            TextBlock labelText = new TextBlock();
            labelText.Text = label;
            labelText.Style = _styles["LabelTextStyle"] as Style;

            TextBlock valueText = new TextBlock();
            valueText.Text = value;
            valueText.Style = _styles["ValueTextStyle"] as Style;
            valueText.Name = valueKey + "Value";
            _owner.RegisterName(valueKey + "Value", valueText);
            _displayElements[valueKey] = valueText;

            TextBlock unitText = null;
            if (!string.IsNullOrEmpty(unit))
            {
                unitText = new TextBlock();
                unitText.Text = unit;
                unitText.Style = _styles["UnitTextStyle"] as Style;
            }

            Grid.SetColumn(labelText, 0);
            Grid.SetColumn(valueText, 1);

            rowGrid.Children.Add(labelText);
            rowGrid.Children.Add(valueText);

            if (unitText != null)
            {
                Grid.SetColumn(unitText, 2);
                rowGrid.Children.Add(unitText);
            }

            Grid.SetRow(rowGrid, row);
            grid.Children.Add(rowGrid);
        }

        #endregion

        #region Veri Güncelleme Metodları

        public void UpdateWindDirection(string arrowName, double direction)
        {
            // Rüzgar oku nesnesini bul
            object arrowObj = _owner.FindName(arrowName);
            if (arrowObj is Rectangle windArrow)
            {
                // Transform grubunu al
                TransformGroup group = windArrow.RenderTransform as TransformGroup;
                if (group != null && group.Children.Count > 0)
                {
                    // İlk transform dönme olmalı
                    RotateTransform rotate = group.Children[0] as RotateTransform;
                    if (rotate != null)
                    {
                        // Açıyı güncelle
                        rotate.Angle = direction;
                    }
                }
            }
        }

        public void UpdateTextElement(string key, string value)
        {
            if (_displayElements.TryGetValue(key, out TextBlock element))
            {
                element.Text = value;
            }
        }

        #endregion
    }
}