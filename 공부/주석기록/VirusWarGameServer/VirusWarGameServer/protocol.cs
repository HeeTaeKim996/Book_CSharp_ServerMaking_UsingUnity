
namespace VirusWarGameServer
{
    public enum PROTOCOL : short 
    {
        BEGIN = 0,

        START_LOADING = 1,

        LOADING_COMPLETED = 2,

        GAME_START = 3,

        START_PLAYER_TURN = 4,

        MOVING_REQ = 5,

        PLAYER_MOVED = 6,

        TURN_FINISHED_REQ = 7,

        ROOM_REMOVED = 8,

        ENTER_GAME_ROOM_REQ = 9,

        GAME_OVER = 10,

        END
    }

}
