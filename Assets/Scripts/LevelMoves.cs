using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    
    public int movesNumber;
    public int targetScore;

    private int movesUsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.MOVES;
        Debug.Log("Number of moves:" + movesNumber +" Target score: " + targetScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnTileSwap()
    {
        movesUsed++;

        Debug.Log("Moves remaining" + (movesNumber - movesUsed));

        if (movesNumber - movesUsed == 0)
        {
            if (currentScore >= targetScore)
            {
                WinGame();
            }

            else
            {
                LoseGame();
            }
        }
    }
}
