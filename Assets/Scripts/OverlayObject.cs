using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayObject : MonoBehaviour
{
    public float fadeDuration;

    private SpriteRenderer spriteRenderer;

    private IEnumerator currentCoroutine;
    private List<IEnumerator> currentExtraCoroutines;

    [SerializeField] private float fadeOutLevel = 0;

    [SerializeField] private List<SpriteRenderer> extraSprites;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    IEnumerator FadeOut()
    {
        while (spriteRenderer.color.a > fadeOutLevel)
        {
            yield return null;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - (Time.deltaTime / fadeDuration));
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, fadeOutLevel);
    }

    IEnumerator FadeOutExtra(SpriteRenderer extraRenderer)
    {
        while (extraRenderer.color.a > fadeOutLevel)
        {
            yield return null;
            extraRenderer.color = new Color(extraRenderer.color.r, extraRenderer.color.g, extraRenderer.color.b, extraRenderer.color.a - (Time.deltaTime / fadeDuration));
        }
        extraRenderer.color = new Color(extraRenderer.color.r, extraRenderer.color.g, extraRenderer.color.b, fadeOutLevel);
    }


    IEnumerator FadeIn()
    {
        while (spriteRenderer.color.a < 1)
        {
            yield return null;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + (Time.deltaTime / fadeDuration));
        }
    }

    IEnumerator FadeInExtra(SpriteRenderer extraRenderer)
    {
        while (extraRenderer.color.a < 1)
        {
            yield return null;
            extraRenderer.color = new Color(extraRenderer.color.r, extraRenderer.color.g, extraRenderer.color.b, extraRenderer.color.a + (Time.deltaTime / fadeDuration));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.tag == "Player")
            {
                StopAllCoroutines();

                currentCoroutine = FadeOut();
                StartCoroutine(currentCoroutine);

                if (extraSprites != null)
                {
                    foreach (SpriteRenderer renderer in extraSprites)
                    {
                        StartCoroutine(FadeOutExtra(renderer));
                    }
                }
            }
            //this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);
        }       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.tag == "Player")
            {
                StopAllCoroutines();

                currentCoroutine = FadeIn();
                StartCoroutine(currentCoroutine);

                if (extraSprites != null)
                {
                    foreach (SpriteRenderer renderer in extraSprites)
                    {
                        StartCoroutine(FadeInExtra(renderer));
                    }
                }
            }
            //this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);
        }       
    }
}
