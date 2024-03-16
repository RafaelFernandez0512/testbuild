using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SigeTools.View
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            Appearing += (object sender, EventArgs e) => {
                HiddenEntry.Focus();
            };

            //this.HiddenEntry.Unfocused += (object sender, FocusEventArgs e) => {
            //    HiddenEntry.Focus();
            //    base.OnAppearing();
            //};

            Disappearing += (object sender, EventArgs e) =>
            {
                HiddenEntry.Unfocus();
            };
        }
    }
}
