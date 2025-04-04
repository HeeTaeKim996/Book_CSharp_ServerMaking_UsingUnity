using System;
using System.Collections.Generic;

namespace VirusWarGameServer
{
    public static class CHelper
    {
        private static byte COLUMN_COUNT = 7;



        private static Vector2 convert_to_xy(short position)
        {
            return new Vector2(calc_row(position), calc_col(position));
        }
        public static short calc_row(short position)
        {
            return (short)(position / COLUMN_COUNT);
        }
        public static short calc_col(short position)
        {
            return (short)(position % COLUMN_COUNT);
        }
        


        public static short get_position(byte row, byte col)
        {
            return (short)(row * COLUMN_COUNT + col);
        }


        
        public static short get_distance(Vector2 pos1, Vector2 pos2)
        {
            Vector2 distance = pos1 - pos2;
            return Math.Max((short)Math.Abs(distance.x), (short)Math.Abs(distance.y));
        }
        public static short get_distance(short pos1, short pos2)
        {
            Vector2 vecPos1 = convert_to_xy(pos1);
            Vector2 vecPos2 = convert_to_xy(pos2);

            return get_distance(vecPos1, vecPos2);
        }


        public static byte howfar_from_clicked_cell(short basis_cell, short cell)
        {
            short row = (short)(basis_cell / COLUMN_COUNT);
            short col = (short)(basis_cell % COLUMN_COUNT);
            Vector2 basisVec = new Vector2(row, col);

            row = (short)(cell / COLUMN_COUNT);
            col = (short)(cell % COLUMN_COUNT);
            Vector2 cellVec = new Vector2(row, col);

            Vector2 distanceVec = basisVec - cellVec;
            return Math.Max((byte)Math.Abs(distanceVec.x), (byte)Math.Abs(distanceVec.y));
        }


        public static List<short> find_neighbor_cells(short basis_cell, List<short> targets, short gap)
        {
            Vector2 pos = convert_to_xy(basis_cell);
            return targets.FindAll(s => get_distance(pos, convert_to_xy(s)) <= gap);
        }

        public static List<short> find_avaiable_cells(short basis_cell, List<short> total_cells, List<CPlayer> players)
        {
            List<short> targets = find_neighbor_cells(basis_cell, total_cells, 2);
            players.ForEach(player =>
            targets.FindAll(targets_short => player.viruses.Exists(virus_short => virus_short == targets_short))
            );

            return targets;
        }

        public static bool can_play_more(List<short> board, CPlayer current_player, List<CPlayer> all_player)
        {
            foreach(short cell in current_player.viruses)
            {
                if (find_avaiable_cells(cell, board, all_player).Count > 0) return true;
            }
            return false;
        }
    }
}
