using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HourlyRate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SessionList sessionList = new SessionList(); 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBlockMain.Background = Brushes.LightGray;
            textBlockMain.TextAlignment = TextAlignment.Center;

            sessionList = (SessionList)Resources["sessions"];
            GetOverallResult();

            Session s = sessionList[sessionList.Count - 1];
            statsDataGrid.ScrollIntoView(s); 

            string[] stakes = { "0.01/0.02", "0.02/0.05", "0.05/0.1", "0.08/0.16", "0.1/0.25", "0.25/0.5", "0.5/1",
            "1/2", "2/5", "5/10", "10/20", "25/50", "50/100", "100/200", "200/400"};

            foreach (string bb in stakes)
            {
                stakesCombo.Items.Add(bb);
                BBCombo.Items.Add(bb);
                BBCombo2.Items.Add(bb);
            }

            GetHandsTotal(); 
        }
                
        //Stakes to Big Blind converter
        private decimal GetBigBlind(string stakes)
        {
            string[] blinds = stakes.Split('/');

            string bigBlindOld = blinds[1];
            string bigBlindNew = "";

            if (bigBlindOld.Contains('.'))
            {
                bigBlindNew = bigBlindOld.Replace('.', ',');
            }
            else
            {
                bigBlindNew = blinds[1];
            }
            decimal.TryParse(bigBlindNew, out decimal bigBlind);

            return bigBlind;
        }

        //Validation

        private bool winRateValidation()
        {
            if (BBCombo.SelectedIndex <= -1)
            {
                MessageBox.Show("Please, select a Big Blind value.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(handsPlayedTxt.Text) || int.TryParse(handsPlayedTxt.Text, out int i) != true || handsPlayedTxt.Text == "0")
            {
                handsPlayedTxt.BorderBrush = Brushes.Red;
                MessageBox.Show("Please, enter the number of hands played.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(amountWonTxt.Text) || decimal.TryParse(amountWonTxt.Text, out decimal d) != true)
            {
                amountWonTxt.BorderBrush = Brushes.Red;
                MessageBox.Show("Please, enter an amount.");
                return false;
            }
            return true;
        }

        private bool expectedWinValidation()
        {
            if (BBCombo2.SelectedIndex <= -1)
            {
                MessageBox.Show("Please, select a Big Blind value.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(handsPlayed2Txt.Text) || int.TryParse(handsPlayed2Txt.Text, out int i)!=true)
            {
                handsPlayed2Txt.BorderBrush = Brushes.Red;
                MessageBox.Show("Please, enter the number of hands played.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(winRateTxt.Text) || double.TryParse(winRateTxt.Text, out double d) != true)
            {
                winRateTxt.BorderBrush = Brushes.Red;
                MessageBox.Show("Please, enter a win rate.");
                return false;
            }
            return true;
        }


        //Win rate and Hourly rate fields on the left

        private void winRateBtn_Click(object sender, RoutedEventArgs e)
        {
            decimal amountWon;
            int handsPlayed;

            if (winRateValidation())
            {
                decimal.TryParse(amountWonTxt.Text, out amountWon);
                string stakes = BBCombo.SelectedItem.ToString();
                decimal bigBlind = GetBigBlind(stakes);
                int.TryParse(handsPlayedTxt.Text, out handsPlayed);

                decimal winRate = amountWon / bigBlind / handsPlayed * 100;
                decimal hourlyRate = winRate * bigBlind * 4;

                textBlockMain.Foreground = Brushes.Black;
                textBlockMain.FontSize = 12;
                
                textBlockMain.Text = $"\nWin-rate: {Math.Round(winRate, 2)} BB / 100 hands\nHourly rate " +
                    $"while four-tabling: ${Math.Round(hourlyRate, 2)}/h";
            }
        }
                

        private void expectedWinBtn_Click(object sender, RoutedEventArgs e)
        {
            decimal winRate;
            int handsPlayed;

            if (expectedWinValidation())
            {
                string stakes = BBCombo2.SelectedItem.ToString();
                decimal bigBlind = GetBigBlind(stakes);
                decimal.TryParse(winRateTxt.Text, out winRate);
                int.TryParse(handsPlayed2Txt.Text, out handsPlayed);

                decimal expectedWin = winRate * bigBlind / 100 * handsPlayed;
                decimal hourlyRate = winRate * bigBlind * 4;

                textBlockMain.Foreground = Brushes.Black;
                textBlockMain.FontSize = 12;
                
                textBlockMain.Text = $"\nExpected win after {handsPlayed} hands: ${Math.Round(expectedWin, 2)}\nHourly rate " +
                    $"while four-tabling: ${Math.Round(hourlyRate, 2)}/h"; 
            }
        }

        private void handsPlayedTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            handsPlayedTxt.BorderBrush = Brushes.Transparent;
            amountWonTxt.BorderBrush = Brushes.Transparent;
        }

        private void amountWonTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            amountWonTxt.BorderBrush = Brushes.Transparent;
            handsPlayedTxt.BorderBrush = Brushes.Transparent;
        }

        private void handsPlayed2Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            handsPlayed2Txt.BorderBrush = Brushes.AliceBlue;
            winRateTxt.BorderBrush = Brushes.AliceBlue; 
        }

        private void winRateTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            winRateTxt.BorderBrush = Brushes.AliceBlue;
            handsPlayed2Txt.BorderBrush = Brushes.AliceBlue; 
        }

        //Top display 

        private void GetOverallResult()
        {
            List<decimal> results = new List<decimal>();

            foreach (Session s in sessionList)
            {
                results.Add(s.Result);
            }

            decimal overallResult = results.Sum();

            textBlockMain.Text = "\nCurrent Result: " + overallResult;

            textBlockMain.FontSize = 16;
            textBlockMain.FontWeight = FontWeights.Bold;
            textBlockMain.Foreground = overallResult >= 0 ? Brushes.Green : Brushes.Red;
        }


        //Main data grid buttons

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            Modify modify = new Modify();
            modify.Owner = this;
            modify.Title = "New Session"; 

            if (modify.ShowDialog() == true)
            {
                Session s = new Session();
                s.SessionDate = (DateTime)modify.sessionDatePicker.SelectedDate;
                s.Stakes = modify.stakesCombo.SelectedItem.ToString();
                s.HandsPlayed = int.Parse(modify.handsPlayedTxt.Text);
                s.Result = decimal.Parse(modify.resultTxt.Text);

                sessionList.Add(s);
                statsDataGrid.ScrollIntoView(s);
                statsDataGrid.UpdateLayout();

                GetOverallResult();
                GetHandsTotal();

                if (SessionDal.NewSession(s) != 1)
                {
                    MessageBox.Show("Couldn't establish connection to the database."); 
                }
            }
        }

        private void modifyButton_Click(object sender, RoutedEventArgs e)
        {
            if(statsDataGrid.SelectedItem != null)
            {
                Modify modify = new Modify();
                modify.Owner = this;
                modify.Title = "Modify Session Details";

                Session s = statsDataGrid.SelectedItem as Session;
                modify.sessionDatePicker.SelectedDate = s.SessionDate;
                modify.stakesCombo.SelectedItem = s.Stakes;
                modify.handsPlayedTxt.Text = s.HandsPlayed.ToString();
                modify.resultTxt.Text = s.Result.ToString();

                if (modify.ShowDialog() == true)
                {
                    s.SessionDate = (DateTime)modify.sessionDatePicker.SelectedDate;
                    s.Stakes = modify.stakesCombo.SelectedItem.ToString();
                    s.HandsPlayed = int.Parse(modify.handsPlayedTxt.Text);
                    s.Result = decimal.Parse(modify.resultTxt.Text);

                    GetOverallResult();
                    GetHandsTotal();

                    if (SessionDal.ModifySession(s) == 1)
                    {
                        MessageBox.Show("Modification successful.");
                    }
                    else
                    {
                        MessageBox.Show("Connection to the database couldn't be established. Please try again later.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please, select a session from the list!");
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(statsDataGrid.SelectedIndex > -1)
            {
                Session s = statsDataGrid.SelectedItem as Session;

                MessageBoxResult result = MessageBox.Show($"Would you like to delete the session from {s.SessionDate.ToShortDateString()}?", "Alert", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    SessionDal.DeleteSession(s);
                    sessionList.Remove(s);
                    statsDataGrid.UpdateLayout();
                    
                    GetOverallResult();
                    GetHandsTotal(); 
                }
            }
            else
            {
                MessageBox.Show("Please, select an item from the list!"); 
            }
        }

        //Statistics by stakes on the right
                

        private void GetResultsByStakes(string stakes)
        {
            StringBuilder sb = new StringBuilder();
            
            IEnumerable<int> handsPlayed = sessionList.Where(s => s.Stakes == stakes).Select(s => s.HandsPlayed);
            IEnumerable<decimal> results = sessionList.Where(s => s.Stakes == stakes).Select(s => s.Result);
            IEnumerable<decimal> winningSessions = results.Where(res => res > 0);
            IEnumerable<decimal> losingSessions = results.Where(res => res < 0);
            
            decimal bigBlind = GetBigBlind(stakes);
            decimal overallResult = results.Sum();
            int overallHandsPlayed = handsPlayed.Sum();
            decimal winRate = overallHandsPlayed > 0 ? overallResult / bigBlind / overallHandsPlayed * 100 : 0;
            decimal hourlyRate = winRate * bigBlind * 4;
            int numberOfWinningSessions = winningSessions.Count();
            int numberOfLosinigSessios = losingSessions.Count();

            sb.AppendLine($"\n STAKES {stakes}:");
            sb.AppendLine($" Hands played: {overallHandsPlayed}");
            sb.AppendLine($" Overall result: {overallResult}");
            sb.AppendLine($" Win rate: {Math.Round(winRate, 2)} BB/100 hands");
            sb.AppendLine($" Hourly rate: ${Math.Round(hourlyRate, 2)}/h");
            sb.AppendLine($" Winning sessions: {numberOfWinningSessions}");
            sb.AppendLine($" Losing sessions: {numberOfLosinigSessios}");

            stakesStatsTxt.AppendText(sb.ToString());
            
            
            sb.AppendLine($"Stakes: {stakes}:");
            sb.AppendLine("No sessions recorded."); 
        }

        private void stakesStatsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (stakesCombo.SelectedIndex > -1)
            {
                string stakes = stakesCombo.SelectedItem.ToString();
                GetResultsByStakes(stakes);
                stakesStatsTxt.ScrollToEnd();
            }
            else
            {
                stakesStatsTxt.AppendText("\nPlease, select stakes!\n"); 
            }
        }

        //Hands total under the top display

        private void GetHandsTotal()
        {
            IEnumerable<int> totalHandsPlayed = sessionList.Select(s => s.HandsPlayed);
            int overallHands = totalHandsPlayed.Sum();

            handsTotalTextBlock.Text = "";
            handsTotalTextBlock.Text = $"Hands Total: {overallHands}";
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            BBCombo.SelectedIndex = -1;
            handsPlayedTxt.Clear();
            amountWonTxt.Clear();
            BBCombo2.SelectedIndex = -1;
            handsPlayed2Txt.Clear();
            winRateTxt.Clear();
            stakesStatsTxt.Clear();
            stakesCombo.SelectedIndex = -1;
            GetOverallResult(); 
        }
    }
}
