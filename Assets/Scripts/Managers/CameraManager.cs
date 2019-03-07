using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] 
    private Camera _camera;

    [SerializeField] 
    private Player _player;

    private Vector3 _offset;

    private void Start ()
    {
        _offset = _camera.transform.position - _player.transform.position;
    }

    private void LateUpdate ()
    {
        _camera.transform.position = _player.transform.position + _offset;
    }
}