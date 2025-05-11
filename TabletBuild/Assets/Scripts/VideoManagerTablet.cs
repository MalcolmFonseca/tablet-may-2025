using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Playables;
using PimDeWitte.UnityMainThreadDispatcher;

public class VideoManagerTablet : MonoBehaviour
{
    TcpListener server = null;
    TcpClient client = null;
    NetworkStream stream = null;
    Thread thread;

    private Texture2D receivedTexture;
    [SerializeField] private RawImage videoImage;
    [SerializeField] Canvas canvas;

    private void Start()
    {
        //fit video to screen

        thread = new Thread(new ThreadStart(SetupServer));
        thread.Start();

        receivedTexture = new Texture2D(640, 480, TextureFormat.RGB24, false);
        canvas.worldCamera = Camera.main;
    }

    private void Update()
    {

    }

    private void SetupServer()
    {
        try
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, 8080);
            server.Start();

            byte[] buffer = new byte[73744];

            while (true)
            {
                Debug.Log("Connecting to Video Server...");
                client = server.AcceptTcpClient();
                Debug.Log("Connected to Video Server");

                stream = client.GetStream();

                int i;

                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    //buffer now contains sent byte array
                    UnityMainThreadDispatcher.Instance().Enqueue(UpdateFrame(buffer));
                }
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
        finally
        {
            server.Stop();
        }
    }

    private void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
        server.Stop();
        thread.Abort();
    }

    public void SendMessageToClient(string message)
    {
        byte[] msg = Encoding.UTF8.GetBytes(message);
        stream.Write(msg, 0, msg.Length);
        Debug.Log("Sent: " + message);
    }

    //needs to use this goofy function call with another class to execute on main frame, images dont update otherwise
    public IEnumerator UpdateFrame(byte[] frameData)
    {
        receivedTexture.LoadImage(frameData);
        receivedTexture.Apply();
        videoImage.texture = receivedTexture;
        yield return null;
    }
}
