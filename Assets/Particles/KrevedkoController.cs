using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class KrevedkoController : MonoBehaviour
{
    public ParticleSystem.MinMaxCurve ConsumeSizeCurve;
    public ParticleSystem.MinMaxGradient ConsumeColorGradient;

    private bool _consumed = false;
    public float CollapseTime = 0.5f;
    //private void Update()
    //{
    //    if (_consumed)
    //    {
    //        var ps = GetComponent<ParticleSystem>();
    //        var particles = new ParticleSystem.Particle[ps.particleCount];
    //        ps.GetParticles(particles, ps.particleCount);
    //        for (int i = 0; i < particles.Length; i++)
    //        {
    //        }
    //    }
    //}

    public void Collect(GameObject target)
    {
        _consumed = true;
        var ps = GetComponent<ParticleSystem>();
        var particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles, ps.particleCount);
        for (int i = 0; i < particles.Length; i++) 
        {
            var part = particles[i];
            part.startSize = part.GetCurrentSize(ps);
            
            part.remainingLifetime = CollapseTime;            
            part.startLifetime = CollapseTime;
            part.velocity = -(part.position - target.transform.position) / CollapseTime;
            particles[i] = part;
            
            var av = part.animatedVelocity;
            av.y = 0;
        }
        ps.SetParticles(particles);

        var sol = ps.sizeOverLifetime;
        sol.size = ConsumeSizeCurve;
        var col = ps.colorOverLifetime;
        col.color = ConsumeColorGradient;
        var psm = ps.main;
        psm.loop = false;
        var em = ps.emission;
        em.enabled = false;
    }

}
