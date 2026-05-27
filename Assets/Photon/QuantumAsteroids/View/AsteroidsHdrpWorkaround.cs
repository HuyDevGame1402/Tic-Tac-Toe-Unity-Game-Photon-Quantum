namespace Quantum.Asteroids
{
  using UnityEngine;
#if QUANTUM_HDRP
  using UnityEngine.Rendering;
  using UnityEngine.Rendering.HighDefinition;
#endif

  /// <summary>
  /// The <c>AsteroidsHdrpWorkaround</c> HDRP does not render correctly without Fog enabled.
  /// Unity enabled it in their default scenes, but not in their default HDRP setup. Here
  /// we detect HDRP and enable Fog.
  /// If HDRP is not detected this GameObject is destroyed.
  /// Production code should not need to use this class as normally only one render pipeline
  /// is supported.
  /// </summary>
  public class AsteroidsHdrpWorkaround : MonoBehaviour
  {
    void Start()
    {
#if QUANTUM_HDRP
      // fixes overbloom in the default setup
      var volume = gameObject.AddComponent<Volume>();
      volume.isGlobal = true;
      volume.weight = 1.0f;

      var profile = volume.profile;
      if (!profile.TryGet<Fog>(out var fog))
      {
        fog = profile.Add<Fog>(false);
      }

      fog.enabled.overrideState = true;
      fog.enabled.value = true;
#else
      // this object is not needed
      Destroy(gameObject);
#endif
    }
  }
}
