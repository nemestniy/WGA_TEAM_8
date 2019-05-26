using System;
using UnityEngine;


public class KeyManager : MoveController
{
    #region Singletone
    public static KeyManager Instance { get; private set; }
    public KeyManager() : base()
    {
        Instance = this;
    }
    #endregion

    private Camera _mainCam;
    private Player _player => Player.Instance;
    
    public override Vector2 GetVelocity()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(horizontal, vertical);

        if (horizontal != 0 && vertical != 0)
            direction /= Mathf.Sqrt(2);

        return direction;
    }

    private void Start()
    {        
        _mainCam = Camera.main;        
    }

    public override float GetAngle()
    {
        Transform playerTransform = _player.transform;
        Vector2 mousePosition = (_mainCam.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;
        float angle = Vector2.Angle(playerTransform.up, mousePosition);
        
        if(Vector2.Angle(playerTransform.right, mousePosition) > 90) //find out where is cursor, on the right or on the left
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

    public override MoseButtonStates GetButtonState()
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

    public override bool GetPauseButton()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
    
    public enum WheelMovment
    {
        None,
        Up,
        Down
    }
    
    
}
