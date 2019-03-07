using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _speed = 100;

    
    private void Update()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        if (_player != null)
        {
            var oldPosition = transform.position;
            transform.position = new Vector3(_player.position.x, _player.position.y, oldPosition.z);
        }
    }
}
