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
using System.Windows.Shapes;

namespace HourlyRate
{
    /// <summary>
    /// Interaction logic for Modify.xaml
    /// </summary>
    public partial class Modify : Window
    {
        public Modify()
        {
            InitializeComponent();
        }

        public bool Validation()
        {
            if (sessionDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Select the date of the session!");
                return false;
            }
            if (stakesCombo.SelectedIndex <= -1)
            {
                MessageBox.Show("Select the session stakes!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(handsPlayedTxt.Text) || int.TryParse(handsPlayedTxt.Text, out int i) == false)
            {
                handsPlayedTxt.BorderBrush = Brushes.Red;
                MessageBox.Show("Please, enter the number of hands for the session!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(resultTxt.Text) || decimal.TryParse(resultTxt.Text, out decimal d) == false)
            {
                resultTxt.BorderBrush = Brushes.Red;
                MessageBox.Show("Please, enter the session result!");
                return false;
            }
            handsPlayedTxt.BorderBrush = Brushes.Blue;
            resultTxt.BorderBrush = Brushes.Blue;
            return true;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                DialogResult = true;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] bigBlinds = { "0.01/0.02", "0.02/0.05", "0.05/0.1", "0.08/0.16", "0.1/0.25", "0.25/0.5", "0.5/1", 
            "1/2", "2/5", "5/10", "10/20", "25/50", "50/100", "100/200", "200/400"};

            foreach (string bb in bigBlinds)
            {
                stakesCombo.Items.Add(bb); 
            }
        }

        private void handsPlayedTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            handsPlayedTxt.BorderBrush = Brushes.AliceBlue;
            resultTxt.BorderBrush = Brushes.AliceBlue;
        }

        private void resultTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            handsPlayedTxt.BorderBrush = Brushes.AliceBlue;
            resultTxt.BorderBrush = Brushes.AliceBlue; 
        }
    }
}
