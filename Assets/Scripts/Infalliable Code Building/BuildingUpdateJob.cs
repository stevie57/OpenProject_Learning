using Unity.Collections;
using Unity.Jobs;

namespace InfallibleCode.Completed
{
    public struct BuildingUpdateJob : IJobParallelFor
    {
        public NativeArray<Building.Data> BuildingDataArray;
        
        public void Execute(int index)
        {
            var data = BuildingDataArray[index];
            data.Update();
            BuildingDataArray[index] = data;
        }
    }
}