using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Board : MonoBehaviour
{
    public enum TileType
    {
        EMPTY,
        NORMAL,
        BLOCKED,
        COUNT,
    };

    [SerializeField] private int xDim; //x
    [SerializeField] private int yDim; //y
    [SerializeField] private float fillTime;

    [SerializeField] private GameObject tileBGPrefab;
    public TilePrefab[] tilePrefabs;

    [System.Serializable] 
    public struct TilePrefab
    {
        public TileType type;
        public GameObject prefab;
    };

    private Dictionary<TileType, GameObject> tilePrefabDict;

    private Tile[,] tiles;

    private Tile PressedTile;
    private Tile EnteredTile;

    void Start()
    {
        tilePrefabDict = new Dictionary<TileType, GameObject>();
        for(int i=0; i<tilePrefabs.Length; i++)
        {
            if (!tilePrefabDict.ContainsKey(tilePrefabs[i].type))
            {
                tilePrefabDict.Add(tilePrefabs[i].type, tilePrefabs[i].prefab);
            }
        }
        InitBoard();
    }

    private void InitBoard()
    {
        tiles = new Tile[xDim, yDim];

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                Vector3 position = new Vector3(x, y, 0);

                // Instantiate tiles background 
                GameObject tileBackground = Instantiate(tileBGPrefab, position, Quaternion.identity);

                // Instantiate tiles content
                CreateNewTile(x,  y, TileType.EMPTY);

            }
        }

        StartCoroutine(FillBoard());
    }

    public Vector2 GetPosition(int x, int y)
    {
        return new Vector2(x, y);
        //return new Vector2(transform.position.x - xDim / 2.0f + x,
            //transform.position.y + yDim / 2.0f - y);
    }

    public Tile CreateNewTile(int x, int y, TileType type)
    {
        GameObject newTile = Instantiate(tilePrefabDict[type], GetPosition(x, y), Quaternion.identity);
        newTile.transform.parent = transform;

        tiles[x,y] = newTile.GetComponent<Tile>();
        tiles[x, y].Init(x, y, this, type);

        return tiles[x, y];
    }

    public IEnumerator FillBoard()
    {
        while (SingleFill())
        {
            yield return new WaitForSeconds(fillTime);
        }
    }

    public bool SingleFill()
    {
        bool isTileMoved = false;
        for(int y=yDim-2; y>=0; y--)
        {
            for(int x=0; x<xDim; x++)
            {
                Tile tile = tiles[x,y];
                if (tile.IsMovable())
                {
                    Tile tileBelow = tiles[x, y + 1];
                    if (tileBelow.Type == TileType.EMPTY){
                        Destroy(tileBelow.gameObject);
                        tile.MovableComponent.MoveTile(x, y + 1, fillTime);
                        tiles[x, y + 1] = tile;
                        CreateNewTile(x, y, TileType.EMPTY);
                        isTileMoved = true;
                    }
                }
            }
        }

        // Check the top row
        for(int x=0; x<xDim; x++)
        {
            Tile tileBelow = tiles[x, 0];
            if(tileBelow.Type == TileType.EMPTY)
            {
                Destroy(tileBelow.gameObject); // destroy the empty game object
                GameObject newTile = Instantiate(tilePrefabDict[TileType.NORMAL], GetPosition(x, -1), Quaternion.identity);
                newTile.transform.parent = transform;

                tiles[x, 0] = newTile.GetComponent<Tile>();
                tiles[x, 0].Init(x, -1, this, TileType.NORMAL);
                tiles[x, 0].MovableComponent.MoveTile(x, 0, fillTime);
                tiles[x, 0].DesignComponent.SetDesign((TileDesign.DesignType)Random.Range(0, tiles[x, 0].DesignComponent.DesignSpritesAmount));
                isTileMoved = true;
            }
        }

        return isTileMoved;
    }

    public bool IsSwapable(Tile tile1, Tile tile2)
    {
        return (tile1.X == tile2.X && (int)Mathf.Abs(tile1.Y - tile2.Y)==1) || (tile1.Y == tile2.Y && (int)Mathf.Abs(tile1.X - tile2.X) == 1);
    }

    public void SwapTiles(Tile tile1, Tile tile2)
    {
        if(IsSwapable(tile1, tile2) && tile1.IsMovable() && tile2.IsMovable())
        {
            tiles[tile1.X, tile1.Y] = tile2;
            tiles[tile2.X, tile2.Y] = tile1;

            if (GetMatch(tile1, tile2.X, tile2.Y)!=null || GetMatch(tile2 ,tile1.X, tile1.Y)!= null)
            {
                int tile1X = tile1.X;
                int tile1Y = tile1.Y;

                tile1.MovableComponent.MoveTile(tile2.X, tile2.Y, fillTime);
                tile2.MovableComponent.MoveTile(tile1X, tile1Y, fillTime);
            }
            else
            {
                tiles[tile1.X, tile1.Y] = tile1;
                tiles[tile2.X, tile2.Y] = tile2;
            }
        }
    }

    public void PressTile(Tile tile)
    {
        PressedTile = tile;
    }

    public void EnterTile(Tile tile)
    {
        EnteredTile = tile;
    }

    public void OnMouseRelease()
    {
        if(IsSwapable(PressedTile, EnteredTile))
        {
            SwapTiles(PressedTile, EnteredTile);
        }
    }

    public List<Tile> GetMatch(Tile tile, int newX, int newY)
    {
        if (tile.HasDesign())
        {
            TileDesign.DesignType design = tile.DesignComponent.Design;
            List<Tile> horizontalTiles = new List<Tile>();
            List<Tile> verticalTiles = new List<Tile>();
            List<Tile> matchingTiles= new List<Tile>();


            // Check horizontal tiles for match
            horizontalTiles.Add(tile);

            for(int dir = 0; dir <= 1; dir++) // loop that changes direction
            {
                for(int xDist=1; xDist < xDim; xDist++) // lop through adjecent tiles
                {
                    int x;
                    
                    if (dir == 0)
                    {
                        x = newX - xDist;
                    }
                    else
                    {
                        x = newX + xDist;
                    }

                    if(x<0 || x>= xDim)
                    {
                        break;
                    }

                    if (tiles[x, newY].HasDesign() && tiles[x, newY].DesignComponent.Design == design)
                    {
                        horizontalTiles.Add(tiles[ x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if(horizontalTiles.Count >= 3) // check if there's a match of 3 or more
            {
                for (int i = 0; i< horizontalTiles.Count;i++)
                {
                    matchingTiles.Add(horizontalTiles[i]);
                }
            }

            if (horizontalTiles.Count >= 3)
            {
                for(int i=0; i<horizontalTiles.Count; i++)
                {
                    for(int dir=0; dir <= 1; dir++)
                    {
                        for(int yDist=1; yDist < yDim; yDist++)
                        {
                            int y;

                            if (dir == 0)
                            {
                                y = newY- yDist;
                            }
                            else
                            {
                                y = newY + yDist;
                            }
                            if(y<0 || y>=yDist)
                            {
                                break;
                            }
                            if (tiles[horizontalTiles[i].X, y].HasDesign() && tiles[horizontalTiles[i].X, y].DesignComponent.Design == design) {
                                verticalTiles.Add(tiles[horizontalTiles[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if(verticalTiles.Count < 2) {
                        verticalTiles.Clear();
                    }
                    else
                    {
                        for(int j=0; j<verticalTiles.Count; j++)
                        {
                            matchingTiles.Add(verticalTiles[j]);
                        }
                        break;
                    }
                }
            }

            if (matchingTiles.Count >= 3)
            {
                return matchingTiles;
            }

            // Check vertical tiles for match
            verticalTiles.Clear ();
            horizontalTiles.Clear ();
            verticalTiles.Add(tile);

            for (int i = 0; i <= 1; i++) // loop that changes direction
            {
                for (int yDist = 1; yDist < yDim; yDist++) // lop through adjecent tiles
                {
                    int y;
                    if (i == 0)
                    {
                        y = newY - yDist;
                    }
                    else
                    {
                        y = newY + yDist;
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    if (tiles[newX, y].HasDesign() && tiles[newX, y].DesignComponent.Design == design)
                    {
                        verticalTiles.Add(tiles[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalTiles.Count >= 3) // check if there's a match of 3 or more
            {
                for (int i = 0; i < verticalTiles.Count; i++)
                {
                    matchingTiles.Add(verticalTiles[i]);
                }
            }

            if (verticalTiles.Count >= 3)
            {
                for (int i = 0; i < verticalTiles.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xDist = 1; xDist < yDim; xDist++)
                        {
                            int x;

                            if (dir == 0) // search to left
                            {
                                x = newX - xDist;
                            }
                            else
                            {
                                // search to right
                                x = newX + xDist;
                            }
                            if (x < 0 || x >= xDist)
                            {
                                break;
                            }
                            if (tiles[x, verticalTiles[i].Y].HasDesign() && tiles[x, verticalTiles[i].Y].DesignComponent.Design == design)
                            {
                                verticalTiles.Add(tiles[x,verticalTiles[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalTiles.Count < 2)
                    {
                        horizontalTiles.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalTiles.Count; j++)
                        {
                            matchingTiles.Add(horizontalTiles[j]);
                        }
                        break;
                    }
                }
            }

            if (matchingTiles.Count >= 3)
            {
                return matchingTiles;
            }
        }

        return null;
    }

}