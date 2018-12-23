using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MoveController
{

    [SerializeField] private float _accuracy = 100;

    public override Vector2 GetVelocity()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        return new Vector2(horizontal, vertical);
    }

    public override float GetAngle()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        float angle = Vector2.Angle(Vector2.up, mousePosition.normalized * _accuracy);
        if (Input.mousePosition.x > Screen.width / 2)
            angle *= -1;
        return angle;
    }
}
