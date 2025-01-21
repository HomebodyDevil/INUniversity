using UnityEngine;

[CreateAssetMenu(fileName = "DefaultCardData", menuName = "Card/DefaultCardData", order = int.MaxValue)]
public class DefaultCardData : ScriptableObject
{
    [SerializeField] public float cost;
    [SerializeField, TextArea(10, 10)] public string description;
}
