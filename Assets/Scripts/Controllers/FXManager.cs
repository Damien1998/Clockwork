using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FXManager
{
    public enum ParticleFX
    {
        NONE = 0,
        SNOWBALL = 1
    }

    public static void PlayFX(ParticleFX particleFX, Vector3 position)
    {
        var particleObject = GameObject.Instantiate(GetFX(particleFX), position, Quaternion.identity);
        particleObject.Play();
        GameObject.Destroy(particleObject, 10f);
    }

    private static ParticleSystem GetFX(ParticleFX fx)
    {
        foreach (FXModel particleModel in FXController.particleList)
        {
            if (particleModel.particleFX == fx)
            {
                return particleModel.particleSystem;
            }
        }
        return null;
    }
}
