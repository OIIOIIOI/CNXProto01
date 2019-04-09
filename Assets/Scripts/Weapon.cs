using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public GameObject shotPrefab;
    public float shotsPerSecond = 1f;
    public int burstShots = 1;
    public float shotSpeed = 22f;
    public float shotSize = 1f;
    public Color32 shotColor = new Color32(255, 255, 255, 255);
    public float shotSpawnX = 6f;
    public float shotSpawnY = 0f;
    [Range(0f, 1f)]
    public float shotSpawnAccuracy = 1f;
    [Range(0f, 1f)]
    public float directionAccuracy = 1f;
    [Range(0f, 1f)]
    public float visualPower = 0f;
    
    public float GetShotSpawnDelay ()
    {
        return 1f / shotsPerSecond;
    }

    public Vector3 GetShotSpawnPosition(float facing = 1f)
    {
        Vector3 basePosition = new Vector3(shotSpawnX * facing, shotSpawnY, 0f);
        if (shotSpawnAccuracy < 1f)
        {
            float ax = 0.5f * (1f - shotSpawnAccuracy);
            float ay = 0.15f * (1f - shotSpawnAccuracy);
            return basePosition + new Vector3(Random.Range(0f, ax), Random.Range(-ay, ay), 0f);
        }
        return basePosition;
    }

    public Vector3 GetShotVelocity (float facing = 1f)
    {
        Vector3 basePosition = new Vector2(shotSpeed * facing, 0f);
        if (directionAccuracy < 1f)
        {
            float a = 2f * (1f - directionAccuracy);
            return basePosition + new Vector3(0f, Random.Range(-a, a), 0f);
        }
        return basePosition;
    }

    public float GetRecoil (float facing = 1f)
    {
        return 5f * visualPower * facing;
    }

}
