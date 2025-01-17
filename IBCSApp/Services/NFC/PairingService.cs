﻿using IBCS.BF.Key;
using IBCSApp.Services.BF;
using IBCSApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using System.Security.Cryptography;
using IBCS.BF.Util;
using IBCSApp.Services.Bluetooth;
using IBCSApp.Entities;

namespace IBCSApp.Services.NFC
{

    public class PairingService : IPairingService
    {
        ///NFC message types
        public const string NFC_IBCS = "Windows.IBCS.Identity";
        public const string NFC_IBCS_ACK = "Windows.IBCS.IdentityACK";
        public const string NFC_IBCS_KEY_ACK = "Windows.IBCS.keyACK";
        public const string NFC_IBCS_KEY = "Windows.IBCS.Key";

        private INFCService nfcService;
        private IBfService bfService;
        private ISettingsService settingsService;
        private IBluetoothService bluetoothService;

        private AesManaged aes;

        private string identity;
        private string myIdentity;
        private string ct;

        private StreamSocket socket;

        public PairingService(INFCService nfcService, IBfService bfService, ISettingsService settingsService, IBluetoothService bluetoothService)
        {
            this.nfcService = nfcService;
            this.bfService = bfService;
            this.settingsService = settingsService;
            this.bluetoothService = bluetoothService;
            aes = new AesManaged();
        }

        public void NfcPairDevices()
        {
            if (nfcService.ConnectDefaultProximityDevice())
            {
                nfcService.SubscribeToMessage(NFC_IBCS);
                nfcService.SubscribeToMessage(NFC_IBCS_KEY);
                nfcService.SubscribeToMessage(NFC_IBCS_ACK);
                nfcService.MessageReceivedCompleted += nfcService_MessageReceivedCompleted;
                NfcPairingCompleted += PairingService_NfcPairingCompleted;
                nfcService.PublishTextMessage(NFC_IBCS, (string)settingsService.Get("email"));
            } 
        }

        private void PairingService_NfcPairingCompleted()
        {
            PairingCompleted(identity, aes, socket);
        }

        private void TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs args)
        {
            switch (args.State)
            {
                case TriggeredConnectState.PeerFound:   // Tapped another phone
                    break;
                case TriggeredConnectState.Connecting:
                    break;
                case TriggeredConnectState.Completed:   // Connection ready to use
                    // Save socket to fields
                    socket = args.Socket;
                    nfcService.PublishTextMessage(NFC_IBCS_ACK, (string)settingsService.Get("email"));
                    // Listen to incoming socket
                    break;
            }   
        }


        public event PairingCompletedArgs PairingCompleted;
        private event NfcPairingCompletedArgs NfcPairingCompleted;

        private async void nfcService_MessageReceivedCompleted(ProximityMessage message)
        {
            switch (message.MessageType)
            {
                case NFC_IBCS:
                    {
                        PeerFinder.TriggeredConnectionStateChanged += TriggeredConnectionStateChanged;
                        PeerFinder.Start();
                        nfcService.UnsubscribeForMessage(NFC_IBCS);
                        break;
                    }
                case NFC_IBCS_ACK:
                    {
                        nfcService.UnsubscribeForMessage(NFC_IBCS_ACK);
                        identity = message.DataAsString;
                        nfcService.PublishTextMessage(NFC_IBCS_ACK, (string) settingsService.Get("email"));
                        aes.GenerateKey();
                        ct = await bfService.CipherMessage(aes.Key, message.DataAsString, (SerializedPrivateKey) settingsService.Get("private"));
                        nfcService.PublishTextMessage(NFC_IBCS_KEY, ct);
                        break;
                    }
                case NFC_IBCS_KEY:
                    {
                        byte[] pt = await bfService.DecipherMessage(message.DataAsString, (SerializedPrivateKey) settingsService.Get("private"));
                        byte[] newKey = BFUtil.Xor(aes.Key, pt);
                        aes.Key = newKey;
                        nfcService.UnsubscribeForMessage(NFC_IBCS_KEY);
                        if (NfcPairingCompleted != null) {
                            NfcPairingCompleted();
                        }
                        break;
                    }
            }
        }
    }
}
