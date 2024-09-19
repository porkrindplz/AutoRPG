using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Utilities;
using TMPro;
using UnityEngine;

public class CountdownGameTimer : MonoBehaviour
{
    [SerializeField] private int countdownStart;
    private TextMeshProUGUI CountdownText;
    private PauseOnEnable _pauseOnEnable;

    private CountdownTimer _timer;
    private bool _hasStarted = false;
    
    // Start is called before the first frame update
    void Start()
    {
        CountdownText = GetComponent<TextMeshProUGUI>();
        _pauseOnEnable = GetComponent<PauseOnEnable>();
        CountdownText.enabled = false;
        countdownStart = 10;
        GameManager.Instance.OnBeforeGameStateChanged += (state, newState) =>
        {
            if (newState == EGameState.SpawnEnemy && GameManager.Instance.IsCountdownEnabled)
            {
                _timer = new CountdownTimer(countdownStart);
                _timer.Start();

                _hasStarted = false;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer is { IsFinished: false })
        {
            if (Mathf.Approximately(Time.timeScale, 1))
            {
                _hasStarted = true;
                CountdownText.enabled = true;
                Time.timeScale = 0;
            }

            if (Time.timeScale == 0 && _hasStarted)
            {
                _timer.Tick(Time.unscaledDeltaTime);
            }
            CountdownText.text = ((int)_timer.Time).ToString();
        }

        if (_timer?.IsFinished == true)
        {
            //_pauseOnEnable.enabled = false;
            CountdownText.enabled = false;
            Time.timeScale = 1;
            _timer = null;
        }
        
    }

    private void FixedUpdate()
    {
    }
}
