using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Win32;
using squares.Model;
using squares.Persistence;
using SquaresWPF.ViewModel;

namespace SquaresWPF
{
    public partial class App : Application
    {
        #region Fields

        private SquareGameModel _model = null!;
        private SquareViewModel _viewModel = null!;
        private MainWindow _view = null!;

        #endregion

        #region Constructors

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new SquareGameModel(new SquaresFileDataAccess());
            _model.GameOver += new EventHandler<SquareEventArgs>(Model_GameOver);
            _model.NewGame(3);

            _viewModel = new SquareViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            _view = new MainWindow(_model.Table.Size);
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            _view.Show();
        }

        #endregion

        #region View event handlers

        private void View_Closing(object? sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Squares", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region ViewModel event handlers

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            _viewModel.PlayFieldSize = _viewModel.TableSize * 2 - 1;
            _model.NewGame(_viewModel.TableSize);
        }

        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
            int size = _viewModel.TableSize;
            _viewModel.TableSize = 3;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Játék tábla betöltése";
                openFileDialog.Filter = "Játék tábla|*.stl";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);
                }
                _viewModel.TableSize = _model.Table.Size;
                _model.OnGameLoaded();
            }
            catch (SquaresDataException)
            {
                _viewModel.TableSize = size;
                MessageBox.Show("A fájl betöltése sikertelen!", "Squares", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Játék tábla mentése";
                saveFileDialog.Filter = "Játék tábla|*.stl";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (SquaresDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("A fájl mentése sikertelen!", "Sudoku", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewModel_ExitGame(object? sender, EventArgs e)
        {
            _view.Close();
        }

        #endregion

        #region Model event handlers

        private void Model_GameOver(object? sender, SquareEventArgs e)
        {
            int p1 = e.P1, p2 = e.P2;
            if (p1 == p2)
            {
                MessageBox.Show("Véget ért a játék, döntetlen lett!" + Environment.NewLine +
                                "Összesen " + (e.P1 + e.P2) + " négyzet lett berajzolva ",
                                "Squares játék",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Véget ért a játék, a nyertes: " + (p1 > p2 ? "Kék." : "Piros.") + Environment.NewLine + $"Pontszáma: {Math.Max(p1, p2)}", 
                                "Square játék", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Asterisk);
            }
        }

        #endregion
    }
}
