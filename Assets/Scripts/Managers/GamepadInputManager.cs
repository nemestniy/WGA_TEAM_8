using System;
using UnityEngine;


public class GamepadInputManager : MoveController
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

    public override float GetAngle()
    {
        Transform playerTransform = _player.transform;
        Vector2 mousePosition = (_mainCam.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized;
        float angle = Vector2.Angle(playerTransform.up, mousePosition);
        
        if(Vector2.Angle(playerTransform.right, mousePosition) > 90) //find out where is cursor, on the right or on the left
            angle *= -1;
        return angle;
    }

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

    public override bool GetPauseButton()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
