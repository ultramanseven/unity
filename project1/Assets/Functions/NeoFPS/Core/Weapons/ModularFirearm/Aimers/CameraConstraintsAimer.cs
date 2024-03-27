using System.Collections;
using UnityEngine;
using NeoFPS.Constants;
using NeoSaveGames.Serialization;

namespace NeoFPS.ModularFirearms
{
    [HelpURL("https://docs.neofps.com/manual/weaponsref-mb-cameraconstraintsaimer.html")]
    public class CameraConstraintsAimer : OffsetBaseAimer, IFirstPersonCameraPositionConstraint, IFirstPersonCameraRotationConstraint
    {
        [SerializeField, Range(0f, 1f), Tooltip("The strength of the rotation component of the camera constraints. Setting this to zero means only the position will be synced")]
        private float m_RotationStrength = 1f;

        [SerializeField, Tooltip("A priority score for this aimer. The highest scoring active constraint will be the one that controls the camera position")]
        private int m_ConstraintsPriority = 100;

        private Transform m_RootTransform = null;
        private FirstPersonCameraTransformConstraints m_CameraConstraints = null;

        public float positionStrength
        {
            get { return 1f; }
        }

        public float rotationStrength
        {
            get { return m_RotationStrength; }
        }

        public UnityEngine.Object owner
        {
            get { return this; }
        }

        public bool positionConstraintActive
        {
            get;
            set;
        }

        public bool rotationConstraintActive
        {
            get;
            set;
        }

        protected void OnValidate()
        {
            if (m_ConstraintsPriority < 1)
                m_ConstraintsPriority = 1;
        }

        public Vector3 GetConstraintPosition(Transform relativeTo)
        {
            return relativeTo.InverseTransformPoint(m_RootTransform.position + m_RootTransform.rotation * Quaternion.Inverse(poseRotation) * -posePosition);
        }

        public Quaternion GetConstraintRotation(Transform relativeTo)
        {
            return Quaternion.Inverse(relativeTo.rotation) * (m_RootTransform.rotation * Quaternion.Inverse(poseRotation));
        }

        void SetConstraints(float blend)
        {
            // Reset the aim constraints
            if (m_CameraConstraints != null)
            {
                m_CameraConstraints.AddPositionConstraint(this, m_ConstraintsPriority, blend);
                if (m_RotationStrength > 0.001f)
                    m_CameraConstraints.AddRotationConstraint(this, m_ConstraintsPriority, blend);
            }

            // Set the fov
            if (firearm.wielder != null)
                firearm.wielder.fpCamera.SetFov(fovMultiplier, inputMultiplier, blend);
        }

        void ResetConstraints(float blend)
        {
            // Reset the aim constraints
            if (m_CameraConstraints != null)
            {
                m_CameraConstraints.RemovePositionConstraint(this, blend);
                if (m_RotationStrength > 0.001f)
                    m_CameraConstraints.RemoveRotationConstraint(this, blend);
            }

            // Reset the fov
            if (firearm.wielder != null)
                firearm.wielder.fpCamera.ResetFov(blend);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (firearm != null)
            {
                m_RootTransform = firearm.transform;

                // Get the camera constraints
                if (firearm.wielder != null)
                    m_CameraConstraints = firearm.wielder.GetComponentInChildren<FirstPersonCameraTransformConstraints>();
            }
            else
                m_RootTransform = transform;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Reset the aim constraints
            if (isAiming)
                ResetConstraints(aimDownDuration);

            m_CameraConstraints = null;
        }

        protected override void AimInternal()
        {
            base.AimInternal();

            // Set the aim constraints
            SetConstraints(aimUpDuration);
        }

        protected override void StopAimInternal(bool instant)
        {
            base.StopAimInternal(instant);

            // Reset the aim constraints
            if (isAiming)
                ResetConstraints(aimDownDuration);
        }

        public override void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            base.ReadProperties(reader, nsgo);

            if (isAiming)
            {
                // Set the camera aim
                SetConstraints(0f);
            }
            else
            {
                // Reset the aim constraints
                ResetConstraints(0f);
            }
        }
    }
}