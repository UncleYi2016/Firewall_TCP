using Firewall.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Firewall {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        TCPInitialize init;
        public MainWindow() {
            init = new TCPInitialize(8085);
            InitializeComponent();
        }

        private void Grid_Initialized(object sender, EventArgs e) {
            init.listenThreadInit();
        }

        private void Window_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        private void denyAddBtn_Click(object sender, RoutedEventArgs e) {
            string address = denyAddrText.Text;
            init.dt.write(address);
        }

        public void addLog(string content) {
        }


    }
}
