using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a whoosh sound effect when objects pass near the ship.
/// </summary>
public class WhooshEffect : MonoBehaviour
{
    public Transform shipTransform;
    public List<AudioSource> effects = new List<AudioSource>();
    
    private bool TurnedOn { get; set; }
    private int indexOfNextEffectToPlay;
    private float volume = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        this.TurnedOn = true;
        indexOfNextEffectToPlay = Random.Range(0, effects.Count);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = shipTransform.position;
    }

    public void TurnOnAtMediumVolume()
    {
        this.TurnedOn = true;
        volume = 0.9f;
    }

    public void TurnOnAtHighVolume()
    {
        this.TurnedOn = true;
        volume = 1.0f;
    }

    public void TurnOff()
    {
        this.TurnedOn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TurnedOn)
        {
            if (other.tag == Tags.Obstacle)
            {
                var effect = effects[indexOfNextEffectToPlay];
                effect.volume = this.volume;
                effect.Play();
                indexOfNextEffectToPlay = Random.Range(0, effects.Count);
            }
            
        }
    }
}
