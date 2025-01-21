using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting; // 필요

public class UnitManage : MonoBehaviour
{

   public GameObject fightPanel;
   public GameObject ItemPan;
   public GameObject DeckPan;
   public GameObject EquipPan;
   public GameObject MiniGamePan;
   public Slider Hp;
   public Slider Exp;

    private GameObject selectedBox; // 현재 선택된 박스를 저장하는 변수


    public bool isPanelActive = false;
    public RespawnRange RespawnRange;

    private void Start()
    {
        ItemPan = GameObject.Find("Groups").transform.Find("ItemGroup").gameObject;
        DeckPan = GameObject.Find("Groups").transform.Find("DeckGroup").gameObject;
        EquipPan = GameObject.Find("Groups").transform.Find("EquipGroup").gameObject;
        fightPanel = GameObject.Find("Groups").transform.Find("FightGroup").gameObject;
        MiniGamePan = GameObject.Find("Canvas").transform.Find("MiniGameButton").gameObject;
        RespawnRange = GameObject.Find("GameManager").GetComponent<RespawnRange>();

        Hp = GameObject.Find("Canvas").transform.Find("HPBar").GetComponent<Slider>();
        Exp = GameObject.Find("Canvas").transform.Find("EXPbar").GetComponent<Slider>();
        
    }

    public void itemButton()
    {
        ItemPan.SetActive(true);
        isPanelActive = true;
    }

    public void itemButtonOff()
    {
        ItemPan.SetActive(false);
        isPanelActive = false;
    }

    public void DeckButton()
    {
        DeckPan.SetActive(true);
        isPanelActive = true;
    }
    public void DeckButtonOff()
    {
        DeckPan.SetActive(false);
        isPanelActive = false;
    }

    public void EquipButton()
    {
        EquipPan.SetActive(true);
        isPanelActive = true;
    }
    public void EquipButtonOff()
    {
        EquipPan.SetActive(false);
        isPanelActive = false;
    }

    public void fightPanelOn()
    {
        fightPanel.SetActive(true);
        isPanelActive = true;
    }

    public void fightPaneloff()
    {
        fightPanel.SetActive(false);
        isPanelActive=false;
    }

    public void SelectBox(GameObject box)
    {
        selectedBox = box; // 클릭한 박스를 저장
        SceneDataManager.Instance.selectedBox = selectedBox;
        SceneDataManager.Instance.selectedBoxPos = selectedBox.transform.position;
        DontDestroyOnLoad(selectedBox);
    }

    public void fightButton()
    {
        //Hp.value -= 10;
        //Exp.value += 1000;

        fightPaneloff();

        SceneDataManager.Instance.Hp = Hp;
        SceneDataManager.Instance.Exp = Exp;
        SceneDataManager.Instance.NextSceneWithNum(1);

    }

    public void MiniGameButton()
    {
        SceneDataManager.Instance.NextSceneWithNum(2);
    }
}
