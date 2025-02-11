using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static Action<float> OnPlayerDamaged;
    public static Action<float> OnEnemyDamaged;
    public static Action<bool> OnStartBattle;
    public static Action OnPauseBattle;
    public static Action OnBattleWin;
    public static Action OnBattleLose;
    public static Action OnEnemyHPisZero;
    public static Action OnEnemyDead;

    private static BattleManager instance;

    public static bool isInBattle;
    public static bool checkDeathImediate = true;

    [SerializeField, Range(0, 1)] private float timeScale;
    [SerializeField] private Transform enemyTransformInBattle;

    // ID로 Battle Scene으로 넘어간 이후, 잡았으면, 적을 비활성화
    // 못 잡았으면 적을 그대로 활성화상태로 냅두도록 함.
    // ID를 통해, BattleScene으로 넘어가, 프리팹?을 로드할 수 있도록 할 예정.

    public DefaultEnemyData currentEnemyData;
    public Transform currentEnemyInMap;
    public int currentEnemysSpawnersID;
    //private Enemy currentEnemy;

    private List<A_PlayerCard> playerDeck = new List<A_PlayerCard>();
    private List<A_PlayerCard> currentPlayerHandCards = new List<A_PlayerCard>();   // 플레이어가 손에 들고 있는 카드.
    private List<A_PlayerCard> currentPlayerReadyCards = new List<A_PlayerCard>();  // 플레이어의 덱.
        
    private GameObject currentEnemyInBattle;

    [Space(10), Header("EnemySpec")]
    public float currentEnemyHP;
    public float currentEnemyMaxHP;
    public float currentEnemyCost;
    public float currentEnemyMaxCost;
    public float currentEnemyCostIncreaseAmount;
    public float currentEnemyEXPAmount;

    //private List<A_Item> dropItemsList;
    private Dictionary<A_Item, int> dropItemsDictionary;
    private List<A_PlayerCard> dropCardsList;
    private List<A_Equipment> dropEquipmentList;

    private List<DropCard> dropCards;

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

    public static BattleManager Instance()
    {
        if (instance == null) return null;
        else return instance;
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        BattleManager.OnStartBattle -= StartIncreaseEnemyCost;
        BattleManager.OnStartBattle += StartIncreaseEnemyCost;

        BattleManager.OnPauseBattle -= StopIncreaseEnemyCost;
        BattleManager.OnPauseBattle += StopIncreaseEnemyCost;

        BattleManager.OnBattleWin -= StopIncreaseEnemyCost;
        BattleManager.OnBattleWin += StopIncreaseEnemyCost;

        BattleManager.OnBattleWin -= RemoveMapEnemy;
        BattleManager.OnBattleWin += RemoveMapEnemy;

        SceneManagerEX.OnSwitchSceneToBattle -= LoadCards;
        SceneManagerEX.OnSwitchSceneToBattle += LoadCards;

        SceneManagerEX.OnSwitchSceneToBattle -= EnableIsInBattle;
        SceneManagerEX.OnSwitchSceneToBattle += EnableIsInBattle;

        SceneManagerEX.OnSwitchSceneToMap -= DisableIsInBattle;
        SceneManagerEX.OnSwitchSceneToMap += DisableIsInBattle;
    }

    // Start is called before the first frame update
    void Start()
    {
        //dropItemsList = new List<A_Item>();
        dropItemsDictionary = new Dictionary<A_Item, int>();
        dropEquipmentList = new List<A_Equipment>();
        dropCardsList = new List<A_PlayerCard>();

        dropCards = new List<DropCard>();

        isInBattle = false;

        // 이걸 주석처리하니까 실행이 안 되네 그래서 변수 선언에서 초기화도 했음.
        // playerDeck = new List<A_PlayerCard>(1);      
        // currentPlayerHandCards = new List<A_PlayerCard>(1);
        // currentPlayerReadyCards = new List<A_PlayerCard>(1);

        SetPlayerDeck();        
    }

    private void OnDestroy()
    {
        BattleManager.OnStartBattle -= StartIncreaseEnemyCost;
        BattleManager.OnPauseBattle -= StopIncreaseEnemyCost;
        BattleManager.OnBattleWin -= StopIncreaseEnemyCost;
        BattleManager.OnBattleWin -= RemoveMapEnemy;
        SceneManagerEX.OnSwitchSceneToBattle -= LoadCards;
        SceneManagerEX.OnSwitchSceneToBattle -= EnableIsInBattle;
        SceneManagerEX.OnSwitchSceneToMap -= DisableIsInBattle;
    }

    private void EnableIsInBattle()
    {
        isInBattle = true;
    }

    private void DisableIsInBattle()
    {
        isInBattle = false;
    }

    public void SetCurrentEnemyData(GameObject currentEnemyGameObject)
    {
        Enemy currentEnemy = currentEnemyGameObject.GetComponent<Enemy>();
        currentEnemyData = currentEnemy.GetDefaultEnemyData();
        if (currentEnemyData == null)
        {
            Debug.Log("No EnemyData");
            return;
        }
    }

    public void SlowDownTimeScale()
    {
        Time.timeScale = timeScale;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    //public void SetCurrentEnemy(Enemy newEnemy)
    //{
    //    if (newEnemy == null)
    //    {
    //        Debug.Log("newEnemy is Null");
    //        return;
    //    }

    //    currentEnemy = newEnemy;
    //}

    //public Enemy GetCurrtentEnemy()
    //{
    //    return currentEnemy;
    //}

    public void SetEnemy()
    {
        string curretnEnemyInBattlePrefabPath = currentEnemyData.battleEnemyPrefabPath;
        GameObject enemyInBattlePrefab = Resources.Load<GameObject>(curretnEnemyInBattlePrefabPath);

        currentEnemyInBattle = Instantiate<GameObject>(enemyInBattlePrefab);

        currentEnemyInBattle.transform.position = enemyTransformInBattle.position;
        currentEnemyInBattle.transform.rotation = enemyTransformInBattle.rotation;

        currentEnemyInBattle.transform.parent = transform;

        currentEnemyHP = currentEnemyData.maxHP;
        currentEnemyEXPAmount = currentEnemyData.EXPAmount;
        currentEnemyCostIncreaseAmount = currentEnemyData.currentCostIncreaseAmount;

        currentEnemyMaxHP = currentEnemyData.maxHP;
        currentEnemyMaxCost = currentEnemyData.maxCost;
    }

    public void RemoveEnemy()
    {
        Destroy(currentEnemyInBattle);
    }

    public void RemoveMapEnemy()
    {
        Debug.Log("RemoveMapEnemy");

        List<Transform> currentEnemysSpawnersList = EnemySpawnerManager.Instance().GetEnemyList(currentEnemysSpawnersID);
        foreach (Transform enemy in currentEnemysSpawnersList)
        {
            if (enemy == currentEnemyInMap)
            {
                currentEnemysSpawnersList.Remove(enemy);
                Destroy(enemy.gameObject);
                break;
            }
        }
    }

    public void StartIncreaseEnemyCost(bool isFirst)
    {
        if (SceneManagerEX.CurrentScene() != SceneManagerEX.Scenes.BattleScene)
            return;

        if (isFirst)
            ReduceEnemyCost(currentEnemyCost);

        StartCoroutine("IncreaseEnemyCostCoroutine");
    }

    public void StopIncreaseEnemyCost()
    {
        Debug.Log("StopIncreaseEnemyCost");
        StopCoroutine("IncreaseEnemyCostCoroutine");
    }

    IEnumerator IncreaseEnemyCostCoroutine()
    {
        while(true)
        {
            currentEnemyCost = Mathf.Max(currentEnemyCost, 0);
            currentEnemyCost += currentEnemyCostIncreaseAmount * Time.deltaTime;
            currentEnemyCost = Mathf.Min(currentEnemyCost, currentEnemyMaxCost);

            yield return null;
        }
    }

    public void ReducePlayerCost(float cost)
    {
        PlayerSpecManager playerSpecManager = PlayerSpecManager.Instance();
        playerSpecManager.currentPlayerCost -= cost;
    }

    public void ReduceEnemyCost(float cost)
    {
        currentEnemyCost -= cost;
        currentEnemyCost = Mathf.Clamp(currentEnemyCost, 0, currentEnemyMaxCost);
    }

    public void DamageToEnemy(float damage)
    {
        if (damage > 0)
        {
            // 구버전 적들용.
            // 수정하기 귀찮아서 냅둠.
            if (EnemyEffectTransform.EnableEnemyHittedEffect != null)
                EnemyEffectTransform.EnableEnemyHittedEffect.Invoke();

            float randVal = Random.Range(0.7f, 1.0f);
            //Color color = Color.black;
            Color color = new Color(randVal, randVal, randVal);

            if (EnemyHittedEffectTransform.PlayEnemyHittedEffect != null)
                EnemyHittedEffectTransform.PlayEnemyHittedEffect.Invoke(color);
        }

        currentEnemyHP -= damage;

        if ((int)currentEnemyHP <= 0)
        {
            if (OnEnemyHPisZero != null)
            {
                Debug.Log("OnEnemyHPisZero in DamageToEnemy");
                OnEnemyHPisZero.Invoke();
            }

            if (checkDeathImediate)
            {
                Debug.Log("checkDeathImediate in DamageToEnemy");
                EnemyDead();
            }
            else
            {
                StartCoroutine("DelayEnemyDead");
            }
        }

        currentEnemyHP = Mathf.Clamp(currentEnemyHP, 0, currentEnemyMaxHP);      
    }

    IEnumerator DelayEnemyDead()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        EnemyDead();
    }

    public void DamageToPlayer(float damage)
    {
        PlayerEffectTransform.EnablePlayerHittedEffect.Invoke();

        HealToPlayer(-damage);
        float currPlayerHP = PlayerSpecManager.Instance().currentPlayerHP;

        if ((int)currPlayerHP <= 0)
        {
            PlayerDead();
        }
        else
        {
            //HealToPlayer(-damage);
        }
    }

    public void PlayerDead()
    {
        Debug.Log("Player Dead");
        //HealToPlayer(1);

        OnBattleLose.Invoke();
    }

    public void HealToEnemy(float heal)
    {
        // 구버전 적들용.
        // 수정하기 귀찮아서 냅둠.
        if (EnemyEffectTransform.EnableEnemyHealedEffect != null)
            EnemyEffectTransform.EnableEnemyHealedEffect.Invoke(Color.yellow);

        if (EnemyHealedEffectTransform.PlayEnemyHealedEffect != null)
            EnemyHealedEffectTransform.PlayEnemyHealedEffect.Invoke(Color.yellow);


        DamageToEnemy(-heal);
    }

    public void HealToPlayer(float heal)
    {
        //float currentHP = PlayerSpecManager.Instance().currentPlayerHP;
        //PlayerSpecManager.Instance().currentPlayerHP = Mathf.Max(currentHP + heal,
        //                                PlayerSpecManager.Instance().maxPlayerHP);      
        if (heal > 0 && isInBattle)
        {
            PlayerEffectTransform.EnablePlayerHealedEffect.Invoke();
        }

        PlayerSpecManager.Instance().AddValueToCurrentPlayerHP(heal);
    }

    public void EnemyDead()
    {
        Debug.Log("EnemyDead");

        if (currentEnemyHP > 0) return;

        PlayerSpecManager.Instance().GainEXP(currentEnemyEXPAmount);

        if (BattleManager.OnBattleWin != null)
        {
            Debug.Log("Invoke OnBattleWin");
            BattleManager.OnBattleWin.Invoke();
        }
    }

    public A_PlayerCard GetActiveCard(int order)
    {
        // Application.Quit();

        if (order < 0 || order >= currentPlayerHandCards.Count)
        {
            Debug.Log("Out Of RANGE!!!!!");
        }

        return currentPlayerHandCards.ElementAt(order);
    }

    public List<A_PlayerCard> GetPlayerDeck()
    {
        return playerDeck;
    }

    public void WaitPlayerDeckIsReady()
    {
        StartCoroutine("WaitPlayerDeckIsReadyCoroutine");
    }

    IEnumerator WaitPlayerDeckIsReadyCoroutine()
    {
        while (playerDeck == null)
            yield return null;
    }

    //public void SetPlayerDeck(List<A_PlayerCard> newDeck)
    //{
    //    //WaitPlayerDeckIsReady();

    //    //playerDeck.Capacity = newDeck.Count;
    //    //foreach(A_PlayerCard card in newDeck)
    //    //    playerDeck.Add(card);

    //    // Battle에서 사용하게될 Deck의 List를 참조하게됨.
    //    playerDeck = newDeck.ToList();
    //}

    public void SetPlayerDeck()
    {
        // Battle에서 사용하게될 Deck의 List를 참조하게됨.
        playerDeck = PlayerCardManager.Instance().GetPlayerDeckCardList();
        //foreach (A_PlayerCard card in playerDeck)
        //{
        //    if (card == null)
        //    {
        //        Debug.Log(String.Format("this is null : {0}", card));
        //    }
        //}
    }

    public void LoadCards()
    {
        SetPlayerDeck();

        //WaitPlayerDeckIsReady();
        int maxInRange = playerDeck.Count;
        //Debug.Log(string.Format("maxInRange : {0}", maxInRange));

        if (maxInRange <= 0)
        {
            Debug.Log("Thers No Cards in Deck!!!");
            return;
        }

        if (maxInRange < 0)
        {
            Debug.Log("MaxInRange is Out of Range");
            return;
        }

        currentPlayerHandCards.Clear();
        currentPlayerReadyCards = playerDeck.ToList();

        List<int> choseCardIndexes = new List<int>();

        int index = 0;

        int indexMaxCount = Player.Instance().GetPlayerData().maxCardAmount;

        for (int i = 0; i < 3; i++)   // 한 번에 들 수 있는 카드의 최대수만큼 반복.
        {
            int loopCount = 0;
            // 랜덤으로 중복되지 않게 선택.
            do
            {
                index = Random.Range(0, maxInRange);
                loopCount++;
                if (loopCount > 10000)
                {
                    Debug.Log("Maybe Endless Loop");
                    break;
                }
            }
            while (choseCardIndexes.Contains(index));

            choseCardIndexes.Add(index);
            currentPlayerHandCards.Add(playerDeck.ElementAt(index));
        }

        foreach (A_PlayerCard card in currentPlayerHandCards)  // 선택된 카드들은 Ready에서 제외되도록 함.
        {
            if (currentPlayerReadyCards.Contains(card))
            {
                currentPlayerReadyCards.Remove(card);
            }
        }

        //Debug.Log(string.Format("Last Cards In Ready : {0}", currentPlayerReadyCards.Count));        
    }

    public void UseCard(int order)
    {
        if (currentPlayerHandCards[order] == null)
        {
            Debug.Log("currentPlayerHandCard is NULL");
            return;
        }

        A_PlayerCard usedCard = currentPlayerHandCards[order];
        currentPlayerReadyCards.Add(usedCard);

        int playerReadyCards = currentPlayerReadyCards.Count;
        int index = Random.Range(0, playerReadyCards);

        //currentPlayerHandCards.RemoveAt(order);
        //currentPlayerHandCards.Add(currentPlayerReadyCards[index]);
        A_PlayerCard newCard = currentPlayerReadyCards[index];
        currentPlayerHandCards[order] = newCard;
        currentPlayerReadyCards.RemoveAt(index);

        newCard.OnDrawCard();

        PlayerCurrentCardHolder.Act_UpdateHandCardsImages.Invoke();
    }

    private void ShuffleList<T>(List<T> list)
    {
        var tmp = list.OrderBy(item => Guid.NewGuid()).ToList();
        list.Clear();
        list.AddRange(tmp);
    }

    public ref List<A_PlayerCard> GetCurrentPlayerHandCard()
    {
        return ref currentPlayerHandCards;
    }

    public void UpdateDropLists()
    {
        dropItemsDictionary.Clear();
        dropEquipmentList.Clear();
        dropCardsList.Clear();

        Random.InitState((int)(Time.time * 1000));
        int randValue = 0;

        if (currentEnemyData == null)
        {
            Debug.Log("CurrentEnemyData is null");
        }

        List<DropItem> dropItems = currentEnemyData.dropItems;
        //Debug.Log(string.Format("DropItems : {0}", dropItems.Count));
        for (int i = 0; i < dropItems.Count; i++)
        {
            randValue = Random.Range(0, 100);
            //Debug.Log(String.Format("Random Value is : {0}", randValue));

            if (randValue < dropItems[i].chance)
            {
                int num = randValue % dropItems[i].maxDropAmount;
                if (num == 0)
                    num += 1;

                dropItemsDictionary.Add((dropItems[i].item).GetComponent<A_Item>(), num);
            }
        }

        List<DropEquipment> dropEquipment = currentEnemyData.dropEquipment;
        //Debug.Log(string.Format("DropEquipment : {0}", dropEquipment.Count));
        for (int i = 0; i < dropEquipment.Count; i++)
        {
            randValue = Random.Range(0, 100);
            if (randValue < dropEquipment[i].chance)
            {
                dropEquipmentList.Add((dropEquipment[i].equipment).GetComponent<A_Equipment>());
            }
        }

        //List<DropCard> dropCards = currentEnemyData.dropCards;
        //Debug.Log(string.Format("DropCards : {0}", dropCards.Count));        
        dropCards = currentEnemyData.dropCards;
        for (int i = 0; i < dropCards.Count; i++)
        {
            if (dropCards[i] == null)
                Debug.Log("FFF null");

            randValue = Random.Range(0, 100);
            if (randValue < dropCards[i].chance)
            {
                dropCardsList.Add((dropCards[i].card).GetComponent<A_PlayerCard>());
            }
        }

        //currentEnemyInBattle.GetComponent<EnemyInBattle>().Drop();
    }

    //private void SetDropItemList()
    //{
    //    int cnt = currentEnemyData.dropItems.Count;
    //    int randValue = 0;
    //    for (int i = 0; i < cnt; i++)
    //    {
    //        DropItem dropItem = currentEnemyData.dropItems[i];
    //        randValue = Random.Range(0, 100);
    //        if (randValue <= dropItem.chance)
    //        {

    //        }

    //        if (cnt > 1000)
    //        {

    //            Debug.Log("Detected Infinite Loop");
    //            break;
    //        }
    //    }
    //}

    public Dictionary<A_Item, int> GetDropItemsDictionary()
    {
        return dropItemsDictionary;
    }

    public List<A_PlayerCard> GetDropCardsList()
    {
        foreach (A_PlayerCard card in dropCardsList)
            if (card == null)
                Debug.Log("Found null in GetDropCardsList");

        return dropCardsList;
    }

    public List<A_Equipment> GetDropEquipmentList()
    {
        return dropEquipmentList;
    }

    public GameObject GetCurrentEnemyInBattle()
    {
        if (currentEnemyInBattle == null)
        {
            Debug.Log("CurrentEnemyInBattle is Null");
            return null;
        }

        return currentEnemyInBattle;
    }

    //public void SetDropItemsList(List<A_Item> itemList)
    //{
    //    dropItemsList = itemList;
    //}

    //public void SetDropCardsList(List<A_PlayerCard> cardList)
    //{
    //    dropCardsList = cardList;
    //}

    //public void SetDropEquipmentList(List<A_Equipment> equipmentList)
    //{
    //    dropEquipmentList = equipmentList;
    //}
}
