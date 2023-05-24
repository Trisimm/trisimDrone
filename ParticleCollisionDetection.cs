
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ParticleCollisionDetection : UdonSharpBehaviour
{
    public AudioSource explode;
    private void OnParticleCollision(GameObject other)
    {
        explode.Play();
    }
}
