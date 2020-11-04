using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace UnblockMe.UnblockMeSolver
{
    public partial class UnblockMeTIle : ContentView
    {
        private UnblockMeTileModel _binding;

        public static BindableProperty TileColorProperty =
            BindableProperty.Create(nameof(TileColor), typeof(Color), typeof(UnblockMeTIle));

        public delegate void TileClicked(object sender, UnblockMeTileModel binding);
        public event TileClicked OnTileClicked;

        public UnblockMeTIle()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            _binding = this.BindingContext as UnblockMeTileModel;

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
            if (OnTileClicked != null)
            {
                OnTileClicked(this, this.BindingContext as UnblockMeTileModel);
            }
        }
    }
}
