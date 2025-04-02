using UnityEngine;
using FreeNet;
using FreeNetUnity;
using System.Collections;


public class CNetworkManager : MonoBehaviour
{
    private CFreeNetUnityService cFreeNetUnityService;
    private CGameMain cGameMain;

    private void Awake()
    {
        this.cFreeNetUnityService = GetComponent<CFreeNetUnityService>();
        cFreeNetUnityService.callback_on_statusChanged += On_statusChanged;
        cFreeNetUnityService.callback_on_message += On_message;

        cGameMain = FindObjectOfType<CGameMain>();
    }
    private void Start()
    {
        Connect();
    }
    private void On_statusChanged(NETWORK_EVENT netEvent)
    {
        switch (netEvent)
        {
            case NETWORK_EVENT.connected:
                {
                    Debug.Log("서버와 연결됨");

                    CPacket msg = CPacket.Create((short)PROTOCOL.CHAT_MSG_REQ);
                    msg.Push("클라이언트에서, 서버와 처음 연결시 보내는 메세지입니다");
                    cFreeNetUnityService.Send(msg);
                }
                break;
            case NETWORK_EVENT.disconnected:
                {
                    Debug.Log("서버와 연결이 해제되었습니다");
                }
                break;
        }
    }
    private void On_message(CPacket msg)
    {
        PROTOCOL protocol = (PROTOCOL)msg.Pop_protocol_id();

        switch (protocol)
        {
            case PROTOCOL.CHAT_MSG_ACK:
                {
                    string recv_string = msg.Pop_string();
                    cGameMain.On_receive_chat_message(recv_string);
                }
                break;
        }
    }
    public void Send(CPacket msg)
    {
        cFreeNetUnityService.Send(msg);
    }
    private void Connect()
    {
        cFreeNetUnityService.Connect("127.0.0.1", 7979);
    }
    public bool is_connected()
    {
        return cFreeNetUnityService.is_connected();
    }
}