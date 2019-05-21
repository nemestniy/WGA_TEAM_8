using System;
using UnityEngine;


public class KeyManager : MoveController
{
    [SerializeField] private float _accuracy = 100;

    #region Singletone
    public static KeyManager Instance { get; private set; }
    public KeyManager() : base()
    {
        Instance = this;
    }
    #endregion
    
    public override Vector2 GetVelocity()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(horizontal, vertical);

        if (horizontal != 0 && vertical != 0)
            direction /= Mathf.Sqrt(2);

        return direction;
    }

    public override float GetAngle()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        float angle = Vector2.Angle(Vector2.up, mousePosition.normalized * _accuracy);
        if (Input.mousePosition.x > Screen.width / 2)
            angle *= -1;
        return angle;
    }

    
    public WheelMovment GetWheelMovment()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            return WheelMovment.Up;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            return WheelMovment.Down;
        }

        return WheelMovment.None;
    }

    public MoseButtonStates GetButtonState()
    {
        if (Input.GetMouseButton(0))
        {
            return MoseButtonStates.LeftDown;
        }
        if (Input.GetMouseButton(1))
        {
            return MoseButtonStates.RightDown;
        }
        return MoseButtonStates.Released;
    }

    public bool GetPauseButton()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
    
    public enum WheelMovment
    {
        None,
        Up,
        Down
    }
    
    public enum MoseButtonStates
    {
        Released,
        LeftDown,
        RightDown
    }
}
