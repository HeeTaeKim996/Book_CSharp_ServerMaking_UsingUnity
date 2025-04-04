using System;
using FreeNet;

namespace VirusWarGameServer
{
    public class CGameUser : IPeer
    {
        CUserToken token;

        public CGameRoom battle_room { get; private set; }

        CPlayer player;

        public CGameUser(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        void IPeer.on_message(Const<byte[]> buffer)
        {
            CPacket msg = new CPacket(buffer.Value, this);    
            //@@@@@@@@@@@@@@@@@@@@@@@@@ 코드를 전체적으로 보면, 메세지를 송신 할 때에는 CPacket.create - CPacket.destroy 로 CPacket을 오브젝트 풀로 관리하여 사용한다. 반면 메세지 수신 때에는 CPacket을 만들고, 작업 완료후 GC가 메모리 수거하게 한다. 
            // 여전히 확실한 이유를 모르겠다. 추정하는 건, 메모리 풀에서 사용하는 CPacket의 buffer는 byte[1024] 이다. 수신 때에는 그보다 크거나, 적은 buffer가 올 수 있기 때문에, 최적화된 크기의 byte[]로 수신하기 위해, 오브젝트 풀을 사용하지 않는 건가 싶다.
            Program.game_main.enqueue_packet(msg, this); 
        }

        void IPeer.on_removed()
        {
            Console.WriteLine("the client disconnected");

            Program.Remove_user(this);
        }

        public void send(CPacket msg)
        {
            this.token.send(msg);
        }
        void IPeer.disconnect() 
        {
            this.token.socket.Disconnect(false);
        }
        void IPeer.process_user_operation(CPacket msg)
        {
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            Console.WriteLine($"Protocol Id : {protocol}");
            switch (protocol)
            {
                case PROTOCOL.ENTER_GAME_ROOM_REQ:
                    {
                        Program.game_main.matching_req(this);
                    }
                    break;
                case PROTOCOL.LOADING_COMPLETED:
                    {
                        this.battle_room.loading_complete(player);
                    }
                    break;
                case PROTOCOL.MOVING_REQ:
                    {
                        short begin_pos = msg.pop_int16();
                        short target_pos = msg.pop_int16();
                        this.battle_room.moving_req(this.player, begin_pos, target_pos);
                    }
                    break;
                case PROTOCOL.TURN_FINISHED_REQ:
                    {
                        this.battle_room.turn_finished(this.player);
                    }
                    break;
            }
        }
        
        public void enter_room(CPlayer player, CGameRoom room)
        {
            this.player = player;
            this.battle_room = room;
        }

    }
}
