﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnblockMe.PathFinder;
using Xamarin.Forms;

namespace UnblockMe
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            this.Navigation.PushAsync(new Board());
        }

        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            this.Navigation.PushAsync(new PathFindingBoard());
        }
    }
}
