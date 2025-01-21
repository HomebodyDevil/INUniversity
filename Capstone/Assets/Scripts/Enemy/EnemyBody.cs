using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyBody : MonoBehaviour, IPointerClickHandler
{  
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject testEnemyObject = gameObject.transform.parent.gameObject;

        //Debug.Log("This is Enemy");

        if (gameObject.tag != "Enemy" ||
            SceneManagerEX.CurrentScene() != SceneManagerEX.Scenes.MapScene) return;

        MapUIManager uiManager = UIManager.Instance().CurrentUIManager() as MapUIManager;
        GameObject enemyBattleCheckPanel = uiManager.enemyBattleCheckPanel;
        EnemyInfoPanel enemyInfoPanel = enemyBattleCheckPanel.GetComponent<EnemyInfoPanel>();

        BattleManager battleManager = BattleManager.Instance();
        battleManager.SetCurrentEnemyData(testEnemyObject);
        battleManager.currentEnemysSpawnersID = transform.parent.GetComponent<Enemy>().GetSpawnersID();
        battleManager.currentEnemyInMap = transform.parent;

        enemyInfoPanel.OnClicked();
        enemyInfoPanel.UpdateContents();
    }

    //private void UpdateEnemyCheckPanel()
    //{

    //}
}
