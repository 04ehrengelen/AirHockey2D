
using UnityEngine;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

[RequireComponent(typeof(Rigidbody2D))]
public class HandTrackingReceiverP2 : MonoBehaviour
{
    [Header("UDP Settings")]
    public int port = 5053;

    [Header("Paddle Boundaries")]
    public float minX = 0.35f;
    public float maxX = 4f;
    public float minY = -2f;
    public float maxY = 2f;

    [Header("Movement Settings")]
    public float smoothSpeed = 15f;

    private Rigidbody2D rb;
    private UdpClient udpClient;
    private Thread receiveThread;

    private volatile bool isRunning;

    private readonly object dataLock = new object();

    private Vector2 receivedPosition;
    private Vector2 targetPosition;

    private bool hasReceivedPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        targetPosition = rb.position;

        smoothSpeed = GameSettings.sensitivity;

        StartUDP();
    }

    private void StartUDP()
    {
        try
        {
            udpClient = new UdpClient(port);

            isRunning = true;

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("UDP Player 2 aktif pada port " + port);
        }
        catch (Exception e)
        {
            Debug.LogError("UDP Player 2 gagal: " + e.Message);
        }
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint =
            new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);

                string message = Encoding.UTF8.GetString(data);
                string[] coordinates = message.Split(',');

                if (coordinates.Length != 2)
                    continue;

                float x = float.Parse(
                    coordinates[0],
                    CultureInfo.InvariantCulture
                );

                float y = float.Parse(
                    coordinates[1],
                    CultureInfo.InvariantCulture
                );

                x = Math.Max(0f, Math.Min(1f, x));
                y = Math.Max(0f, Math.Min(1f, y));

                lock (dataLock)
                {
                    receivedPosition = new Vector2(x, y);
                    hasReceivedPosition = true;
                }
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (SocketException)
            {
                if (!isRunning)
                    break;
            }
            catch (Exception e)
            {
                Debug.LogWarning(
                    "Kesalahan UDP Player 2: " + e.Message
                );
            }
        }
    }

    private void Update()
    {
        lock (dataLock)
        {
            if (hasReceivedPosition)
            {
                targetPosition = new Vector2(
                    Mathf.Lerp(minX, maxX, receivedPosition.x),
                    Mathf.Lerp(maxY, minY, receivedPosition.y)
                );
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector2 nextPosition = Vector2.Lerp(
            rb.position,
            targetPosition,
            smoothSpeed * Time.fixedDeltaTime
        );

        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);

        rb.MovePosition(nextPosition);
    }

    private void OnDestroy()
    {
        isRunning = false;

        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(500);
        }
    }
}
