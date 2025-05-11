using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Video : MonoBehaviourPunCallbacks
{
    private Texture2D receivedTexture;
    private GameObject videoImageObject;
    private RawImage videoImage;
    private void Start()
    {
        videoImageObject = transform.GetChild(0).gameObject;
        videoImage = videoImageObject.GetComponent<RawImage>();
        receivedTexture = new Texture2D(640, 480, TextureFormat.RGB24, false);
    }
    [PunRPC] void UpdateFrame(byte[] frameData) {
        receivedTexture.LoadImage(frameData);
        receivedTexture.Apply();
        videoImage.texture = receivedTexture;
    }
}
