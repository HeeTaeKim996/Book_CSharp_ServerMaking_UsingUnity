using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;
using FreeNetUnity;
using System;


namespace FreeNetUnity
{
    public class CRemoteServerPeer : IPeer
    {
        public CUserToken token { get; private set; }
        private WeakReference weakRef_netEventManager;

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }
        public void set_eventManager(CFreeNetEventManager netEventManager)
        {
            this.weakRef_netEventManager = new WeakReference(netEventManager);
        }


        void IPeer.on_message(Const<byte[]> buffer)
        {
            CPacket msg = new CPacket(buffer.Value, this);
            (this.weakRef_netEventManager.Target as CFreeNetEventManager).enqueue_net_message(msg);
        }

        void IPeer.send(CPacket msg)
        {
            this.token.send(msg);  // @@ 해당 매서드를 호출하는 CFreeNetEventManager에서, msg.Destroy 호출 
        }
        void IPeer.on_removed()
        {
            (this.weakRef_netEventManager.Target as CFreeNetEventManager).enqueue_network_event(NETWORK_EVENT.disconnected);
        }

        void IPeer.disconnect() { }

        void IPeer.process_user_operation(FreeNet.CPacket msg) { }

    }
}

