using UnityEngine;

namespace Input
{
    public abstract class MoveController : MonoBehaviour {

        public abstract Vector2 GetVelocity();

        public abstract float GetAngle();
    }
}
