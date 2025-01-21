using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlayerData", menuName = "Player / DefaultPlayerData", order = int.MaxValue)]
public class DefaultPlayerData : ScriptableObject
{
    [Header("Default Player Spec")]
    public float attackPoint;
    public float maximumHp;
    public float maximumCost;
    public float maximumExp;
    public float costIncreaseAmount;

    [Space(10), Header("Growth Rate")]
    public float expIncreaseRatio;

    [Space(10), Header("About Skill Card")]
    public int maxCardAmount;

    A_Item test;

    [Space(10), Header("About Items with Equipment")]
    public int ItemSlotsNumber;
    public int EquipmentSlotsNumber;
    public List<A_Item> playerItems;
    public List<A_Equipment> playerEquipment;

    // Dictionary는 SerializeField로 볼 수 없음.
    //public Dictionary<A_Item, int> playerItemsListDictionary;

    [Space(10), Header("Player Deck")]
    public List<A_PlayerCard> playerDeck;
}
