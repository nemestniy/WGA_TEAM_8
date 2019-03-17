using System;
using UnityEngine;


public class KeyController : MoveController
{
    private const int LIGHT_MODES_COUNT = 3;
    
    [SerializeField] private float _accuracy = 100;
    [SerializeField] private ModeControl _modeControl;

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
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - UnityEngine.Camera.main.transform.position;
        float angle = Vector2.Angle(Vector2.up, mousePosition.normalized * _accuracy);
        if (UnityEngine.Input.mousePosition.x > Screen.width / 2)
            angle *= -1;
        return angle;
    }
    
    public int GetLightMode()
    {
        switch (_modeControl)
        {
            case ModeControl.SwitchByWheel:
                return SwitchingByWeel();
            case ModeControl.ClickButtons:
                return SwitchingByClick();
            case ModeControl.HoldButtons:
                return SwitchingByHold();
            default: 
                return -1;
        }
    }

    [ShowOnly, SerializeField]
    private int _prevLightMode;

    private int SwitchingByWeel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            _prevLightMode++;
            _prevLightMode = _prevLightMode % LIGHT_MODES_COUNT;
            return _prevLightMode;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            _prevLightMode--;
            if(_prevLightMode < 0) _prevLightMode =  _prevLightMode + LIGHT_MODES_COUNT;
            return _prevLightMode;
        }
        return _prevLightMode;
    }
    
    private int SwitchingByClick()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (_prevLightMode == 0)
            {
                _prevLightMode = 1;
                return 1;
            } 
            _prevLightMode = 0;
            return 0;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (_prevLightMode == 0)
            {
                _prevLightMode = 2;
                return 2;
            }
            _prevLightMode = 0;
            return 0;
        }
        return _prevLightMode;
    }
    
    private int SwitchingByHold()
    {
        if (Input.GetButton("Fire1"))
            return 1;
        if (Input.GetButton("Fire2"))
            return 2;
        return 0;
    }
    
    [Serializable]
    private enum ModeControl
    {
        SwitchByWheel,
        HoldButtons,
        ClickButtons
    }
}
