using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public event System.Action OnExplosionFinished;
    public ExplosionEffectListener Listener { get; set; }

    public GameObject sparksEffect;
    public GameObject flashEffect;
    public GameObject fireEffect;
    public GameObject smokeEffect;
    private ParticleSystem sparks;
    private ParticleSystem flash;
    private ParticleSystem fire;
    private ParticleSystem smoke;

    // Start is called before the first frame update
    void Start()
    {
        sparks = sparksEffect.GetComponent<ParticleSystem>();
        flash = flashEffect.GetComponent<ParticleSystem>();
        fire = fireEffect.GetComponent<ParticleSystem>();
        smoke = smokeEffect.GetComponent<ParticleSystem>();

        StartCoroutine(WaitForParticleSystemsToFinish());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForParticleSystemsToFinish()
    {
        yield return new WaitUntil(() => 
            sparks.isStopped && flash.isStopped && fire.isStopped && smoke.isStopped
        );

        // Explosion particle systems are all done
        OnExplosionFinished?.Invoke();
    }
}

public interface ExplosionEffectListener
{
    void ExplosionDidFinish();
}
