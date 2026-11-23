
import cv2
import mediapipe as mp

# Inisialisasi MediaPipe
mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

# Membuka webcam laptop
cap = cv2.VideoCapture(0)

if not cap.isOpened():
    print("Webcam tidak dapat dibuka!")
    exit()

# Konfigurasi deteksi tangan
with mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=1,
    min_detection_confidence=0.7,
    min_tracking_confidence=0.7
) as hands:

    while True:
        success, frame = cap.read()

        if not success:
            print("Gagal mengambil gambar dari webcam.")
            break

        # Membalik gambar agar seperti cermin
        frame = cv2.flip(frame, 1)

        # Mengubah BGR menjadi RGB
        rgb_frame = cv2.cvtColor(
            frame,
            cv2.COLOR_BGR2RGB
        )

        # Mendeteksi tangan
        results = hands.process(rgb_frame)

        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:

                # Menggambar 21 landmark tangan
                mp_drawing.draw_landmarks(
                    frame,
                    hand_landmarks,
                    mp_hands.HAND_CONNECTIONS
                )

                # Mengambil landmark pergelangan tangan
                wrist = hand_landmarks.landmark[0]

                x = wrist.x
                y = wrist.y

                print(
                    f"Koordinat tangan: X={x:.3f}, Y={y:.3f}"
                )

        cv2.imshow("Air Hockey - Hand Tracking", frame)

        # Tekan Q untuk keluar
        if cv2.waitKey(1) & 0xFF == ord("q"):
            break

cap.release()
cv2.destroyAllWindows()
