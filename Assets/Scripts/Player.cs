using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Dictionary<Vector2Int,Tile> PlayerTiles;
}

public enum PlayerType
{
    Available,
    Player1,
    Player2,
}
