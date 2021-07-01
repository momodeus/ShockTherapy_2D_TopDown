using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class CameraScroller : MonoBehaviour
{
    public Toggle panToggle;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float zoomStep, minCamSize, maxCamSize;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private float border;

    private Vector3 dragOrigin;


    // Update is called once per frame
    void Update()
    {
        if (panToggle.isOn) return;
#if UNITY_EDITOR
        PanCameraMouse();
#elif UNITY_ANDROID || UNITY_IOS
        PanCameraTouch();
#endif
    }

    private void PanCameraMouse()
    {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position = ClampCamera(cam.transform.position + difference);

        }
    }

    private void PanCameraTouch()
    {
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

            if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
                cam.transform.position = ClampCamera(cam.transform.position + difference);
            }
        }
    }

    public void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float mapMinX = tilemap.transform.position.x - tilemap.cellBounds.size.x / 2f - border*3;
        float mapMaxX = tilemap.transform.position.x + tilemap.cellBounds.size.x / 2f + border;
        float mapMinY = tilemap.transform.position.y - tilemap.cellBounds.size.y / 2f - border;
        float mapMaxY = tilemap.transform.position.y + tilemap.cellBounds.size.y / 2f + border;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
