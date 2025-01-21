using UnityEngine.EventSystems;
using UnityEngine;
using Unity.VisualScripting;

public class TouchBox : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.tag);
        UnitManage UnitManger = GameObject.Find("UnitManager").GetComponent<UnitManage>();
        UnitManger.fightPanelOn();
        UnitManger.SelectBox(gameObject); // 클릭한 박스를 선택
    }
}
