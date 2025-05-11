using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class Line : MonoBehaviourPunCallbacks
{
    private float lineLifetime = 2f;
    private float timer;
    private GameObject mainCamera;

    private void Start()
    {
        timer = lineLifetime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    void StartLine(float lineWidth, float red, float green, float blue)
    {
        LineRenderer currentLine = GetComponent<LineRenderer>();
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 0;
        currentLine.useWorldSpace = false;
        currentLine.numCornerVertices = 10;
        currentLine.numCapVertices = 10;
        Color selectedColor = new Color(red, green, blue);
        currentLine.startColor = selectedColor;
        currentLine.endColor = selectedColor;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    [PunRPC]
    void UpdateLine(int pointCount, Vector3 touchPos)
    {
        LineRenderer currentLine = GetComponent<LineRenderer>();
        currentLine.positionCount = pointCount;
        currentLine.SetPosition(pointCount - 1, touchPos + mainCamera.transform.position + new Vector3(0, 0, 10));
        transform.rotation = mainCamera.transform.rotation;
        timer = lineLifetime;
    }
}
