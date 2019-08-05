using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _player;
    
    private void Update()
    {
        if (_player == null)
        {
            _player = Player.Instance.transform;
        }
        
        if (_player != null)
        {
            var oldPosition = transform.position;
            var position = _player.position;
            transform.position = new Vector3(position.x, position.y, oldPosition.z);
        }
    }
}
