using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class MapChanger : MonoBehaviour
    {

        [SerializeField] 
        private List<GameObject> _maps;
    
        [SerializeField]
        private float _delta = 100;
    
        [SerializeField]
        private float _speedOfChanging = 5;

        private float _currentDelta;
        private int _counter = 0;

        private void Start()
        {
            _currentDelta = _delta;
        }

        private void Update()
        {
            _currentDelta -= Time.deltaTime * _speedOfChanging;
            if (_currentDelta < 0)
            {
                _maps[_counter++].SetActive(false);
                if (_counter == _maps.Count)
                {
                    _counter = 0;
                }
                _maps[_counter].SetActive(true);
                Debug.Log("Change");
                _currentDelta = _delta;
            }
        }
    }
}
