using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MovableTile : MonoBehaviour
{
    private Tile tile;

    private void Awake()
    {
        tile = GetComponent<Tile>();
    }


    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void MoveTile(int newCol, int newRow)
    {
        tile.Column = newCol;
        tile.Row = newRow;

        tile.transform.localPosition = tile.BoardRef.GetPosition(newCol, newRow);

    }

}
