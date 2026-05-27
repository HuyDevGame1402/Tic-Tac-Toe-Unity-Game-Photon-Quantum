namespace Quantum
{
    using UnityEngine;

    public class ViewEntity : QuantumEntityViewComponent
    {
        [Header("Sprite")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Animation Settings")]
        [Range(0.1f, 1f)] public float appearDuration = 0.5f;
        [Range(1f, 2f)] public float overshootScale = 1.25f;
        [Range(0f, 60f)] public float startRotation = 45f;
        [Range(0f, 0.5f)] public float glowIntensity = 0.25f;

        private Vector3 startScale;
        private Vector3 targetScale;
        private Color startColor;
        private Color endColor;

        private bool hasPlayedSpawnAnim = false;
        private float timer;

        private void Awake()
        {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();

            // Khởi tạo giá trị ban đầu
            transform.localScale = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, 0, startRotation);

            startScale = Vector3.zero;
            targetScale = Vector3.one;

            endColor = spriteRenderer.color;
            startColor = new Color(endColor.r, endColor.g, endColor.b, 0f);
            spriteRenderer.color = startColor;
        }

        private void Start()
        {
            if (!hasPlayedSpawnAnim)
            {
                hasPlayedSpawnAnim = true;
                timer = 0f;
            }
        }

        private void Update()
        {
            if (!hasPlayedSpawnAnim) return;

            if (timer < appearDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / appearDuration);

                // Easing mềm mại (OutBack)
                float easedT = EaseOutBack(t);

                // Scale bật ra
                transform.localScale = Vector3.LerpUnclamped(startScale, targetScale * overshootScale, easedT);

                // Xoay giảm dần về 0°
                float currentRot = Mathf.Lerp(startRotation, 0f, EaseOutCubic(t));
                transform.rotation = Quaternion.Euler(0, 0, currentRot);

                // Fade màu
                spriteRenderer.color = Color.Lerp(startColor, endColor, EaseOutQuad(t));

                // Nháy sáng nhẹ khi gần kết thúc
                if (t > 0.8f)
                {
                    float glow = Mathf.Sin((t - 0.8f) * Mathf.PI * 4f) * glowIntensity;
                    spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, glow);
                }
            }
            else
            {
                // Hoàn thiện anim
                transform.localScale = targetScale;
                transform.rotation = Quaternion.identity;
                spriteRenderer.color = endColor;
                hasPlayedSpawnAnim = false; // để không chạy lại
            }
        }

        #region Easing Functions
        private float EaseOutBack(float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3) + c1 * Mathf.Pow(t - 1f, 2);
        }

        private float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);

        private float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
        #endregion
    }
}
