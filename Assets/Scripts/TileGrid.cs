using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    #region Constants
    private const int TILE_OFFSET = 120;
    #endregion

    #region Serialized Fields
    [SerializeField]
    private Tile _tilePrefab;
    [SerializeField]
    private RectTransform _tilesAnchor;
    #endregion

    #region Properties
    public Dictionary<Vector2Int, Tile> Tiles
    {
        get;
        private set;
    }
    #endregion

    #region Class Members
    private int _gridSize;
    #endregion

    #region Public Methods
    public void InitializeGrid(int gridSize)
    {
        if(Tiles != null)
        {
            return;
        }

        Tiles = new Dictionary<Vector2Int, Tile>();
        _gridSize = gridSize;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var tilePosiotionOffset = new Vector3
                {
                    x = x % gridSize * TILE_OFFSET,
                    y = y % gridSize * TILE_OFFSET,
                    z = 0
                };

                var instanstioaionPosition = _tilesAnchor.position + tilePosiotionOffset;
                Tile tile = Instantiate(_tilePrefab, _tilesAnchor);
                tile.transform.position = instanstioaionPosition;
                tile.InitializeTile(x, y);
                var tilePosition = new Vector2Int { x = x, y = y };
                Tiles.Add(tilePosition, tile);
            }
        }
    }

    public void InitializeTiles()
    {
        foreach (Tile tile in Tiles.Values)
        {
            tile.AdjacentTiles = GetAdjacentTilesHelper(1, tile.TilePosition);
            tile.SecondDegreeAdjacentTiles = GetAdjacentTilesHelper(2, tile.TilePosition);
        }
    }

    public int GetScorePlayerScore(PlayerType player)
    {
        int playerScore = 0;
        foreach (Tile tile in Tiles.Values)
        {
            if (tile.Owner == player)
            {
                playerScore++;
            }
        }
        return playerScore;
    }

    internal void ChangeTileOwner(PlayerType player, Vector2Int tilePosition)
    {
        Tiles[tilePosition].SetOwner(player);
    }
    #endregion

    #region Private Methods
    private Dictionary<Vector2Int, Tile> GetAdjacentTilesHelper(int tilesOffset, Vector2Int tilePosition)
    {
        var adjecentTiles = new Dictionary<Vector2Int, Tile>(new Vector2Comparer());
        var leftEdge = tilePosition.x -  tilesOffset < 0;
        var rightEdge = tilePosition.x +  tilesOffset >= _gridSize;
        var topEdge = tilePosition.y + tilesOffset >= _gridSize;
        var buttomEdge = tilePosition.y - tilesOffset < 0;

        //Left adjecent tiles
        for (int i = tilePosition.y - tilesOffset; i <= tilePosition.y + tilesOffset; i++)
        {
            if (i < 0 || i >= _gridSize || leftEdge)
            {
                continue;
            }

            var tmpPosition = new Vector2Int { x = tilePosition.x - tilesOffset, y = i };
            if (adjecentTiles.ContainsKey(tmpPosition))
            {
                continue;
            }
            adjecentTiles.Add(tmpPosition, Tiles[tmpPosition]);
        }

        //right adjecent tiles
        for (int i = tilePosition.y - tilesOffset; i <= tilePosition.y + tilesOffset; i++)
        {
            if (i < 0 || i  >= _gridSize || rightEdge)
            {
                continue;
            }


            var tmpPosition =  new Vector2Int { x = tilePosition.x + tilesOffset, y = i };
            if (adjecentTiles.ContainsKey(tmpPosition))
            {
                continue;
            }

            adjecentTiles.Add(tmpPosition, Tiles[tmpPosition]);
        }

        //top adjecent tiles
        for (int i = tilePosition.x - tilesOffset; i <= tilePosition.x + tilesOffset; i++)
        {
            if (i < 0 || i  >= _gridSize || topEdge)
            {
                continue;
            }

            var tmpPosition =  new Vector2Int { x = i , y = tilePosition.y + tilesOffset };
            if (adjecentTiles.ContainsKey(tmpPosition))
            {
                continue;
            }

            adjecentTiles.Add(tmpPosition, Tiles[tmpPosition]);
        }

        //buttom adjecent tiles
        for (int i = tilePosition.x - tilesOffset; i <= tilePosition.x + tilesOffset; i++)
        {
            if (i < 0 || i  >= _gridSize || buttomEdge)
            {
                continue;
            }

            var tmpPosition =  new Vector2Int { x = i , y = tilePosition.y - tilesOffset };
            if (adjecentTiles.ContainsKey(tmpPosition))
            {
                continue;
            }

            adjecentTiles.Add(tmpPosition, Tiles[tmpPosition]);
        }
        return adjecentTiles;
    }
    #endregion
}

#region Comperator
public class Vector2Comparer : IEqualityComparer<Vector2Int>
{

    public bool Equals(Vector2Int vec1, Vector2Int vec2)
    {
        return vec1.x == vec2.x && vec1.y == vec2.y;;
    }

    public int GetHashCode(Vector2Int vec)
    {
        return Mathf.FloorToInt(vec.x) ^ Mathf.FloorToInt(vec.y) << 2;
    }
}
#endregion