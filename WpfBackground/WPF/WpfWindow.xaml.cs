using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBackground
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WpfWindow : Window
    {
        public static WpfWindow Instance;

        public static void ShowWindow()
        {
            if(Instance == null)
            {
                Instance = new WpfWindow();
            }
            Instance.Show();
            Instance.Activate();
        }

        public WpfWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
