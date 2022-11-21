using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTrackedMultiImageManager : MonoBehaviour
{
    [Header("이미지를 인식했을 때 출력되는 프리팹 목록")]
    [SerializeField] GameObject[] trackedPrefabs;

    [Header("이미지를 인식했을 때 출력되는 오브젝트 목록")]
    Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        // AR Session Origin 오브젝트에 컴포넌트로 적용했을 때 사용 가능
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        // trackedPrefabs 배열에 있는 모든 프리팹을 Instantiate()로 생성한 후
        // spawnedObjects Dictionary에 저장, 비활성화
        // 카메라에 이미지가 인식되면 이미지와 동일한 이름의 key에 있는 value 오브젝트를 출력
        foreach (GameObject prefab in trackedPrefabs)
        {
            // 오브젝트 생성
            GameObject clone = Instantiate(prefab);
            // 생성한 오브젝트의 이름 설정
            clone.name = prefab.name;
            // 오브젝트 비활성화
            clone.SetActive(false);
            // Dictionary 컬렉션에 오브젝트 저장
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
        // 카메라에 이미지가 인식되었을 때
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        // 카메라에 이미지가 인식되어 업데이트 중일 때
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        // 인식되고 있는 이미지가 카메라에서 사라졌을 때
        foreach (var trackedImage in eventArgs.removed)
        {
            spawnedObjects[trackedImage.name].SetActive(false);
        }
    }

    // 트래킹 중인 오브젝트
    GameObject trackedObject;

    void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        trackedObject = spawnedObjects[name];

        // 이미지의 추적 상태가 추적중(Tracking)일 때
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            trackedObject.transform.position = Vector3.MoveTowards(trackedObject.transform.position, new Vector3(trackedImage.transform.position.x, trackedImage.transform.position.y + 1f, trackedImage.transform.position.z), Time.deltaTime * 0.25f);
            trackedObject.transform.rotation = trackedImage.transform.rotation;
            trackedObject.SetActive(true);
        }
        else
        {
            trackedObject.transform.position = trackedImage.transform.position;
            trackedObject.SetActive(false);
        }
    }
}