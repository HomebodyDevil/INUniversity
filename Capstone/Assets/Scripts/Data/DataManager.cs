using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    [SerializeField] private GameObject cloudPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject optionButton;
    [SerializeField] private Transform childTransform;

    [Space(10), Header("Cloud Buttons")]
    [SerializeField] private Button cloudSaveButton;
    [SerializeField] private Button cloudLoadButton;

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

    string defaultPath;
    string saveDataDirectoryPath;
    string saveDataFilePath;

    private bool canLoad;

    void Start()
    {
        Initialize();

        cloudSaveButton.onClick.AddListener(
            async () =>
            {
                await SaveCloud();
            }
            );

        cloudLoadButton.onClick.AddListener(
            async () =>
            {
                await LoadCloud();
            });

        // 경로 참고
        // 회사명은 기본값 DefaultCompany
        // PC : "C:/Users/사용자/AppData/LocalLow/회사명/프로젝트/"
        // IOS : ".../Android/data/com.회사명.프로젝트/files/"

        canLoad = true;

        //defaultPath = Application.dataPath;
        defaultPath = Application.persistentDataPath;
        saveDataDirectoryPath = $"{defaultPath}/savedata";
        saveDataFilePath = $"{saveDataDirectoryPath}/Save.json";

        //BattleManager.OnBattleWin -= SaveData;
        //BattleManager.OnBattleWin += SaveData;

        //BattleManager.OnBattleLose -= SaveData;
        //BattleManager.OnBattleLose += SaveData;

        SceneManagerEX.OnSwitchSceneToBattle -= DisableOptionButton;
        SceneManagerEX.OnSwitchSceneToBattle += DisableOptionButton;

        SceneManagerEX.OnSwitchSceneToMap -= EnableOptionButton;
        SceneManagerEX.OnSwitchSceneToMap += EnableOptionButton;

        //Debug.Log(saveDataDirectoryPath);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveData();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            LoadData();
        }
    }

    private void OnDestroy()
    {
        //BattleManager.OnBattleWin -= SaveData;
        //BattleManager.OnBattleLose -= SaveData;
        SceneManagerEX.OnSwitchSceneToBattle -= DisableOptionButton;
        SceneManagerEX.OnSwitchSceneToMap -= EnableOptionButton;
    }

    public static DataManager Instance()
    {
        return instance;
    }

    public void ResetData()
    {
        if (File.Exists(saveDataFilePath))
            File.Delete(saveDataFilePath);
    }

    public void SaveData()
    {
        Debug.Log("Save");

        SaveData data = new SaveData();

        SaveSpec(ref data);
        SaveItems(ref data);
        SaveEquipment(ref data);
        SaveCards(ref data);

        string json = JsonUtility.ToJson(data);

        CheckFile();

        File.WriteAllText(saveDataFilePath, json);
    }

 #region SaveFuncions
    private void SaveSpec(ref SaveData data)
    {
        PlayerSpecManager manager = PlayerSpecManager.Instance();
        if (manager == null)
        {
            Debug.Log("manager is null");
            return;
        }

        data.playerSpec = new PlayerSpecData();

        data.playerSpec.currentPlayerLevel = manager.currentPlayerLevel;
        data.playerSpec.currentPlayerHP = manager.currentPlayerHP;
        data.playerSpec.currentPlayerAttackPoint = manager.currentPlayerAttackPoint;
        data.playerSpec.currentPlayerCost = manager.currentPlayerCost;
        data.playerSpec.currentPlayerEXP = manager.currentPlayerEXP;

        data.playerSpec.maxPlayerHP = manager.maxPlayerHP;
        data.playerSpec.maxPlayerEXP = manager.maxPlayerEXP;
        data.playerSpec.maxPlayerAttackPoint = manager.maxPlayerAttackPoint;
        data.playerSpec.maxPlayerCost = manager.maxPlayerCost;

        data.playerSpec.currentCostIncreaseAmount = manager.currentCostIncreaseAmount;

        data.playerSpec.bgmVolume = VolumeSlider.bgmVolume;
        data.playerSpec.effectVolume = VolumeSlider.effectVolume;

        InitGuid();
        data.playerSpec.guid = manager.guid.ToString();
    }

    public void InitGuid()
    {
        if (PlayerSpecManager.Instance().guid == default(Guid))
        {
            PlayerSpecManager.Instance().guid = Guid.NewGuid();
        }
    }

    private void SaveItems(ref SaveData data)
    {
        PlayerItemsManager manager = PlayerItemsManager.Instance();
        if (manager == null)
        {
            Debug.Log("manager is null");
            return;
        }

        data.playerItems = new PlayerItemsData();
        data.playerItems.playerItemIDs = new List<int>();
        data.playerItems.playeItemCount = new List<int>();

        //int count = manager.GetPlayerItemCount().Count;        
        foreach (int i in manager.GetPlayerItemDictionary().Keys)
        {
            int itemID = i;
            if (!manager.GetPlayerItemCount().ContainsKey(itemID))
            {
                Debug.Log("Key Exist X");
                continue;
            }

            int itemCount = manager.GetPlayerItemCount()[itemID];

            data.playerItems.playerItemIDs.Add(itemID);
            data.playerItems.playeItemCount.Add(itemCount);
        }
    }

    private void SaveEquipment(ref SaveData data)
    {
        PlayerEquipmentManager manager = PlayerEquipmentManager.Instance();
        if (manager == null)
        {
            Debug.Log("manager is null");
            return;
        }

        data.playerEquipment = new PlayerEquipmentData();

        if (manager.currentWeaponEquip != null)
            data.playerEquipment.currentWeaponEquipID = manager.currentWeaponEquip.equipmentID;

        if (manager.currentHeadEquip != null)
            data.playerEquipment.currentHeadEquipID = manager.currentHeadEquip.equipmentID;

        if (manager.currentBodyEquip != null)
            data.playerEquipment.currentBodyEquipID = manager.currentBodyEquip.equipmentID;

        if (manager.currentShoesEquip != null)
            data.playerEquipment.currentShoesEquipID = manager.currentShoesEquip.equipmentID;

        data.playerEquipment.playerHaveEquipmentIDs = new List<int>();
        data.playerEquipment.playerHaveEquipmentCount = new List<int>();
        foreach (int id in manager.GetPlayerHaveEquipmentDictionary().Keys)
        {
            data.playerEquipment.playerHaveEquipmentIDs.Add(id);

            int count = manager.GetPlayerHaveEquipmentCount()[id];
            data.playerEquipment.playerHaveEquipmentCount.Add(count);
        }
    }

    private void SaveCards(ref SaveData data)
    {
        PlayerCardManager manager = PlayerCardManager.Instance();
        if (manager == null)
        {
            Debug.Log("manager is null");
            return;
        }

        data.playerCards = new PlayerCardsData();

        data.playerCards.haveCardIDs = new List<int>();
        data.playerCards.haveCardCounts = new List<int>();
        data.playerCards.deckCardIDs = new List<int>();

        foreach (int id in manager.playerHaveCardsDictionary.Keys)
        {
            data.playerCards.haveCardIDs.Add(id);

            int count = manager.playerHaveCardsCount[id];
            data.playerCards.haveCardCounts.Add(count);
        }

        foreach(A_PlayerCard deckCard in manager.playerDeckCardList)
        {
            int id = deckCard.cardID;
            data.playerCards.deckCardIDs.Add(id);
        }
    }
    #endregion
        
    public void LoadData()
    {
        if (!canLoad)
        {
            Debug.Log("Cant Load");
        }

        Debug.Log("Load");

        if (!File.Exists(saveDataFilePath))
            return;        

        CheckFile();

        string json = File.ReadAllText(saveDataFilePath);

        while (childTransform.childCount > 0)
            DestroyImmediate(childTransform.GetChild(0).gameObject);

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        LoadSpec(ref data);
        LoadItems(ref data);
        LoadEquipment(ref data);
        LoadCards(ref data);

        StartCoroutine(LoadCool());

        //Debug.Log(data);
    }

    IEnumerator LoadCool()
    {
        canLoad = false;
        yield return new WaitForSeconds(6.0f);
        canLoad = true;
    }

