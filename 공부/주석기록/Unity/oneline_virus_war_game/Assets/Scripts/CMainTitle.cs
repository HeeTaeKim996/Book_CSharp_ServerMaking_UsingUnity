using System.Collections;
using System.Collections.Generic;
using FreeNet;
using UnityEditor;
using UnityEngine;
using VirusWarGameServer;

public class CMainTitle : MonoBehaviour
{
    private enum USER_STATE 
    {
        NOT_CONNECTED,
        CONNECTED,
        WAITING_MATCH
    }

    private Texture bg;
    private CBattleRoom battle_room;

    private CNetworkManager network_manager;
    private USER_STATE user_state;

    private Texture waiting_img;

    private Coroutine coroutine_after_connected;

    private void Start()
    {
        this.user_state = USER_STATE.NOT_CONNECTED;
        this.bg = Resources.Load("images/title_blue") as Texture;
        this.battle_room = FindObjectOfType<CBattleRoom>();
        this.battle_room.gameObject.SetActive(false);

        this.network_manager = FindObjectOfType<CNetworkManager>();
        this.waiting_img = Resources.Load("images/waiting") as Texture;

        this.user_state = USER_STATE.NOT_CONNECTED;
        enter();
    }
    private void OnEnable()
    {
        this.network_manager.on_message_CNetworkManager += on_recv;
    }
    private void OnDisable()
    {
        this.network_manager.on_message_CNetworkManager -= on_recv;
    }

    public void enter()
    {
        if (coroutine_after_connected != null)
        {
            StopCoroutine(coroutine_after_connected);
        }


        if (!this.network_manager.is_connected())
        {
            this.network_manager.connect();
        }
        else
        {
            on_connected();
        }
    }
    public void on_connected()
    {
        this.user_state = USER_STATE.CONNECTED;
        coroutine_after_connected =  StartCoroutine(after_connected());
    }
    private IEnumerator after_connected()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if(this.user_state == USER_STATE.CONNECTED)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    this.user_state = USER_STATE.WAITING_MATCH;

                    CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
                    this.network_manager.send(msg);


                    coroutine_after_connected = null;
                    yield break;
                }
            }

            yield return null;
        }
    }
    
    public void on_recv(CPacket msg)
    {
        PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

        switch (protocol_id)
        {
            case PROTOCOL.START_LOADING:
                {
                    byte player_index = msg.pop_byte();

                    this.battle_room.gameObject.SetActive(true);
                    this.battle_room.start_loading(player_index);
                    gameObject.SetActive(false);
                }
                break;
        }
    }


    private void OnGUI()
    {
        switch (this.user_state)
        {
            case USER_STATE.NOT_CONNECTED:
                break;

            case USER_STATE.CONNECTED:
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.bg);
                break;

            case USER_STATE.WAITING_MATCH:
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.bg);
                GUI.DrawTexture(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 82),
                    this.waiting_img);
                break;
        }
    }

}
