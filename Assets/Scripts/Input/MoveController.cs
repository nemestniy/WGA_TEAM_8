using UnityEngine;


public abstract class MoveController : MonoBehaviour {

    public abstract Vector2 GetVelocity();

    public abstract float GetAngle();

    public abstract bool GetPauseButton();

    public abstract MoseButtonStates GetButtonState();
   
    public enum MoseButtonStates
    {
        Released,
        LeftDown,
        RightDown
    } 
}
