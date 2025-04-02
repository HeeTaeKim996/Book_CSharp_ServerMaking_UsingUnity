using UnityEngine;
using System;
using System.Net;
using FreeNet;


namespace FreeNetUnity
{
    public class CFreeNetUnityService : MonoBehaviour
    {
        public CFreeNetEventManager netEventManager;
        private CNetworkService cNetworkService;
        private IPeer gameServer;

        public delegate void StatusChangeHandler(NETWORK_EVENT netEvent);
        public StatusChangeHandler callback_on_statusChanged;

        public delegate void MessageHandler(CPacket msg);
        public MessageHandler callback_on_message;



        private void Awake()
        {
            CPacketBufferManager.Initialize(20);
            this.netEventManager = new CFreeNetEventManager();
        }

        public void Connect(string host, int port)
        {
            if(this.cNetworkService != null)
            {
                Debug.LogError("�̹� ������ �����");
                return;
            }

            this.cNetworkService = new CNetworkService();
            CConnector cConnector = new CConnector(cNetworkService);
            cConnector.connected_callback += On_connected_gameServer;

            IPEndPoint remote_endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            cConnector.Connect(remote_endPoint);
        }
        private void On_connected_gameServer(CUserToken token)
        {
            this.gameServer = new CRemoteServerPeer(token);
            ((CRemoteServerPeer)this.gameServer).Set_eventManager(this.netEventManager);
            this.netEventManager.enqueue_networkEvent(NETWORK_EVENT.connected);
        }

        private void Update()
        {
            if (netEventManager.has_networkEvent())
            {
                NETWORK_EVENT netEvent = netEventManager.dequeue_networkEvent();
                callback_on_statusChanged(netEvent);
            }
            if (netEventManager.has_networkMessage())
            {
                CPacket msg = netEventManager.dequeue_networkMessage();
                callback_on_message(msg);
                CPacket.Destroy(msg);
                // CPacket.Destroy �߰�. �ڵ� ���� �����κ��� ���� �� Const<byte[]> buffer �� struct�� �ް�, ���� CPacket.Create�� Ǯ���� CPacket�� ������. �׷��� �޼����� ������ �� CPacket.Destroy�� �����Ȱ� ���� �߰���
                // �ϴ� �ڵ� ���� �˴ٽ��� ���� ���翡�� �޼����� ���� ������ CPacket.Destroy ���
            }
        }

        public void Send(CPacket msg)
        {
            try
            {
                this.gameServer.Send(msg);
            }
            catch(Exception e)
            {
                Debug.Log($"CFreeNetUnityService : {e.Message}");
            }
            finally
            {
                CPacket.Destroy(msg); // ���翡�� try���� Destroy�� ��ġ�ϴµ�, �ֱ׷��� �ߴ��� ���� �ȵ�. finally�� �ű�
            }
        }
        public bool is_connected()
        {
            return this.gameServer != null;
        }
        private void OnApplicationQuit()
        {
            if(this.gameServer != null)
            {
                ((CRemoteServerPeer)this.gameServer).token.Disconnect();
            }
        }


    }

}