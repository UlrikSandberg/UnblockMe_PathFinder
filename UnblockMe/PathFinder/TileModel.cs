using System;
using System.ComponentModel;
using UnblockMe.PriorityQue;
using Xamarin.Forms;

namespace UnblockMe.PathFinder
{
    public class TileModel : INotifyPropertyChanged
    {
        public delegate void TileBackgroundChanged(object sender, System.EventArgs e);
        public event TileBackgroundChanged OnTileBackgroundChanged;

        public MapObjects Occupant = MapObjects.None;

        public int GridRow = 0;
        public int GridCol = 0;


        private Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                if(OnTileBackgroundChanged != null)
                {
                    OnTileBackgroundChanged(this, new System.EventArgs());
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
