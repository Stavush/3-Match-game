using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType{
        TIMER, 
        OBSTACLE,
        MOVES,
    }

    public Board board;
    public UiElements uiElements;

    public int score1Star;
    public int score2Star;
    public int score3Star;


    protected LevelType type;

    public LevelType Type
    {
        get { return type; }
    }

    protected int currentScore;

    protected bool didWin;

    // Start is called before the first frame update
    void Start()
    {
        uiElements.SetScore(currentScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void WinGame()
    {
        board.GameOver();
        didWin = true;
        uiElements.OnGameWin(currentScore);
    }

    public virtual void LoseGame()
    {
        board.GameOver();
        didWin = false;
        uiElements.OnGameLose(currentScore);
    }


    public virtual void OnTileSwap()
    {
        Debug.Log("Swap has occured");
    }

    public virtual void OnTileClear(Tile tile) 
    {
        // A function that updates score
        currentScore += tile.Score;
        uiElements.SetScore(currentScore);
    }
}
