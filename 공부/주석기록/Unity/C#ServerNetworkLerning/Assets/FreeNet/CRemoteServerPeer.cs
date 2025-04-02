using System;
using FreeNet;


namespace FreeNetUnity
{
    public class CRemoteServerPeer : IPeer
    {
        public CUserToken token;
        private WeakReference weakRef_eventManager;

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            token.Set_peer(this);
        }

        public void Set_eventManager(CFreeNetEventManager eventManager)
        {
            weakRef_eventManager = new WeakReference(eventManager);
        }

        void IPeer.On_message(FreeNet.Const<byte[]> buffer)
        {
            byte[] recvBuffer = new byte[buffer.Value.Length];
            Buffer.BlockCopy(buffer.Value, 0, recvBuffer, 0, buffer.Value.Length);
            CPacket recv_message = new CPacket(recvBuffer, this);
            (weakRef_eventManager.Target as CFreeNetEventManager).enqueue_networkMessage(recv_message);
        }
        void IPeer.Send(CPacket msg)
        {
            this.token.Send(msg);
        }
        void IPeer.On_removed()
        {
            (this.weakRef_eventManager.Target as CFreeNetEventManager).enqueue_networkEvent(NETWORK_EVENT.disconnected);
        }
        void IPeer.Disconnect()
        {

        }
        void IPeer.Process_user_operation(FreeNet.CPacket msg)
        {

        }
    }
}