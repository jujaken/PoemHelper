using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PoemHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string vowels = "ауиыэояюёе";
        private const string defalutText = "Мимо ристалищ, капищ\nМимо храмов и баров";

        public MainWindow()
        {
            InitializeComponent();
            Input.Text = defalutText;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            var shadowBuilder = new StringBuilder();
            var statBuilder = new StringBuilder();
            var msgBuilder = new StringBuilder();

            var numVowelsInLine = 0;

            foreach (var line in Input.Text.Split('\n'))
            {
                msgBuilder.Append('-');
                foreach (var c in line)
                {
                    if (vowels.Contains(char.ToLower(c)))
                    {
                        shadowBuilder.Append(c);
                        msgBuilder.Append(c);
                        msgBuilder.Append('-');
                        numVowelsInLine++;
                    }
                    else
                    {
                        shadowBuilder.Append(' ');
                    }
                }

                shadowBuilder.Append('\n');
                statBuilder.AppendLine(numVowelsInLine.ToString());
                msgBuilder.AppendLine();
                numVowelsInLine = 0;
            }
            
            // странно, но почему-то нужно проверять. возможно, из-за дефолтного значения в конструкторе. Оставь, чтобы компилилось крч
            if (Shadow == null) return;

            Shadow.Text = shadowBuilder.ToString();
            OutputStat.Text = statBuilder.ToString();
            OutputMsg.Text = msgBuilder.ToString();
            StatHeader.Content = $"Stat: {Input.Text.Length}";
        }

        private string GetVowelsReport()
        {
            var sb = new StringBuilder();

            var inputSplit = Input.Text.Split('\n');
            var statSplit = OutputStat.Text.Split('\n');
            var msgSplit = OutputMsg.Text.Split('\n');

            for (var i = 0; i < inputSplit.Length; i++)
                sb.AppendLine($"{msgSplit[i].Trim()} ({statSplit[i].Trim()}) ({inputSplit[i].Trim()})");

            return sb.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                FileName = Input.Text.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries)[0],
                Filter = "Text format (.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
                File.WriteAllText(dialog.FileName, Input.Text);
        }

        private void SaveVowels_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                FileName = Input.Text.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries)[0] + " отчёт",
                Filter = "Text format (.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
                File.WriteAllText(dialog.FileName, GetVowelsReport());
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Text format (.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
                Input.Text = File.ReadAllText(dialog.FileName);
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (Input.FontSize + 1 > 26) return;
            Input.FontSize += 1;
            Shadow.FontSize += 1;
            OutputMsg.FontSize += 1;
            OutputStat.FontSize += 1;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (Input.FontSize - 1 < 4) return;
            Input.FontSize -= 1;
            Shadow.FontSize -= 1;
            OutputMsg.FontSize -= 1;
            OutputStat.FontSize -= 1;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.Add || e.Key == Key.OemPlus)
                    Plus_Click(sender, e);

                if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                    Minus_Click(sender, e);
            }
        }
    }
}