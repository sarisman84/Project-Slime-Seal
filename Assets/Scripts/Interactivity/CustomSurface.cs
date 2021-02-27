using System;
using Player;
using UnityEngine;

namespace Interactivity
{
    public class CustomSurface : MonoBehaviour
    {
        [Range(0, 1)] public float inputBias, physicsConstraintBias;
        public float playerMass = 1;
        public ForceMode physicsPushMode = ForceMode.Acceleration;
        public bool resetOnCollisionExit;


        private BallController m_FoundPlayer;
        private Rigidbody m_PlayerRigidbody;
        private float m_DefaultInputBias, m_DefaultPhysicsBias, m_DefaultPlayerMass;
        private ForceMode m_DefaultPhysicsPushMode;


        private void Awake()
        {
            m_FoundPlayer = FindObjectOfType<BallController>();
            m_PlayerRigidbody = m_FoundPlayer.GetComponent<Rigidbody>();
            
            m_DefaultInputBias = m_FoundPlayer.InputBias;
            m_DefaultPhysicsBias = m_FoundPlayer.InputBias;
            m_DefaultPhysicsPushMode = m_FoundPlayer.PhysicsMode;
            m_DefaultPlayerMass = m_PlayerRigidbody.mass;
        }

        public void OnCollisionEnter(Collision other)
        {
            if (other.collider.GetComponent<BallController>() != null)
            {
                m_FoundPlayer.PhysicsBias = physicsConstraintBias;
                m_FoundPlayer.PhysicsMode = physicsPushMode;
                m_FoundPlayer.InputBias = inputBias;
                m_PlayerRigidbody.mass = playerMass;
            }
        }


        public void OnCollisionExit(Collision other)
        {
            if (other.collider.GetComponent<BallController>() != null && resetOnCollisionExit)
            {
                m_FoundPlayer.PhysicsBias = m_DefaultPhysicsBias;
                m_FoundPlayer.PhysicsMode = m_DefaultPhysicsPushMode;
                m_FoundPlayer.InputBias = m_DefaultInputBias;
                m_PlayerRigidbody.mass = m_DefaultPlayerMass;
            }
        }
    }
}