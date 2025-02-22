using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerSpecManager : MonoBehaviour
{
    private static PlayerSpecManager instance;

    public bool isInitialized;

    [SerializeField] float hpAddAmount;
    [SerializeField] float attackPointAddAmount;

    [Space(10), Header("Settings")]
    public int currentPlayerLevel;
    public float currentPlayerHP;
    public float currentPlayerEXP;
    public float currentPlayerAttackPoint;
    public float currentPlayerCost;

    private float originalPlayerMaxHP;
    private float originalPlayerAttackPoint;
    private float originalPlayerMaxCost;
    private float originalCostIncreaseAmount;

    public float maxPlayerHP;
    public float maxPlayerEXP;
    public float maxPlayerAttackPoint;
    public float maxPlayerCost;

    public float currentCostIncreaseAmount;     // 1초당 증가할 Cost

    public int playerItemSlotsNumber;
    public List<A_Item> playerItemList;
    public List<A_Equipment> playerEquipmentList;
    public List<A_PlayerCard> playerCardList;
    List<A_PlayerCard> playerDeckFromPlayerData;
    // public Dictionary<A_Item, int> playerItemsListDictionary;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public static PlayerSpecManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Awake()
    {
        BattleManager.OnStartBattle -= StartIncreaseCost;
        BattleManager.OnStartBattle += StartIncreaseCost;

        BattleManager.OnPauseBattle -= StopIncreaseCost;
        BattleManager.OnPauseBattle += StopIncreaseCost;

        SceneManagerEX.OnSwitchSceneToMap -= UpdateSpec;
        SceneManagerEX.OnSwitchSceneToMap += UpdateSpec;

        SceneManagerEX.OnSwitchSceneToBattle -= SaveOriginalSpec;
        SceneManagerEX.OnSwitchSceneToBattle += SaveOriginalSpec;

        SceneManagerEX.OnSwitchSceneToMap -= RestoreOriginalSpec;
        SceneManagerEX.OnSwitchSceneToMap += RestoreOriginalSpec;

        Initialize();
    }

    private void OnDestroy()
    {
        BattleManager.OnStartBattle -= StartIncreaseCost;
        BattleManager.OnPauseBattle -= StopIncreaseCost;
        SceneManagerEX.OnSwitchSceneToMap -= UpdateSpec;
        SceneManagerEX.OnSwitchSceneToBattle -= SaveOriginalSpec;
        SceneManagerEX.OnSwitchSceneToMap -= RestoreOriginalSpec;
    }

    //private void OnEnable()
    //{
    //    BattleManager.OnStartBattle -= StartIncreaseCost;
    //    BattleManager.OnStartBattle += StartIncreaseCost;

    //    BattleManager.OnPauseBattle -= StopIncreaseCost;
    //    BattleManager.OnPauseBattle += StopIncreaseCost;
    //}

    // Start is called before the first frame update

    void Start()
    {
        isInitialized = false;

        InitializePlayerSpec();
        MapUIManager.Instance().UpdatePlayerLevelText();

        UpdateSpec();
    }

    private void SaveOriginalSpec()
    {
        originalPlayerMaxHP = maxPlayerHP;
        originalPlayerAttackPoint = currentPlayerAttackPoint;
        originalPlayerMaxCost = maxPlayerCost;
        originalCostIncreaseAmount = currentPlayerAttackPoint;
    }

    private void RestoreOriginalSpec()
    {
        maxPlayerHP = originalPlayerMaxHP;
        currentPlayerAttackPoint = originalPlayerAttackPoint;
        maxPlayerCost = originalPlayerMaxCost;
        currentPlayerAttackPoint = originalCostIncreaseAmount;
    }

    public void UpdateSpec()
    {
        float hpRatio = currentPlayerHP / maxPlayerHP;
        maxPlayerHP = Mathf.Max(maxPlayerHP + currentPlayerLevel * hpAddAmount, 0.0f);
        currentPlayerHP = maxPlayerHP * hpRatio;

        maxPlayerAttackPoint = Mathf.Max(currentPlayerAttackPoint + currentPlayerLevel * attackPointAddAmount, 0.0f);
    }

    public void InitializePlayerSpec()
    {
        if (!isInitialized)
        {
            DefaultPlayerData playerData = Player.Instance().GetPlayerData();

            currentPlayerLevel = 0;
            currentPlayerHP = playerData.maximumHp;
            //currentPlayerEXP = 0;
            currentPlayerAttackPoint = playerData.attackPoint;
            //currentPlayerCost;

            maxPlayerHP = playerData.maximumHp;
            maxPlayerEXP = playerData.maximumExp;
            //maxPlayerAttackPoint;
            maxPlayerCost = playerData.maximumCost;

            playerItemSlotsNumber = playerData.ItemSlotsNumber;
            playerItemList = playerData.playerItems;
            playerEquipmentList = playerData.playerEquipment;

            //playerItemsListDictionary = playerData.playerItemsListDictionary;

            //playerDeckFromPlayerData = playerData.playerDeck;
            //PlayerCardManager.Instance().SetDeckList(ref playerDeckFromPlayerData);

            //PlayerCardManager.isInitialized = true;

            //Debug.Log("COUNT : " + PlayerCardManager.Instance().GetPlayerDeckCardList().Count);

            isInitialized = true;
        }
        else
        {

        }
    }

    public void StartIncreaseCost(bool isFirst)
    {
        if (SceneManagerEX.CurrentScene() != SceneManagerEX.Scenes.BattleScene)
            return;

        if (isFirst)
            RemoveCost(currentPlayerCost);

        StartCoroutine("IncreaseCostCoroutine");
    }

    public void RemoveCost(float cost)
    {
        currentPlayerCost -= cost;
    }

    public void StopIncreaseCost()
    {
        StopCoroutine("IncreaseCostCoroutine");
    }

    IEnumerator IncreaseCostCoroutine()
    {
        while (true)
        {
            currentPlayerCost = Mathf.Max(currentPlayerCost, 0);
            currentPlayerCost += currentCostIncreaseAmount * Time.deltaTime;
            currentPlayerCost = Mathf.Min(currentPlayerCost, maxPlayerCost);

            yield return null;
        }
    }

    public void GainEXP(float exp)
    {
        currentPlayerEXP += exp;
        if (currentPlayerEXP >= maxPlayerEXP)
        {
            LevelUp();
        }
    }    

    public void LevelUp()
    {
        currentPlayerEXP = currentPlayerEXP - maxPlayerEXP;
        currentPlayerLevel++;
        maxPlayerEXP *= 1.3f;
    }

    public void AddValueToCurrentPlayerHP(float n)
    {
        currentPlayerHP = currentPlayerHP + n;
        currentPlayerHP = Mathf.Clamp(currentPlayerHP, 0, maxPlayerHP);
    }

    public void SetValueToCurrentPlayerHP(float n)
    {
        currentPlayerHP = Mathf.Clamp(n, 0, maxPlayerHP);
    }

    public void SetCurrentPlayerHP(float n)
    {
        currentPlayerHP = Mathf.Clamp(n, 0, maxPlayerHP);
    }

    public float GetCurrentPlayerAttackPoint()
    {
        return currentPlayerAttackPoint;
    }

    public void SetCurrentPlayerAttackPoint(float attackPoint)
    {
        currentPlayerAttackPoint = Mathf.Max(0.0f, attackPoint);
    }
}
