﻿using System;
using System.Windows;

using Microsoft.Phone.Controls;

namespace Boom
{
    /// <summary>
    /// Main xaml page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Simple button Click event handler to take us to the second page
        private void buttonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }
    }
}