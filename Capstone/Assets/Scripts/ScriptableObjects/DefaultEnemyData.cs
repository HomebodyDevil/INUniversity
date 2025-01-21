using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DefaultEnemyData", menuName = "Enemy/Default Enemy Data", order = int.MaxValue)]
public class DefaultEnemyData : ScriptableObject
{
    [SerializeField, Multiline] public string enemyName;

    [Space(10), Header("Basic Enemy Spec")]
    public float attackPoint;
    public float maxHP;
    public float maxCost;
    public float currentCostIncreaseAmount;
    public float EXPAmount;

    [Space(10), Header("Other Options")]
    public List<GameObject> enemyDeck;
    //[SerializeField, Multiline] public string normalEnemyPrefabPath;
    [SerializeField, Multiline] public string battleEnemyPrefabPath;

    [Space(10), Header("Description")]
    [TextArea] public string enemyDescription;
    [TextArea] public string enemyImagePath;
    [TextArea] public string enemyPlayerWinProfileImagePath;
    [TextArea] public string enemyPlayerLoseProfileImagePath;
    [TextArea] public string enemyPlayerWinDialogueText;
    [TextArea] public string enemyPlayerLoseDialogueText;

    [Space(10), Header("Drops")]
    public List<DropItem> dropItems;
    public List<DropCard> dropCards;
    public List<DropEquipment> dropEquipment;
}

[Serializable]
public class DropItem
{
    public GameObject item;
    [Range(0, 100)] public int chance;
    [Range(0, 100)] public int maxDropAmount;
}

[Serializable]
public class DropCard
{
    public GameObject card;
    [Range(0, 100)] public int chance;
}

[Serializable]
public class DropEquipment
{
    public GameObject equipment;
    [Range(0, 100)] public int chance;
}
