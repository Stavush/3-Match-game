using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearableTile : MonoBehaviour
{
    [SerializeField] private AnimationClip clearAnimation; 

    private bool isCleared = false;

    protected Tile tile;

    public bool IsCleared
    {
        get { return isCleared; }
    }

    private void Awake()
    {
        tile = GetComponent<Tile>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        isCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);

            Destroy(gameObject);
        }
    }
}
