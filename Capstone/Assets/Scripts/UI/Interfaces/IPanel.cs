using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanel
{
    void Initialize();
    void OnClicked();
    void GetUIManager();
    IEnumerator GetUIManagerCoroutine();
}
