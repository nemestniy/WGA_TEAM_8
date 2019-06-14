﻿using System;
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
            _isGamepadUsing = true;
        }
        else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            _isGamepadUsing = false;
        }

        if (_isGamepadUsing) //if using gamepad
        {
            return Mathf.Atan2(Input.GetAxis("Joy X"), 0) * Mathf.Rad2Deg;
        }
        
        //if using mouse
        Transform playerTransform = _player.transform;
        Vector2 mousePosition = (_mainCam.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;
        float angle = Vector2.Angle(playerTransform.up, mousePosition);
        
        if(Vector2.Angle(playerTransform.right, mousePosition) > 90) //find out where is cursor, on the right or on the left
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
