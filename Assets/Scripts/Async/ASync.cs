using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.Profiling;
using Unity.Jobs;
using Unity.Collections;

public class ASync : MonoBehaviour
{
    private bool _isCalculationOver;
    [SerializeField]
    private int _size;
    [SerializeField]
    private bool _isAsync;

    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        _isCalculationOver = true;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isAsync)
                PerformSlowCalculations();
            else
                PerformSlowCalculationsAsync();
        }
    }

    private void PerformSlowCalculations()
    {
        Profiler.BeginSample("Perform Slow Calculation");
        Stopwatch watch = new Stopwatch();
        watch.Start();

        _isCalculationOver = false;
        float[,] mapValues = new float[_size,_size];

        for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                mapValues[x, y] = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
            }
        }

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        print($"Operation took : {elapsedTime/1000}");
        _isCalculationOver = true;
        Profiler.EndSample();
    }

    private async void PerformSlowCalculationsAsync()
    {
        if (!_isCalculationOver) return;

        var token = _cancellationTokenSource.Token;
        Stopwatch watch = new Stopwatch();
        watch.Start();
        _isCalculationOver = false;

        var result = await Task.Run(() =>
        {

            float[,] mapValues = new float[_size, _size];
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    mapValues[x, y] = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        return mapValues;
                    }
                }
            }
            return mapValues;

        },token);

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            print($"calculation cancelled");
            return;
        }

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;        
        print($"Operation took : {elapsedTime / 1000} | result is size [{result.GetLength(0)},{result.GetLength(1)}]");
        _isCalculationOver = true;
    }

    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
    }
}
