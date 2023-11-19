using System.Collections;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public int seconds;
    void Update()
    {
        if (gameObject.activeSelf)
            StartCoroutine(AutoDestroyInSeconds());
    }

    IEnumerator AutoDestroyInSeconds()
    {
        if (!gameObject.activeSelf)
            yield return null;

        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
