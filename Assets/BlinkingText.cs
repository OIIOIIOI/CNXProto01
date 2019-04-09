using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{

    Text textComponent;
    public float delay = 0.5f;

    void Start()
    {
        textComponent = GetComponent<Text>();
        StartCoroutine(Blink());
    }

    public IEnumerator Blink()
    {
        while (true)
        {
            textComponent.enabled = false;
            yield return new WaitForSeconds(this.delay);
            textComponent.enabled = true;
            yield return new WaitForSeconds(this.delay);
        }
    }

}
