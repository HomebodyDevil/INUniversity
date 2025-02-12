using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGround : MonoBehaviour
{
    private void Start()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= DisableMapGround;
        SceneManagerEX.OnSwitchSceneToBattle += DisableMapGround;

        SceneManagerEX.OnSwitchSceneToMap -= EnableMapGround;
        SceneManagerEX.OnSwitchSceneToMap += EnableMapGround;

        EnableMapGround();
    }

    private void OnDestroy()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= DisableMapGround;
        SceneManagerEX.OnSwitchSceneToMap -= EnableMapGround;
    }

    private void DisableMapGround()
    {
        gameObject.SetActive(false);
    }

    private void EnableMapGround()
    {
        gameObject.SetActive(true);
    }
}
