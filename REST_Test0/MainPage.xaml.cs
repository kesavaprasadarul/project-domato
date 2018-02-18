using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Flurl.Http;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace REST_Test0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Core.InitializeCore("my-secret-domato", "AIzaSyBwZTuiRuOCaOjlPjZTLNlforrzgPd2i30");
        }

        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            await Core.SignUpWithEmailPasswordAsync("kesavaprasadarul@outlook.com", "password123");
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            await Core.SignInWithEmailPasswordAsync("kesavaprasadarul@outlook.com", "password123");
        }

        private void ResetPW_Click(object sender, RoutedEventArgs e)
        {
            Core.ResetPassword("kesavaprasadarul@outlook.com");
        }
    }
}
