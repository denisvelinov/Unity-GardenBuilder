using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScenery : MonoBehaviour
{
    private void Start()
    {
        if (SceneryValues.showMushrooms)
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
