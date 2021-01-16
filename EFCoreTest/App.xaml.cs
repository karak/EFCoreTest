using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EFCoreTest.Services;
using EFCoreTest.Views;

namespace EFCoreTest
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            // DependencyService.Register<MockDataStore>();
            DependencyService.Register<SqliteDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
