using UnityEngine;

namespace InfestorFix
{
    public class DelayTrailRendererStart : MonoBehaviour
    {
        public float Delay = 0f;

        public TrailRenderer[] TrailRenderers = [];

        float _age = 0f;

        void Awake()
        {
            setTrailsEnabled(false);
        }

        void FixedUpdate()
        {
            _age += Time.fixedDeltaTime;
            if (_age >= Delay)
            {
                setTrailsEnabled(true);
                enabled = false;
            }
        }

        void setTrailsEnabled(bool enabled)
        {
            foreach (TrailRenderer trailRenderer in TrailRenderers)
            {
                if (trailRenderer)
                {
                    trailRenderer.Clear();
                    trailRenderer.forceRenderingOff = !enabled;
                }
            }
        }
    }
}
