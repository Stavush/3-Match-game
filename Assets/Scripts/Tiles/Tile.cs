using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int x;
    private int y;
    [SerializeField] private int score;

    
    public MovableTile movableComponent;
    public TileDesign tileDesign;

    public int X
    {
        get { return x; } 
        set {
            if (IsMovable())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get { return y; }
        set {
            if (IsMovable())
            {
                y = value;
            }
             }
    }

    private Board.TileType type;

    public Board.TileType Type
    {
        get { return type; }
    }

    private Board board;

    public Board BoardRef
    {
        get { return board; }
    }

    public MovableTile MovableComponent
    {
        get { return movableComponent; }
    }

    public TileDesign DesignComponent
    {
        get { return tileDesign; }
    }

    private ClearableTile clearableComponent;

    public ClearableTile ClearableComponent
    {
        get { return clearableComponent; }
    }

    public int Score
    {
        get { return score; }
    }


    private void Awake()
    {
        movableComponent = GetComponent<MovableTile>();
        tileDesign = GetComponent<TileDesign>();
        clearableComponent = GetComponent<ClearableTile>();
    }

    public void Init(int _x, int _y, Board _board, Board.TileType _type)
    {
        x= _x;
        y= _y;
        board= _board;
        type= _type;
    }

    public bool IsMovable()
    {
        return movableComponent != null;
    }

    public bool IsClearable()
    {
        return clearableComponent != null;
    }

    public bool HasDesign()
    {
        return tileDesign != null;
    }

    private void OnMouseEnter()
    {
        board.EnterTile(this);
    }

    private void OnMouseDown() 
    { 
        // When pressed
        board.PressTile(this);
    }

    private void OnMouseUp()
    {
        // When released
        board.OnMouseRelease();
    }
}
