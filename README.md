# Unity 탱크 배틀
<img width="620" height="420" alt="Image" src="https://github.com/user-attachments/assets/9a38e365-fb9f-442e-a975-49d5e3a93792" />

> Unity에서 이동, 회전, Rigidbody, Collider의 기본 조작과 물리 처리 방식을 학습한 뒤,
> 포탄 발사 기능을 추가하여 배틀 형태로 확장한 미니 프로젝트입니다.
> 여기에 C# 비동기 기반 TCP 서버를 구현하여,
> 클라이언트 입력을 서버로 전송한 뒤 서버에서 계산한 탱크 상태를 다시 반영하는 구조로 확장하였습니다.

## 프로젝트 개요

| 항목 | 내용 |
|---|---|
| 프로젝트명 | TankBattle |
| 개발 기간 | 2026.05.22 ~ 2026.05.25 |
| 개발 인원 | 1명 |
| 개발 환경 | Unity 6.3 (6000.3.7f1), C# |
| 실행 환경 | Windows |
| IDE | Unity Editor, Visual Studio 2022 |

## 관련 저장소
| 구분 | 링크 |
|---|---|
| Unity Client | [TankBattle](https://github.com/fanjae/TankBattle) |
| C# TCP Server | [TankBattleServer](https://github.com/fanjae/TankBattleServer) |

## 실행 방법
### 싱글 플레이
1. Unity 프로젝트를 엽니다.
2. Unity Editor에서 Play 버튼을 눌러 실행합니다.

### 멀티 플레이
1. `TankBattleServer` 프로젝트를 먼저 실행합니다.
2. 서버가 연결 대기 상태인지 확인합니다.
3. Unity 프로젝트를 엽니다.
4. Unity Editor에서 Play 버튼을 눌러 클라이언트를 실행합니다.

※ 멀티 플레이 테스트는 ParrelSync 환경에서 진행하였습니다.


## 구현 기능
<img width="600" height="500" alt="Image" src="https://github.com/user-attachments/assets/6bd9c187-0c16-484a-975e-4a1ea0c77010" />
<img width="600" height="500" alt="Image" src="https://github.com/user-attachments/assets/f38637d0-bc9b-491c-90a3-fa6df91a3117" />

### 게임 기능 (싱글)
- Rigidbody 기반 이동 및 회전
- 터렛 / 포신 회전 구현
- 포탄 발사 및 충돌 처리
- 발사 방향 보조 조준점 구현
- 포신 기준 1인칭 시점 카메라 구현

### 게임 기능 (멀티)
- 비동기 TCP 서버 연결
- 클라이언트 입력 전송
- 서버 계산 결과 게임 오브젝트 반영

## 구조 설계 

### 싱글 플레이
- 싱글 플레이에서는 탱크의 기능을 역할별 스크립트로 분리하여 구성하였습니다.
  
- `TankMove` : Rigidbody 기반 전진, 후진, 차체 회전 처리
- `TankTurretAim` : 터렛 좌우 회전 처리
- `TankGunAim` : 포신 상하 각도 조절 및 회전 제한 처리
- `TankFire` : 포탄 생성, 발사 쿨타임, 발사 힘 적용 처리

### 멀티 플레이
- 멀티 플레이에서는 클라이언트가 탱크 상태를 직접 확정하지 않고, 입력값을 서버로 전송한 뒤 서버에서 계산된 결과를 클라이언트에 반영하는 방식으로 구성하였습니다.

- `TankInputSender` : 현재 입력 상태를 패킷으로 생성하여 서버에 전송
- `ServerConnector` : TCP 서버 연결, 패킷 송수신 처리
- `NetworkTankView` : 서버에서 받은 탱크 상태를 Unity 오브젝트에 반영
- `TankInputSender`는 일정 주기마다 이동, 회전, 터렛, 포신, 발사 입력을 `InputPacket`으로 만들어 서버에 전송합니다.

## 조작 방법

| 키 | 기능 |
|:---:|---|
| W / S | 탱크 전진 / 후진 |
| A / D | 탱크 차체 회전 |
| ← → | 터렛 회전 |
| ↑ ↓ | 포신 회전 |
| Space | 포탄 발사 |

## 플레이 영상
- [싱글 플레이 영상](https://www.youtube.com/watch?v=7Lyk9WAsVgI)
- [멀티 플레이 영상](https://www.youtube.com/watch?v=orXmegdUMps)

## 개선 예정 사항
- 서버 기준 충돌 판정 처리
- 포탄 발사 결과 동기화
- 플레이어 체력 및 승패 처리
- 접속 종료 및 예외 처리 보완
- 클라이언트 보간 처리 적용

## 개발 기록
- 추후 추가 예정

