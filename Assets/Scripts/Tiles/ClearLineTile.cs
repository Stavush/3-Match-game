using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLineTile : ClearableTile
{
    public bool isRow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Clear()
    {
        base.Clear();
        // clear row
        if(isRow)
        {
            tile.BoardRef.ClearRow(tile.Y);
        }
        // clear column
        else
        {
            tile.BoardRef.ClearColumn(tile.X);
        }
    }
}
