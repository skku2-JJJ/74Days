using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

 public class DayManager : MonoBehaviour
  {
      public static DayManager Instance { get; private set; }

      [Header("Day Settings")]
      [SerializeField] private int currentDay = 1;
      [SerializeField] private int maxDays = 74;

      [Header("Day Phases")]
      public DayPhase currentPhase = DayPhase.None;

      // 이벤트 시스템
      public event Action<int> OnDayStart;
      public event Action<int> OnDayEnd;
      public event Action<DayPhase> OnPhaseChange;

      public int CurrentDay => currentDay;
      public int MaxDays => maxDays;
      public DayPhase CurrentPhase => currentPhase;
      public bool IsGameOver => currentDay >= maxDays || IsAllCrewDead() || IsShipDestroyed();

      // ========== 일일 수확량 추적 ==========

      /// <summary>
      /// 오늘 수집한 자원 (읽기 전용 복사본 - UI 표시용)
      /// 실제 자원은 SceneTransitionManager에서 ShipInventory로 직접 전달됨
      /// </summary>
      private Inventory _todayHarvest = new Inventory();

      /// <summary>
      /// 오늘의 수확량 (읽기 전용 - UI 표시 전용)
      /// </summary>
      public IReadOnlyDictionary<ResourceType, int> TodayHarvest => _todayHarvest.Items;

      void Awake()
      {
          // 싱글톤
          if (Instance == null)
          {
              Instance = this;
              DontDestroyOnLoad(gameObject);

              // 씬 로드 완료 이벤트 구독
              SceneManager.sceneLoaded += OnSceneLoaded;
          }
          else
          {
              Destroy(gameObject);
          }
      }

      void OnDestroy()
      {
          // 이벤트 구독 해제
          if (Instance == this)
          {
              SceneManager.sceneLoaded -= OnSceneLoaded;
          }
      }

      void Start()
      {
          StartDay();
      }

      // 씬 로드 완료 시 자동 호출 (Unity 이벤트)
      private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
      {
          // Loading Scene은 무시
          if (scene.name == "Loading")
          {
              Debug.Log($"[DayManager] Loading Scene 로드됨 - 페이즈 변경 안 함");
              return;
          }

          // SceneTransitionManager가 설정한 목표 페이즈 확인
          if (SceneTransitionManager.TargetPhase != DayPhase.None)
          {
              Debug.Log($"[DayManager] {scene.name} 씬 로드 완료 - 목표 페이즈: {SceneTransitionManager.TargetPhase}");

              // 1프레임 대기 후 페이즈 변경 (UI Awake 완료 보장)
              StartCoroutine(ChangePhaseAfterFrame(SceneTransitionManager.TargetPhase));

              // 페이즈 변경 완료 후 초기화
              SceneTransitionManager.TargetPhase = DayPhase.None;
          }
          else
          {
              Debug.Log($"[DayManager] {scene.name} 씬 로드됨 - 목표 페이즈 없음 (현재 페이즈 유지: {currentPhase})");
          }
      }

      // 1프레임 후 페이즈 변경 (UI Awake 완료 보장)
      private IEnumerator ChangePhaseAfterFrame(DayPhase targetPhase)
      {
          // UI의 Awake가 완료될 때까지 1프레임 대기
          yield return null;

          ChangePhase(targetPhase);
          Debug.Log($"[DayManager] 페이즈 변경 완료: {targetPhase}");

          // 씬 로드 완료 후 페이드 인
          if (FadeManager.Instance != null)
          {
              FadeManager.Instance.FadeIn(1f);
              Debug.Log($"[DayManager] 페이드 인 시작 - 게임 화면 표시");
          }
      }

      // 하루 시작
      public void StartDay()
      {
          OnDayStart?.Invoke(currentDay);
          ResetDailyHarvest(); // 새로운 날 시작 시 수확량 초기화
          ChangePhase(DayPhase.Morning);
      }

      // 페이즈 변경
      public void ChangePhase(DayPhase newPhase)
      {
          currentPhase = newPhase;
          OnPhaseChange?.Invoke(currentPhase);

          switch (currentPhase)
          {
              case DayPhase.Morning:
                  HandleMorningPhase();
                  break;
              case DayPhase.Diving:
                  HandleDivingPhase();
                  break;
              case DayPhase.Evening:
                  HandleEveningPhase();
                  break;
          }
      }

      // ========== 페이즈 전환 함수 ==========

      // Morning → Diving
      public void GoToDiving()
      {
          if (currentPhase != DayPhase.Morning)
          {
              Debug.LogWarning($"[페이즈 전환 실패] Morning 페이즈에서만 Diving으로 갈 수 있습니다! (현재: {currentPhase})");
              return;
          }

          ChangePhase(DayPhase.Diving);
          Debug.Log("[페이즈 전환] Morning → Diving");
      }

      // Diving → Evening
      public void GoToEvening()
      {
          if (currentPhase != DayPhase.Diving)
          {
              Debug.LogWarning($"[페이즈 전환 실패] Diving 페이즈에서만 Evening으로 갈 수 있습니다! (현재: {currentPhase})");
              return;
          }

          ChangePhase(DayPhase.Evening);
          Debug.Log("[페이즈 전환] Diving → Evening");
      }

      // 하루 종료
      public void EndDay()
      {
          OnDayEnd?.Invoke(currentDay);

          // GameOver 조건을 날짜 증가 전에 체크하여 불필요한 StartDay() 호출 방지
          bool willBeGameOver = (currentDay + 1 >= maxDays) || IsAllCrewDead() || IsShipDestroyed();

          currentDay++;

          if (willBeGameOver)
          {
              HandleGameEnd();
          }
          else
          {
              StartDay();
          }
      }

      private void HandleMorningPhase()
      {
          // 아침: 배/선원 상태 확인
          ShipManager.Instance?.CheckShipStatus();
          CrewManager.Instance?.UpdateCrewNeeds();
      }

      private void HandleDivingPhase()
      {
          // 잠수 페이즈 시작
          // 플레이어가 잠수를 시작할 수 있음
      }

      private void HandleEveningPhase()
      {
          // 저녁: 자원 분배 시간

          // SceneTransitionManager에서 pending된 DiverBag 자원을 ShipInventory에 적용
          if (SceneTransitionManager.Instance != null)
          {
              SceneTransitionManager.Instance.ApplyPendingResources();
          }

          // UI로 자원 분배 화면 표시 (ResourceDistributionUI.OnPhaseChanged에서 자동)
          // 완료 버튼 클릭 시 ResourceDistributionUI에서 CompleteEvening() 호출
      }

      private void HandleGameEnd()
      {
          Debug.Log("[DayManager] 게임 종료 처리 시작");

          // 통계 수집
          int survivedCrew = CrewManager.Instance.GetSurvivedCrewCount();
          int totalCrew = CrewManager.Instance.TotalCrew;
          float shipHp = ShipManager.Instance.Ship.Hp;

          // 게임 오버 이유 판단
          bool isVictory = false;
          GameOverReason reason = GameOverReason.None;

          if (currentDay >= maxDays)
          {
              isVictory = true;
              reason = GameOverReason.Victory;
              Debug.Log($"[게임 종료] 🎉 승리! 74일 생존 성공!");
          }
          else if (IsAllCrewDead())
          {
              isVictory = false;
              reason = GameOverReason.AllCrewDead;
              Debug.Log($"[게임 종료] 💀 패배 - 선원 전멸");
          }
          else if (IsShipDestroyed())
          {
              isVictory = false;
              reason = GameOverReason.ShipDestroyed;
              Debug.Log($"[게임 종료] 💀 패배 - 배 파괴");
          }

          // GameOverData에 저장
          GameOverData.RecordGameState(
              currentDay,
              survivedCrew,
              totalCrew,
              shipHp,
              isVictory,
              reason
          );

          // 통계 출력
          GameOverData.Print();

          // 게임 오버 씬으로 전환
          if (SceneTransitionManager.Instance != null)
          {
              SceneTransitionManager.Instance.GoToGameOver();
          }
          else
          {
              Debug.LogError("[DayManager] SceneTransitionManager를 찾을 수 없습니다!");
          }
      }

      // ========== Evening 완료 처리 ==========

      /// <summary>
      /// Evening 완료 처리 (자원 분배 → 노화 → 다음 날 전환)
      /// ResourceDistributionUI에서 호출
      /// </summary>
      public void CompleteEvening()
      {
          Debug.Log("[DayManager] Evening 완료 처리 시작");

          // 1. 자원 분배 적용
          ApplyResourceDistribution();

          // 2. 일일 노화 처리 (배/선원)
          ProcessDailyDeterioration();

          // 3. 다음날로 전환 (EndDay에서 게임 종료 체크)
          EndDay();
      }

      /// <summary>
      /// 모든 선원의 DivisionBox를 순회하여 할당된 자원 적용
      /// </summary>
      private void ApplyResourceDistribution()
      {
          Debug.Log("[DayManager] 자원 분배 적용 시작");

          if (ResourceDistributionUI.Instance == null)
          {
              Debug.LogWarning("[DayManager] ResourceDistributionUI.Instance가 null입니다!");
              return;
          }

          int totalResourcesApplied = 0;

          // ResourceDistributionUI에서 모든 CrewResourceItem 가져오기
          var crewItems = ResourceDistributionUI.Instance.GetComponentsInChildren<CrewResourceItem>();

          foreach (var crewItem in crewItems)
          {
              if (crewItem == null || !crewItem.gameObject.activeInHierarchy) continue;

              // 각 선원의 DivisionBox 가져오기
              var divisionBoxes = crewItem.GetComponentsInChildren<DivisionBoxSlot>();

              foreach (var box in divisionBoxes)
              {
                  if (box.HasResource)
                  {
                      ResourceType resourceType = box.AssignedResource.Value;

                      // 자원 소비는 AssignResourceToCrew() 내부에서 처리됨 (중복 방지)
                      // AssignResourceToCrew()가 내부적으로 ShipManager.UseResource()를 호출함

                      // 선원에게 자원 적용
                      bool assigned = CrewManager.Instance.AssignResourceToCrew(
                          box.GetAssignedCrew(),
                          resourceType,
                          1
                      );

                      if (assigned)
                      {
                          totalResourcesApplied++;
                          Debug.Log($"[DayManager] {box.GetAssignedCrew().CrewName}에게 {resourceType} 적용 완료");
                      }
                      else
                      {
                          Debug.LogWarning($"[DayManager] {box.GetAssignedCrew().CrewName}에게 {resourceType} 할당 실패 (자원 부족 또는 유효하지 않은 자원)");
                      }
                  }
              }
          }

          Debug.Log($"[DayManager] 총 {totalResourcesApplied}개 자원 적용 완료");
      }

      /// <summary>
      /// 일일 노화 처리 (배/선원)
      /// </summary>
      private void ProcessDailyDeterioration()
      {
          Debug.Log("[DayManager] 일일 노화 처리 시작");

          // 선원 일일 노화 및 사망 체크
          if (CrewManager.Instance != null)
          {
              CrewManager.Instance.ProcessDailyNeeds();
          }

          // 배 일일 노화
          if (ShipManager.Instance != null)
          {
              ShipManager.Instance.ProcessDailyShipDeterioration();
          }

          Debug.Log("[DayManager] 일일 노화 처리 완료");
      }

      // ========== 게임 종료 조건 체크 ==========

      /// <summary>
      /// 선원이 모두 사망했는지 확인
      /// </summary>
      private bool IsAllCrewDead()
      {
          return CrewManager.Instance != null &&
                 CrewManager.Instance.GetSurvivedCrewCount() == 0;
      }

      /// <summary>
      /// 배가 파괴되었는지 확인
      /// </summary>
      private bool IsShipDestroyed()
      {
          return ShipManager.Instance != null &&
                 ShipManager.Instance.Ship != null &&
                 ShipManager.Instance.Ship.Hp <= 0;
      }

      // ========== 일일 수확량 관리 ==========

      /// <summary>
      /// 씬 전환 시 DiverBag의 자원을 TodayHarvest에 기록 (UI 표시 전용 복사본)
      /// 실제 자원은 SceneTransitionManager.GoToShip()에서 ShipInventory로 직접 전달됨
      /// SceneTransitionManager.GoToShip()에서 호출
      /// </summary>
      public void RecordTodayHarvest(Inventory diverBag)
      {
          if (diverBag == null)
          {
              Debug.LogWarning("[DayManager] DiverBag이 null입니다!");
              return;
          }

          Debug.Log("[DayManager] 오늘의 수확량 기록 시작 (UI 표시용)");

          // DiverBag의 모든 자원을 TodayHarvest에 복사 (읽기 전용)
          int totalHarvested = 0;
          foreach (var item in diverBag.Items)
          {
              _todayHarvest.Add(item.Key, item.Value);
              totalHarvested += item.Value;
              Debug.Log($"[DayManager] {item.Key}: {item.Value}개");
          }

          Debug.Log($"[DayManager] 총 수확량: {totalHarvested}개 (UI 표시 전용)");
      }

      // TransferHarvestToShip() 메서드 제거됨
      // 자원은 SceneTransitionManager.GoToShip()에서 ShipInventory로 직접 전달됨

      /// <summary>
      /// 일일 수확량 초기화 (새로운 날 시작 시)
      /// </summary>
      private void ResetDailyHarvest()
      {
          _todayHarvest.Clear();
          Debug.Log("[DayManager] 일일 수확량 초기화");
      }

      /// <summary>
      /// 오늘의 수확량을 모두 제거 (다이버 사망 시 사용)
      /// </summary>
      public void ClearTodayHarvest()
      {
          _todayHarvest.Clear();
          Debug.Log("[DayManager] 오늘의 수확량 초기화 (다이버 사망)");
      }

      /// <summary>
      /// 특정 자원의 오늘 수확량 조회
      /// </summary>
      public int GetTodayHarvestAmount(ResourceType type)
      {
          return _todayHarvest.GetAmount(type);
      }

      // ========== 게임 재시작을 위한 데이터 리셋 ==========

      /// <summary>
      /// 게임 재시작 시 DayManager의 모든 상태를 초기화
      /// MainMenuUI.ResetAllGameData()에서 호출
      /// </summary>
      public void ResetGameState()
      {
          Debug.Log("[DayManager] 게임 상태 초기화 시작");

          // 날짜 초기화
          currentDay = 1;

          // 페이즈 초기화
          currentPhase = DayPhase.None;

          // 일일 수확량 초기화
          _todayHarvest.Clear();

          // 이벤트 구독자들은 유지 (이벤트는 초기화 안 함)
          Debug.Log("[DayManager] 게임 상태 초기화 완료 - Day 1로 리셋");
      }
  }
