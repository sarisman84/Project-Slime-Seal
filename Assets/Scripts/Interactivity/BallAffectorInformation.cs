using UnityEngine;

namespace Interactivity
{
    [CreateAssetMenu(fileName = "New Preset", menuName = "Information/BallAffector", order = 0)]
    public class BallAffectorInformation : ScriptableObject
    {
        public enum ScaleType
        {
            ScaleDown,
            ScaleUp
        }

        public ScaleType scaleType;
        [SerializeField] private float scaleRate;
        public float minSizeToGrab = 0;
        public float sizeLimit = 0;

        public float ScaleRate(SphereCollider currentSize, bool ingoreSizeLimit = false)
        {
            bool v = currentSize.radius + scaleRate > sizeLimit && !ingoreSizeLimit && sizeLimit > 0;

            if (v)
                currentSize.radius = Mathf.Clamp(currentSize.radius, 0, sizeLimit);
            return v ? 0 : scaleRate;
        }
    }
}