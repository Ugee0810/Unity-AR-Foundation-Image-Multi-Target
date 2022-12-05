using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTrackedMultiImageManager : MonoBehaviour
{
    [Header("이미지를 인식했을 때 출력되는 프리팹 목록")]
    [SerializeField] GameObject[] trackedPrefabs;

    [Header("이미지를 인식했을 때 출력되는 오브젝트 목록")]
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

        // AR Session Origin 오브젝트에 컴포넌트로 적용했을 때 사용 가능
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        // trackedPrefabs 배열에 있는 모든 프리팹을 Instantiate()로 생성한 후 spawnedObjects Dictionary에 저장, 비활성화
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
        //// 카메라에 이미지가 인식되었을 때
        //foreach (ARTrackedImage trackedImage in eventArgs.added)
        //{
        //    UpdateImage(trackedImage);
        //}

        // 카메라에 이미지가 인식되어 업데이트 중일 때
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        //// 인식되고 있는 이미지가 카메라에서 사라졌을 때
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

        // 이미지의 추적 상태가 추적중(Tracking)일 때
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