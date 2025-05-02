using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImages : MonoBehaviour
{
    // Reference to AR tracked image manager component
    private ARTrackedImageManager _trackedImagesManager;

    // List of prefabs to instantiate - these should be named the same
    // as their corresponding 2D images in the reference image library 
    public GameObject[] ArPrefabs;

    // Keep dictionary array of created prefabs
    private readonly Dictionary<string, List<GameObject>> _instantiatedPrefabs = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        // Cache a reference to the Tracked Image Manager component
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        // Attach event handler when tracked images change
        _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        // Remove event handler 
        _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    // Event Handler
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.updated)
        {
            var imageName = trackedImage.referenceImage.name;

            // Check if prefabs are already instantiated for this tracked image
            if (!_instantiatedPrefabs.ContainsKey(imageName))
            {
                // Instantiate both prefabs for the tracked image
                InstantiatePrefab(ArPrefabs[0], trackedImage);
                InstantiatePrefab(ArPrefabs[1], trackedImage);
            }
        }

        // For all prefabs that have been created so far, set them active or not depending
        // on whether their corresponding image is currently being tracked
        foreach (var trackedImage in eventArgs.updated)
        {
            if (_instantiatedPrefabs.TryGetValue(trackedImage.referenceImage.name, out List<GameObject> prefabList))
            {
                foreach (var prefab in prefabList)
                {
                    prefab.SetActive(trackedImage.trackingState == TrackingState.Tracking);
                }
            }
        }

        // If the AR subsystem has given up looking for a tracked image
        foreach (var trackedImage in eventArgs.removed)
        {
            // Check if the prefabs exist before attempting to destroy them
            if (_instantiatedPrefabs.TryGetValue(trackedImage.referenceImage.name, out List<GameObject> prefabList))
            {
                foreach (var prefabInstance in prefabList)
                {
                    Destroy(prefabInstance);
                }
                // Remove the instances from the dictionary
                _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
            }
        }
    }

    // Method to instantiate prefabs and associate them with tracked images
    // Method to instantiate prefabs and associate them with tracked images
    private void InstantiatePrefab(GameObject prefab, ARTrackedImage trackedImage)
    {
        if (!_instantiatedPrefabs.ContainsKey(trackedImage.referenceImage.name))
        {
            _instantiatedPrefabs[trackedImage.referenceImage.name] = new List<GameObject>();
        }

        // Instantiate the prefab, parenting it to the ARTrackedImage
        var newPrefab = Instantiate(prefab, trackedImage.transform);
        // Add the created prefab to the list in the dictionary
        _instantiatedPrefabs[trackedImage.referenceImage.name].Add(newPrefab);

        // Adjust the position of the Bloch sphere if necessary
        if (prefab == ArPrefabs[0])
        {
            newPrefab.transform.localPosition = new Vector3(0, 0, 0f); // Adjust the Z-position
        }

        // Get all Renderer components from the instantiated GameObject and its children
        Renderer[] renderers = newPrefab.GetComponentsInChildren<Renderer>();

        // Set the Render Queue for each Renderer
        foreach (Renderer renderer in renderers)
        {
            // Set the material to be transparent
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            renderer.material.SetInt("_ZWrite", 0);
            renderer.material.DisableKeyword("_ALPHATEST_ON");
            renderer.material.DisableKeyword("_ALPHABLEND_ON");
            renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.material.renderQueue = 3000;

            if (prefab == ArPrefabs[0])
            { // Assuming ArPrefabs[0] is the Bloch sphere
                renderer.material.renderQueue = 3001;
            }
            else if (prefab == ArPrefabs[1])
            { // Assuming ArPrefabs[1] is the plate
                renderer.material.renderQueue = 3000;
            }
        }
    }
}
