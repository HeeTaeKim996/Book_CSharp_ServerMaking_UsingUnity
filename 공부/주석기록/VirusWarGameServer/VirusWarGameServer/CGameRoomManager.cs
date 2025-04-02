using System.Collections.Generic;


namespace VirusWarGameServer
{
    public class CGameRoomManager
    {
        List<CGameRoom> rooms;

        public CGameRoomManager()
        {
            rooms = new List<CGameRoom>();
        }
        public void create_room(CGameUser user1, CGameUser user2)
        {
            CGameRoom battleRoom = new CGameRoom();
            battleRoom.enter_gameroom(user1, user2);

            this.rooms.Add(battleRoom);
        }
        public void remove_room(cGameRoom room)
        {
            room.destroy();
            this.rooms.Remove(room);
        }
    }
}
