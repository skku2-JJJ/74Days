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
          // 아침: 배/선원 상태 확인
          ShipManager.Instance?.UpdateShipStatus();
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
          // 밤: 선원 요구사항 체크 및 하루 마무리
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
