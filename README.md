# ❌ Tic-Tac-Toe Online Multiplayer Game (Photon Quantum) ⭕

![Unity Version](https://img.shields.io/badge/Unity-2022.3.40f1-blue.svg)
![Network Framework](https://img.shields.io/badge/Network-Photon%20Quantum-red.svg)
![Language](https://img.shields.io/badge/Language-C%23-green.svg)
![Platform](https://img.shields.io/badge/Platform-PC%20%2F%20Android%20%2F%20iOS-orange.svg)

Một dự án game cờ ca-rô (Tic-Tac-Toe) trực tuyến dành cho 2 người chơi được phát triển bằng **Unity Engine** và framework mạng mã nguồn cao cấp **Photon Quantum**. Dự án tập trung hoàn toàn vào việc triển khai kiến trúc **Deterministic Predict-Rollback** (Dự đoán - Hoàn trả trạng thái đồng bộ tuyệt đối) để loại bỏ hoàn toàn độ trễ (Zero Delay) giữa các người chơi trực tuyến, kết hợp hệ thống phòng chờ (Lobby) hoàn chỉnh để tìm trận và kết nối.

---

## 📌 Các Tính Năng Chính (Features)

- **Cơ chế Mạng Photon Quantum:**
  - `Deterministic Simulation`: Mọi logic trận đấu được xử lý đồng bộ tuyệt đối trên các client. Người chơi chỉ gửi lệnh Input nhấn ô cờ, đảm bảo không bao giờ xảy ra tình trạng hack vị trí hoặc lệch pha dữ liệu (Desync).
  - `Zero Delay Gameplay`: Người chơi cảm nhận được nước đi của mình ngay lập tức (Local Prediction) trong khi hệ thống mạng tự động xử lý việc xác thực phía sau.
- **Hệ thống Lobby & Matchmaking:**
  - `Lobby State`: Màn hình phòng chờ trực quan hiển thị trạng thái kết nối mạng (Connecting, In Lobby, Matchmaking, In Game).
  - `Custom Room / Matchmaking`: Hỗ trợ người chơi nhấn nút để tự động tìm kiếm phòng ngẫu nhiên hoặc ghép cặp trực tiếp 2 người vào một trận đấu.
- **Logic Gameplay 2 Người Chuẩn Chỉnh:**
  - Tự động phân chia lượt đi (Player 1 đi trước - X, Player 2 đi sau - O) dựa trên thông tin Actor ID từ server Photon.
  - Hệ thống tự động kiểm tra điều kiện thắng/thua/hòa ngay lập tức ở mỗi lượt đi tại tầng Simulation.
---

## 🛠️ Kiến Trúc Hệ Thống (Architecture & Code Structure)

Dự án tách biệt hoàn toàn Logic trò chơi (Simulation) dựa trên mô hình **ECS (Entity Component System)** thuần C# của Quantum ra khỏi tầng hiển thị (View) của Unity.

### 1. Sơ đồ Luồng Kết Nối & Gameplay
```text
[ Người chơi mở Game ] ──► Kết nối tới Photon Master Server
                                   │
                                   ▼
[ Tại màn hình Lobby ] ──► Nhấn "Tìm Trận" (Matchmaking)
                                   │
                                   ▼
[ Ghép cặp thành công ] ──► Khởi tạo Quantum Session (2 Players)
                                   │
┌──────────────────────────────────┴──────────────────────────────────┐
▼                                                                     ▼
[Client 1: Player X]                                   [Client 2: Player O]
  │ Send Input                                           │ Send Input
  ▼                                                      ▼
[Quantum Simulation] ──(Đồng bộ Deterministic Frame)──► [Quantum Simulation]
  │                                                      │
  ▼ (Cập nhật ECS Component)                             ▼ (Cập nhật ECS Component)
[Unity View: Vẽ X lên bàn cờ]                           [Unity View: Vẽ O lên bàn cờ]
```
## 2. Quản lý Cấu trúc Thư mục trong `Assets/`
```text
Assets/
├── Photon/                         # SDK Photon Realtime & Quantum Core
└── _Project/                       # Thư mục chính chứa mã nguồn trò chơi
├── QuantumUser/                 # Tầng mô phỏng thuần C# (Quantum Simulation)
│   └── Simulation/             # Toàn bộ Logic cốt lõi của game Tic-Tac-Toe
│       ├── TicTacToeSystem.cs  # Hệ thống ECS quản lý lượt đi, lượt đánh và kiểm tra thắng cuộc
│       ├── Command.qtn         # Định nghĩa cấu trúc lệnh gửi vị trí bấm ô cờ (Input)
│       └── DSL.qtn             # Định nghĩa dữ liệu bàn cờ (Board Component, Player Data Struct)
└── UnityUser/                  # Tầng hiển thị và tương tác người dùng (Unity View)
├── Scenes/                 # Các màn chơi chính
│   ├── MainMenu.unity      # Scene phòng chờ, kết nối mạng và tìm phòng
│   └── QuantumGameScene.unity  # Scene bàn cờ thực tế khi vào trận
├── Simulation/Scripts/     # C# Scripts kết nối Unity UI với Quantum Session
│   ├── PlayerController.cs # Xử lý logic chính của game
│   └── PlayerSpawner.cs    # Lắng nghe player vào setup lobby với quân X O của player
└── Prefabs/                # Giao diện ô cờ, bàn cờ, hiệu ứng và UI Elements
```
## 🔬 Chi Tiết Kỹ Thuật (Technical Specification)

### 🔢 Số Thực Cố Định (Fixed Point - FP)
- Tránh sử dụng `float` hay `double` vốn có thể gây sai lệch dấu thập phân giữa các thiết bị/nền tảng
- Toàn bộ tính toán trạng thái game sử dụng kiểu `FP` của Quantum để đảm bảo tính đồng nhất tuyệt đối

### 📡 Quantum Commands
- Khi người chơi chạm vào một ô, một **Command** chứa tọa độ ô cờ được gửi lên máy chủ Quantum
- Lệnh được phát tán đồng thời tới cả 2 máy để hệ thống `TicTacToeSystem` xử lý trên dữ liệu gốc

### 🖥️ Tách Biệt Hiển Thị (View Separation)
- Khối Unity UI chỉ đóng vai trò **"đọc"** dữ liệu từ Frame hiện tại của Quantum và render lên màn hình
- Không có bất kỳ logic game nào được viết bên phía Unity Component

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Dự Án (Installation & Setup)

### 🖥️ Yêu Cầu Hệ Thống (Prerequisites)
- **Unity Editor:** Phiên bản `2022.3.40f1` (hoặc các phiên bản 2022.3 LTS tương thích)
- **Photon Quantum SDK:** `[Điền phiên bản Quantum sử dụng, ví dụ: 2.1 / 3.0]`

### 📋 Các Bước Thực Hiện

**1. Clone mã nguồn từ GitHub**
```bash
git clone https://github.com/HuyDevGame1402/Tic-Tac-Toe-Unity-Game-Photon-Quantum.git
```

**2. Cấu hình Photon AppID**
- Truy cập [Photon Engine Dashboard](https://dashboard.photonengine.com)
- Tạo một ứng dụng mới với **AppKind** là `Quantum`, sao chép `AppId`
- Trong Unity Editor, dán `AppId` vào cửa sổ cấu hình mạng của Photon Quantum

**3. Chạy và Kiểm thử (Testing)**
- Mở scene `MainMenu` tại `Assets/_Project/UnityUser/Scenes/`
- Để test Online 2 người: tạo bản Build qua **File → Build Settings** ra một cửa sổ riêng
- Hoặc sử dụng công cụ kiểm thử local của Quantum để giả lập 2 instances kết nối vào Lobby và ghép trận

---

## 👤 Tác Giả (Author)

| | |
|---|---|
| **Họ và Tên** | Nguyễn Đức Huy |
| **Email** | [huyco14022004@gmail.com](mailto:huyco14022004@gmail.com) |
| **LinkedIn** | [nguyễn-đức-huy](https://www.linkedin.com/in/nguy%E1%BB%85n-%C4%91%E1%BB%A9c-huy-081a73411/) |
