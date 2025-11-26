using UnityEngine;
using System;

 public class DayManager : MonoBehaviour
  {
      public static DayManager Instance { get; private set; }

      [Header("Day Settings")]
      [SerializeField] private int currentDay = 1;
      [SerializeField] private int maxDays = 100;

      [Header("Day Phases")]
      public DayPhase currentPhase = DayPhase.Morning;

      // 이벤트 시스템
      public event Action<int> OnDayStart;
      public event Action<int> OnDayEnd;
      public event Action<DayPhase> OnPhaseChange;

      public int CurrentDay => currentDay;
      public int MaxDays => maxDays;
      public DayPhase CurrentPhase => currentPhase;
      public bool IsGameOver => currentDay >= maxDays;

      void Awake()
      {
          // 싱글톤
          if (Instance == null)
          {
              Instance = this;
              DontDestroyOnLoad(gameObject);
          }
          else
          {
              Destroy(gameObject);
          }
      }

      void Start()
      {
          StartDay();
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
              case DayPhase.Night:
                  HandleNightPhase();
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

      // Evening → Night
      public void GoToNight()
      {
          if (currentPhase != DayPhase.Evening)
          {
              Debug.LogWarning($"[페이즈 전환 실패] Evening 페이즈에서만 Night로 갈 수 있습니다! (현재: {currentPhase})");
              return;
          }

          ChangePhase(DayPhase.Night);
          Debug.Log("[페이즈 전환] Evening → Night");
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
      }

      private void HandleNightPhase()
      {
          // 밤: 배/선원 일일 노화 처리 및 하루 마무리
          ShipManager.Instance?.ProcessDailyShipDeterioration();
          CrewManager.Instance?.ProcessDailyNeeds();
          EndDay();
      }

      private void HandleGameEnd()
      {
          // 게임 종료 처리
          int survivedCrew = CrewManager.Instance.GetSurvivedCrewCount();
          Debug.Log($"Game Over! Survived Crew: {survivedCrew}/{CrewManager.Instance.TotalCrew}");
      }
  }
