using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{

    public CinemachineVirtualCamera cineCam;

    protected Vector3 lastVector;
    protected float sinceShakeTime = 0.0f;
    protected float shakeIntensity = 0.2f;
    protected float lastRotation;
    protected float currentZoom;

    private void Awake ()
    {
        currentZoom = cineCam.m_Lens.OrthographicSize;
    }

    private void OnPreRender ()
    {
        if (sinceShakeTime > 0.0f)
        {
            lastVector = Random.insideUnitCircle * shakeIntensity;
            transform.localPosition = transform.localPosition + lastVector;

            lastRotation = Random.Range(-2, 2);
            cineCam.m_Lens.Dutch = lastRotation;
        }
    }

    private void Update ()
    {
        cineCam.m_Lens.OrthographicSize = Mathf.MoveTowards(cineCam.m_Lens.OrthographicSize, currentZoom, 0.2f);
    }

    private void OnPostRender ()
    {
        if (sinceShakeTime > 0.0f)
        {
            transform.localPosition = transform.localPosition - lastVector;
            sinceShakeTime -= Time.deltaTime;
        }
        else
            cineCam.m_Lens.Dutch = 0;
    }

    public void Shake (float amount, float time)
    {
        shakeIntensity = amount;
        sinceShakeTime = time;
    }

    public void Zoom (float zoom)
    {
        currentZoom = zoom;
    }

}
