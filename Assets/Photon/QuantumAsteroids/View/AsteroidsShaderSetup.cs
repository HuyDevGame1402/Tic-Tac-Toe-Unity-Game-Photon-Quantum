namespace Quantum.Asteroids
{
  using UnityEngine;
  using UnityEngine.Rendering;
  using Quantum;

  /// <summary>
  /// The <c>AsteroidsShaderSetup</c> class handles simple shader swaps to ensure correct shaders
  /// across render pipelines. Not intended for use in production code, which will only use a
  /// single render pipeline.
  /// </summary>
  public class AsteroidsShaderSetup : QuantumEntityViewComponent
  {
    public override void OnInitialize()
    {
      FixShaders(gameObject);
    }

    /// <summary>
    /// If a Scriptable Render Pipeline (SRP) is present swap over to the default shaders for that pipeline.
    /// Not intended for use in production code, which will only use a single pipeline.
    /// </summary>
    public static void FixShaders(GameObject gameObject)
    {
      var renderPipeline = GraphicsSettings.defaultRenderPipeline;

      if (renderPipeline == null)
        return;

      if (renderPipeline.defaultMaterial == null)
        return;

      var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);

      foreach (var meshRenderer in meshRenderers)
      {
        var materials = meshRenderer.sharedMaterials;

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = renderPipeline.defaultMaterial;
        }

        meshRenderer.sharedMaterials = materials;
      }
    }
  }
}
