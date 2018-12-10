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
}
