using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopBTN : MonoBehaviour
{
    Button btnStop;

    private void Awake()
    {
        btnStop = GetComponent<Button>();
        btnStop.onClick.AddListener(() => { ARTrackedMultiImageManager.BTN(); });
    }
}