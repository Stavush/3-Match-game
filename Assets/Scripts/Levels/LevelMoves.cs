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

        uiElements.SetLevelType(type);
        uiElements.SetScore(currentScore);
        uiElements.SetTarget(targetScore);
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
