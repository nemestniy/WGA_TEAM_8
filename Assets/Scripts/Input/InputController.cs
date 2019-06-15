using UnityEngine;


public abstract class InputController : MonoBehaviour {

    #region Singletone
    public static InputController Instance { get; private set; }
    public InputController() : base()
    {
        Instance = this;
    }
    #endregion
    
    public abstract Vector2 GetMovingDirection();

    public abstract Vector2 GetAimingDirection();

    public abstract bool GetPauseButton();

    public abstract bool GetSkipButton();

    public abstract MoseButtonStates GetButtonState();

    public enum MoseButtonStates
    {
        Released,
        LeftDown,
        RightDown
    } 
}