#region LoadFunctions
    private void LoadSpec(ref SaveData data)
    {
        //PlayerSpecManager manager = PlayerSpecManager.Instance();
        if (PlayerSpecManager.Instance() == null)
        {
            Debug.Log("manager is null");
            return;
        }

        PlayerSpecManager.Instance().guid = Guid.Parse(data.playerSpec.guid);

        PlayerSpecManager.Instance().currentPlayerLevel = data.playerSpec.currentPlayerLevel;
        PlayerSpecManager.Instance().currentPlayerHP = data.playerSpec.currentPlayerHP;
        PlayerSpecManager.Instance().currentPlayerAttackPoint = data.playerSpec.currentPlayerAttackPoint;
        PlayerSpecManager.Instance().currentPlayerCost = data.playerSpec.currentPlayerCost;
        PlayerSpecManager.Instance().currentPlayerEXP = data.playerSpec.currentPlayerEXP;

        PlayerSpecManager.Instance().maxPlayerHP = data.playerSpec.maxPlayerHP;
        PlayerSpecManager.Instance().maxPlayerEXP = data.playerSpec.maxPlayerEXP;
        PlayerSpecManager.Instance().maxPlayerAttackPoint = data.playerSpec.maxPlayerAttackPoint;
        PlayerSpecManager.Instance().maxPlayerCost = data.playerSpec.maxPlayerCost;

        PlayerSpecManager.Instance().currentCostIncreaseAmount = data.playerSpec.currentCostIncreaseAmount;

        VolumeSlider.bgmVolume = data.playerSpec.bgmVolume;
        VolumeSlider.effectVolume = data.playerSpec.effectVolume;

        Debug.Log($"{data.playerSpec.bgmVolume}\n{data.playerSpec.effectVolume}");
        SoundManager.Instance().SetAudioVolume("BGM", data.playerSpec.bgmVolume);
        SoundManager.Instance().SetAudioVolume("Effect", data.playerSpec.effectVolume);

        if (MapUIManager.Instance() != null)
            MapUIManager.Instance().UpdatePlayerLevelText();

        if (VolumeSlider.OnUpdateSlider != null)
            VolumeSlider.OnUpdateSlider.Invoke();

        if (PlayerStatusTextInEquipment.Act_UpdatePlayerStatusTextInEquipment != null)
            PlayerStatusTextInEquipment.Act_UpdatePlayerStatusTextInEquipment.Invoke();
        
        if (PlayerStatusText.Act_UpdatePlayerStatusValueText != null)
            PlayerStatusText.Act_UpdatePlayerStatusValueText.Invoke();
    }

    private void LoadItems(ref SaveData data)
    {
        if (PlayerItemsManager.Instance() == null)
        {
            Debug.Log("manager is null");
            return;
        }

        PlayerItemsManager.Instance().GetPlayerItemCount().Clear();
        PlayerItemsManager.Instance().GetPlayerItemDictionary().Clear();
        int count = data.playerItems.playerItemIDs.Count;

        int i = 0;
        foreach (int id in data.playerItems.playerItemIDs)
        {
            string path = GetPrefabPath(id);
            //Debug.Log(path);

            GameObject itemObject = Resources.Load<GameObject>(path);
            itemObject = Instantiate(itemObject);

            itemObject.transform.SetParent(childTransform, false);

            A_Item currItem = itemObject.GetComponent<A_Item>();
            PlayerItemsManager.Instance().GetPlayerItemDictionary().Add(id, currItem);

            int currItemCount = data.playerItems.playeItemCount[i];
            PlayerItemsManager.Instance().GetPlayerItemCount().Add(id, currItemCount);

            i++;
        }

        if (ItemPanel.UpdateItemPanel != null)
            ItemPanel.UpdateItemPanel.Invoke(PlayerItemsManager.Instance().GetPlayerItemDictionary());
    }

    private void LoadEquipment(ref SaveData data)
    {
        if (PlayerEquipmentManager.Instance() == null)
        {
            Debug.Log("manager is null");
            return;
        }

        if (data.playerEquipment.currentHeadEquipID != 0)
        {
            int id = data.playerEquipment.currentHeadEquipID;
            string objectPath = GetPrefabPath(id);

            GameObject equipmentObject = Resources.Load<GameObject>(objectPath);
            equipmentObject = Instantiate(equipmentObject);

            equipmentObject.transform.SetParent(childTransform, false);

            A_Equipment currEquipment = equipmentObject.GetComponent<A_Equipment>();
            PlayerEquipmentManager.Instance().currentHeadEquip = currEquipment;
        }

        if (data.playerEquipment.currentWeaponEquipID != 0)
        {
            int id = data.playerEquipment.currentWeaponEquipID;
            string objectPath = GetPrefabPath(id);

            GameObject equipmentObject = Resources.Load<GameObject>(objectPath);
            equipmentObject = Instantiate(equipmentObject);

            equipmentObject.transform.SetParent(childTransform, false);

            A_Equipment currEquipment = equipmentObject.GetComponent<A_Equipment>();
            PlayerEquipmentManager.Instance().currentWeaponEquip = currEquipment;
        }

        if (data.playerEquipment.currentBodyEquipID != 0)
        {
            int id = data.playerEquipment.currentBodyEquipID;
            string objectPath = GetPrefabPath(id);

            GameObject equipmentObject = Resources.Load<GameObject>(objectPath);
            equipmentObject = Instantiate(equipmentObject);

            equipmentObject.transform.SetParent(childTransform, false);

            A_Equipment currEquipment = equipmentObject.GetComponent<A_Equipment>();
            PlayerEquipmentManager.Instance().currentBodyEquip = currEquipment;
        }

        if (data.playerEquipment.currentShoesEquipID != 0)
        {
            int id = data.playerEquipment.currentShoesEquipID;
            string objectPath = GetPrefabPath(id);

            GameObject equipmentObject = Resources.Load<GameObject>(objectPath);
            equipmentObject = Instantiate(equipmentObject);

            equipmentObject.transform.SetParent(childTransform, false);

            A_Equipment currEquipment = equipmentObject.GetComponent<A_Equipment>();
            PlayerEquipmentManager.Instance().currentShoesEquip = currEquipment;
        }

        int i = 0;
        PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentDictionary().Clear();
        PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentCount().Clear();
        foreach (int id in data.playerEquipment.playerHaveEquipmentIDs)
        {
            string equipPath = GetPrefabPath(id);
            GameObject currEquipObject = Resources.Load<GameObject>(equipPath);
            currEquipObject = Instantiate(currEquipObject);

            currEquipObject.transform.SetParent(childTransform, false);

            A_Equipment currEquip = currEquipObject.GetComponent<A_Equipment>();
            PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentDictionary().Add(id, currEquip);

            int count = data.playerEquipment.playerHaveEquipmentCount[i];
            PlayerEquipmentManager.Instance().GetPlayerHaveEquipmentCount().Add(id, count);

            i++;
        }

        if (EquipmentPanel.UpdateEquipmentSlots != null)
            EquipmentPanel.UpdateEquipmentSlots.Invoke();

        if (EquipmentImageHolder.UpdateEquipmentImageHolderImage != null)
            EquipmentImageHolder.UpdateEquipmentImageHolderImage.Invoke();
    }

    private void LoadCards(ref SaveData data)
    {
        if (PlayerCardManager.Instance() == null)
        {
            Debug.Log("Manager is Null");
            return;
        }

        Debug.Log($"1-{PlayerCardManager.Instance().playerDeckCardList == null}");

        //List<A_PlayerCard> deck = PlayerCardManager.Instance().GetPlayerDeckCardList();
        //Dictionary<int, A_PlayerCard> haveCards = PlayerCardManager.Instance().GetPlayerHaveCardDictionary();
        //Dictionary<int, int> haveCardCounts = PlayerCardManager.Instance().GetPlayerHaveCardsCount();
        //deck.Clear();
        //haveCards.Clear();
        //haveCardCounts.Clear();

        //PlayerCardManager.Instance().playerHaveCardsDictionary.Clear();
        //PlayerCardManager.Instance().playerDeckCardList.Clear();
        //PlayerCardManager.Instance().playerHaveCardsCount.Clear();

        PlayerCardManager.Instance().ClearDeckCardList();
        PlayerCardManager.Instance().ClearHaveCardDictionary();
        PlayerCardManager.Instance().ClearCardCountDictionary();

        int i = 0;
        foreach (int id in data.playerCards.haveCardIDs)
        {
            string path = GetPrefabPath(id);
            GameObject cardObject = Resources.Load<GameObject>(path);
            cardObject = Instantiate(cardObject);

            cardObject.transform.SetParent(childTransform, false);

            A_PlayerCard currCard = cardObject.GetComponent<A_PlayerCard>();
            PlayerCardManager.Instance().AddNewHaveCard(currCard);            

            //cardObject = Resources.Load<GameObject>(path);
            //cardObject = Instantiate(cardObject);
            if (data.playerCards.deckCardIDs.Contains(id))
                PlayerCardManager.Instance().AddNewDeckCard(currCard);

            int count = data.playerCards.haveCardCounts[i];
            PlayerCardManager.Instance().AddNewCardCount(id, count);

            //haveCardCounts.Add(id, count);

            i++;
        }

        //int i = 0;
        //foreach (int id in data.playerCards.haveCardIDs)
        //{
        //    string path = GetPrefabPath(id);
        //    GameObject cardObject = Resources.Load<GameObject>(path);
        //    cardObject = Instantiate(cardObject);

        //    A_PlayerCard currCard = cardObject.GetComponent<A_PlayerCard>();
        //    haveCards.Add(id, currCard);

        //    //cardObject = Resources.Load<GameObject>(path);
        //    //cardObject = Instantiate(cardObject);
        //    if (data.playerCards.deckCardIDs.Contains(id))
        //        deck.Add(currCard);

        //    int count = data.playerCards.haveCardCounts[i];
        //    haveCardCounts.Add(id, count);

        //    i++;
        //}

        Debug.Log($"2-{PlayerCardManager.Instance().playerDeckCardList == null}");

        if (DeckPanel.Act_UpdateDeckImages != null)
            DeckPanel.Act_UpdateDeckImages.Invoke();

        if (DeckPanel.UpdateCardSlots != null)
            DeckPanel.UpdateCardSlots.Invoke();
    }

