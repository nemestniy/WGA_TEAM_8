using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class HexagonMap : MonoBehaviour
    {
        [SerializeField] private GameObject _startWall;

        [SerializeField] private int _mapWidth;
        [SerializeField] private int _mapHeight;

        private List<List<GameObject>> _walls;

        private void Start()
        {
            StartCoroutine(CreateMap());
        }

        IEnumerator CreateMap()
        {
            float xPos = _startWall.transform.position.x;
            float yPos = _startWall.transform.position.y;
            float newYPos = yPos;

            Sprite sprite = _startWall.GetComponent<SpriteRenderer>().sprite;
            float width = sprite.rect.width / sprite.pixelsPerUnit;
            float height = sprite.rect.height / sprite.pixelsPerUnit;

            int k = 0;

            if (width > height)
            {
                _startWall.transform.eulerAngles += new Vector3(0, 0, 90);
                var change = width;
                width = height;
                height = change;
            }
            float wallAngle = _startWall.transform.eulerAngles.z;
            float correction = Mathf.Cos(30) * height + width/2;

            for (int i = 0; i < _mapHeight; i++)
            {
                float indentX;
                float indentY;
                int rotateAngle;
                int mapWidth;

                if(i % 2 != 0)
                {
                    rotateAngle = 0;
                    indentX = 2 * Mathf.Cos(30);
                    indentY = Mathf.Sin(30) * height;
                    mapWidth = _mapWidth / 2;
                    k++;
                }
                else
                {
                    rotateAngle = 120;
                    indentX = Mathf.Cos(30);
                    indentY = Mathf.Sin(30) * height;
                    mapWidth = _mapWidth;
                    if (i % 4 != 0)
                        rotateAngle *= -1;
                }

                for (int j = 0; j < mapWidth; j++)
                {
                    var newAngle = new Vector3(0, 0, rotateAngle);
                    rotateAngle *= (-1);
                    Vector2 newPosition;
                    if (k >= 2)
                    {
                        newPosition = new Vector2(xPos + j * indentX * height * 5 + correction +  height, newYPos);
                    }
                    else
                        newPosition = new Vector2(xPos + j * indentX * height * 5 + correction, newYPos);
                    var newWall = Instantiate(_startWall);
                    newWall.transform.eulerAngles = newAngle;
                    newWall.transform.position = newPosition;

                    yield return new WaitForSeconds(.1f);
                }
                if (k >= 2)
                    k = 0;
                correction *= (-1);
                newYPos += indentY;
            }
        }
    }
}
