using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    #region Constants
    private const string PLAYER_1 = "Player1 ";
    private const string PLAYER_2 = "Player2 ";
    private const string WON = "Won!!!  ";
    private const string TIE = "OMG It's a tie!!!   ";
    private const string GAME_OVER = "GameOver";
    #endregion

    #region Serialized fields
    [SerializeField]
    private TileGrid _tileGrid;
    [SerializeField]
    private TextMeshProUGUI _player1Score;
    [SerializeField]
    private TextMeshProUGUI _player2Score;
    [SerializeField]
    private GameObject _playAgainButton;
    [SerializeField]
    private GameObject _mainMenuButton;
    [SerializeField]
    private int _gridSize;
    [SerializeField]
    public float _moveTime = 0.5f;
    #endregion

    #region Class Members
    private PlayerType _playerTurn;
    private bool _gameInSession;
    private List<Player> _players;
    private float time = 0.0f;
    private Tile _currentTile;
    private Tile _addedTile;
    #endregion

    #region Unity Methods
    private void Start()
    {
        StartGame();
    }

    private void Update()
    {

        if (!_gameInSession)
        {
            return;
        }

        time += Time.deltaTime;

        if (time >= _moveTime)
        {
            time = 0.0f;
            MakeMove();
            CheckPlayersScore();
            SwitchTurns();
        }
    }
    #endregion

    #region Buttons Functions
    public void PlayAgain()
    {
        ResetGrid();
        InstantiatePlayers();
        _gameInSession = true;
        _playAgainButton.SetActive(false);
        _mainMenuButton.SetActive(false);
        _player2Score.color = Color.white;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    #endregion

    #region Private Methods
    private void StartGame()
    {
        _tileGrid.InitializeGrid(_gridSize);
        _tileGrid.InitializeTiles();
        InstantiatePlayers();
        _gameInSession = true;
        _playAgainButton.SetActive(false);
        _mainMenuButton.SetActive(false);
    }

    private void MakeMove()
    {
        var adjcentTileMove = GetValidAdjcentTiles(_playerTurn);
        var secondaryAdjcentTileMove = GetValidSecondaryAdjcentTiles(_playerTurn);
        if (adjcentTileMove != null && secondaryAdjcentTileMove != null)
        {
            Random r = new Random();
            var randomMove = r.Next(0, 2);
            if (randomMove < 0.5)
            {
                _addedTile = _tileGrid.Tiles[adjcentTileMove.TilePosition];
                ChangeTileOwner(_playerTurn, _addedTile.TilePosition);
                ConvertAdjcentTiles(_playerTurn, _addedTile);
                return;
            }
            else
            {
                _addedTile = _tileGrid.Tiles[secondaryAdjcentTileMove.TilePosition];
                ChangeTileOwner(_playerTurn, _addedTile.TilePosition);
                ConvertAdjcentTiles(_playerTurn, _addedTile);
                ResetTileOwner(_playerTurn, _currentTile.TilePosition);
                return;
            }
        }
    }

    private void CheckPlayersScore()
    {
        var player1Score = _tileGrid.GetScorePlayerScore(PlayerType.Player1);
        var player2Score = _tileGrid.GetScorePlayerScore(PlayerType.Player2);

        _player1Score.text = PLAYER_1 + ": " + player1Score;
        _player2Score.text = PLAYER_2 + ": " + player2Score;

        if (player1Score + player2Score == Mathf.Pow(_gridSize, 2))
        {
            _gameInSession = false;
            _player1Score.text = GAME_OVER;
            if (player1Score > player2Score)
            {
                _player2Score.color = Color.red;
                _player2Score.text = PLAYER_1 + WON + player1Score + " : " + player2Score;

            }
            else if(player1Score < player2Score)
            {
                _player2Score.color = Color.blue;
                _player2Score.text = PLAYER_1 + WON + player2Score + " : " + player1Score;
            }
            else
            {
                _player2Score.text = TIE + player2Score + " : " + player1Score;
            }
            _playAgainButton.SetActive(true);
            _mainMenuButton.SetActive(true);
        }
    }
    private void InstantiatePlayers()
    {
        _players = new List<Player>();
        var player1 = new Player();
        var player2 = new Player();
        player1.PlayerTiles = new Dictionary<Vector2Int, Tile>();
        player2.PlayerTiles = new Dictionary<Vector2Int, Tile>();
        _players.Add(player1);
        _players.Add(player2);
        AssignStartingTiles();
        _playerTurn = PlayerType.Player1;
    }

    private void AssignStartingTiles()
    {
        var gridButtomLeft = new Vector2Int { x = 0, y = 0 };
        var gridButtomRight = new Vector2Int { x = _gridSize - 1, y = 0 };
        var gridTopLeft = new Vector2Int { x = 0, y = _gridSize - 1 };
        var gridTopRight = new Vector2Int { x = _gridSize - 1, y = _gridSize - 1 };
        ChangeTileOwner(PlayerType.Player1, gridButtomLeft);
        ChangeTileOwner(PlayerType.Player1, gridTopRight);
        ChangeTileOwner(PlayerType.Player2, gridButtomRight);
        ChangeTileOwner(PlayerType.Player2, gridTopLeft);
    }

    private void ChangeTileOwner(PlayerType player, Vector2Int tilePosition)
    {
        _tileGrid.ChangeTileOwner(player, tilePosition);
        var tileToAdd = _tileGrid.Tiles[tilePosition];
        if (player == PlayerType.Player1)
        {
            if (!_players[0].PlayerTiles.ContainsKey(tilePosition))
            {
                _players[0].PlayerTiles.Add(tilePosition, tileToAdd);
            }
        }
        else if (player == PlayerType.Player2)
        {
            if (!_players[1].PlayerTiles.ContainsKey(tilePosition))
            {
                _players[1].PlayerTiles.Add(tilePosition, tileToAdd);
            }
        }
    }

    private void ResetTileOwner(PlayerType player, Vector2Int tilePosition)
    {
        if (player == PlayerType.Player1)
        {
            _players[0].PlayerTiles.Remove(tilePosition);
        }
        else if (player == PlayerType.Player2)
        {
            _players[1].PlayerTiles.Remove(tilePosition);
        }
        _tileGrid.ChangeTileOwner(PlayerType.Available, tilePosition);
    }

    private void ResetGrid()
    {
        foreach(Tile tile in _tileGrid.Tiles.Values)
        { 
            _tileGrid.ChangeTileOwner(PlayerType.Available, tile.TilePosition);
        }
    }

    private void ConvertAdjcentTiles(PlayerType player, Tile addedTile)
    {
        //Debug.Log("Adjcent Tiles: " + addedTile.AdjacentTiles.Values.Count);
        foreach (Tile tile in addedTile.AdjacentTiles.Values)
        {
            if (tile.Owner != player && tile.Owner != PlayerType.Available)
            {
                ChangeTileOwner(player, tile.TilePosition);
            }
        }
    }

    private Tile GetValidAdjcentTiles(PlayerType playerTurn)
    {
       Dictionary<Vector2Int,Tile> playerTiles = new Dictionary<Vector2Int,Tile>();
       if (playerTurn == PlayerType.Player1)
        {
            playerTiles = _players[0].PlayerTiles;
        }
       else if(playerTurn == PlayerType.Player2)
        {
            playerTiles = _players[1].PlayerTiles;
        }

       foreach (Tile tile in playerTiles.Values)
        {
            Random random = new Random();
            var adjacentTiles = tile.AdjacentTiles;
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                int randomTile = random.Next(0, adjacentTiles.Count);
                if (adjacentTiles.Values.ElementAt(randomTile).Owner == PlayerType.Available)
                {
                    _currentTile = tile;
                    return adjacentTiles.Values.ElementAt(randomTile);
                }
            }
        }

        return null;
    }

    private Tile GetValidSecondaryAdjcentTiles(PlayerType playerTurn)
    {
        Dictionary<Vector2Int, Tile> playerTiles = new Dictionary<Vector2Int, Tile>();
        if (playerTurn == PlayerType.Player1)
        {
            playerTiles = _players[0].PlayerTiles;
        }
       else if(playerTurn == PlayerType.Player2)
        {
            playerTiles = _players[1].PlayerTiles;
        }

       foreach (Tile tile in playerTiles.Values)
        {
            Random random = new Random();
            var secondDegreeAdjacentTiles = tile.SecondDegreeAdjacentTiles;
            for (int i = 0; i < secondDegreeAdjacentTiles.Count; i++)
            {
                var randomTile = random.Next(0, secondDegreeAdjacentTiles.Count);
                if (secondDegreeAdjacentTiles.Values.ElementAt(randomTile).Owner == PlayerType.Available)
                {
                    _currentTile = tile;
                    return secondDegreeAdjacentTiles.Values.ElementAt(randomTile);
                }
            }
        }

        return null;
    }

    private void SwitchTurns()
    {
        if (_playerTurn == PlayerType.Player1)
        {
            _playerTurn = PlayerType.Player2;
        }
        else if( _playerTurn == PlayerType.Player2)
        {
            _playerTurn = PlayerType.Player1;
        }
    }
    #endregion
}
