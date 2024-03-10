using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NewBehaviourScript : MonoBehaviour
{
    public bool brun = false;
    public GameObject hud;

    // Update is called once per frame
    void Update()
    {
        if (brun)
        {
            run();
            brun = false;
        }
    }

    public void run()
    {
        GameObject go = GameObject.Find("CitiesCanvas");
        foreach(Transform t in go.transform)
        {
            GameObject ch = t.gameObject;
            CityUI city = ch.GetComponent<CityUI>();
            GameObject chch = ch.GetComponentInChildren<CanvasGroup>().gameObject;
            if (chch.GetComponentInChildren<Animation>() == null)
            {
                GameObject hudgo = Instantiate(hud, chch.transform);
                hudgo.name = "CityHUDMessage";
                city.message = hudgo;
                hudgo.SetActive(false);
            }
        }
    }
}
