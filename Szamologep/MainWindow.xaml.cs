using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Szamologep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentInput = "";
        private string currentExpression = "";
        private Stack<string> history = new Stack<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AnimateButton(button);
            currentInput += button.Content.ToString();
            currentExpression += button.Content.ToString();
            Display.Text = currentExpression;
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AnimateButton(button);
            if (!string.IsNullOrEmpty(currentInput) || currentExpression.EndsWith(")"))
            {
                currentExpression += " " + button.Content.ToString() + " ";
                currentInput = "";
                Display.Text = currentExpression;
            }
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AnimateButton(button);
            try
            {
                history.Push(currentExpression); // Save current expression in history
                var result = EvaluateExpression(currentExpression);
                Display.Text = result.ToString();
                currentExpression = result.ToString();
                currentInput = result.ToString();
            }
            catch (EvaluateException)
            {
                Display.Text = "Syntax Error";
                currentInput = "";
                currentExpression = history.Count > 0 ? history.Peek() : "";
            }
            catch (DivideByZeroException)
            {
                Display.Text = "Math Error: Division by Zero";
                currentInput = "";
                currentExpression = history.Count > 0 ? history.Peek() : "";
            }
            catch (Exception ex)
            {
                Display.Text = "Unknown Error: " + ex.Message;
                currentInput = "";
                currentExpression = history.Count > 0 ? history.Peek() : "";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AnimateButton(button);
            currentInput = "";
            currentExpression = "";
            Display.Text = "0";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AnimateButton(button);
            if (!string.IsNullOrEmpty(currentExpression))
            {
                currentExpression = currentExpression.Remove(currentExpression.Length - 1);
                Display.Text = string.IsNullOrEmpty(currentExpression) ? "0" : currentExpression;
                currentInput = currentExpression;
            }
        }

        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AnimateButton(button);
            currentExpression += button.Content.ToString();
            Display.Text = currentExpression;
        }

        private void AnimateButton(Button button)
        {
            var scaleTransform = new ScaleTransform(1.0, 1.0);
            button.RenderTransform = scaleTransform;
            button.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(100),
                AutoReverse = true
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        private double EvaluateExpression(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return Convert.ToDouble(table.Rows[0]["expression"]);
        }

        private void StartScrollingAnimation()
        {
            var animation = new DoubleAnimation
            {
                From = Display.ActualWidth,
                To = -Display.ActualWidth,
                Duration = TimeSpan.FromSeconds(5),
                RepeatBehavior = RepeatBehavior.Forever
            };
            var transform = new TranslateTransform();
            Display.RenderTransform = transform;
            Display.RenderTransformOrigin = new Point(0.5, 0.5);
            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
    }
}