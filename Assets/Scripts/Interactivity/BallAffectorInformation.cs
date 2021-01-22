using UnityEngine;

namespace Interactivity
{
    [CreateAssetMenu(fileName = "New Preset", menuName = "Information/BallAffector", order = 0)]
    public class BallAffectorInformation : ScriptableObject
    {
        public enum ScaleType
        {
            ScaleDown, ScaleUp
        }

        public ScaleType scaleType;
        public float scaleRate;
    }
}