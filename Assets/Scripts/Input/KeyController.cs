using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MoveController
{
    public override Vector2 GetVelocity()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        return new Vector2(horizontal, vertical);
    }

    public override float GetAngle()
    {
        float angle = Vector2.Angle(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition).normalized);
        if (Input.mousePosition.x > Screen.width / 2)
            angle *= -1;
        return angle;
    }
}
