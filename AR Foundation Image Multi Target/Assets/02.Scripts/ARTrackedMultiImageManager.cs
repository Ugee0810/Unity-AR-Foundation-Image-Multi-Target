using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTrackedMultiImageManager : MonoBehaviour
{
    [Header("�̹����� �ν����� �� ��µǴ� ������ ���")]
    [SerializeField] GameObject[] trackedPrefabs;

    [Header("�̹����� �ν����� �� ��µǴ� ������Ʈ ���")]
    Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    ARTrackedImageManager trackedImageManager;

    [SerializeField] GameObject go_Indicator;

    GameObject trackedObject;

    bool isPlay;
    float curTime = 0.0f;

    public static System.Action BTN;
    public void STOP()
    {
        trackedObject.SetActive(false);
        isPlay = false;
        curTime = 0.0f;
        go_Indicator.SetActive(true);
    }

    private void Awake()
    {
        BTN = () => { STOP(); };

        // AR Session Origin ������Ʈ�� ������Ʈ�� �������� �� ��� ����
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        // trackedPrefabs �迭�� �ִ� ��� �������� Instantiate()�� ������ �� spawnedObjects Dictionary�� ����, ��Ȱ��ȭ
        // ī�޶� �̹����� �νĵǸ� �̹����� ������ �̸��� key�� �ִ� value ������Ʈ�� ���
        foreach (GameObject prefab in trackedPrefabs)
        {
            // ������Ʈ ����
            GameObject clone = Instantiate(prefab);
            // ������ ������Ʈ�� �̸� ����
            clone.name = prefab.name;
            // ������Ʈ ��Ȱ��ȭ
            clone.SetActive(false);
            // Dictionary �÷��ǿ� ������Ʈ ����
            spawnedObjects.Add(clone.name, clone);
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void FixedUpdate()
    {
        if (isPlay && trackedObject.transform.GetChild(0).GetComponent<VideoPlayer>().isPlaying)
        {
            curTime += Time.deltaTime;
            if (trackedObject.transform.GetChild(0).GetComponent<VideoPlayer>().length < curTime)
            {
                trackedObject.SetActive(false);
                isPlay = false;
                curTime = 0.0f;
                go_Indicator.SetActive(true);
            }
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //// ī�޶� �̹����� �νĵǾ��� ��
        //foreach (ARTrackedImage trackedImage in eventArgs.added)
        //{
        //    UpdateImage(trackedImage);
        //}

        // ī�޶� �̹����� �νĵǾ� ������Ʈ ���� ��
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        //// �νĵǰ� �ִ� �̹����� ī�޶󿡼� ������� ��
        //foreach (ARTrackedImage trackedImage in eventArgs.removed)
        //{
        //    spawnedObjects[trackedImage.name].SetActive(false);
        //}
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
        if (isPlay) return;

        string name = trackedImage.referenceImage.name;
        trackedObject = spawnedObjects[name];

        // �̹����� ���� ���°� ������(Tracking)�� ��
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            trackedObject.SetActive(true);
            isPlay = true;
            go_Indicator.SetActive(false);
        }
        //else
        //{
        //    trackedObject.SetActive(false);
        //}
    }
}