using squares.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SquaresWPF.ViewModel
{
    public class SquareViewModel : ViewModelBase
    {
        #region Fields

        private SquareGameModel _model;
        private int _tableSize = 3;
        private int _playFieldSize = 5;


        #endregion

        #region Properties 

        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }

        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand ChangeSize { get; private set; }

        public ObservableCollection<SquareField> Fields { get; set; }

        public Int32 GameStepCount { get { return _model.EmptyLines; } }

        public String CurrentPlayer
        {
            get
            {
                switch (_model.Player)
                {
                    case 1:
                        return "Kék";
                    case 2:
                        return "Piros";
                    default:
                        return "N/A";
                }
            }
        }

        public Int32 PlayFieldSize
        {
            get { return _playFieldSize; }
            set { _playFieldSize = value; }
        }

        public Int32 TableSize
        {
            get { return _tableSize; }
            set
            {
                _tableSize = value;
                _playFieldSize = _tableSize * 2 - 1;
                OnPropertyChanged(nameof(_playFieldSize));
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler? NewGame;

        public event EventHandler? LoadGame;

        public event EventHandler? SaveGame;

        public event EventHandler? ExitGame;

        #endregion

        #region Constructors

        public SquareViewModel(SquareGameModel model)
        {
            _model = model;
            _model.FieldChanged += new EventHandler<SquareFieldEventArgs>(Model_FieldChanged);
            _model.GameAdvanced += new EventHandler<SquareEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<SquareEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<SquareEventArgs>(Model_GameCreated);
            _model.SquareClaimed += new EventHandler<SquareFieldEventArgs>(Model_SquareClaimed);
            Fields = new ObservableCollection<SquareField>();

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            ChangeSize = new DelegateCommand(param =>
            {
                TableSize = int.Parse(param!.ToString()!);
            });

            InitializeTable();
            RefreshTable();
        }

        #endregion

        #region Private methods
        private void InitializeTable()
        {
            if (Fields != null)
            {
                Fields.Clear(); 
            }
            for (int i = 0; i < _tableSize * 2 - 1; i++)
            {
                for (int j = 0; j < _tableSize * 2 - 1; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            Fields.Add(new SquareField
                            {
                                Filler = true,
                                Enabled = false,
                                Visibility = Visibility.Visible,
                                ViewX = i,
                                ViewY = j,
                            });
                        }
                        else
                        {
                            Fields.Add(new SquareField
                            {
                                Visibility = Visibility.Hidden,
                                Enabled = false,
                                X = i / 2,
                                Y = (j - 1) / 2,
                                ViewX = i,
                                ViewY = j,
                                Vertical = false
                            });
                        }
                    }
                    else
                    {
                        if (j % 2 == 0)
                        {
                            Fields.Add(new SquareField
                            {
                                Visibility = Visibility.Hidden,
                                Enabled = false,
                                X = (i - 1) / 2,
                                Y = j / 2,
                                ViewX = i,
                                ViewY = j,
                                Vertical = true
                            });
                        }
                        else
                        {
                            Fields.Add(new SquareField
                            {
                                Square = true,
                                Visibility = Visibility.Visible,
                                Enabled = false,
                                X = (i - 1) / 2,
                                Y = (j - 1) / 2,
                                ViewX = i,
                                ViewY = j
                            });
                        }
                    }
                }
            }
        }
        private void RefreshTable()
        {
            InitializeTable();
            foreach (SquareField field in Fields)
            {
                
                if (field.Filler)
                {
                    continue;
                }
                if (!field.Square)
                {
                    if (field.Vertical)
                    {
                        field.Visibility = Visibility.Visible;
                        field.Enabled = true;
                        switch (_model.Table[field.X, field.Y, field.Vertical])
                        {
                            case 1:
                                field.Color = Brushes.Blue;
                                field.StepCommand = null;
                                break;
                            case 2:
                                field.Color = Brushes.Red;
                                field.StepCommand = null;
                                break;
                            default:
                                field.Color = Brushes.White;
                                field.StepCommand = new DelegateCommand(param =>
                                {
                                    if (param is Tuple<int, int, bool> position)
                                        StepGame(position.Item1, position.Item2, position.Item3);
                                });
                                break;
                        }
                    }
                    if (!field.Vertical)
                    {
                        field.Visibility = Visibility.Visible;
                        field.Enabled = true;
                        switch (_model.Table[field.X, field.Y, field.Vertical])
                        {
                            case 1:
                                field.Color = Brushes.Blue;
                                field.StepCommand = null;
                                break;
                            case 2:
                                field.Color = Brushes.Red;
                                field.StepCommand = null;
                                break;
                            default:
                                field.Color = Brushes.White;
                                field.StepCommand = new DelegateCommand(param =>
                                {
                                    if (param is Tuple<int, int, bool> position)
                                        StepGame(position.Item1, position.Item2, position.Item3);
                                });
                                break;
                        }
                    }
                }
                else
                {
                    if (field.X < _tableSize - 1 && field.Y < _tableSize - 1)
                    {
                        field.Visibility = Visibility.Visible;
                        field.Enabled = true;
                        switch (_model.Table.GetSquare(field.X, field.Y))
                        {
                            case 1:
                                field.Color = Brushes.Blue;
                                break;
                            case 2:
                                field.Color = Brushes.Red;
                                break;
                            default:
                                field.Color = Brushes.White;
                                break;
                        }
                    }
                }
                
            }
            OnPropertyChanged(nameof(PlayFieldSize));
            OnPropertyChanged(nameof(GameStepCount));
        }

        private void StepGame(int x, int y, bool vertical)
        {
            _model.Step(y, x, vertical);
            OnPropertyChanged(nameof(GameStepCount));
        }

        #endregion

        #region Game event handler

        private void Model_FieldChanged(object? sender, SquareFieldEventArgs e)
        {
            SquareField field = Fields.Single(f => f.X == e.X && f.Y == e.Y && f.Vertical == e.Vertical && f.Square == false && f.Filler == false);

            switch (_model.Table[field.X, field.Y, field.Vertical])
            {
                case 1:
                    field.Color = Brushes.Blue;
                    break;
                case 2:
                    field.Color = Brushes.Red;
                    break;
                default:
                    field.Color = Brushes.White;
                    break;
            }
            field.StepCommand = null;
            OnPropertyChanged(nameof(GameStepCount));
        }

        private void Model_SquareClaimed(object? sender, SquareFieldEventArgs e)
        {
            SquareField field = Fields.Single(f => f.X == e.X && f.Y == e.Y && f.Square == true);
            switch (_model.Table.GetSquare(field.X, field.Y))
            {
                case 1:
                    field.Color = Brushes.Blue;
                    break;
                case 2:
                    field.Color = Brushes.Red;
                    break;
                default:
                    field.Color = Brushes.White;
                    break;
            }
        }

        private void Model_GameOver(object? sender, SquareEventArgs e)
        {
        }

        private void Model_GameAdvanced(object? sender, SquareEventArgs e)
        {
            OnPropertyChanged(nameof(CurrentPlayer));
        }

        private void Model_GameCreated(object? sender, SquareEventArgs e)
        {
            RefreshTable();
        }

        #endregion

        #region Event methods 

        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(PlayFieldSize));
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
