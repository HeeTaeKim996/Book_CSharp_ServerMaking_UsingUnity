using System.Collections.Generic;
using System.Threading;

namespace VirusWarGameServer
{
    using FreeNet;

    internal class CGameServer
    {
        object cs_operation;
        Queue<CPacket> user_operations;

        Thread logic_thread;
        AutoResetEvent loop_event;

        public CGameRoomManager room_manager { get; private set; }
        List<CGameUser> matching_waiting_users;


        public CGameServer()
        {
            this.cs_operation = new object();
            this.loop_event = new AutoResetEvent(false);
            this.user_operations = new Queue<CPacket>();

            this.room_manager = new CGameRoomManager();
            this.matching_waiting_users = new List<CGameUser>();

            this.logic_thread = new Thread(gameloop);
            this.logic_thread.Start();
        }
        private void gameloop()
        {
            while (true)
            {
                CPacket packet = null;

                lock (cs_operation)
                {
                    if (user_operations.Count > 0)
                    {
                        packet = user_operations.Dequeue();
                    }
                }

                if (packet != null)
                {
                    process_receive(packet);
                }

                if (user_operations.Count <= 0)
                {
                    loop_event.WaitOne();
                }
            }
        }
        private void process_receive(CPacket msg)
        {
            msg.owner.process_user_operation(msg);
        }


        public void enqueue_packet(CPacket packet, CGameUser user)
        {
            lock (cs_operation)
            {
                this.user_operations.Enqueue(packet);
            }
            this.loop_event.Set();
        }

        public void matching_req(CGameUser user)
        {
            if (this.matching_waiting_users.Contains(user)) return;

            matching_waiting_users.Add(user);

            if(matching_waiting_users.Count == 2)
            {
                this.room_manager.create_room(this.matching_waiting_users[0], this.matching_waiting_users[1]);
                this.matching_waiting_users.Clear();
            }
        }

        public void user_disconnected(CGameUser user)
        {
            if (this.matching_waiting_users.Contains(user))
            {
                this.matching_waiting_users.Remove(user);
            }
        }
    }
}
