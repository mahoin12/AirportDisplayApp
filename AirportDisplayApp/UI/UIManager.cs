using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportDisplayApp.UI
{
    /// <summary>
    /// UI Yöneticisi - Tüm UI bileşenlerini oluşturur ve koordine eder
    /// </summary>
    public class UIManager
    {
        private readonly Window _owner;
        private readonly Dictionary<string, TextBlock> _displayElements;
        private readonly ResourceDictionary _styles;
        
        // UI Bileşenleri
        private HeaderComponent _headerComponent;
        private TabBarComponent _tabBarComponent;
        private StatusBarComponent _statusBarComponent;
        private RunwayPanelComponent _leftRunwayComponent;
        private CenterPanelComponent _centerPanelComponent;
        private RunwayPanelComponent _rightRunwayComponent;
        private MetarPanelComponent _metarPanelComponent;
        private FooterComponent _footerComponent;
        
        // Ana Grid
        private Grid _mainGrid;

        public UIManager(Window owner)
        {
            _owner = owner;
            _displayElements = new Dictionary<string, TextBlock>();
            _styles = LoadStyles();
            
            // UI bileşenlerini oluştur
            CreateComponents();
        }
        
        /// <summary>
        /// Stil dosyasını yükler
        /// </summary>
        private ResourceDictionary LoadStyles()
        {
            try
            {
                // Styles.xaml'ı pack URI formatında yükle
                ResourceDictionary styles = new ResourceDictionary();
                styles.Source = new Uri("/AirportDisplayApp;component/Styles/Styles.xaml", UriKind.RelativeOrAbsolute);
                _owner.Resources.MergedDictionaries.Add(styles);
                return styles;
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılan stilleri kullan
                MessageBox.Show($"Stil dosyası yüklenemedi: {ex.Message}\nVarsayılan stiller kullanılacak.",
                    "Stil Yükleme Hatası",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                // Varsayılan stil sözlüğü oluştur
                return CreateDefaultStyles();
            }
        }
        
        /// <summary>
        /// Stillerin yüklenemediği durumda varsayılan stilleri oluşturur
        /// </summary>
        private ResourceDictionary CreateDefaultStyles()
        {
            ResourceDictionary styles = new ResourceDictionary();

            // Temel renk tanımları - Referans görüntüye göre mavi/turkuaz tonları
            styles["HeaderBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
            styles["DefaultBackgroundBrush"] = new SolidColorBrush(Colors.WhiteSmoke);
            styles["BorderBrush"] = new SolidColorBrush(Colors.LightGray);
            styles["ValueTextBrush"] = new SolidColorBrush(Colors.Black);
            styles["LabelTextBrush"] = new SolidColorBrush(Color.FromRgb(68, 68, 68));
            styles["HeaderTextBrush"] = new SolidColorBrush(Colors.White);
            styles["RunwayHighlightBrush"] = new SolidColorBrush(Color.FromRgb(0, 156, 178)); // Turkuaz
            styles["WindSpeedBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // Beyaz arka plan

            // Temel metin stilleri
            Style headerTextStyle = new Style(typeof(TextBlock));
            headerTextStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 20.0));
            headerTextStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            headerTextStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, styles["HeaderTextBrush"]));
            headerTextStyle.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            headerTextStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            styles["HeaderTextStyle"] = headerTextStyle;

            return styles;
        }
        
        /// <summary>
        /// Tüm UI bileşenlerini oluşturur
        /// </summary>
        private void CreateComponents()
        {
            // Bileşenleri oluştur
            _headerComponent = new HeaderComponent(_owner, _displayElements, _styles);
            _tabBarComponent = new TabBarComponent(_owner, _displayElements, _styles);
            _statusBarComponent = new StatusBarComponent(_owner, _displayElements, _styles);
            _leftRunwayComponent = new RunwayPanelComponent(_owner, _displayElements, _styles, "RWY 35");
            _centerPanelComponent = new CenterPanelComponent(_owner, _displayElements, _styles);
            _rightRunwayComponent = new RunwayPanelComponent(_owner, _displayElements, _styles, "RWY 17");
            _metarPanelComponent = new MetarPanelComponent(_owner, _displayElements, _styles);
            _footerComponent = new FooterComponent(_owner, _displayElements, _styles);
            
            // Buton event'lerini bağla
            _footerComponent.RefreshClicked += (s, e) => OnRefreshRequested?.Invoke(this, EventArgs.Empty);
            _footerComponent.SettingsClicked += (s, e) => OnSettingsRequested?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Ana UI'ı oluşturur ve bileşenleri ekler
        /// </summary>
        public void BuildUI()
        {
            // Ana Grid'i oluştur
            _mainGrid = CreateMainGrid();
            _owner.Content = _mainGrid;
            
            // Bileşenleri ekle
            _headerComponent.Create(_mainGrid, 0, 0);
            _tabBarComponent.Create(_mainGrid, 1, 0);
            _statusBarComponent.Create(_mainGrid, 2, 0);
            
            // İçerik paneli
            Grid contentGrid = CreateContentPanel();
            Grid.SetRow(contentGrid, 3);
            _mainGrid.Children.Add(contentGrid);
            
            // Pist panelleri ve merkez panel
            _leftRunwayComponent.Create(contentGrid, 0, 0);
            _centerPanelComponent.Create(contentGrid, 0, 1);
            _rightRunwayComponent.Create(contentGrid, 0, 2);
            
            // METAR ve alt bilgi
            _metarPanelComponent.Create(_mainGrid, 4, 0);
            _footerComponent.Create(_mainGrid, 5, 0);
        }
        
        /// <summary>
        /// Ana Grid'i oluşturur
        /// </summary>
        private Grid CreateMainGrid()
        {
            Grid mainGrid = new Grid();
            mainGrid.Background = (SolidColorBrush)_styles["DefaultBackgroundBrush"];
            
            // Ana satır tanımları
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });  // Başlık
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });  // Tab Bar
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });  // Durum Çubuğu
            mainGrid.RowDefinitions.Add(new RowDefinition());                               // Ana İçerik
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });  // METAR
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });  // Alt Bilgi
            
            return mainGrid;
        }
        
        /// <summary>
        /// İçerik paneli Grid'ini oluşturur
        /// </summary>
        private Grid CreateContentPanel()
        {
            Grid contentGrid = new Grid();
            contentGrid.Margin = new Thickness(5);
            
            // 3 sütunlu düzen
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());  // Sol Pist (RWY 35)
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());  // Merkez Panel
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());  // Sağ Pist (RWY 17)
            
            return contentGrid;
        }
        
        /// <summary>
        /// Görüntüleme elemanını günceller
        /// </summary>
        public void UpdateDisplayElement(string key, string value)
        {
            if (_displayElements.TryGetValue(key, out TextBlock element))
            {
                element.Text = value;
            }
        }
        
        /// <summary>
        /// Rüzgar yönü okunu günceller
        /// </summary>
        public void UpdateWindDirection(string runway, double direction)
        {
            string arrowName = runway == "RWY 35" ? "LeftWindArrow" : "RightWindArrow";
            
            // RunwayPanelComponent'e yönlendir
            if (runway == "RWY 35")
            {
                _leftRunwayComponent.UpdateWindDirection(direction);
            }
            else
            {
                _rightRunwayComponent.UpdateWindDirection(direction);
            }
        }
        
        /// <summary>
        /// Bağlantı durumunu günceller
        /// </summary>
        public void UpdateConnectionStatus(string status)
        {
            _footerComponent.UpdateConnectionStatus(status);
        }
        
        // Event'ler
        public event EventHandler OnRefreshRequested;
        public event EventHandler OnSettingsRequested;
    }
}