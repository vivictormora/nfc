﻿using IBCS.BF.Key;
using IBCSApp.Entities;
using IBCSApp.Resources;
using IBCSApp.Services.API;
using IBCSApp.Services.BF;
using IBCSApp.Services.Dispatcher;
using IBCSApp.Services.Navigation;
using IBCSApp.Services.Settings;
using IBCSApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IBCSApp.ViewModels
{
    public class VMDecryptMessage : VMBase
    {
        //Services
        private INavigationService navService;
        private IBfService bfService;
        private ISettingsService settingsService;
        private IDispatcherService dispatcherService;
        private IApiService apiService;

        //Commands
        private DelegateCommand navigateToHomeCommand;

        private string progressMessage;
        private string myIdentity;
        private SerializedPrivateKey key;

        private string identity;
        private string ciphertext;

        private string recipient;
        private string message;
        private System.Windows.Visibility panelVisibility = System.Windows.Visibility.Collapsed;

        /// <summary>
        /// Command to be binded in UI, navigate to main page
        /// </summary>
        public ICommand NavigateToHomeCommand
        {
            get { return navigateToHomeCommand; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="navService"></param>
        public VMDecryptMessage(INavigationService navService, IBfService bfService, ISettingsService settingsService, IDispatcherService dispatcherService, IApiService apiService)
        {
            this.navService = navService;
            this.bfService = bfService;
            this.settingsService = settingsService;
            this.dispatcherService = dispatcherService;
            this.apiService = apiService;

            this.navigateToHomeCommand = new DelegateCommand(NavigateToHomeExecute);

            myIdentity = (string) settingsService.Get("email");
        }

        /// <summary>
        /// Method to be executed by the NavigateHomeCommand.
        /// Use the navigation service instance to navigate to another page.
        /// </summary>
        private void NavigateToHomeExecute()
        {
            navService.NavigateToMainPage();
        }


        public string Recipient
        {
            get { return recipient; }
            set
            {
                recipient = value;
                RaisePropertyChanged();
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged();
            }
        }

        public System.Windows.Visibility PanelVisibility
        {
            get { return panelVisibility; }
            set
            {
                panelVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// This property can be binded in the view to show a message when we are doing an operation 
        /// that require large time to complete.
        /// </summary>
        public string ProgressMessage
        {
            get { return progressMessage; }
            set
            {
                progressMessage = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Decrypt a message using the user private key if it was intented to them
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="message"></param>
        public async void DecryptMessage(string identity, string message) 
        {

            if (identity == myIdentity) {
                string result = await bfService.DecipherText(message, key);
                Message = result;
            }
            else
            {
                Message = AppResources.DecryptMessageMessageError;
            }
            Recipient = identity;
            PanelVisibility = System.Windows.Visibility.Visible;
            dispatcherService.CallDispatcher(() => {
                IsBusy = false;
                ProgressMessage = String.Empty;
            });
        }

        /// <summary>
        /// Method to check if user keys have been obtained
        /// </summary>
        public void CheckKeysAndDecrypt(string identity, string message)
        {
            dispatcherService.CallDispatcher(() =>
            {
                IsBusy = true;
                ProgressMessage = AppResources.DecryptingMessage;
            });
            if (!settingsService.Contains("private"))
            {
                apiService.GetUserKeyCompleted += settingsService_GetUserKeyCompleted;
                this.identity = identity;
                this.ciphertext = message;
                apiService.GetUserKey((string)settingsService.Get("email"), (LoginToken)settingsService.Get("token"));
            }
            else
            {
                key = (SerializedPrivateKey)settingsService.Get("private");
                DecryptMessage(identity, message);
            }
        }

        private void settingsService_GetUserKeyCompleted(SerializedPrivateKey key)
        {
            if (key.CurveA == null)
            {
                CheckKeysAndDecrypt(identity,ciphertext);
            }
            else
            {
                settingsService.Set("private", key);
                this.key = key;
                DecryptMessage(identity, ciphertext);
            }
        }
    }
}
