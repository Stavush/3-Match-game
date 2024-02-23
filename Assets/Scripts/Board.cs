using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Board : MonoBehaviour
{
    public enum TileType
    {
        NORMAL,
        COUNT,
    };

    [SerializeField] private int columns; //x
    [SerializeField] private int rows; //y

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
        tiles = new Tile[columns, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 position = new Vector3(x, y, 0);

                // Instantiate tiles background 
                GameObject tileBackground = Instantiate(tileBGPrefab, position, Quaternion.identity);

                // Instantiate tiles content
                GameObject newTile = (GameObject)Instantiate(tilePrefabDict[TileType.NORMAL], position, Quaternion.identity);
                newTile.name = "Tile(" + x + "," + y + ")";
                newTile.transform.parent = transform;

                tiles[x, y] = newTile.GetComponent<Tile>();
                tiles[x, y].Init(x, y, this, TileType.NORMAL);

                if (tiles[x, y].IsMovable())
                {
                    tiles[x, y].MovableComponent.MoveTile(x, y);
                }
                if (tiles[x, y].HasDesign())
                {
                    tiles[x, y].DesignComponent.SetDesign((TileDesign.DesignType)Random.Range(0, tiles[x, y].tileDesign.DesignSpritesAmount));
                }

            }
        }
    }

    public Vector2 GetPosition(int x, int y)
    {
        return new Vector2(x, y);
        //return new Vector2(transform.position.x - columns / 2.0f + x,
            //transform.position.y + rows / 2.0f - y);
    }

    void Update()
    {
        
    }
}
