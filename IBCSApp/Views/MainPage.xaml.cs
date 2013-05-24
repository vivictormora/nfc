﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using IBCSApp.Resources;
using IBCSApp.ViewModels;

namespace IBCSApp.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var entry = NavigationService.BackStack.FirstOrDefault();
            if (entry != null && entry.Source.OriginalString.Contains("Login"))
            {
                NavigationService.RemoveBackEntry();   
            }
            ((VMMainPage)this.DataContext).CheckKeys();
            ((VMMainPage)this.DataContext).StartPublishingIdentity();
        }
    }
}