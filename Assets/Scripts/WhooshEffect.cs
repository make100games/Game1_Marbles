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

    // Start is called before the first frame update
    void Start()
    {
        this.TurnOnAtLowVolume();
        indexOfNextEffectToPlay = Random.Range(0, effects.Count);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = shipTransform.position;
    }

    private void TurnOnAtLowVolume()
    {
        this.TurnedOn = true;
        GetComponent<AudioSource>().volume = 0.4f;   
    }

    public void TurnOnAtMediumVolume()
    {
        this.TurnedOn = true;
        GetComponent<AudioSource>().volume = 0.7f;
    }

    public void TurnOnAtHighVolume()
    {
        this.TurnedOn = true;
        GetComponent<AudioSource>().volume = 1.0f;
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
                effect.Play();
                indexOfNextEffectToPlay = Random.Range(0, effects.Count);
            }
            
        }
    }
}
