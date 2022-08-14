using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasserbySpawner : MonoBehaviour
{
    public float spawnMinInterval, spawnMaxInterval;
    public float spawnPercent = 100f;
    public float yRange;
    public int burstMin, burstMax;
    public float direction;

    public Sprite[] passerbySprites;
    public Color[] passerbyColors;

    public float passerbySpeedMin, passerbySpeedMax;

    public bool spawnerActive = true;

    public Passerby passerbyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPassersby());
    }

    IEnumerator SpawnPassersby()
    {
        while (spawnerActive)
        {
            float delay = Random.Range(spawnMinInterval, spawnMaxInterval);
            
            var randomRoll = Random.Range(0, 100);
            if (randomRoll < spawnPercent)
            {
                var burst = Random.Range(burstMin, burstMax + 1);
                for (int i = 0; i < burst; i++)
                {
                    float yPos = transform.position.y + Random.Range(-yRange, yRange);

                    Vector2 spawnPos = new Vector2(transform.position.x, yPos);

                    Passerby passerby = Instantiate(passerbyPrefab, spawnPos, Quaternion.identity);
                    passerby.direction = direction;
                    passerby.speed = Random.Range(passerbySpeedMin, passerbySpeedMax);
                    passerby.spriteRenderer.color = passerbyColors[Random.Range(0, passerbyColors.Length)];
                    passerby.spriteRenderer.sprite = passerbySprites[Random.Range(0, passerbySprites.Length)];
                }
            }

            yield return new WaitForSeconds(delay);
        }

    }
}
