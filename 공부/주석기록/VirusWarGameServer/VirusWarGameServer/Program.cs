using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using FreeNet;

namespace VirusWarGameServer
{
    class Program 
    {
        static List<CGameUser> userList;
        public static CGameServer game_main = new CGameServer();

        static void Main(string[] args)
        {
            CPacketBufferManager.Initialize(2_000);
            userList = new List<CGameUser>();

            CNetworkService service = new CNetworkService();
            service.session_created_callback += on_session_created;
            service.Initialize();
            service.listen("0.0.0.0", 7979, 100);

            Console.WriteLine("Server Started");
            while (true)
            {
                string input = Console.ReadLine();

                Thread.Sleep(1_000);
            }
        }

        private static void on_session_created(CUserToken token)
        {
            CGameUser user = new CGameUser(token);
            lock (userList)
            {
                userList.Add(user);
            }
        }

        public static void Remove_user(CGameUser user)
        {
            lock (userList)
            {
                userList.Remove(user);
                game_main.user_disconnected(user);

                CGameRoom room = user.battle_room;
                if(room != null)
                {
                    game_main.room_manager.remove_room(user.battle_room);
                }
            }
        }
    }

}
