using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;
using VirusWarGameServer;

public class CBattleRoom : MonoBehaviour
{
    private enum GAME_STATE
    {
        READY = 0,
        STARTED
    }
    public static readonly int COL_COUNT = 7;

    private List<CPlayer> players;

    private List<short> board;

    private List<short> table_board;

    private List<short> avaiable_attack_cells;

    private byte current_player_index;

    private byte player_me_index;

    private byte step;

    private CMainTitle main_title;

    private CNetworkManager cNetworkManager;

    private GAME_STATE game_state;

    private delegate void GUIFUNC();
    private GUIFUNC draw;

    private byte win_player_index;

    private CImageNumber score_images;

    private CBattleInfoPanel battle_info;

    private bool is_game_finished;

    List<Texture> img_players;
    Texture background;
    Texture blank_image;
    Texture game_board;

    Texture graycell;
    Texture focus_cell;

    Texture win_img;
    Texture lose_img;
    Texture draw_img;
    Texture gray_transparent;

    private void Awake()
    {
        this.table_board = new List<short>();
        this.board = new List<short>();

        this.avaiable_attack_cells = new List<short>();

        this.cNetworkManager = FindObjectOfType<CNetworkManager>();

        this.game_state = GAME_STATE.READY;

        this.main_title = FindObjectOfType<CMainTitle>();
        this.score_images = gameObject.AddComponent<CImageNumber>();

        this.win_player_index = byte.MaxValue;
        this.draw = this.on_gui_playing;
        this.battle_info = gameObject.AddComponent<CBattleInfoPanel>();

        this.graycell = Resources.Load("images/graycell") as Texture;
        this.focus_cell = Resources.Load("images/border") as Texture;

        this.blank_image = Resources.Load("images/blank") as Texture;
        this.game_board = Resources.Load("images/gameboard") as Texture;
        this.background = Resources.Load("images/gameboard_bg") as Texture;
        this.img_players = new List<Texture>();
        this.img_players.Add(Resources.Load("images/red") as Texture);
        this.img_players.Add(Resources.Load("images/blue") as Texture);

        this.win_img = Resources.Load("images/win") as Texture;
        this.lose_img = Resources.Load("images/lose") as Texture;
        this.draw_img = Resources.Load("images/draw") as Texture;
        this.gray_transparent = Resources.Load("images/gray_transparent") as Texture;
    }

    private void reset()
    {
        this.board.Clear();
        this.table_board.Clear();
        for(int i = 0; i < COL_COUNT; i++)
        {
            this.board.Add(short.MaxValue);
            this.table_board.Add((short)i);
        }

        this.players.ForEach(player =>
        {
            player.
        })
    }
}
