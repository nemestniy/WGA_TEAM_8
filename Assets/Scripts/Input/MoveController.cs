using UnityEngine;


public abstract class MoveController : MonoBehaviour {

    public abstract Vector2 GetVelocity();

    public abstract float GetAngle();
}
