using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerCard
{
    void OnDraw();          // 덱으로부터 현재 패로 보충됐을 때
    void OnClickDown();     // 눌렀을 때
    void OnClickUp();       // 뗐을 때(만약 드래그중인 상태에서  뗐을 경우, OnPlay가 되도록 하자)
    void OnClicked();       // 선택(눌렀다 뗐을때)
    void OnDragging();      // 드래그 중일때
    void OnPlay();          // 실행? 수행? 발동? 할때
    void OnDiscard();       // 제거됐을 때
}
