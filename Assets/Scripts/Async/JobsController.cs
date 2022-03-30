using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading;
using System.Threading.Tasks;

public enum ProcessType { None, Async, Jobs }
public class JobsController : MonoBehaviour
{
    [SerializeField]
    private ProcessType _setup;

    //async
    private CancellationTokenSource _cancellationTokenSource;

    // jobs
    private NativeArray<JobHandle> _jobHandles;

    private void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }


    private void Update()
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();

        switch (_setup)
        {
            case ProcessType.None:
                for (int i = 0; i < 10; i++)
                {
                    ReallyToughTask();
                }
                break;

            case ProcessType.Async:
                for (int i = 0; i < 10; i++)
                {
                    ReallyToughTaskAsync();
                }
                break;

            case ProcessType.Jobs:
                _jobHandles = new NativeArray<JobHandle>(10, Allocator.Persistent);
                for (int i = 0; i < 10; i++)
                {
                    var jobHandle = ReallyToughTaskJob();
                    _jobHandles[i] = jobHandle;
                }
                break;
        }
        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        print($"Operation took : {elapsedTime }");
    }

    private void LateUpdate()
    {
        if (_setup == ProcessType.Jobs)
        {
            foreach (JobHandle job in _jobHandles)
                job.Complete();

            _jobHandles.Dispose();
        }
    }
    private void ReallyToughTask()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++) 
            value = math.exp10(math.sqrt(value));
    }

    private async void ReallyToughTaskAsync()
    {
        var token = _cancellationTokenSource.Token;
        await Task.Run(() =>
        {
            ReallyToughCalculationTaskAsync(token);
        }, token);
    }

    private void ReallyToughCalculationTaskAsync(CancellationToken token)
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            if (token.IsCancellationRequested) return;
            value = math.exp10(math.sqrt(value));
        }
    }

    private JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
    }
}

[BurstCompile]
public struct ReallyToughJob : IJob
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++) value = math.exp10(math.sqrt(value));
    }
}