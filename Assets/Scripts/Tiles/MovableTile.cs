using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MovableTile : MonoBehaviour
{
    private Tile tile;
    private IEnumerator moveTileCoroutine;

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

    public void MoveTile(int newX, int newY, float moveTime)
    {
        if(moveTileCoroutine != null)
        {
            StopCoroutine(moveTileCoroutine);
        }

        moveTileCoroutine = MoveTileCoroutine(newX, newY, moveTime);
        StartCoroutine(moveTileCoroutine);

        tile.X = newX;
        tile.Y = newY;

        tile.transform.localPosition = tile.BoardRef.GetPosition(newX, newY);

    }

    private IEnumerator MoveTileCoroutine(int newX, int newY, float moveTime)
    {
        tile.X = newX;
        tile.Y = newY;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = tile.BoardRef.GetPosition(newX, newY);

        for(float t=0; t<=1 * moveTime; t+=Time.deltaTime) {
            tile.transform.position = Vector3.Lerp(startPosition, endPosition, t/moveTime);
            yield return 0;
        }

        tile.transform.position = endPosition;
    }

}
