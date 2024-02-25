using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Board : MonoBehaviour
{
    public enum TileType
    {
        EMPTY,
        NORMAL,
        OBSTACLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        JOKER,
        COUNT,
    };

    [SerializeField] private int width; //x
    [SerializeField] private int height; //y
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

    private Tile pressedTile;
    private Tile enteredTile;

    void Start()
    {
        InitializeTilePrefabDictionary();
        InitBoard();
    }

    private void InitializeTilePrefabDictionary()
    {
        tilePrefabDict = new Dictionary<TileType, GameObject>();
        foreach (var prefab in tilePrefabs)
        {
            if (!tilePrefabDict.ContainsKey(prefab.type))
            {
                tilePrefabDict[prefab.type] = prefab.prefab;
            }
        }
    }

    private void InitBoard()
    {
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
        //return new Vector2(transform.position.x - width / 2.0f + x,
            //transform.position.y + height / 2.0f - y);
    }

    public Tile CreateNewTile(int x, int y, TileType type)
    {
        GameObject newTile = Instantiate(tilePrefabDict[type], GetPosition(x, y), Quaternion.identity);
        newTile.transform.parent = transform;

        tiles[x,y] = newTile.GetComponent<Tile>();
        tiles[x, y].Init(x, y, this, type);

        return tiles[x, y];
    }


    // A function that fills the whole board
    public IEnumerator FillBoard()
    {
        bool refillNeeded = true;

        while (refillNeeded)
        {
            yield return new WaitForSeconds(fillTime);

            while (SingleFill())
            {
                yield return new WaitForSeconds(fillTime);
            }

            refillNeeded = ClearAllMatches();
        }
        
    }

    public bool SingleFill()
    {
        bool isTileMoved = false;
        for(int y=height-2; y>=0; y--)
        {
            for(int x=0; x<width; x++)
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
        for(int x=0; x<width; x++)
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
        if (IsSwapable(tile1, tile2) && tile1.IsMovable() && tile2.IsMovable())
        {
            tiles[tile1.X, tile1.Y] = tile2;
            tiles[tile2.X, tile2.Y] = tile1;

            if (GetMatch(tile1, tile2.X, tile2.Y) != null || GetMatch(tile2, tile1.X, tile1.Y) != null || tile1.Type == TileType.JOKER || tile2.Type == TileType.JOKER)
            {
                int tile1X = tile1.X;
                int tile1Y = tile1.Y;

                tile1.MovableComponent.MoveTile(tile2.X, tile2.Y, fillTime);
                tile2.MovableComponent.MoveTile(tile1X, tile1Y, fillTime);

                // clear joker tile
                if(tile1.Type == TileType.JOKER && tile1.IsClearable() && tile2.HasDesign())
                {
                    ClearDesignTile clearDesign = tile1.GetComponent<ClearDesignTile>();
                    if (clearDesign)
                        {
                        clearDesign.Design = tile2.DesignComponent.Design;
                    }
                    ClearTile(tile1.X, tile1.Y);
                }

                if (tile2.Type == TileType.JOKER && tile2.IsClearable() && tile1.HasDesign())
                {
                    ClearDesignTile clearDesign = tile2.GetComponent<ClearDesignTile>();
                    if (clearDesign)
                    {
                        clearDesign.Design = tile1.DesignComponent.Design;
                    }
                    ClearTile(tile2.X, tile2.Y);

                }

                ClearAllMatches();

                pressedTile = null;
                enteredTile = null;

                if (tile1.Type == TileType.ROW_CLEAR || tile1.Type == TileType.COLUMN_CLEAR)
                {
                    ClearTile(tile1.X, tile1.Y);
                }
                if (tile2.Type == TileType.ROW_CLEAR || tile2.Type == TileType.COLUMN_CLEAR)
                {
                    ClearTile(tile2.X, tile2.Y);
                }

                

                StartCoroutine(FillBoard());
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
        pressedTile = tile;
    }

    public void EnterTile(Tile tile)
    {
        enteredTile = tile;
    }

    public void OnMouseRelease()
    {
        if(IsSwapable(pressedTile, enteredTile))
        {
            SwapTiles(pressedTile, enteredTile);
        }
    }

    public List<Tile> GetMatch(Tile tile, int newX, int newY)
    {
        if (!tile.HasDesign()) return null;

        TileDesign.DesignType design = tile.DesignComponent.Design;
        var horizontalMatches = FindMatches(tile, newX, newY, true);
        var verticalMatches = FindMatches(tile, newX, newY, false);

        var allMatches = new HashSet<Tile>(horizontalMatches.Concat(verticalMatches));
        return allMatches.Count >= 3 ? allMatches.ToList() : null;
    }

    private List<Tile> FindMatches(Tile originTile, int newX, int newY, bool horizontal)
    {
        List<Tile> matches = new List<Tile> { originTile };
        int[] directions = { -1, 1 }; // Represents left/up and right/down

        foreach (var dir in directions)
        {
            int distance = 1;
            while (true)
            {
                int x = horizontal ? newX + dir * distance : newX;
                int y = horizontal ? newY : newY + dir * distance;

                if (x < 0 || x >= width || y < 0 || y >= height) break;
                Tile tile = tiles[x, y];

                if (tile.HasDesign() && tile.DesignComponent.Design == originTile.DesignComponent.Design)
                {
                    matches.Add(tile);
                    distance++;
                }
                else break;
            }
        }

        return matches;
    }

    public bool ClearAllMatches()
    {
        bool refillNeeded = false;
        for(int y=0; y<height; y++)
        {
            for(int x=0; x<width; x++)
            {
                if (tiles[x, y].IsClearable())
                {
                    List<Tile> match = GetMatch(tiles[x, y], x, y);

                    if (match != null)
                    {
                        TileType specialTileType = TileType.COUNT;

                        Tile randomTile = match[Random.Range(0, match.Count)];
                        int specialTileX = randomTile.X;
                        int specialTileY = randomTile.Y;

                        if(match.Count == 4)
                        {
                            if(pressedTile == null || enteredTile == null )
                            {
                                specialTileType = (TileType)Random.Range((int)TileType.ROW_CLEAR, (int)TileType.COLUMN_CLEAR);
                            }
                            else if(pressedTile.Y == enteredTile.Y)
                            {
                                specialTileType = TileType.ROW_CLEAR;
                            }
                            else
                            {
                                specialTileType = TileType.COLUMN_CLEAR;
                            }
                        } else if(match.Count > 4)
                        {
                            specialTileType = TileType.JOKER;
                        }

                        foreach(Tile tile in match)
                        {
                            if (ClearTile(tile.X, tile.Y))
                            {
                                refillNeeded = true;

                                if (tile == pressedTile || tile == enteredTile)
                                {
                                    specialTileX = tile.X;
                                    specialTileY = tile.Y;
                                }
                            }
                        }
                        if(specialTileType != TileType.COUNT)
                        {
                            Destroy(tiles[specialTileX, specialTileY]);
                            Tile newTile = CreateNewTile(specialTileX, specialTileY, specialTileType);

                            if ((specialTileType == TileType.ROW_CLEAR || specialTileType == TileType.COLUMN_CLEAR) && newTile.HasDesign() && match[0].HasDesign())
                            {
                                newTile.DesignComponent.SetDesign(match[0].DesignComponent.Design);
                            }
                            else if (specialTileType == TileType.JOKER && newTile.HasDesign())
                            {
                                newTile.DesignComponent.SetDesign(TileDesign.DesignType.ANY);
                            }
                        }
                    }
                }
            }
        }
        return refillNeeded;
    }
    
    public bool ClearTile(int x, int y)
    {
        if (tiles[x, y].IsClearable() && !tiles[x, y].ClearableComponent.IsCleared)
        {
            tiles[x, y].ClearableComponent.Clear();
            CreateNewTile(x, y, TileType.EMPTY);

            return true;
        }
        return false;
    }

    public void ClearRow(int row)
    {
        for(int x=0; x<width; x++)
        {
            ClearTile(x, row);
        }
    }

    public void ClearColumn(int col)
    {
        for (int y = 0; y < height; y++)
        {
            ClearTile(col, y);
        }
    }

    public void ClearDesign(TileDesign.DesignType design)
    {
        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                if (tiles[x, y].HasDesign() && (tiles[x,y].DesignComponent.Design == design) || (design == TileDesign.DesignType.ANY))
                {
                    ClearTile(x, y);

                }
            }
        }
    }
}