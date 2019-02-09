using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int mazeSize;

    private ElementType[][] mazeMap;
    private int colSize;
    private int rowSize;
    void Start()
    {
        colSize = mazeSize * 2 + 1;
        rowSize = mazeSize;
        
        for (int i = 0; i < colSize; i++)
        {
            
            
            for (int j = 0; i < rowSize; i++)
            {
                
                if (i % 2 == 0)
                {
                    mazeMap[i][j] = ElementType.Wall;
                }
                else
                {
                    if (j % 2 == 0)
                    {
                        mazeMap[i][j] = ElementType.Wall;
                    }
                    else
                    {
                        mazeMap[i][j] = ElementType.Cell;
                    }
                }
            }
            
            if (i <= colSize / 2 )
            {
                rowSize++;
            }
            else
            {
                rowSize--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum ElementType
{
    Cell,
    Wall,
    Pass
}
