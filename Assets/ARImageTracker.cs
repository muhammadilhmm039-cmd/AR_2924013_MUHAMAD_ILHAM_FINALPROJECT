
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;

public class ARImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager imageManager;

    [SerializeField]
    private GameObject housePrefab;

    [SerializeField]
    private float hideDelay = 2f;

    private GameObject house;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        house = Instantiate(housePrefab);
        house.SetActive(false);
    }

    private void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnImagesChanged);
    }

    private void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnImagesChanged);
    }

    private void OnImagesChanged(
        ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var image in args.added)
            UpdateImage(image);

        foreach (var image in args.updated)
            UpdateImage(image);
    }

    private void UpdateImage(ARTrackedImage image)
    {
        if (image.trackingState == TrackingState.Tracking)
        {
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }

            house.SetActive(true);
            house.transform.SetPositionAndRotation(
                image.transform.position,
                image.transform.rotation
            );
        }
        else if (hideCoroutine == null)
        {
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        house.SetActive(false);
        hideCoroutine = null;
    }
}
