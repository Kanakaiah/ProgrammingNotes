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
using System.Threading;

namespace TaskCancelWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource ts;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ts = new CancellationTokenSource();
            textBlock1.Text = "";

            List<Task> tasks = new List<Task>();

            for (int i = 2; i <=100; i++)
            {
                int tmp = i;
                Task<double> adder = Task.Factory.StartNew(() => AddMultiple(tmp), ts.Token);

                tasks.Add(adder);
                var show = adder.ContinueWith(resultTask => textBlock1.Text += tmp.ToString() + " _ " + adder.Result.ToString() + Environment.NewLine, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
                var showCancel = adder.ContinueWith(resultTask => textBlock1.Text += tmp.ToString() + " canceled " + Environment.NewLine, CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.FromCurrentSynchronizationContext());

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ts.Cancel();
        }

        public double AddMultiple(int number)
        {
            double result = 1;
            for (int i = 1; i < 100000000; i++)
            {
                ts.Token.ThrowIfCancellationRequested(); result = result + (number * i);
            }
            return result;
        }
    }
}
