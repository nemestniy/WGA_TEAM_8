using UnityEngine;
using System.Collections.Generic;

public class CommonUtils : MonoBehaviour
{
    public GameObject hexagons;
    [SerializeField] [ShowOnly] private List<GameObject> hexagonsArraylist = new List<GameObject>();

    public static CommonUtils Instance { get; private set; }

    public CommonUtils() : base()
    {
        Instance = this;
    }

    public void InitializeCommonUtils()
    {
        for (int i = 0; i < hexagons.transform.childCount; i++)
        {
            hexagonsArraylist.Add(hexagons.transform.GetChild(i).gameObject);
        }

    }

    public GameObject GetHexagonByPoint(Vector3 point)
    {
        GameObject hexagon = null;
        float distanceX = 100.0f;
        foreach (GameObject hex in hexagonsArraylist)
        {
            float distance = Vector3.Distance(hex.transform.position, point);
            if (distanceX > distance)
            {
                //Debug.Log(distanceX + " " + distance);
                distanceX = distance;
                hexagon = hex;
            }
        }

        return hexagon;
    }
    
}
