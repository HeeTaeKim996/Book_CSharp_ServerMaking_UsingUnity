using System.Collections.Generic;
using System.Diagnostics;
using FreeNet;
using FreeNetUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CGameMain : MonoBehaviour
{
    private CNetworkManager cNetworkManager;
    public TMP_InputField tmp_inputField;
    public Text recv_text;
    private List<string> recv_strings;

    private void Awake()
    {
        cNetworkManager = FindObjectOfType<CNetworkManager>();
        tmp_inputField.onEndEdit.AddListener(On_end_edit);
        recv_strings = new List<string>();
    }

    private void On_end_edit(string msg)
    {
        CPacket send_packet = CPacket.Create((short)PROTOCOL.CHAT_MSG_REQ);
        send_packet.Push(msg);
        cNetworkManager.Send(send_packet);

        tmp_inputField.text = "";
    }
    public void On_receive_chat_message(string recv_message)
    {
        recv_strings.Add(recv_message);
        recv_text.text = string.Join('\n', recv_strings.ToArray());
    }
}