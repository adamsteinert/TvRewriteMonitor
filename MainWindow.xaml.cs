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

namespace TvRewriteMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RewriteMonitor monitor;

        private string[] shows = new string[]
            {
                @"E:\Share\Video\MCEBuddy\Primal Grill With Steven Raichlen",
                @"E:\Share\Video\MCEBuddy\Barbecue University With Steven Raichlen",
                @"E:\Share\Video\MCEBuddy\This Old House",
                @"E:\Share\Video\MCEBuddy\America's Test Kitchen From Cook's Illustrated",
                @"E:\Share\Video\MCEBuddy\Cook's Country From America's Test Kitchen",
                @"E:\Share\Video\MCEBuddy\Inside Nature's Giants"
            };

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //monitor = new RewriteMonitor(@"E:\Share\Video") {Logger = Logger};
            //monitor.Start();

            Logger("Starting Activity");
            foreach (var dir in shows)
            {
                var showOp = new ShowNameOperator(dir, Logger);
                showOp.MoveAllFiles();
            }
            Logger("Done");
        }

        private void Logger(string s)
        {
            output.Text += string.Format("{0}{1}{1}", s, Environment.NewLine);
        }
    }
}
