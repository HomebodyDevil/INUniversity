using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyBody : MonoBehaviour, IPointerClickHandler
{  
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GPSManager.isIn)
            return;

        float distance = Vector3.Distance(transform.position, Player.Instance().gameObject.transform.position);
        if (distance > CameraManager.Instance().clickDistance)
            return;

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
