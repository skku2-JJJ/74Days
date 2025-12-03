using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [SerializeField] private Light2D _light2D;
    [SerializeField] private GameObject[] torchesLights;

    [SerializeField] private float _nightIntensity = 0.07f;

    void Start()
    {
        DayManager.Instance.OnPhaseChange += OnPhaseChanged;
    }

    private void OnPhaseChanged(DayPhase phase)
    {
        if (phase == DayPhase.Morning)
        {
            _light2D.intensity = 1;
            foreach (GameObject go in torchesLights)
            {
                go.SetActive(false);
            }
        }
        else if (phase == DayPhase.Evening)
        {
            _light2D.intensity = _nightIntensity;
            foreach (GameObject go in torchesLights)
            {
                go.SetActive(true);
            }
        }
    }


    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }
}
