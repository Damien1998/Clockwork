using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New ParticleFX", menuName = "Clockwork/FX")]
public class FXModel : ScriptableObject
{
    public FXManager.ParticleFX particleFX;
    public ParticleSystem particleSystem;
}
