using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garage_ItemEffects : MonoBehaviour
{
    public ParticleSystem thruster;
    public ParticleSystemRenderer thrusterRenderer;
    public ParticleSystem explosion;
    public ParticleSystemRenderer explosionRenderer;

    public void PlayThruster(Material material)
    {
        thrusterRenderer.material = material;
        thruster.Play();
    }

    public void StopThruster()
    {
        thruster.Stop();
    }

    public void PlayExplosion(Material material)
    {
        explosionRenderer.material = material;
        explosion.Play();
    }

    public void StopExplosion()
    {
        explosion.Stop();
    }
}
