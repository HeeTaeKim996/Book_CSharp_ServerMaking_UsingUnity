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
                Debug.LogError("이미 서버와 연결됨");
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
                // CPacket.Destroy 추가. 코드 보면 서버로부터 받을 때 Const<byte[]> buffer 인 struct로 받고, 이후 CPacket.Create로 풀에서 CPacket을 가져옴. 그런데 메세지를 수신할 때 CPacket.Destroy가 누락된것 같아 추가함
                // 하단 코드 보면 알다시피 기존 교재에도 메세지를 보낼 때에는 CPacket.Destroy 사용
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
                CPacket.Destroy(msg); // 교재에는 try내에 Destroy가 위치하는데, 왜그렇게 했는지 이해 안됨. finally로 옮김
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