using System.Collections.Generic;
using UnityEngine;

public class DiceInstantiator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> dices;
    
    public void Initialize(int dicesToThrow)
    {
        int num_dices = Mathf.Min(dices.Count, dicesToThrow);
        for(int i=0; i<num_dices; i++)
            dices[i].SetActive(i < num_dices);
    }
}
