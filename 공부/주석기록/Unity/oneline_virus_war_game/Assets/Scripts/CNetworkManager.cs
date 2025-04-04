using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;
using FreeNetUnity;
using System;

public class CNetworkManager : MonoBehaviour
{
    private CFreeNetUnityService cFreeNetUnityService;
    private string recv_msg;

    public event Action<CPacket> on_message_CNetworkManager;

    private CMainTitle cMainTitle;

    private void Awake()
    {
        cMainTitle = FindObjectOfType<CMainTitle>();
        this.recv_msg = "";
        this.cFreeNetUnityService = GetComponent<CFreeNetUnityService>();
        this.cFreeNetUnityService.appcallback_on_status_changed += on_status_changed;
        this.cFreeNetUnityService.appcallback_on_message += on_message;
    }
    private void on_status_changed(NETWORK_EVENT status)
    {
        switch (status)
        {
            case NETWORK_EVENT.connected:
                {
                    Debug.Log("접속성공");
                    this.recv_msg += "on_connected";
                    cMainTitle.on_connected();
                }
                break;
            case NETWORK_EVENT.disconnected:
                {
                    Debug.Log("네트워크와 연결이 해제됨");
                    this.recv_msg = "disconnected\n";
                }
                break;
        }
    }
    private void on_message(CPacket msg)
    {
        this.on_message_CNetworkManager?.Invoke(msg);
    }
    public void send(CPacket msg)
    {
        this.cFreeNetUnityService.send(msg);
    }

    public void connect()
    {
        this.cFreeNetUnityService.connect("127.0.0.1", 7979);
    }
    public bool is_connected()
    {
        return this.cFreeNetUnityService.is_connected();
    }
}
