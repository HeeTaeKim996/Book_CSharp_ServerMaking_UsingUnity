using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;
using System.Net;
using System;

namespace FreeNetUnity
{
    public class CFreeNetUnityService : MonoBehaviour
    {
        private CFreeNetEventManager netEventManager;
        private CNetworkService cNetworkService;
        private IPeer gameServer;

        public delegate void StatusChangeHandler(NETWORK_EVENT netEvent);
        public StatusChangeHandler appcallback_on_status_changed;

        public delegate void MessageHandler(CPacket msg);
        public MessageHandler appcallback_on_message;


        private void Awake()
        {
            CPacketBufferManager.Initialize(20);
            this.netEventManager = new CFreeNetEventManager();
        }
        
        public void connect(string host, int port)
        {
            if(this.cNetworkService != null)
            {
                return;
            }
            this.cNetworkService = new CNetworkService();
            CConnector cConnector = new CConnector(cNetworkService);
            cConnector.connected_callback += on_connected_gameServer;

            IPEndPoint remote_endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            cConnector.connect(remote_endPoint);
        }
        private void on_connected_gameServer(CUserToken token)
        {
            this.gameServer = new CRemoteServerPeer(token);
            ((CRemoteServerPeer)this.gameServer).set_eventManager(this.netEventManager);
            this.netEventManager.enqueue_network_event(NETWORK_EVENT.connected);
        }
        public bool is_connected()
        {
            return this.gameServer != null;
        }


        private void Update()
        {
            if (this.netEventManager.has_event())
            {
                NETWORK_EVENT netEvent = this.netEventManager.dequeue_network_event();
                if(netEvent != null)
                {
                    appcallback_on_status_changed(netEvent);
                }
            }
            if (this.netEventManager.has_message())
            {
                CPacket msg = this.netEventManager.dequeue_net_message();
                if(msg != null)
                {
                    appcallback_on_message(msg);
                }
            }
        }

        public void send(CPacket msg)
        {
            try
            {
                this.gameServer.send(msg);
                CPacket.destroy(msg);
            }
            catch(Exception e)
            {
                Debug.LogError($"CFreeNetUnityService : {e.Message}");
            }
        }

        private void OnApplicationQuit()
        {
            if(this.gameServer != null)
            {
                ((CRemoteServerPeer)this.gameServer).token.disconnect();
            }
        }
    }
}
