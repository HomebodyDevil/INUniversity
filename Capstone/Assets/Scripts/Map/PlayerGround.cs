using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGround : MonoBehaviour
{
    [SerializeField] private GameObject ground;

    void Start()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= enableBattleGround;
        SceneManagerEX.OnSwitchSceneToBattle += enableBattleGround;

        SceneManagerEX.OnSwitchSceneToMap -= disableBattleGround;
        SceneManagerEX.OnSwitchSceneToMap += disableBattleGround;
    }

    private void OnDestroy()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= enableBattleGround;
        SceneManagerEX.OnSwitchSceneToMap -= disableBattleGround;
    }

    private void enableBattleGround()
    {
        ground.SetActive(true);
    }

    private void disableBattleGround()
    {
        ground.SetActive(false);
    }
}
