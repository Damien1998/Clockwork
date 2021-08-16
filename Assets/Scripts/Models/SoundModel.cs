using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Sound", menuName = "Clockwork/Sound")]
public class SoundModel : ScriptableObject
{
    public SoundManager.Sound sound;
    public AudioClip[] audioClip;
}
