using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Board;

public class LevelObstacles : Level
{
    [SerializeField] int movesNumber;
    private int movesUsed = 0;

    [SerializeField] Board.TileType[] obstacleTiles;
    private int obstacleLeft;


    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.OBSTACLE;
        for(int i = 0; i < obstacleTiles.Length; i++)
        {
            obstacleLeft += board.GetTilesOfType(obstacleTiles[i]).Count;
        }

        uiElements.SetLevelType(type);
        uiElements.SetScore(currentScore);
        uiElements.SetTarget(obstacleLeft);
        uiElements.SetRemaining(movesNumber.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnTileSwap()
    {
        movesUsed++;
        uiElements.SetRemaining((movesNumber - movesUsed).ToString());

        if(movesNumber - movesUsed == 0 && obstacleLeft > 0)
        {
            LoseGame();
        }

    }

    public override void OnTileClear(Tile tile)
    {
        base.OnTileClear(tile);

        for(int i = 0;i < obstacleTiles.Length; i++)
        {
            if (obstacleTiles[i] == tile.Type)
            {
                obstacleLeft--;
                uiElements.SetTarget(obstacleLeft);

                if(obstacleLeft == 0)
                {
                    currentScore += 1000 * (movesNumber - movesUsed);
                    uiElements.SetScore(currentScore);
                    WinGame();
                }
            }
        }
    }
}
