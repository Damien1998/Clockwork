using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Clockwork/Dialogue Data")]
public class DialogueSpriteData : ScriptableObject
{
    public string characterID;
    public Sprite portrait;
    public SoundManager.Sound sound;
    public FXManager.ParticleFX particleFX;
}
