
import cv2
import mediapipe as mp
import socket

# ==========================================
# KONFIGURASI UDP
# ==========================================

UNITY_IP = "127.0.0.1"

PLAYER1_PORT = 5052
PLAYER2_PORT = 5053

udp_socket = socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)

# ==========================================
# MEDIAPIPE
# ==========================================

mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

camera = cv2.VideoCapture(0, cv2.CAP_DSHOW)

camera.set(cv2.CAP_PROP_FRAME_WIDTH, 1280)
camera.set(cv2.CAP_PROP_FRAME_HEIGHT, 720)
camera.set(cv2.CAP_PROP_FPS, 30)
camera.set(cv2.CAP_PROP_BUFFERSIZE, 1)

if not camera.isOpened():
    print("Webcam tidak dapat dibuka!")
    exit()

print("Dual Hand Tracking UDP aktif.")
print("Player 1: UDP 5052")
print("Player 2: UDP 5053")
print("Tekan Q untuk keluar.")

try:
    with mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=2,
    model_complexity=0,
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5
    ) as hands:

        while True:

            success, frame = camera.read()

            if not success:
                print("Gagal membaca webcam.")
                break

            frame = cv2.flip(frame, 1)

            height, width, _ = frame.shape

            rgb_frame = cv2.cvtColor(
                frame,
                cv2.COLOR_BGR2RGB
            )

            results = hands.process(rgb_frame)

            detected_hands = []

            if results.multi_hand_landmarks:

                for hand_landmarks in results.multi_hand_landmarks:

                    wrist = hand_landmarks.landmark[0]

                    detected_hands.append({
                        "landmarks": hand_landmarks,
                        "x": wrist.x,
                        "y": wrist.y
                    })

                # Urutkan berdasarkan posisi horizontal
                detected_hands.sort(
                    key=lambda hand: hand["x"]
                )

                for index, hand in enumerate(detected_hands):

                    x = max(0.0, min(1.0, hand["x"]))
                    y = max(0.0, min(1.0, hand["y"]))

                    landmarks = hand["landmarks"]

                    pixel_x = int(x * width)
                    pixel_y = int(y * height)

                    if index == 0:

                        player_name = "PLAYER 1"
                        port = PLAYER1_PORT
                        color = (255, 100, 0)

                    else:

                        player_name = "PLAYER 2"
                        port = PLAYER2_PORT
                        color = (0, 80, 255)

                    # Format pesan: x,y
                    message = f"{x:.5f},{y:.5f}"

                    udp_socket.sendto(
                        message.encode("utf-8"),
                        (UNITY_IP, port)
                    )

                    mp_drawing.draw_landmarks(
                        frame,
                        landmarks,
                        mp_hands.HAND_CONNECTIONS
                    )

                    cv2.circle(
                        frame,
                        (pixel_x, pixel_y),
                        12,
                        color,
                        -1
                    )

                    cv2.putText(
                        frame,
                        player_name,
                        (pixel_x - 50, pixel_y - 25),
                        cv2.FONT_HERSHEY_SIMPLEX,
                        0.8,
                        color,
                        2
                    )

            # Status kamera
            cv2.putText(
                frame,
                f"Hands Detected: {len(detected_hands)}/2",
                (20, 40),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.8,
                (0, 255, 0),
                2
            )

            cv2.namedWindow(
                "Air Hockey - Dual Hand UDP",
                cv2.WINDOW_NORMAL
            )

            cv2.resizeWindow(
                "Air Hockey - Dual Hand UDP",
                1280,
                720
            )

            cv2.imshow(
                "Air Hockey - Dual Hand UDP",
                frame
            )

            if cv2.waitKey(1) & 0xFF == ord("q"):
                break

finally:
    camera.release()
    cv2.destroyAllWindows()
    udp_socket.close()

    print("Dual Hand Tracking UDP dihentikan.")
