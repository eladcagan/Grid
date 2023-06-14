using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Image _tileUI;

    #region Properties
    public Dictionary<Vector2Int, Tile> AdjacentTiles
    {
        get;
        set;
    }

    public Dictionary<Vector2Int, Tile> SecondDegreeAdjacentTiles
    {
        get;
        set;
    }

    public PlayerType Owner
    {
        get;
        private set;
    }

    public Vector2Int TilePosition
    {
        get;
        private set;
    }
    #endregion

    public void InitializeTile(int coordinateX, int coordinateY)
    {
        SetTilePosition(coordinateX, coordinateY);
    }

    private void SetTilePosition(int coordinateX, int coordinateY)
    {
        Vector2Int tilePosition = new Vector2Int
        {
            x = coordinateX,
            y = coordinateY
        };

        TilePosition = tilePosition;
    }

    public void SetOwner(PlayerType player)
    {
        Owner = player;
        UpdateTileVisuals(player);
    }


    private void UpdateTileVisuals(PlayerType player)
    {
        switch (player)
        {
            case PlayerType.Player1:
                _tileUI.color = Color.red;
                break;
            case PlayerType.Player2:
                _tileUI.color = Color.blue;
                break;
            case PlayerType.Available:
                _tileUI.color = Color.white;
                break;
        }
    }
}
