using UnityEngine;



public class KeyController : MoveController
{

    [SerializeField] private float _accuracy = 100;

    public override Vector2 GetVelocity()
    {
        float horizontal = UnityEngine.Input.GetAxis("Horizontal");
        float vertical = UnityEngine.Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(horizontal, vertical);

        if (horizontal != 0 && vertical != 0)
            direction /= Mathf.Sqrt(2);

        return direction;
    }

    public override float GetAngle()
    {
        Vector2 mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition) - UnityEngine.Camera.main.transform.position;
        float angle = Vector2.Angle(Vector2.up, mousePosition.normalized * _accuracy);
        if (UnityEngine.Input.mousePosition.x > Screen.width / 2)
            angle *= -1;
        return angle;
    }
}
