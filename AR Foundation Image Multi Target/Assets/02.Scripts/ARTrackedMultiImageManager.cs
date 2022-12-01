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

    public Text text;
    public Transform target;

    private void Awake()
    {
        text.GetComponent<GameObject>();
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

    // Ʈ��ŷ ���� ������Ʈ
    GameObject trackedObject;

    void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        trackedObject = spawnedObjects[name];

        //if (trackedImage.trackingState == TrackingState.None)
        //{
        //    trackedObject.transform.position = trackedImage.transform.position;
        //}

        // �̹����� ���� ���°� ������(Tracking)�� ��
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            text.enabled = true;

            //trackedObject.transform.position = Vector3.MoveTowards(trackedObject.transform.position, new Vector3(trackedImage.transform.position.x, trackedImage.transform.position.y + 1.0f, trackedImage.transform.position.z), Time.deltaTime);
            trackedObject.transform.position = trackedImage.transform.position;
            //trackedObject.transform.rotation = trackedImage.transform.rotation;
            //Vector3 relativePos = target.position - trackedObject.transform.position;
            //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            //trackedObject.transform.rotation = rotation;

            trackedObject.SetActive(true);
        }
        else
        {
            text.enabled = false;
            trackedObject.SetActive(false);
        }
    }
}