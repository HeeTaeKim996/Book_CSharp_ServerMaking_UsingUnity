using System;
using System.Collections.Generic;

namespace VirusWarGameServer
{
    using System.Data.SqlTypes;
    using FreeNet;
    public class CGameRoom
    {
        private enum PLAYER_STATE : byte
        {
            ENTER_ROOM,
            LOADING_COMPLETE,
            READY_TO_TURN,
            CLIENT_TURN_FINISHED
        }

        private List<CPlayer> players;
        private Dictionary<byte, PLAYER_STATE> players_state;
        byte current_turn_player;

        private List<short> gameboard;
        private List<short> table_board;

        private static byte COLUMN_COUNT = 7;

        private readonly short EMPTY_SLOT = short.MaxValue;


        public CGameRoom()
        {
            this.players = new List<CPlayer>();
            this.players_state = new Dictionary<byte, PLAYER_STATE>();
            this.current_turn_player = 0;

            this.gameboard = new List<short>();
            this.table_board = new List<short>();
            for(byte i = 0; i < COLUMN_COUNT * COLUMN_COUNT; i++)
            {
                this.gameboard.Add(EMPTY_SLOT);
                this.table_board.Add(i); 
            }
        }

        private void broadcast(CPacket msg)
        {
            this.players.ForEach(player => player.send_for_broadcast(msg));
            CPacket.destroy(msg);
        }

        private void change_playerstate(CPlayer player, PLAYER_STATE state)
        {
            if (this.players_state.ContainsKey(player.player_index))
            {
                this.players_state[player.player_index] = state;
            }
            else
            {
                this.players_state.Add(player.player_index, state);
            }
        }
        
        private bool allPlayers_ready(PLAYER_STATE state)
        {
            foreach(KeyValuePair<byte, PLAYER_STATE> kvp in players_state)
            {
                if(kvp.Value == state)
                {
                    return true;
                }
            }

            return false;
        }

        public void enter_gameRoom(CGameUser user1, CGameUser user2)
        {
            CPlayer player1 = new CPlayer(user1, 0);
            CPlayer player2 = new CPlayer(user2, 1);

            this.players.Clear();
            this.players.Add(player1);
            this.players.Add(player2);

            this.players_state.Clear();
            this.change_playerstate(player1, PLAYER_STATE.ENTER_ROOM);
            this.change_playerstate(player2, PLAYER_STATE.ENTER_ROOM);

            this.players.ForEach(player =>
            {
                CPacket msg = CPacket.create((short)PROTOCOL.START_LOADING);
                msg.push(player.player_index);
                player.send(msg);
            });

            user1.enter_room(player1, this);
            user2.enter_room(player2, this);
        }

        public void loading_complete(CPlayer player)
        {
            change_playerstate(player, PLAYER_STATE.LOADING_COMPLETE);

            if (!allPlayers_ready(PLAYER_STATE.LOADING_COMPLETE))
            {
                return;
            }


        }

        private void battle_start()
        {
            reset_gameData();
        }
        private void reset_gameData()
        {
            for(int i = 0; i < this.gameboard.Count; i++)
            {
                this.gameboard[i] = EMPTY_SLOT;
            }
            put_virus(0, 0, 0);
            put_virus(0, 0, 6);
            put_virus(1, 6, 0);
            put_virus(1, 6, 6);

            this.current_turn_player = 0;
        }

        private void put_virus(byte player_index, short position)
        {
            this.gameboard[position] = player_index;
            get_player(player_index).add_cell(position);
        }

        private void put_virus(byte player_index, byte row, byte col)
        {
            short position = CHelper.get_position(row, col);
            put_virus(player_index, position);
        }

        private CPlayer get_player(byte player_index)
        {
            return this.players.Find(player => player.player_index == player_index);
        }

        private void remove_virus(byte player_index, short position)
        {
            this.gameboard[position] = EMPTY_SLOT;
            get_player(player_index).remove_cell(position);
        }


        public void infect(short basis_cell, CPlayer attacker, CPlayer victim)
        {
            List<short> neighbors = CHelper.find_neighbor_cells(basis_cell, victim.viruses, 1);
            foreach(short position in neighbors)
            {
                remove_virus(victim.player_index, position);
                put_virus(attacker.player_index, position);
            }
        }


        private CPlayer get_current_player()
        {
            return this.players.Find(player => player.player_index == this.current_turn_player);
        }
        private CPlayer get_oponent_player()
        {
            return this.players.Find(player => player.player_index != this.current_turn_player);
        }

        public void moving_req(CPlayer sender, short begin_pos, short target_pos)
        {
            if (this.current_turn_player != sender.player_index) return;

            if (this.gameboard[begin_pos] != sender.player_index) return;

            if (this.gameboard[target_pos] != EMPTY_SLOT) return;


            short distance = CHelper.get_distance(begin_pos, target_pos);
            if(distance > 2)
            {
                return;
            }
            else if(distance <= 0)
            {
                return;
            }

            if(distance == 1)
            {
                put_virus(sender.player_index, target_pos);
            }
            else if(distance == 2)
            {
                remove_virus(sender.player_index, begin_pos);
                put_virus(sender.player_index, target_pos);
            }


            CPlayer opponent = get_oponent_player();
            infect(target_pos, sender, opponent);


            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_MOVED);
            msg.push(sender.player_index); // 누가
            msg.push(begin_pos); // 어디서
            msg.push(target_pos); // 어디로 이동했는지
            broadcast(msg);
        }

        public void turn_finished(CPlayer sender)
        {
            change_playerstate(sender, PLAYER_STATE.CLIENT_TURN_FINISHED);

            if (!allPlayers_ready(PLAYER_STATE.CLIENT_TURN_FINISHED))
            {
                return;
            }

            turn_end();
        }
        
        private void turn_end()
        {
            if(!CHelper.can_play_more(this.table_board, get_oponent_player(), this.players))
            {
                game_over();
                return;
            }
            if(this.current_turn_player < this.players.Count - 1)
            {
                ++this.current_turn_player;
            }
            else
            {
                this.current_turn_player = this.players[0].player_index;
            }

            start_turn();
        }
        private void start_turn()
        {
            this.players.ForEach(player => change_playerstate(player, PLAYER_STATE.READY_TO_TURN));

            CPacket msg = CPacket.create((short)PROTOCOL.START_PLAYER_TURN);
            msg.push(this.current_turn_player);
            broadcast(msg);
        }
        private void game_over()
        {
            byte win_player_index = byte.MaxValue;

            int count_1p = this.players[0].get_virus_count();
            int count_2p = this.players[01].get_virus_count();

            if(count_1p > count_2p)
            {
                win_player_index = this.players[0].player_index;
            }
            else if(count_1p < count_2p)
            {
                win_player_index = this.players[1].player_index;
            }


            CPacket msg = CPacket.create((short)PROTOCOL.GAME_OVER);
            msg.push(win_player_index);
            msg.push(count_1p);
            msg.push(count_2p);
            broadcast(msg);


            Program.game_main.room_manager.remove_room(this);
        }

        public void destroy()
        {
            CPacket msg = CPacket.create((short)PROTOCOL.ROOM_REMOVED);
            broadcast(msg);

            this.players.Clear();
        }
    }
}
