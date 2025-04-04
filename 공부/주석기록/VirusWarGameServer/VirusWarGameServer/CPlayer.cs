using System;
using System.Collections.Generic;


namespace VirusWarGameServer
{
    using System.Linq;
    using FreeNet;

    public class CPlayer
    {
        CGameUser owner;

        public byte player_index { get; private set; }
        public List<short> viruses { get; private set; }
        public CPlayer(CGameUser owner, byte player_index)
        {
            this.owner = owner;
            this.player_index = player_index;
            this.viruses = new List<short>();
        }
        
        public void reset()
        {
            this.viruses.Clear();
        }
        public void add_cell(short position)
        {
            this.viruses.Add(position);
        }
        public void remove_cell(short position)
        {
            this.viruses.Remove(position);
        }
        public void send(CPacket msg)
        {
            this.owner.send(msg);
            CPacket.destroy(msg);
        }

        public void send_for_broadcast(CPacket msg)
        {
            this.owner.send(msg);
        }
        public int get_virus_count()
        {
            return this.viruses.Count();
        }
    }
}
