using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS.Samples
{
    public class ShakeDemoBall : MonoBehaviour
    {
        public float m_Speed = 5f;

        private Rigidbody m_RigidBody = null;

        private void Awake()
        {
            m_RigidBody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            Vector3 pos = m_RigidBody.position;
            pos.y = 0;

            Vector3 forceDirection = Vector3.Cross(pos.normalized, Vector3.up);

            m_RigidBody.velocity = forceDirection * m_Speed;

            //m_RigidBody.AddForce(forceDirection * m_Speed, ForceMode.VelocityChange);
        }
    }
}