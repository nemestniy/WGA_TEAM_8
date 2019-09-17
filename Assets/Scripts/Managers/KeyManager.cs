using System;
using UnityEngine;

public class KeyManager : InputController
{
    private Camera _mainCam;
    private Player _player => PlayerManager.Instance.player;
    
    
    private void Start()
    {        
        _mainCam = Camera.main;        
    }
    
    public override Vector2 GetMovingDirection()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(horizontal, vertical);

        if (horizontal != 0 && vertical != 0)
            direction /= Mathf.Sqrt(2);

        return direction;
    }

    private bool _gamepadUsedLast; //last frame gamepad was used
    
    public override Vector2 GetAimingDirection()
    {
        Vector2 aimDirection;

        bool mouseInputs = Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0;
        bool gamepadInputs = Input.GetAxis("Joy X") != 0 || Input.GetAxis("Joy Y") != 0;
        if (mouseInputs && !gamepadInputs) //mouse is inputting and gamepad is not
        {
            _gamepadUsedLast = false;
            aimDirection = (_mainCam.ScreenToWorldPoint(Input.mousePosition) - _player.transform.position).normalized; //use mouse
        }
        else if (!mouseInputs && gamepadInputs) //gamepad is inputting and mouse is not
        {
            _gamepadUsedLast = true;
            aimDirection = new Vector2(Input.GetAxis("Joy X"), Input.GetAxis("Joy Y")); //use gamepad
        }
        else if (_gamepadUsedLast) //gamepad was used last frame
        {
            _gamepadUsedLast = true;
            aimDirection = new Vector2(Input.GetAxis("Joy X"), Input.GetAxis("Joy Y")); //use gamepad
        }
        else //mouse was used last frame
        {
            _gamepadUsedLast = false;
            aimDirection = (_mainCam.ScreenToWorldPoint(Input.mousePosition) - _player.transform.position).normalized; //use mouse
        }
        
        return aimDirection;
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

    public override bool GetSkipButton()
    {
        return Input.GetAxis("Fire1") > 0 || Input.GetAxis("Fire2") > 0 || Input.GetAxis("Submit") > 0;
    }

//    public enum WheelMovment
//    {
//        None,
//        Up,
//        Down
//    }
}
