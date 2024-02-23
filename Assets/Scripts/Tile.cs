using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int column;
    private int row;
    private Board.TileType type;
    private Board board;
    [SerializeField] private string Score;
    public MovableTile movableComponent;
    public TileDesign tileDesign;

    public int Column
    {
        get { return column; } 
        set {
            if (IsMovable())
            {
                column = value;
            }
        }
    }

    public int Row
    {
        get { return row; }
        set {
            if (IsMovable())
            {
                row = value;
            }
             }
    }

    public Board.TileType Type
    {
        get { return type; }
    }

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

    private void Awake()
    {
        movableComponent = GetComponent<MovableTile>();
        tileDesign = GetComponent<TileDesign>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int _col, int _row, Board _board, Board.TileType _type)
    {
        column= _col;
        row= _row;
        board= _board;
        type= _type;
    }

    public bool IsMovable()
    {
        return movableComponent != null;
    }

    public bool HasDesign()
    {
        return tileDesign != null;
    }
}
