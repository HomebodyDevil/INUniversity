using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupManage : MonoBehaviour
{

    public GameObject ItemGroup;
    public GameObject DeckGroup;
    public GameObject EquipGroup;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ClosePAnAll());
    }

    IEnumerator ClosePAnAll()
    {
        yield return new WaitForSeconds(1.0f);

        ItemGroup.SetActive(false);
        EquipGroup.SetActive(false);
        DeckGroup.SetActive(false);
    }

}
