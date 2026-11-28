
import cv2
import mediapipe as mp

# ==========================================
# KONFIGURASI WEBCAM
# ==========================================

CAMERA_INDEX = 0

# ==========================================
# MEDIAPIPE
# ==========================================

mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

camera = cv2.VideoCapture(CAMERA_INDEX)

if not camera.isOpened():
    print("Webcam tidak dapat dibuka!")
    exit()

print("Dual Hand Tracking aktif.")
print("Tampilkan dua tangan di depan webcam.")
print("Tekan Q untuk keluar.")

try:
    with mp_hands.Hands(
        static_image_mode=False,
        max_num_hands=2,
        model_complexity=1,
        min_detection_confidence=0.7,
        min_tracking_confidence=0.7
    ) as hands:

        while True:
            success, frame = camera.read()

            if not success:
                print("Gagal membaca webcam.")
                break

            # Membalik gambar seperti kamera selfie
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

                # Urutkan tangan berdasarkan posisi horizontal
                detected_hands.sort(
                    key=lambda hand: hand["x"]
                )

                for index, hand in enumerate(detected_hands):

                    landmarks = hand["landmarks"]

                    x = hand["x"]
                    y = hand["y"]

                    pixel_x = int(x * width)
                    pixel_y = int(y * height)

                    if index == 0:
                        player_name = "PLAYER 1"
                        color = (255, 100, 0)
                    else:
                        player_name = "PLAYER 2"
                        color = (0, 80, 255)

                    # Menggambar kerangka tangan
                    mp_drawing.draw_landmarks(
                        frame,
                        landmarks,
                        mp_hands.HAND_CONNECTIONS
                    )

                    # Menampilkan posisi pergelangan tangan
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

                    cv2.putText(
                        frame,
                        f"X: {x:.2f} Y: {y:.2f}",
                        (pixel_x - 50, pixel_y + 35),
                        cv2.FONT_HERSHEY_SIMPLEX,
                        0.5,
                        color,
                        2
                    )

            # Status jumlah tangan
            hand_count = len(detected_hands)

            cv2.putText(
                frame,
                f"Hands Detected: {hand_count}/2",
                (20, 40),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.8,
                (0, 255, 0),
                2
            )

            if hand_count < 2:
                cv2.putText(
                    frame,
                    "Show both hands!",
                    (20, 80),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    0.7,
                    (0, 0, 255),
                    2
                )

            cv2.imshow(
                "Air Hockey - Dual Hand Tracking",
                frame
            )

            if cv2.waitKey(1) & 0xFF == ord("q"):
                break

finally:
    camera.release()
    cv2.destroyAllWindows()

    print("Dual Hand Tracking dihentikan.")
