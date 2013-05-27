﻿using IBCSApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking.Proximity;
using IBCSApp.Resources;
using IBCSApp.Services.Settings;
using Windows.Networking.Sockets;

namespace IBCSApp.Services.Bluetooth
{
    public class BluetoothService : IBluetoothService
    {

        private ISettingsService settingsService;

        public BluetoothService(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public PeerDiscoveryTypes StartBluetooth()
        {
            PeerDiscoveryTypes type = PeerDiscoveryTypes.None;

            PeerFinder.Start();
            PeerFinder.DisplayName = (string)settingsService.Get("email");

            type = PeerFinder.SupportedDiscoveryTypes;

            return type;
        }

        public async Task<List<Peer>> FindPeers()
        {
            List<Peer> data = null;

            try
            {
                var peer = await PeerFinder.FindAllPeersAsync();

                data = (from p in peer
                        select new Peer()
                        {
                            Name = p.DisplayName,
                            Information = p
                        }).ToList();
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    if (MessageBox.Show(AppResources.BluetoothActivateMessage, AppResources.BluetoothActivateCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        Microsoft.Phone.Tasks.ConnectionSettingsTask cst = new Microsoft.Phone.Tasks.ConnectionSettingsTask();
                        cst.ConnectionSettingsType = Microsoft.Phone.Tasks.ConnectionSettingsType.Bluetooth;
                        cst.Show();
                    }
                }
            }

            return data;
        }


        public async Task<StreamSocket> ConnectToDevice(List<Peer> peers, string identity)
        {
            Peer myPeer = peers.First(s => s.Name == identity);
            return await PeerFinder.ConnectAsync(myPeer.Information);
        }
    }
}
