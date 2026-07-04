using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[MaterialProperty("_TintColor1")]
public struct TintColor1 : IComponentData
{
    public float4 Value;
}

[MaterialProperty("_TintColor2")]
public struct TintColor2 : IComponentData
{
    public float4 Value;
}

[MaterialProperty("_TintColor3")]
public struct TintColor3 : IComponentData
{
    public float4 Value;
}

#if UNITY_EDITOR
public class TintColorAuthoring : MonoBehaviour
{
    private static readonly int s_TintColor1 = Shader.PropertyToID("_TintColor1");
    private static readonly int s_TintColor2 = Shader.PropertyToID("_TintColor2");
    private static readonly int s_TintColor3 = Shader.PropertyToID("_TintColor3");

    public Color Color1;
    public Color Color2;
    public Color Color3;

    private class Baker : Baker<TintColorAuthoring>
    {
        public override void Bake(TintColorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            Color linear;
            linear = authoring.Color1.linear;
            AddComponent(entity, new TintColor1 { Value = new float4(linear.r, linear.g, linear.b, linear.a) });
            linear = authoring.Color2.linear;
            AddComponent(entity, new TintColor2 { Value = new float4(linear.r, linear.g, linear.b, linear.a) });
            linear = authoring.Color3.linear;
            AddComponent(entity, new TintColor3 { Value = new float4(linear.r, linear.g, linear.b, linear.a) });
        }
    }

    void OnValidate()
    {
        var block = new MaterialPropertyBlock();
        block.SetColor(s_TintColor1, Color1);
        block.SetColor(s_TintColor2, Color2);
        block.SetColor(s_TintColor3, Color3);

        var renderer = GetComponentInChildren<Renderer>();
        renderer.SetPropertyBlock(block);
    }
}

#endif
