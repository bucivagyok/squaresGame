using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SquaresWPF.ViewModel
{
    public class SquareField : ViewModelBase
    {
        private bool _enabled;
        private bool _square = false;
        private System.Windows.Media.Brush _color = Brushes.White;
        private Visibility _visibility;
        private DelegateCommand? _stepCommand;
        private bool _filler = false;
        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        public bool Filler
        {
            get { return _filler; }
            set { _filler = value; }
        }

        public bool Enabled 
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged();
            }
        }
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }
        public bool Square
        {
            get { return _square; }
            set { _square = value; }
        }

        public Int32 X { get; set; }

        public Int32 Y { get; set; }

        public Int32 ViewX { get; set; }

        public Int32 ViewY { get; set; }

        public bool Vertical { get; set; }

        public Tuple<int, int, bool> XYV
        {
            get { return new(Y, X, Vertical); }
        }

        public DelegateCommand? StepCommand 
        {
            get { return _stepCommand; } 
            set
            {
                _stepCommand = value;
                OnPropertyChanged();
            }
        }
    }
}
