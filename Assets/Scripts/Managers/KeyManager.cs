using System;
using UnityEngine;


public class KeyManager : MoveController
{
    private Camera _mainCam;
    private Player _player => Player.Instance;
    
    
    private void Start()
    {        
        _mainCam = Camera.main;        
    }
    
    public override Vector2 GetVelocity()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(horizontal, vertical);

        if (horizontal != 0 && vertical != 0)
            direction /= Mathf.Sqrt(2);

        return direction;
    }

    private bool _isGamepadUsing;
    
    public override float GetAngle()
    {
        //check what input is in use
        if (Input.GetAxis("Joy X") != 0) 
        {
            _isGamepadUsing = true; //using gamepad
        }
        else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            _isGamepadUsing = false; //using mouse
        }

        Transform playerTransform = _player.transform;
        Vector2 aimDirection;
        if (_isGamepadUsing) //if using gamepad
        {
            aimDirection = new Vector2(Input.GetAxis("Joy X"), Input.GetAxis("Joy Y"));
        }
        else //if using mouse
        {
            aimDirection = (_mainCam.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;
        }
        
        var angle = Vector2.Angle(playerTransform.up, aimDirection); //angel between player's gaze direction and mouse position
        if(Vector2.Angle(playerTransform.right, aimDirection) > 90) //find out where is cursor, on the right or on the left of player's gaze direction
            angle *= -1;
        
        return angle;
    }

    
//    public WheelMovment GetWheelMovment()
//    {
//        if (Input.GetAxis("Mouse ScrollWheel") > 0)
//        {
//            return WheelMovment.Up;
//        }
//        if (Input.GetAxis("Mouse ScrollWheel") < 0)
//        {
//            return WheelMovment.Down;
//        }
//
//        return WheelMovment.None;
//    }

    public override MoseButtonStates GetButtonState()
    {
        if (Input.GetAxis("Fire1") > 0)
        {
            return MoseButtonStates.LeftDown;
        }
        if (Input.GetAxis("Fire2") > 0)
        {
            return MoseButtonStates.RightDown;
        }
        return MoseButtonStates.Released;
    }

    private bool _pauseHolded;

    public override bool GetPauseButton()
    {
        if (Input.GetAxis("Cancel") > 0)
        {
            if (!_pauseHolded) //check if button has already pushed
            {
                _pauseHolded = true;
                return true;
            }
            return false;
        }
        _pauseHolded = false;
        return false;
    }
    
//    public enum WheelMovment
//    {
//        None,
//        Up,
//        Down
//    }
}
