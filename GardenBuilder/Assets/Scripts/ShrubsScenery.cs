using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrubsScenery : MonoBehaviour
{
    private void Start()
    {
        if (SceneryValues.showFerns)
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
