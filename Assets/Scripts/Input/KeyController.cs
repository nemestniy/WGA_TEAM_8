using System;
using UnityEngine;


public class KeyController : MoveController
{
    private const int LIGHT_MODES_COUNT = 3; //because the last mode in the list is PreDeathMode
    
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
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        float angle = Vector2.Angle(Vector2.up, mousePosition.normalized * _accuracy);
        if (Input.mousePosition.x > Screen.width / 2)
            angle *= -1;
        return angle;
    }
    
    public int GetLightMode(bool isPreDeath)
    {
        if (isPreDeath)
        {
            return 3;
        }
        switch (_modeControl)
        {
            case ModeControl.SwitchByWheel:
                return SwitchingByWeel();
            case ModeControl.ClickButtons:
                return SwitchingByClick();
            case ModeControl.HoldButtons:
                return SwitchingByHold();
            case ModeControl.UpAndDownRoll:
                return UpAndDownRoll();
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
                return _prevLightMode;
            } 
            _prevLightMode = 0;
            return _prevLightMode;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (_prevLightMode == 0)
            {
                _prevLightMode = 2;
                return _prevLightMode;
            }
            _prevLightMode = 0;
            return _prevLightMode;
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

    private int UpAndDownRoll()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            _prevLightMode++;
        } 
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            _prevLightMode--;
        }

        _prevLightMode = Mathf.Clamp(_prevLightMode, 0, LIGHT_MODES_COUNT - 1);

        switch (_prevLightMode)
        {
            case 0:
                return 2;
            case 1:
                return 0;
            case 2:
                return 1;
            default:
                return -1; //in case of error
        }
    }
    
    [Serializable]
    private enum ModeControl
    {
        SwitchByWheel,
        HoldButtons,
        ClickButtons,
        UpAndDownRoll
    }
}
