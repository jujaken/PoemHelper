using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PoemHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string vowels = "ауиыэояюёеа́у́и́ы́э́о́я́ю́е́";

        // thx mr. Brodskiy
        // https://www.culture.ru/poems/30448/piligrimy
        private const string defalutText = "Мимо ристалищ, капищ\nМимо храмов и баров\n\n© Иосиф Бродский";
        private string? selectedFile;


        public MainWindow()
        {
            InitializeComponent();
            Input.Text = defalutText;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            var idBuilder = new StringBuilder();
            var shadowBuilder = new StringBuilder();
            var statBuilder = new StringBuilder();
            var msgBuilder = new StringBuilder();
            var shadowMsgBuilder = new StringBuilder();

            var numVowelsInLine = 0;
            var id = 0;

            foreach (var line in Input.Text.Split('\n'))
            {
                idBuilder.AppendLine((++id).ToString());
                foreach (var c in line)
                {
                    if (vowels.Contains(char.ToLower(c)))
                    {
                        shadowBuilder.Append(c);
                        msgBuilder.Append('-');
                        msgBuilder.Append(c);
                        numVowelsInLine++;
                        shadowMsgBuilder.Append(char.ToUpper(c) == c ? $" {c}" : "  ");
                    }
                    else
                    {
                        shadowBuilder.Append(' ');
                    }
                }

                if (numVowelsInLine == 0)
                {
                    statBuilder.AppendLine();
                }
                else
                {
                    msgBuilder.Append('-');
                    statBuilder.AppendLine(numVowelsInLine.ToString());
                }

                shadowBuilder.AppendLine();
                msgBuilder.AppendLine();
                shadowMsgBuilder.AppendLine();
                numVowelsInLine = 0;
            }

            // странно, но почему-то нужно проверять. возможно, из-за дефолтного значения в конструкторе. Оставь, чтобы компилилось крч
            if (Shadow == null) return;
            if (OutputMsgShadow == null) return;

            IdLine.Text = idBuilder.ToString();
            Shadow.Text = shadowBuilder.ToString();
            OutputStat.Text = statBuilder.ToString();
            OutputMsg.Text = msgBuilder.ToString();
            OutputMsgShadow.Text = shadowMsgBuilder.ToString();
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
            if (selectedFile != null && File.Exists(selectedFile))
            {
                File.WriteAllText(selectedFile, Input.Text);
                return;
            }

            if (Input.Text.Length == 0) return;

            var dialog = new SaveFileDialog()
            {
                FileName = Input.Text.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries)[0],
                Filter = "Text format (.txt)|*.txt"
            };

            if (dialog.ShowDialog() != true) return;

            File.WriteAllText(dialog.FileName, Input.Text);
            selectedFile = dialog.FileName;
            SelectedFile.Content = Path.GetFileName(selectedFile);
            CloseFile.Visibility = Visibility.Visible;
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
            IdLine.FontSize += 1;
            Input.FontSize += 1;
            Shadow.FontSize += 1;
            OutputMsg.FontSize += 1;
            OutputStat.FontSize += 1;
            OutputMsgShadow.FontSize += 1;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (Input.FontSize - 1 < 4) return;
            IdLine.FontSize -= 1;
            Input.FontSize -= 1;
            Shadow.FontSize -= 1;
            OutputMsg.FontSize -= 1;
            OutputStat.FontSize -= 1;
            OutputMsgShadow.FontSize -= 1;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.Add || e.Key == Key.OemPlus)
                    Plus_Click(sender, e);

                if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                    Minus_Click(sender, e);

                if (e.Key == Key.S)
                    Save_Click(sender, e);
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                    Plus_Click(sender, e);
                else
                    Minus_Click(sender, e);

                e.Handled = true;
            }
        }

        private void ColorButton_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Shadow.Foreground = new SolidColorBrush(e.NewValue);
            Resources["CurrentMainColor"] = e.NewValue;
        }

        private void UseShadow_Checked(object sender, RoutedEventArgs e)
        {
            if (Shadow == null) return;
            Shadow.Visibility = Visibility.Visible;
        }

        private void UseShadow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Shadow == null) return;
            Shadow.Visibility = Visibility.Hidden;
        }

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            selectedFile = null;
            SelectedFile.Content = null;
            CloseFile.Visibility = Visibility.Collapsed;
        }

        private void UseAccents_Checked(object sender, RoutedEventArgs e)
        {
            OutputMsgShadow.Visibility = Visibility.Visible;
        }

        private void UseAccents_Unchecked(object sender, RoutedEventArgs e)
        {
            OutputMsgShadow.Visibility = Visibility.Hidden;
        }
    }
}