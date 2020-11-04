using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace UnblockMe.PathFinder
{
    public partial class Tile : ContentView
    {
        private TileModel _binding;

        public static BindableProperty TileColorProperty =
            BindableProperty.Create(nameof(TileColor), typeof(Color), typeof(Tile));

        public delegate void TileClicked(object sender, TileModel binding);
        public event TileClicked OnTileClicked;

        public Tile()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            _binding = this.BindingContext as TileModel;

            TileBackground.BackgroundColor = _binding.Color;
            _binding.OnTileBackgroundChanged += TileBackgroundChanged;
        }

        private void TileBackgroundChanged(object sender, System.EventArgs e)
        {
            TileBackground.BackgroundColor = _binding.Color;
        }

        public Color TileColor
        {
            get { return (Color)GetValue(TileColorProperty); }
            set { SetValue(TileColorProperty, value); }
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var model = this.BindingContext;
            if(OnTileClicked != null)
            {
                OnTileClicked(this, this.BindingContext as TileModel);
            }
        }
    }
}
