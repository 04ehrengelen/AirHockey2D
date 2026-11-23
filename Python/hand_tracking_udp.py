
import cv2
import mediapipe as mp
import socket

# ==========================================
# KONFIGURASI UDP
# ==========================================

UNITY_IP = "127.0.0.1"
UNITY_PORT = 5052

udp_socket = socket.socket(
    socket.AF_INET,
    socket.SOCK_DGRAM
)

unity_address = (UNITY_IP, UNITY_PORT)

# ==========================================
# AREA KALIBRASI TANGAN
# ==========================================

# Area kamera yang digunakan untuk menggerakkan paddle.
# Semakin kecil rentangnya, semakin sensitif gerakannya.

CAMERA_MIN_X = 0.20
CAMERA_MAX_X = 0.80

CAMERA_MIN_Y = 0.15
CAMERA_MAX_Y = 0.85


def map_coordinate(value, min_value, max_value):
    mapped = (value - min_value) / (max_value - min_value)
    return max(0.0, min(1.0, mapped))


# ==========================================
# MEDIAPIPE
# ==========================================

mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

cap = cv2.VideoCapture(0)

if not cap.isOpened():
    print("Webcam tidak dapat dibuka!")
    exit()

print("Hand Tracking aktif.")
print("Mengirim koordinat ke Unity...")
print("Tekan Q untuk keluar.")

# ==========================================
# PROSES DETEKSI
# ==========================================

try:
    with mp_hands.Hands(
        static_image_mode=False,
        max_num_hands=1,
        min_detection_confidence=0.7,
        min_tracking_confidence=0.7
    ) as hands:

        while True:
            success, frame = cap.read()

            if not success:
                print("Gagal membaca webcam.")
                break

            # Mirror kamera
            frame = cv2.flip(frame, 1)

            # BGR ke RGB
            rgb_frame = cv2.cvtColor(
                frame,
                cv2.COLOR_BGR2RGB
            )

            results = hands.process(rgb_frame)

            if results.multi_hand_landmarks:

                hand_landmarks = results.multi_hand_landmarks[0]

                # Landmark 0 = wrist
                wrist = hand_landmarks.landmark[0]

                # Koordinat asli kamera
                raw_x = max(0.0, min(1.0, wrist.x))
                raw_y = max(0.0, min(1.0, wrist.y))

                # Petakan ke area kontrol
                x = map_coordinate(
                    raw_x,
                    CAMERA_MIN_X,
                    CAMERA_MAX_X
                )

                y = map_coordinate(
                    raw_y,
                    CAMERA_MIN_Y,
                    CAMERA_MAX_Y
                )

                # Kirim koordinat ke Unity
                message = f"{x:.5f},{y:.5f}"

                udp_socket.sendto(
                    message.encode("utf-8"),
                    unity_address
                )

                # Gambar landmark
                mp_drawing.draw_landmarks(
                    frame,
                    hand_landmarks,
                    mp_hands.HAND_CONNECTIONS
                )

                cv2.putText(
                    frame,
                    f"X: {x:.2f} Y: {y:.2f}",
                    (20, 40),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    0.8,
                    (0, 255, 0),
                    2
                )

            else:
                cv2.putText(
                    frame,
                    "Hand not detected",
                    (20, 40),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    0.8,
                    (0, 0, 255),
                    2
                )

            cv2.imshow(
                "Air Hockey - UDP Hand Tracking",
                frame
            )

            if cv2.waitKey(1) & 0xFF == ord("q"):
                break

finally:
    cap.release()
    cv2.destroyAllWindows()
    udp_socket.close()

    print("Hand Tracking dihentikan.")
