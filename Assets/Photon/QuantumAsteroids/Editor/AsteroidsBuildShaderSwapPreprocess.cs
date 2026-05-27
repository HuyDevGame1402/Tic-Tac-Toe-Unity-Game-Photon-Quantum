namespace Quantum.Asteroids
{
  using Quantum;
  using UnityEditor.Build;
  using UnityEditor.Build.Reporting;

  /// <summary>
  /// Handles simple shader swaps to ensure correct shaders across render pipelines.
  /// Not intended for use in production code, which will only use a single render
  /// pipeline. For builds the shaders must be setup before the build, otherwise the
  /// shaders will not be included and the assets will not render.
  /// </summary>
  public class AsteroidsBuildShaderSwapPreprocess : IPreprocessBuildWithReport
  {
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
      var query = new AssetObjectQuery(typeof(EntityView));

      var quantumUnityDB = QuantumUnityDB.Global;

      var guids = quantumUnityDB.FindAssetGuids(query);

      foreach (var guid in guids)
      {
        var entityView = (EntityView)quantumUnityDB.GetAsset(guid);

        AsteroidsShaderSetup.FixShaders(entityView.Prefab);
      }
    }
  }
}
