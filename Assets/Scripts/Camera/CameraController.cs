using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _speed = 100;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        var oldPosition = transform.position;
        transform.position = new Vector3(_player.position.x, _player.position.y, oldPosition.z);
    }
}
