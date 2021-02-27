using System;
using UnityEngine;

namespace Extensions
{
    public static class RigidbodyExtensions
    {
        public static void AddClampedForce(this Rigidbody rigidbody, Vector3 force, ref float velocity,
            float clampedVelocity,
            float clampRate = 0.15f, ForceMode forceMode = ForceMode.Force,float downwardClampedVelocity = 25f, float downwardAccelerationRate = 0.5f, float downwardClampRate = 0.15f )
        {
            bool areWeGoingDown = rigidbody.velocity.y < -0.1f;

            if (areWeGoingDown)
            {
                Vector3 lVelocity = rigidbody.velocity;
                lVelocity = Vector3.Lerp(lVelocity,
                    Vector3.ClampMagnitude(lVelocity.normalized * velocity, downwardClampedVelocity), downwardClampRate);
                rigidbody.velocity = lVelocity;
                
                velocity += downwardAccelerationRate;
            }


            rigidbody.AddForce(areWeGoingDown ? force / 2f : force, forceMode);
            if (!areWeGoingDown)
            {
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity,
                    Vector3.ClampMagnitude(rigidbody.velocity, clampedVelocity), clampRate);
                velocity = 1;
            }
        }
    }
}