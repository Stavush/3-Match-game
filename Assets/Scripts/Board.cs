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

    public Level level;

    public UiElements uiElements;

    private bool gameOver = false;

    private bool isFilling = false;

    public bool IsFilling
    {
        get { return isFilling; }
    }

    [SerializeField] private GameObject tileBGPrefab;
    public TilePrefab[] tilePrefabs;

    [System.Serializable] 
    public struct TilePrefab
    {
        public TileType type;
        public GameObject prefab;
    };

    [System.Serializable]
    public struct TilePosition
    {
        public TileType type;
        public int x;
        public int y;
    };

    public TilePosition tilePosition;
    public TilePosition[] initialTiles;

    private Dictionary<TileType, GameObject> tilePrefabDict;

    private Tile[,] tiles;

    private bool inverse = false;

    private Tile pressedTile;
    private Tile enteredTile;

    void Awake()
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

        for(int i = 0; i<initialTiles.Length; i++)
        {
            if (initialTiles[i].x >= 0 && initialTiles[i].x < width && initialTiles[i].y >= 0 && initialTiles[i].y < height)
            {
                CreateNewTile(initialTiles[i].x, initialTiles[i].y, initialTiles[i].type);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, y, 0);

                // Instantiate tiles background 
                GameObject tileBackground = Instantiate(tileBGPrefab, position, Quaternion.identity);

                if (!tiles[x, y])
                {
                    // Instantiate tiles content
                    CreateNewTile(x, y, TileType.EMPTY);
                }

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
        isFilling = true;

        while (refillNeeded)
        {
            yield return new WaitForSeconds(fillTime);

            while (SingleFill())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }

            refillNeeded = ClearAllMatches();
        }
        isFilling = false;
    }

    public bool SingleFill()
    {
        bool isTileMoved = false;
        for(int y=height-2; y>=0; y--)
        {
            for(int loopX=0; loopX<width; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = width - 1 - loopX;
                }

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
                    else
                    {
                        // diagonally fill board
                        for(int diag=-1; diag <=1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;
                                if (inverse)
                                {
                                    diagX = x - diag;
                                }
                                if(diagX >=0 && diagX < width)
                                {
                                    Tile diagonalTile = tiles[diagX, y + 1];
                                    if(diagonalTile.Type == TileType.EMPTY)
                                    {
                                        bool hasTileAbove = true;

                                        for(int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            Tile tileAbove = tiles[diagX, aboveY];
                                            if (tileAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if(!tileAbove.IsMovable() && tileAbove.Type != TileType.EMPTY)
                                            {
                                                hasTileAbove = false;
                                                break;
                                            }
                                        }
                                        if (!hasTileAbove)
                                        {
                                            Destroy(diagonalTile.gameObject);
                                            tile.movableComponent.MoveTile(diagX, y + 1, fillTime);
                                            tiles[diagX, y + 1] = tile;
                                            CreateNewTile(x, y, TileType.EMPTY);
                                            isTileMoved = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
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
        // A function that checks if 2 tiles are adjecent and can be swapped
        return (tile1.X == tile2.X && (int)Mathf.Abs(tile1.Y - tile2.Y)==1) || (tile1.Y == tile2.Y && (int)Mathf.Abs(tile1.X - tile2.X) == 1);
    }

    public void SwapTiles(Tile tile1, Tile tile2)
    {
        // A function that swaps 2 tiles 
        if (gameOver)
        {
            return;
        }

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

                if (tile1.Type == TileType.ROW_CLEAR || tile1.Type == TileType.COLUMN_CLEAR)
                {
                    ClearTile(tile1.X, tile1.Y);
                }
                if (tile2.Type == TileType.ROW_CLEAR || tile2.Type == TileType.COLUMN_CLEAR)
                {
                    ClearTile(tile2.X, tile2.Y);
                }

                pressedTile = null;
                enteredTile = null;

                StartCoroutine(FillBoard());

                level.OnTileSwap();
            }
            else
            {
                // if there's no match, swap the tiles back to original position
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
        if (tile.HasDesign())
        {
            TileDesign.DesignType design = tile.DesignComponent.Design;
            List<Tile> horizontalTiles = new List<Tile>();
            List<Tile> verticalTiles = new List<Tile>();
            List<Tile> matchingTiles = new List<Tile>();

            // horitontal check
            horizontalTiles.Add(tile);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xDist = 1; xDist < width; xDist++)
                {
                    int x;

                    if (dir == 0) // left
                    {
                        x = newX - xDist;
                    }
                    else // right
                    {
                        x = newX + xDist;
                    }

                    if (x < 0 || x >= width)
                    {
                        break;
                    }

                    if (tiles[x, newY].HasDesign() && tiles[x, newY].DesignComponent.Design == design)
                    {
                        horizontalTiles.Add(tiles[x, newY]);
                    }
                    else
                    {
                        // no match
                        break;
                    }
                }
            }

            if (horizontalTiles.Count >= 3)
            {
                for (int i = 0; i < horizontalTiles.Count; i++)
                {
                    matchingTiles.Add(horizontalTiles[i]);
                }
            }

            // traverse vertically to find a L or T match
            if(horizontalTiles.Count >= 3)
            {
                for(int i=0; i<horizontalTiles.Count; i++)
                {
                    for(int dir = 0; dir <= 1; dir++)
                    {
                        for(int yDist=0; yDist < height; yDist++)
                        {
                            int y;

                            if(dir == 0)
                            {
                                y = newY - yDist;
                            }
                            else
                            {
                                y = newY + yDist;
                            }

                            if(y<0 || y >= height)
                            {
                                break;
                            }

                            if (tiles[horizontalTiles[i].X, y].HasDesign() &&  tiles[horizontalTiles[i].X, y].DesignComponent.Design == design)
                            {
                                verticalTiles.Add(tiles[horizontalTiles[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if(verticalTiles.Count < 2)
                    {
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



            // vertical check
            verticalTiles.Clear ();
            horizontalTiles.Clear ();

            verticalTiles.Add(tile);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yDist = 1; yDist < width; yDist++)
                {
                    int y;

                    if (dir == 0) // down
                    {
                        y = newY - yDist;
                    }
                    else // up
                    {
                        y = newY + yDist;
                    }

                    if (y < 0 || y >= width)
                    {
                        break;
                    }

                    if (tiles[newX, y].HasDesign() && tiles[newX, y].DesignComponent.Design == design)
                    {
                        verticalTiles.Add(tiles[newX, y]);
                    }
                    else
                    {
                        // no match
                        break;
                    }
                }
            }

            if (verticalTiles.Count >= 3)
            {
                for (int i = 0; i < verticalTiles.Count; i++)
                {
                    matchingTiles.Add(verticalTiles[i]);
                }
            }

            // traverse horizontally to find a L or T match
            if (verticalTiles.Count >= 3)
            {
                for (int i = 0; i < verticalTiles.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xDist = 0; xDist < width; xDist++)
                        {
                            int x;

                            if (dir == 0) // left 
                            {
                                x = newX - xDist;
                            }
                            else // right
                            {
                                x = newX + xDist;
                            }

                            if (x < 0 || x >= width)
                            {
                                break;
                            }

                            if (tiles[x, verticalTiles[i].Y].HasDesign() && tiles[x, verticalTiles[i].Y].DesignComponent.Design == design)
                            {
                                horizontalTiles.Add(tiles[x, verticalTiles[i].Y]);
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

            ClearObstacle(x, y);

            return true;
        }
        return false;
    }

    public void ClearObstacle(int x, int y)
    {
        for(int adjecentX = x-1; adjecentX <= x+1; adjecentX++)
        {
            if(adjecentX != x &&  adjecentX >= 0 && adjecentX < width) 
            {
                if (tiles[adjecentX, y].Type == TileType.OBSTACLE && tiles[adjecentX, y].IsClearable())
                {
                    tiles[adjecentX, y].ClearableComponent.Clear();
                    CreateNewTile(adjecentX, y, TileType.EMPTY);
                }
            }
        }
        for(int adjecentY = y-1; adjecentY <= y+1; adjecentY++)
        {
            if (adjecentY != y && adjecentY >= 0 && adjecentY < height)
            {
                if (tiles[x, adjecentY].Type == TileType.OBSTACLE && tiles[x, adjecentY].IsClearable())
                {
                    tiles[x, adjecentY].ClearableComponent.Clear();
                    CreateNewTile(x, adjecentY, TileType.EMPTY);
                }
            }
        }
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

    public void GameOver()
    {
        gameOver = true;
    }

    public List<Tile> GetTilesOfType(TileType type)
    {
        List<Tile> tilesOfType = new List<Tile>();

        for(int x=0; x<width ;x++)
        {
            for(int y=0; y<height ;y++)
            {
                if (tiles[x,y].Type == type)
                {
                    tilesOfType.Add(tiles[x,y]);
                }
            }
        }
        return tilesOfType;
    }
}