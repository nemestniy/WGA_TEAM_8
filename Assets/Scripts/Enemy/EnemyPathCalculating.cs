using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathCalculating : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject pathMark;
    private Vector2[] path;
    void Start()
    {
        pathMark.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int[] CheckWalls()
    {
        int[] walls = new int[6];
        
            //pathMark.transform.position
        
        return walls;
    }

    
}
