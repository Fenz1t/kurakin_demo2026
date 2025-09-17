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

namespace kurakin_demo2026
{
    /// <summary>
    /// Логика взаимодействия для CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
            // Храним информацию о кусочках
            private List<PuzzlePieceInfo> _pieces = new List<PuzzlePieceInfo>();

            public CaptchaWindow()
            {
                InitializeComponent();
                SetupDragDrop();
                LoadPuzzlePieces();
            }

            private class PuzzlePieceInfo
            {
                public int CorrectPosition { get; set; }
                public int CurrentPosition { get; set; }
                public Image ImageControl { get; set; }
            }

            private void SetupDragDrop()
            {
                // Включаем перетаскивание для всех ячеек
                foreach (Border cell in PuzzleGrid.Children)
                {
                    cell.AllowDrop = true;
                    cell.DragEnter += Cell_DragEnter;
                    cell.DragLeave += Cell_DragLeave;
                    cell.Drop += Cell_Drop;
                }
            }

        private void LoadPuzzlePieces()
        {
            _pieces.Clear();

            var random = new Random();
            var shuffledPositions = new List<int> { 1, 2, 3, 4 }.OrderBy(x => random.Next()).ToList();

            // Получаем все Border'ы из PiecesPanel
            var borders = PiecesPanel.Children.OfType<Border>().ToList();

            for (int i = 0; i < 4; i++)
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{shuffledPositions[i]}.png")),
                    Stretch = Stretch.Uniform,
                    Cursor = Cursors.Hand,
                    Width = 70,  
                    Height = 70,
                    Tag = shuffledPositions[i]
                };
                image.MouseDown += PuzzlePiece_MouseDown;

                borders[i].Child = image;

                _pieces.Add(new PuzzlePieceInfo
                {
                    CorrectPosition = shuffledPositions[i],
                    CurrentPosition = 0,
                    ImageControl = image  
                });
            }
        }

        private void PuzzlePiece_MouseDown(object sender, MouseButtonEventArgs e)
            {
                var image = (Image)sender;
                DragDrop.DoDragDrop(image, image, DragDropEffects.Move);
            }

            private void Cell_DragEnter(object sender, DragEventArgs e)
            {
                var cell = (Border)sender;
                cell.Background = System.Windows.Media.Brushes.LightYellow;
            }

            private void Cell_DragLeave(object sender, DragEventArgs e)
            {
                var cell = (Border)sender;
                cell.Background = System.Windows.Media.Brushes.WhiteSmoke;
            }

            private void Cell_Drop(object sender, DragEventArgs e)
            {
                var cell = (Border)sender;
                cell.Background = System.Windows.Media.Brushes.WhiteSmoke;

                if (e.Data.GetData(typeof(Image)) is Image draggedImage)
                {
                    if (cell.Child != null)
                    {
                        cell.Child = null;
                    }

                    var newImage = new Image
                    {
                        Source = draggedImage.Source,
                        Width = 80,
                        Height = 80,
                        Tag = draggedImage.Tag
                    };

                    cell.Child = newImage;

                    // Обновляем текущую позицию кусочка
                    int correctPosition = (int)draggedImage.Tag;
                    int cellPosition = int.Parse(cell.Tag.ToString());

                    var piece = _pieces.FirstOrDefault(p => p.CorrectPosition == correctPosition);
                    if (piece != null)
                    {
                        piece.CurrentPosition = cellPosition;
                    }

                    draggedImage.Visibility = Visibility.Hidden;

                    if (_pieces.All(p => p.CurrentPosition > 0))
                    {
                        CheckPuzzle();
                    }
                }
            }

            private void CheckButton_Click(object sender, RoutedEventArgs e)
            {
                CheckPuzzle();
            }

            private void CheckPuzzle()
            {
                bool isCorrect = true;

                foreach (var piece in _pieces)
                {
                    if (piece.CurrentPosition != piece.CorrectPosition)
                    {
                        isCorrect = false;
                        break;
                    }
                }

                if (isCorrect)
                {
                    MessageBox.Show("Капча пройдена! Доступ разрешен.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
            }
            else
                {
                    MessageBox.Show("Неправильный порядок. Попробуйте еще раз.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    ResetPuzzle();
                }
            }

            private void ResetPuzzle()
            {
                foreach (Border cell in PuzzleGrid.Children)
                {
                    cell.Child = null;
                }

                foreach (var piece in _pieces)
                {
                    piece.ImageControl.Visibility = Visibility.Visible;
                    piece.CurrentPosition = 0;
                }
            }
        }
    }
