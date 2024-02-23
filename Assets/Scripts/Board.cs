using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public enum TileType
    {
        NORMAL,
        COUNT,
    };

    [System.Serializable] 
    public struct TilePrefab
    {
        public TileType type;
        public GameObject prefab;
    };

    [SerializeField] private int columns; //x
    [SerializeField] private int rows; //y

    public TilePrefab[] tilePrefabs;

    private Dictionary<TileType, GameObject> tilePrefabDict;


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
    }

    void Update()
    {
        
    }
}
