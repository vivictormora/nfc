﻿namespace IBCSApp.ViewModels.Base
{
    using Autofac;
    using IBCSApp.Services.Dispatcher;
    using IBCSApp.Services.Navigation;
    using IBCSApp.Services.API;
    using IBCSApp.Services.Settings;
    using IBCSApp.Services.NFC;
    using IBCSApp.Services.Bluetooth;
    using IBCSApp.Services.BF;
    using IBCSApp.Services.UX;

    /// <summary>
    /// This class allows us to resolve our ViewModels in one unique point.
    /// </summary>
    public class VMLocator
    {
        /// <summary>
        /// Autofac container.
        /// </summary>
        IContainer container;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VMLocator()
        {
            BuildContainer();
        }

        /// <summary>
        /// This method build the Autofac container with the registered types and instances.
        /// </summary>
        private void BuildContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<DispatcherService>().As<IDispatcherService>().SingleInstance();
            builder.RegisterType<ApiService>().As<IApiService>().SingleInstance();
            builder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();
            builder.RegisterType<NFCService>().As<INFCService>().SingleInstance();
            builder.RegisterType<BluetoothService>().As<IBluetoothService>().SingleInstance();
            builder.RegisterType<PairingService>().As<IPairingService>().SingleInstance();
            builder.RegisterType<BfService>().As<IBfService>().SingleInstance();
            builder.RegisterType<UxService>().As<IUxService>().SingleInstance();
            builder.RegisterType<VMMainPage>();
            builder.RegisterType<VMLoginPage>();
            builder.RegisterType<VMCreateAccountPage>();
            builder.RegisterType<VMAboutPage>();
            builder.RegisterType<VMDecryptMessage>();
            builder.RegisterType<VMInstructionsPage>();
            container = builder.Build();
        }

        /// <summary>
        /// LoginPage ViewModel instance.
        /// </summary>
        public VMLoginPage LoginViewModel
        {
            get { return this.container.Resolve<VMLoginPage>(); }
        }

        /// <summary>
        /// MainPage ViewModel instance.
        /// </summary>
        public VMMainPage MainViewModel
        {
            get { return this.container.Resolve<VMMainPage>(); }
        }

        /// <summary>
        /// CreateAccountPage ViewModel instance.
        /// </summary>
        public VMCreateAccountPage CreateAccountViewModel
        {
            get { return this.container.Resolve<VMCreateAccountPage>(); }
        }

        /// <summary>
        /// AboutPage ViewModel instance
        /// </summary>
        public VMAboutPage AboutViewModel
        {
            get { return this.container.Resolve<VMAboutPage>(); }
        }

        /// <summary>
        /// DecryptMessage ViewModel instance
        /// </summary>
        public VMDecryptMessage DecryptMessageViewModel
        {
            get { return this.container.Resolve<VMDecryptMessage>(); }
        }

        /// <summary>
        /// Instructions ViewModel instance
        /// </summary>
        public VMInstructionsPage InstructionsViewModel
        {
            get { return this.container.Resolve<VMInstructionsPage>(); }
        }
    }
}
