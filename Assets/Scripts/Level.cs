using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType{
        TIMER, 
        OBSTACLE,
        MOVES,
    }

    public Board board;

    [SerializeField] private int score1Star;
    [SerializeField] private int score2Star;
    [SerializeField] private int score3Star;


    protected LevelType type;

    public LevelType Type
    {
        get { return type; }
    }

    protected int currentScore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void WinGame()
    {
        Debug.Log("You win!");
        board.GameOver();
    }

    public virtual void LoseGame()
    {
        Debug.Log("You Lose...");
        board.GameOver();
    }

    public virtual void OnTileSwap()
    {
        Debug.Log("Swap has occured");
    }

    public virtual void OnTileClear(Tile tile) 
    {
        // A function that updates score
        currentScore += tile.Score;
        Debug.Log("Scor:"+ currentScore);
    }
}
