using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

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
          currentDay++;

          if (IsGameOver)
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
          // 아침: 배/선원 상태 확인 (노화 처리 X)
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
          // UI로 자원 분배 화면 표시
          // 완료 버튼 클릭 시 ResourceDistributionUI에서 EndDay() 호출
      }

      private void HandleGameEnd()
      {
          // 게임 종료 처리
          int survivedCrew = CrewManager.Instance.GetSurvivedCrewCount();
          Debug.Log($"Game Over! Survived Crew: {survivedCrew}/{CrewManager.Instance.TotalCrew}");
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

          Debug.Log("[DayManager] Evening 완료 처리 완료");
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
  }
