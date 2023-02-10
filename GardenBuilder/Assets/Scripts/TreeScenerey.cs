using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScenerey : MonoBehaviour
{
    private void Start()
    {
        if (SceneryValues.showTrees)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
