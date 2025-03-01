using System;
using System.Windows;

namespace AirportDisplayApp
{
    /// <summary>
    /// App.xaml için etkileşim mantığı
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Hata yönetimi için global exception handler
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}\n\nDetaylar: {ex.StackTrace}", 
                    "Uygulama Hatası", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            };
        }
    }
}