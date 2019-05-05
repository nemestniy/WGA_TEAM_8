using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    
    [SerializeField] private Material _normalViewMat;
    [SerializeField] private Material _detectiveViewMat;
    [SerializeField] private Energy _lampEnergy;
    [SerializeField] public List<LampMode> _lampModes;
    
    private List<FieldOfView> _fieldOfViews;
    private List<MeshRenderer> _lampsMeshRenderers;
    private Animator _animator;
    [HideInInspector] public bool _isFrying; //to frighten enemy
    private static readonly int EnergyLvl = Animator.StringToHash("EnergyLvl");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _lampsMeshRenderers = new List<MeshRenderer>(gameObject.GetComponentsInChildren<MeshRenderer>());
        _fieldOfViews = new List<FieldOfView>(GetComponentsInChildren<FieldOfView>());
    }

    private void Update()
    {
        _animator.SetFloat(EnergyLvl, _lampEnergy.CurrentEnergyLvl);
    }

    public void SetLightMode(int newMode, int prevMode, float changingState)
    {
        _fieldOfViews[0].SetLightMode(newMode, prevMode, changingState);
        _fieldOfViews[1].SetLightMode(newMode, prevMode, changingState);
    }

    public void SetDetectiveMaterial()
    {
        _lampsMeshRenderers[0].material = _detectiveViewMat;
        _lampsMeshRenderers[1].material = _detectiveViewMat;
    }
    
    public void SetNormalMaterial()
    {
        _lampsMeshRenderers[0].material = _normalViewMat;
        _lampsMeshRenderers[1].material = _normalViewMat;
    }
    
    public void BlinkLight(AnimationCurve curve, float duration, List<Period> periods)
    {
        StartCoroutine(Blinking(curve, duration, periods));
    }
    
    private IEnumerator Blinking(AnimationCurve curve, float duration,List<Period> periods)
    {
        float timePast = 0;
        while (timePast < duration)
        {
            var curMultiplier = BlinkingMath(curve, timePast / duration, periods);
            
            _fieldOfViews[0]._currentIntensityMult = curMultiplier;
            _fieldOfViews[1]._currentIntensityMult = curMultiplier;
            timePast += Time.deltaTime;
            yield return null;
        }
        _fieldOfViews[0]._currentIntensityMult = 1;
        _fieldOfViews[1]._currentIntensityMult = 1;
    }

    private float BlinkingMath(AnimationCurve fadingCurve, float t, List<Period> periods)
    {
        foreach (var period in periods)
        {
            if (t < period.end)
            {
                return period.mult;
            }
        }

        return fadingCurve.Evaluate(t);
    }
    
    [Serializable]
    public struct Period
    {   
        [UnityEngine.Range(0,1)]
        public float end;
        public float mult;
    }
    
    [Serializable]
    public struct LampMode
    {
        public string name;
        public LightMode mainLight;
        public LightMode backLight;
    }
    
    [Serializable]
    public struct LightMode
    {
        public float viewRadius;
        public float spotLightRadius;
        [UnityEngine.Range(0, 360)]
        public float viewAngle;
        [UnityEngine.Range(0, 360)]
        public float spotLightAngle;
        public float intensity;
        public float lightHeight;
        public float coordinateY;
        public Color lightColor;
    }
}
