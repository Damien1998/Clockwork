using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityClickManager : MonoBehaviour
{
    public ParticleSystem[] clickParticles;

    private Vector3 mouseWorldPos;

    [SerializeField]
    private CityMaterialType defaultMaterial;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !UIManager.instance.mouseBlocked && !UIManager.instance.IsPaused)
        {
            var v3 = Input.mousePosition;
            v3.z = 10f;
            mouseWorldPos = Camera.main.ScreenToWorldPoint(v3);

            var foundSurface = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("CityClick"));

            if (foundSurface != null)
            {
                if(foundSurface.TryGetComponent(out CityClickMaterial cityMaterial))
                {
                    SpawnClickFX(cityMaterial.materialType, cityMaterial);
                }
            }
            else
            {
                Debug.Log("Click-Snow");
                SpawnClickFX(defaultMaterial, null);
            }

        }
    }

    private void SpawnClickFX(CityMaterialType materialType, CityClickMaterial surface)
    {
        mouseWorldPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
        switch (materialType)
        {
            case CityMaterialType.NONE:
                break;
            case CityMaterialType.SNOW:
                ParticleSystem ps = Instantiate(clickParticles[0], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXSnow);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.WOOD:
                ps = Instantiate(clickParticles[1], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXWood);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.LEAVES_EVERGREEN:
                ps = Instantiate(clickParticles[2], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXChristmasTree);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.BRICKS:
                ps = Instantiate(clickParticles[3], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXBrick);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.WATER:
                ps = Instantiate(clickParticles[4], mouseWorldPos, clickParticles[4].transform.rotation);
                SoundManager.PlaySound(SoundManager.Sound.SFXWater);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.STONE:
                ps = Instantiate(clickParticles[5], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXPavement);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.GRASS:
                ps = Instantiate(clickParticles[6], mouseWorldPos, clickParticles[6].transform.rotation);
                SoundManager.PlaySound(SoundManager.Sound.SFXGrass);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.PLASTER:
                ps = Instantiate(clickParticles[7], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXBuilding);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.METAL:
                ps = Instantiate(clickParticles[8], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXMetal);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.POI:
                ps = Instantiate(clickParticles[9], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.PoiInteraction);
                ps.Play();
                Destroy(ps, 3f);
                break;
            case CityMaterialType.LIGHT:
                ps = Instantiate(clickParticles[10], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXLight);
                ps.Play();
                Destroy(ps, 3f);
                break;
            case CityMaterialType.LEAVES:
                ps = Instantiate(clickParticles[11], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.Placeholder);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.FLOWERS:
                ps = Instantiate(clickParticles[12], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.Placeholder);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.BRIDS:
                ps = Instantiate(clickParticles[13], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.Placeholder);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.COPPER:
                ps = Instantiate(clickParticles[14], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.SFXMetal);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.DIRT:
                ps = Instantiate(clickParticles[15], mouseWorldPos, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.Placeholder);
                ps.Play();
                Destroy(ps, 2f);
                break;
            case CityMaterialType.PASSERBY:
                if(surface != null)
                {
                    if (surface.TryGetComponent(out Passerby passerby))
                    {
                        passerby.JumpUp();
                        SoundManager.PlaySound(SoundManager.Sound.Placeholder);
                    }
                }

                break;
        }
    }
}
