using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDesignTile : ClearableTile
{
    public TileDesign.DesignType design;

    public TileDesign.DesignType Design
    {
        get { return design; }
        set { design = value; }
    }

    public override void Clear()
    {
        base.Clear();
        tile.BoardRef.ClearDesign(design);


    }

}
