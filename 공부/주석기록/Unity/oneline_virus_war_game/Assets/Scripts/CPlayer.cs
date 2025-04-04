using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PLAYER_STATE
{
    HUMAN,
    AI
}

public class CPlayer : MonoBehaviour
{
    public List<short> cell_indexes { get; private set; }
    public byte player_index { get; private set; }
    public PLAYER_STATE state { get; private set; }
    private CPlayer
}
