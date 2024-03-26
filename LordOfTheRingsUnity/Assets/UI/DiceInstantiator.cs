using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DiceInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject dicePrefab;
    [SerializeField]
    private Camera dicesCamera;
    [SerializeField]
    private List<Material> diceMaterials;
    [SerializeField]
    private List<GameObject> dicePlaceholders;
    [SerializeField]
    private int debugDices;

    private int launchNumberOfDices;
    
    private GameObject[] dicesLaunched;
    private int[] dicesResults;

    private bool isDicing;

    public void Awake()
    {
        isDicing = false;
        launchNumberOfDices = 0;
        debugDices = 0;
    }
    public void Initialize(int dicesToThrow)
    {
        launchNumberOfDices = Mathf.Min(dicePlaceholders.Count, dicesToThrow);
        dicesResults = new int[launchNumberOfDices];
        dicesLaunched = new GameObject[launchNumberOfDices];
        for (int i = 0; i < dicePlaceholders.Count; i++)
            DeleteDie(i);
        for (int i = 0; i < launchNumberOfDices; i++)
            ThrowDie(i);   
        Vector3 rotationEuler = dicesCamera.transform.rotation.eulerAngles;
        rotationEuler.z = Random.Range(0, 355);
        dicesCamera.transform.rotation = Quaternion.Euler(rotationEuler);
        isDicing = true;
    }

    void ThrowDie(int dieNum)
    {
        dicesResults[dieNum] = -1;
        GameObject dice = Instantiate(dicePrefab, dicePlaceholders[dieNum].transform);
        dice.name = "dice_" + dieNum.ToString();
        dice.GetComponentInChildren<MeshRenderer>().material = diceMaterials[dieNum];
        dicesLaunched[dieNum] = dice;
    }
    void DeleteDie(int dieNum)
    {
        if (dicePlaceholders[dieNum].transform.childCount > 0)
            DestroyImmediate(dicePlaceholders[dieNum].transform.GetChild(0).gameObject);
    }
    void Update()
    {
        if (debugDices > 0)
        {
            Initialize(debugDices);
            debugDices = 0;
        }
        if (isDicing)
        {
            for (int i = 0; i < dicesLaunched.Length; i++)
            {
                if (dicesLaunched[i].GetComponentInChildren<Die>().value == -1)
                {
                    DeleteDie(i);
                    ThrowDie(i);
                }
                else if (dicesLaunched[i].GetComponentInChildren<Die>().value > 0)
                {
                    dicesResults[i] = dicesLaunched[i].GetComponentInChildren<Die>().value;
                    dicesLaunched[i].GetComponentInChildren<Die>().enabled = false;

                    Debug.Log(string.Format("Retrieved dice {0} results {1}", dicesLaunched[i].name, dicesResults[i]));
                    Transform GO1 = dicesLaunched[i].transform;
                    Transform GO2 = dicesLaunched[i].transform.Find(dicesResults[i].ToString()).transform;
                    Quaternion rotation = Quaternion.LookRotation(dicesCamera.transform.position - GO2.position, Vector3.up);
                    GO1.rotation = rotation * Quaternion.Inverse(GO2.localRotation);
                }
            }
            isDicing = new List<int>(dicesResults).IndexOf(-1) != -1;
        }
    }

    public List<int> GetDicesResults()
    {
        if (!isDicing)
            return new List<int>();
        
        return new List<int>(dicesResults);
    }

    public bool IsDicing()
    {
        return isDicing;
    }

}
