using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTrackedMultiImageManager : MonoBehaviour
{
    [Header("�̹����� �ν����� �� ��µǴ� ������ ���")]
    [SerializeField] GameObject[] trackedPrefabs;

    [Header("�̹����� �ν����� �� ��µǴ� ������Ʈ ���")]
    Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        // AR Session Origin ������Ʈ�� ������Ʈ�� �������� �� ��� ����
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        // trackedPrefabs �迭�� �ִ� ��� �������� Instantiate()�� ������ ��
        // spawnedObjects Dictionary�� ����, ��Ȱ��ȭ
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

    bool isTracking;

    private void Update()
    {
        // Ʈ��ŷ ���� ��
        if (isTracking)
        {
            // y ���� �̵�
            go_TrackedObject.transform.position = Vector3.MoveTowards(vector3_TrackedImagePosition, new Vector3(vector3_TrackedImagePosition.x, vector3_TrackedImagePosition.y * 1.0f, vector3_TrackedImagePosition.z), Time.deltaTime);
            // ���̵� �ƿ�
            color_Fadeout.a = Mathf.MoveTowards(0, 1, Time.deltaTime);
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

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // ī�޶� �̹����� �νĵǾ��� ��
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        // ī�޶� �̹����� �νĵǾ� ������Ʈ ���� ��
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        // �νĵǰ� �ִ� �̹����� ī�޶󿡼� ������� ��
        foreach (var trackedImage in eventArgs.removed)
        {
            spawnedObjects[trackedImage.name].SetActive(false);
        }
    }

    // ���� ������Ʈ
    GameObject go_TrackedObject;
    // ��ġ
    Vector3 vector3_TrackedImagePosition;
    // �÷�
    Color color_Fadeout;

    void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        go_TrackedObject = spawnedObjects[name];

        vector3_TrackedImagePosition = trackedImage.transform.position;
        color_Fadeout = go_TrackedObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;

        // �̹����� ���� ���°� ������(Tracking)�� ��
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //go_TrackedObject.transform.position = trackedImage.transform.position;
            go_TrackedObject.transform.rotation = trackedImage.transform.rotation;

            isTracking = true;
            go_TrackedObject.SetActive(true);
        }
        else
        {
            go_TrackedObject.transform.position = trackedImage.transform.position;
            vector3_TrackedImagePosition = trackedImage.transform.position;
            color_Fadeout.a = 1;

            isTracking = false;
            go_TrackedObject.SetActive(false);
        }
    }
}