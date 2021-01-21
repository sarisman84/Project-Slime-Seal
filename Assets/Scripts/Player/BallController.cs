using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Input))]
    public class BallController : MonoBehaviour
    {
        // Start is called before the first frame update

        private Input m_InputComponent;
        private Rigidbody m_Rigidbody;

        public float movementSpeed = 4f;

        private float TrueSpeed => movementSpeed * 100f;
        
        
        void Awake()
        {
            m_InputComponent = GetComponent<Input>();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            m_Rigidbody.AddForce(m_InputComponent.GetInputMovementRaw(AxisType.Axis3D) * (TrueSpeed * Time.fixedDeltaTime));
        }
    }
}
