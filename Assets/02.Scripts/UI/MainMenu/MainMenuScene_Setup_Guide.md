# MainMenu 씬 Unity 에디터 설정 가이드

MainMenu 씬을 Unity 에디터에서 설정하는 전체 과정을 안내합니다.

---

## 📋 목차
1. [씬 생성 및 기본 설정](#1-씬-생성-및-기본-설정)
2. [Canvas 설정](#2-canvas-설정)
3. [배경 패널](#3-배경-패널)
4. [타이틀 텍스트](#4-타이틀-텍스트)
5. [버튼 설정](#5-버튼-설정)
6. [MainMenuController 오브젝트](#6-mainmenucontroller-오브젝트)
7. [Build Settings 구성](#7-build-settings-구성)
8. [최종 점검](#8-최종-점검)

---

## 1. 씬 생성 및 기본 설정

### 1.1 새 씬 생성
```
Assets/01.Scenes/ 폴더에서:
- 우클릭 → Create → Scene
- 이름: "MainMenu"
```

### 1.2 기존 오브젝트 정리
- Main Camera 유지 (기본 설정 그대로)
- Directional Light 삭제 (UI 전용 씬이므로 불필요)

---

## 2. Canvas 설정

### 2.1 Canvas 생성
```
Hierarchy에서:
- 우클릭 → UI → Canvas
- 이름: "MainMenuCanvas"
```

### 2.2 Canvas 컴포넌트 설정
```
Inspector → Canvas:
- Render Mode: Screen Space - Overlay
- Pixel Perfect: ☑ (체크)
- Sort Order: 0
```

### 2.3 Canvas Scaler 설정
```
Inspector → Canvas Scaler:
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1920 x 1080
- Screen Match Mode: Match Width Or Height
- Match: 0.5 (중간값)
```

### 2.4 Graphic Raycaster
```
Inspector → Graphic Raycaster:
- 기본 설정 유지
```

---

## 3. 배경 패널

### 3.1 Background Panel 생성
```
Hierarchy에서:
- MainMenuCanvas 우클릭 → UI → Panel
- 이름: "BackgroundPanel"
```

### 3.2 RectTransform 설정
```
Inspector → Rect Transform:
- Anchor: Stretch-Stretch (전체 화면)
- Left: 0, Right: 0, Top: 0, Bottom: 0
```

### 3.3 Image 컴포넌트 설정
```
Inspector → Image:
- Color: 어두운 색상 추천 (예: R:20, G:20, B:30, A:255)
- 또는 배경 이미지 사용 시: Source Image에 스프라이트 할당
- Image Type: Simple
```

---

## 4. 타이틀 텍스트

### 4.1 Title 텍스트 생성
```
Hierarchy에서:
- BackgroundPanel 우클릭 → UI → Text - TextMeshPro
- 이름: "TitleText"
- (TextMeshPro Importer 창 뜨면 "Import TMP Essentials" 클릭)
```

### 4.2 RectTransform 설정
```
Inspector → Rect Transform:
- Anchor: Top-Center
- Pos X: 0, Pos Y: -200 (화면 상단에서 200픽셀 아래)
- Width: 800, Height: 150
```

### 4.3 TextMeshProUGUI 설정
```
Inspector → TextMeshPro - Text:
- Text: "74 Days"
- Font: (원하는 폰트 - 기본 LiberationSans SDF 사용 가능)
- Font Style: Bold
- Font Size: 100
- Alignment: Center-Middle (가운데 정렬)
- Color: White (R:255, G:255, B:255)
- Vertex Color: 필요시 그라디언트 효과 추가
```

### 4.4 추가 효과 (선택사항)
```
Outline 추가:
- TextMeshPro → Extra Settings → Outline
- Outline Color: Black
- Outline Thickness: 0.2

그림자 추가:
- Inspector 하단 → Add Component → Shadow
- Effect Color: Black (반투명)
- Effect Distance: X:5, Y:-5
```

---

## 5. 버튼 설정

### 5.1 Buttons Panel 생성 (버튼 그룹화)
```
Hierarchy에서:
- BackgroundPanel 우클릭 → UI → Panel
- 이름: "ButtonsPanel"
```

### 5.2 ButtonsPanel RectTransform 설정
```
Inspector → Rect Transform:
- Anchor: Middle-Center
- Pos X: 0, Pos Y: -150 (화면 중앙보다 약간 아래)
- Width: 400, Height: 300
```

### 5.3 ButtonsPanel Image 제거
```
Inspector → Image:
- 컴포넌트 우클릭 → Remove Component
- (투명 컨테이너로만 사용)
```

### 5.4 Start Button 생성
```
Hierarchy에서:
- ButtonsPanel 우클릭 → UI → Button - TextMeshPro
- 이름: "StartButton"
```

### 5.5 StartButton RectTransform 설정
```
Inspector → Rect Transform:
- Anchor: Top-Center
- Pos X: 0, Pos Y: 0
- Width: 350, Height: 80
```

### 5.6 StartButton 컴포넌트 설정
```
Inspector → Button:
- Interactable: ☑
- Transition: Color Tint
- Normal Color: R:34, G:139, B:34 (녹색)
- Highlighted Color: R:50, G:180, B:50 (밝은 녹색)
- Pressed Color: R:20, G:100, B:20 (어두운 녹색)
- Selected Color: R:34, G:139, B:34
- Disabled Color: R:128, G:128, B:128
- Color Multiplier: 1
- Fade Duration: 0.1
```

### 5.7 StartButton 텍스트 설정
```
StartButton 하위의 "Text (TMP)" 선택:
Inspector → TextMeshPro - Text:
- Text: "게임 시작"
- Font Size: 36
- Alignment: Center-Middle
- Color: White
```

### 5.8 Quit Button 생성
```
Hierarchy에서:
- ButtonsPanel 우클릭 → UI → Button - TextMeshPro
- 이름: "QuitButton"
```

### 5.9 QuitButton RectTransform 설정
```
Inspector → Rect Transform:
- Anchor: Top-Center
- Pos X: 0, Pos Y: -120 (StartButton 아래로 120픽셀)
- Width: 350, Height: 80
```

### 5.10 QuitButton 컴포넌트 설정
```
Inspector → Button:
- Interactable: ☑
- Transition: Color Tint
- Normal Color: R:178, G:34, B:34 (빨간색)
- Highlighted Color: R:220, G:50, B:50 (밝은 빨간색)
- Pressed Color: R:120, G:20, B:20 (어두운 빨간색)
- Selected Color: R:178, G:34, B:34
- Disabled Color: R:128, G:128, B:128
- Color Multiplier: 1
- Fade Duration: 0.1
```

### 5.11 QuitButton 텍스트 설정
```
QuitButton 하위의 "Text (TMP)" 선택:
Inspector → TextMeshPro - Text:
- Text: "게임 종료"
- Font Size: 36
- Alignment: Center-Middle
- Color: White
```

---

## 6. MainMenuController 오브젝트

### 6.1 빈 GameObject 생성
```
Hierarchy에서:
- MainMenuCanvas 우클릭 → Create Empty
- 이름: "MainMenuController"
```

### 6.2 MainMenuUI 스크립트 추가
```
Inspector에서:
- Add Component → 검색: "MainMenuUI"
- MainMenuUI.cs 스크립트 추가
```

### 6.3 MainMenuUI 인스펙터 참조 연결
```
Inspector → MainMenuUI:

[UI References]
- Start Button: StartButton 드래그 (Hierarchy에서)
- Quit Button: QuitButton 드래그

[Title]
- Title Text: TitleText 드래그 (Hierarchy에서)
```

**연결 확인:**
- Start Button 슬롯에 StartButton이 할당되었는지
- Quit Button 슬롯에 QuitButton이 할당되었는지
- Title Text 슬롯에 TitleText가 할당되었는지

---

## 7. Build Settings 구성

### 7.1 Build Settings 열기
```
Unity 상단 메뉴:
- File → Build Settings
```

### 7.2 MainMenu 씬을 Index 0으로 설정
```
Build Settings 창에서:
1. "Add Open Scenes" 클릭 (MainMenu.unity가 열려있는 상태에서)
2. MainMenu를 가장 위로 드래그 (Index 0)
3. 기존 씬들 순서 조정:
   - 0: MainMenu
   - 1: Ship
   - 2: GameOver
   - 3: Loading (있다면)
```

### 7.3 씬 빌드 순서 확인
```
최종 Build Settings 리스트:
☑ Scenes/MainMenu.unity           (Index 0)
☑ Scenes/Ship.unity                (Index 1)
☑ Scenes/GameOver.unity            (Index 2)
☑ Scenes/Loading.unity             (Index 3, 선택사항)
```

---

## 8. 최종 점검

### 8.1 Hierarchy 구조 확인
```
MainMenu (씬)
├── Main Camera
└── MainMenuCanvas
    ├── BackgroundPanel (Image)
    │   ├── TitleText (TextMeshProUGUI - "74 Days")
    │   └── ButtonsPanel (빈 패널)
    │       ├── StartButton (Button + TextMeshProUGUI - "게임 시작")
    │       └── QuitButton (Button + TextMeshProUGUI - "게임 종료")
    └── MainMenuController (MainMenuUI 스크립트)
```

### 8.2 MainMenuUI 참조 점검
```
MainMenuController 선택 후 Inspector 확인:
✅ Start Button: StartButton 연결됨
✅ Quit Button: QuitButton 연결됨
✅ Title Text: TitleText 연결됨
```

### 8.3 기능 테스트
```
Play Mode에서 테스트:
1. "게임 시작" 버튼 클릭 → Ship 씬으로 전환되는지 확인
2. "게임 종료" 버튼 클릭 → 에디터 플레이 모드 종료되는지 확인
3. 콘솔 로그 확인:
   - "[MainMenu] 메인 메뉴 초기화 완료"
   - "[MainMenu] 게임 시작 버튼 클릭"
   - "[MainMenu] 게임 데이터 초기화 시작/완료"
```

### 8.4 FadeManager 통합 확인
```
Ship 씬에 FadeManager가 있다면:
- 게임 시작 시 Fade Out → Ship 씬 전환 효과 확인
- 없다면 즉시 전환됨 (정상 동작)
```

---

## 🎨 추가 커스터마이징 (선택사항)

### 배경 이미지 사용
```
BackgroundPanel → Image:
- Source Image에 배경 스프라이트 할당
- Image Type: Simple 또는 Sliced
- Preserve Aspect: 필요에 따라 체크
```

### 타이틀 애니메이션 추가
```
TitleText에 간단한 스케일 애니메이션:
1. Animation 창 열기 (Window → Animation → Animation)
2. TitleText 선택 후 "Create" 클릭
3. Scale 애니메이션 추가 (1 → 1.1 → 1 반복)
```

### 버튼 사운드 효과
```
StartButton/QuitButton에:
- Audio Source 컴포넌트 추가
- Button → Navigation → OnClick() 이벤트에 AudioSource.Play() 연결
```

---

## ⚠️ 주의사항

1. **씬 순서**: MainMenu가 Build Settings에서 Index 0이어야 게임 실행 시 가장 먼저 로드됩니다.

2. **FadeManager**: FadeManager는 DontDestroyOnLoad 싱글톤이므로, Ship 씬에서 한 번 생성되면 MainMenu로 돌아와도 유지됩니다.

3. **GameOverData**: MainMenuUI의 ResetAllGameData()가 GameOverData.Reset()을 호출하므로, 새 게임 시작 시 이전 통계가 초기화됩니다.

4. **Manager 초기화**: DayManager, CrewManager, ShipManager 등은 Ship 씬에서 다시 초기화됩니다.

---

## 📝 완료 체크리스트

- [ ] MainMenu.unity 씬 생성 완료
- [ ] Canvas 및 Canvas Scaler 설정 완료
- [ ] 배경 패널 생성 및 색상 설정 완료
- [ ] "74 Days" 타이틀 텍스트 생성 및 스타일 설정 완료
- [ ] "게임 시작" 버튼 생성 및 스타일 설정 완료
- [ ] "게임 종료" 버튼 생성 및 스타일 설정 완료
- [ ] MainMenuController GameObject 생성 완료
- [ ] MainMenuUI 스크립트 추가 및 참조 연결 완료
- [ ] Build Settings에서 MainMenu를 Index 0으로 설정 완료
- [ ] Play Mode에서 "게임 시작" 기능 테스트 완료
- [ ] Play Mode에서 "게임 종료" 기능 테스트 완료

---

이제 MainMenu 씬 설정이 완료되었습니다! 🎉
게임을 실행하면 MainMenu가 가장 먼저 표시되며, "게임 시작" 버튼을 누르면 Ship 씬으로 전환됩니다.