#endregion

    private void CheckFile()
    {
        Debug.Log("Check File");

        if (!Directory.Exists(saveDataDirectoryPath))
            Directory.CreateDirectory(saveDataDirectoryPath);

        FileStream file = null;
        if (!File.Exists(saveDataFilePath))
        {
            file = File.Create(saveDataFilePath);            
        }

        if (file != null)
            file.Close();
    }

    //public void CloudLoad()
    //{
    //    if (!AuthenticationService.Instance.IsSignedIn)
    //    {
    //        loginPanel.SetActive(true);
    //    }

    //    StartCoroutine(CloudLoadRoutine());
    //}

    IEnumerator WaitDone()
    {
        while(!CloudData.isDone)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void DisableOptionButton()
    {
        optionButton.SetActive(false);
    }

    public void EnableOptionButton()
    {
        optionButton.SetActive(true);
    }

    public void EnableOptionPanel()
    {
        optionPanel.SetActive(true);
        CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        cinemachineBrain.enabled = false;
    }

    public async Task SaveCloud()
    {
        SoundManager.OnButtonUp.Invoke();

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            CloudData.Instance().SetCloudWarningPanel(true, "The Internet is not connected");
            //logInPanel.SetActive(false);

            return;
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            loginPanel.SetActive(true);
            cloudPanel.SetActive(true);
            return;
        }

        StartCoroutine(WaitDone());

        SaveData data = new SaveData();

        SaveSpec(ref data);
        SaveItems(ref data);
        SaveEquipment(ref data);
        SaveCards(ref data);

        string dataJson = JsonUtility.ToJson(data);
        string key = "SaveData";

        Dictionary<string, object> dataPair = new Dictionary<string, object> { { key, dataJson } };
        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(dataPair);
        }
        catch(AuthenticationException ae)
        {

        }
        catch(RequestFailedException re)
        {

        }
    }

    public async Task LoadCloud()
    {
        SoundManager.OnButtonUp.Invoke();

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            CloudData.Instance().SetCloudWarningPanel(true, "The Internet is not connected");
            //logInPanel.SetActive(false);

            return;
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            cloudPanel.SetActive(true);
            loginPanel.SetActive(true);
            return;
        }

        StartCoroutine(WaitDone());

        var key = new HashSet<string>{ "SaveData" };

        try
        {
            var getData = await CloudSaveService.Instance.Data.Player.LoadAsync(key);
            string dataJson = getData["SaveData"].Value.GetAsString();

            SaveData data = JsonUtility.FromJson<SaveData>(dataJson);
            LoadSpec(ref data);
            LoadItems(ref data);
            LoadEquipment(ref data);
            LoadCards(ref data);
        }
        catch (AuthenticationException ae)
        {

        }
        catch (RequestFailedException re)
        {

        }
    }

    public void ActivateCloudPanel()
    {
        cloudPanel.gameObject.SetActive(true);
    }












    string GetPrefabPath(int id)
    {
        string path = "Prefabs";
        int type = (int)(id / 1000);
        id = id % 1000;
        if (type == 0)
        {
            //Card
            path = $"{path}/Cards/PlayerCards";
            switch (id)
            {
                case 1:
                    path = $"{path}/NormalAttack";
                    break;
                case 2:
                    path = $"{path}/HealSelf";
                    break;
                case 3:
                    path = $"{path}/HealMax";
                    break;
                case 4:
                    path = $"{path}/DoubleAttack";
                    break;
                case 5:
                    path = $"{path}/AddIncreaseCostAmount";
                    break;
                case 6:
                    path = $"{path}/IncreaseMaxCost";
                    break;
                case 7:
                    path = $"{path}/PoisonAttack";
                    break;
                case 8:
                    path = $"{path}/RepeatHeal";
                    break;
                case 9:
                    path = $"{path}/StrongAttack";
                    break;
                case 10:
                    path = $"{path}/TakeCost";
                    break;
            }
        }
        else if (type == 1)
        {
            //Equipment
            path = $"{path}/Equipment";
            type = (int)(id / 10);
            id = id % 10;
            switch (type)
            {
                case 1:
                    //Equip_Sword_A
                    path = $"{path}/Weapon/Equip_Sword_A[{id}]";
                    break;
                case 2:
                    //Equip_Sword_B
                    path = $"{path}/Weapon/Equip_Sword_B[{id}]";
                    break;
                case 3:
                    //Equip_Spear_A
                    path = $"{path}/Weapon/Equip_Spear_A[{id}]";
                    break;
                case 4:
                    //Equip_Spear_B
                    path = $"{path}/Weapon/Equip_Spear_B[{id}]";
                    break;
                case 5:
                    //CostBody
                    path = $"{path}/Body/CostBody_{id}";
                    break;
                case 6:
                    //HpBody
                    path = $"{path}/Body/HpBody_{id}";
                    break;
                case 7:
                    //SpeedBody
                    path = $"{path}/Body/SpeedBody_{id}";
                    break;
                case 8:
                    //Shoes
                    path = $"{path}/Other/Shoes_{id}";
                    break;
                case 9:
                    //Head
                    path = $"{path}/Other/Head_{id}";
                    break;
            }
        }
        else if (type == 2)
        {
            //Item
            path = $"{path}/Items";
            switch (id)
            {
                case 1:
                    path = $"{path}/Item_Potion_Little";
                    break;
                case 2:
                    path = $"{path}/Item_Potion_Middle";
                    break;
                case 3:
                    path = $"{path}/Item_Potion_Large";
                    break;
            }            
        }

        return path;
    }
}
