using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class InputEvent : MonoBehaviour
{
    public static event Action GamePauseChanged;
    public static event Action SimulationPauseChanged;
    public static event Action LeftDoubleClick;
    public static event Action SimulationTimeUpscaled;
    public static event Action SimulationTimeDownscaled;
    private bool _IsLeftMouseClicked;
    private TimeSpan _MinTimeBetweenClicks = TimeSpan.FromMilliseconds(200);
    private CancellationTokenSource _Source = new CancellationTokenSource();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GamePauseChanged?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.Space))
            SimulationPauseChanged?.Invoke();
        
        if (CheckForDoubleClick())
            LeftDoubleClick?.Invoke();

        if (Input.GetKeyDown(KeyCode.Comma))
            SimulationTimeDownscaled?.Invoke();

        if (Input.GetKeyDown(KeyCode.Period))
            SimulationTimeUpscaled?.Invoke();
    }

    private bool CheckForDoubleClick()
    {
        bool IsMouseClicked = Input.GetMouseButtonDown(0);

        if (!IsMouseClicked)
            return false;

        if (IsMouseClicked && !_IsLeftMouseClicked)
        {
            UpdateDoubleClick(_Source.Token);
            return false;
        }

        if (IsMouseClicked && _IsLeftMouseClicked)
        {
            _Source.Cancel();
            _Source = new CancellationTokenSource();
            return true;
        }

        return false;
    }

    private async void UpdateDoubleClick(CancellationToken token)
    {
        _IsLeftMouseClicked = true;
        try
        {
            await Task.Delay(_MinTimeBetweenClicks, token);
        }
        catch
        {

        }
        _IsLeftMouseClicked = false;
    }

    private class EventCallCondition
    {
        public EventCallCondition(Func<bool> condition, Action _event)
        {
            Condition = condition;
            Event = _event;
        }

        public Func<bool> Condition;
        public Action Event;
    }
}
