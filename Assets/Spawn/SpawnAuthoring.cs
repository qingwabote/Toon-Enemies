using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnAuthoring : MonoBehaviour
{
    public GameObject[] Prefabs;

    public uint Num;
}

public struct SpawnPrefab : IBufferElementData
{
    public Entity Value;
}

public struct Spawn : IComponentData
{
    public struct Initializer : IComponentData { }

    public uint Num;
}

class SpawnBaker : Baker<SpawnAuthoring>
{
    public override void Bake(SpawnAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        var prefabs = AddBuffer<SpawnPrefab>(entity);
        if (authoring.Prefabs != null)
        {
            foreach (var item in authoring.Prefabs)
            {
                prefabs.Add(new SpawnPrefab() { Value = GetEntity(item, TransformUsageFlags.Dynamic) });
            }
        }

        AddComponent(entity, new Spawn
        {
            Num = authoring.Num
        });
        AddComponent(entity, new Spawn.Initializer());
    }
}

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct SpawnSystem : ISystem
{
    private Unity.Mathematics.Random m_RandomPrefab;
    private Unity.Mathematics.Random m_RandomPosition;

    public void OnCreate(ref SystemState state)
    {
        m_RandomPrefab = new Unity.Mathematics.Random(1);
        m_RandomPosition = new Unity.Mathematics.Random(1);
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<Spawn>())
        {
            return;
        }

        var spawn_entity = SystemAPI.GetSingletonEntity<Spawn>();
        if (state.EntityManager.HasComponent<Spawn.Initializer>(spawn_entity))
        {
            var spawn = state.EntityManager.GetComponentData<Spawn>(spawn_entity);
            var prefabs = SystemAPI.GetBuffer<SpawnPrefab>(spawn_entity);
            for (int i = 0; i < spawn.Num; i++)
            {
                var entity = state.EntityManager.Instantiate(prefabs[m_RandomPrefab.NextInt(0, prefabs.Length)].Value);
                SystemAPI.GetComponentRW<LocalTransform>(entity).ValueRW.Position = new float3(m_RandomPosition.NextFloat(-5, 5), 0, m_RandomPosition.NextFloat(-10, 10));
                // SystemAPI.GetComponentRW<Graphix.AnimationState>(entity).ValueRW.ClipIndex = m_Random.NextInt(0, 2);
            }
            state.EntityManager.RemoveComponent<Spawn.Initializer>(spawn_entity);
        }
        else
        {
            // var sysHandle = World.DefaultGameObjectInjectionWorld.GetExistingSystem<LocalToWorldSystem>();
            // ref var sysState = ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(sysHandle);
            // sysState.Enabled = false;
        }
    }
}
