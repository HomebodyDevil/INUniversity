using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    public enum Equipments
    {
        Head = 0,
        Body = 1,
        Shoes = 2,
        Weapon = 3,
    }

    public static Action MadeEquipmentDictionary;
    public static Action EquipEquipment;
    public static Action UnEquipEquipment;

    public static bool isInitialized = false;

    private static PlayerEquipmentManager instance;

    [SerializeField] private List<A_Equipment> equipmentForDictionary;
    [SerializeField] private List<int> countForDictionary;

    private A_Equipment currentHeadEquip;
    private A_Equipment currentBodyEquip;
    private A_Equipment currentShoesEquip;
    private A_Equipment currentWeaponEquip;

    private EquipSlot currentSelectedEquipSlot;

    //private Dictionary<A_Equipment, int> playerEquipmentListDictionary;
    private Dictionary<int, A_Equipment> playerHaveEquipmentDictionary;
    private Dictionary<int, int> playerHaveEquipmentCount;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static PlayerEquipmentManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Awake()
    {
        Initialize();

        EquipmentPanel.SelectCurrentEquipmentSlot -= OnSelectCurrentEquipmentSlot;
        EquipmentPanel.SelectCurrentEquipmentSlot += OnSelectCurrentEquipmentSlot;

        BattleManager.OnBattleWin -= AddNewEquipmentToPlayer;
        BattleManager.OnBattleWin += AddNewEquipmentToPlayer;
    }

    private void Start()
    {
        //playerEquipmentListDictionary = new Dictionary<A_Equipment, int>();
        playerHaveEquipmentDictionary = new Dictionary<int, A_Equipment>();
        playerHaveEquipmentCount = new Dictionary<int, int>();

        // Test용임 나중에 꼭 지워줄 것.
        for (int i = 0; i < equipmentForDictionary.Count; i++)
        {
            //playerEquipmentListDictionary.Add(equipmentForDictionary[i], countForDictionary[i]);
            A_Equipment equip = equipmentForDictionary[i];
            playerHaveEquipmentDictionary.Add(equip.equipmentID, equip);
            playerHaveEquipmentCount.Add(equip.equipmentID, countForDictionary[i]);
        }

        // Panel을 Enable상태로 실행시 오류가 나지 않기 위해 했던 방법.,
        // 근데 Panel을 Disable상태로 실행하면 오류가 발생하지 않음...
        //MadeEquipmentDictionary.Invoke();
        isInitialized = true;
    }

    //private void OnDisable()
    //{
    //    EquipmentPanel.SelectCurrentEquipmentSlot -= OnSelectCurrentEquipmentSlot;
    //}

    private void OnDestroy()
    {
        EquipmentPanel.SelectCurrentEquipmentSlot -= OnSelectCurrentEquipmentSlot;
        BattleManager.OnBattleWin -= AddNewEquipmentToPlayer;
    }

    public ref Dictionary<int, A_Equipment> GetPlayerHaveEquipmentDictionary()
    {
        return ref playerHaveEquipmentDictionary;
    }

    public ref Dictionary<int, int> GetPlayerHaveEquipmentCount()
    {
        return ref playerHaveEquipmentCount;
    }

    // 현재 선택중인 Slot을 기억하도록 한다.
    public void OnSelectCurrentEquipmentSlot(EquipSlot equipSlot)
    {
        currentSelectedEquipSlot = equipSlot;
    }

    public void EquipCurrentEquipment()
    {
        currentSelectedEquipSlot.Equip();
    }

    public void UnEquipCurrentEquipment()
    {
        currentSelectedEquipSlot.UnEquip();
    }

    public A_Equipment GetCurrentPlayerEquipment(Equipments equip)
    {
        switch (equip)
        {
            case Equipments.Head:
                return currentHeadEquip;
            case Equipments.Body:
                return currentBodyEquip;
            case Equipments.Shoes:
                return currentShoesEquip;
            case Equipments.Weapon:
                return currentWeaponEquip;
            default:
                Debug.Log(string.Format("There is no Equipment : {0}", equip.ToString()));
                return null;
        }
    }

    public void SetCurrentPlayerEquipment(A_Equipment equipment)
    {
        switch (equipment.equipmentType)
        {
            case Equipments.Head:
                if (currentHeadEquip != null)
                    currentHeadEquip.TakeOff();
                currentHeadEquip = equipment;
                currentHeadEquip.PutOn();
                break;
            case Equipments.Body:
                if (currentBodyEquip != null)
                    currentBodyEquip.TakeOff();
                currentBodyEquip = equipment;
                currentBodyEquip.PutOn();
                break;
            case Equipments.Shoes:
                if (currentShoesEquip != null)
                    currentShoesEquip.TakeOff();
                currentShoesEquip = equipment;
                currentShoesEquip.PutOn();
                break;
            case Equipments.Weapon:
                if (currentWeaponEquip != null)
                    currentWeaponEquip.TakeOff();
                currentWeaponEquip = equipment;
                currentWeaponEquip.PutOn();
                break;
            default:
                Debug.Log(string.Format("There is no Equipment : {0}", equipment.ToString()));
                break;
        }
    }

    public void RidCurrentPlayerEquipment(A_Equipment equipment)
    {
        switch (equipment.equipmentType)
        {
            case Equipments.Head:
                currentHeadEquip = null;
                break;
            case Equipments.Body:
                currentBodyEquip = null;
                break;
            case Equipments.Shoes:
                currentShoesEquip = null;
                break;
            case Equipments.Weapon:
                currentWeaponEquip = null;
                break;
            default:
                Debug.Log(string.Format("There is no Equipment : {0}", equipment.ToString()));
                break;
        }
    }

    public void AddNewEquipmentToPlayer()
    {
        Debug.Log("AddNewEquipmentToPlayer");

        List<A_Equipment> equipmentList = BattleManager.Instance().GetDropEquipmentList();
        foreach(A_Equipment newEquipment in equipmentList)
        {
            int equipmentID = newEquipment.equipmentID;

            if (playerHaveEquipmentDictionary.ContainsKey(equipmentID))
            {
                playerHaveEquipmentCount[equipmentID] = Math.Min(playerHaveEquipmentCount[equipmentID] + 1, 99);
            }
            else
            {
                playerHaveEquipmentDictionary.Add(equipmentID, newEquipment);
                playerHaveEquipmentCount.Add(equipmentID, 1);
            }
        }
    }
}
