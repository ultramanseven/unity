using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS
{
    public class AnimatedWeaponInspect : MonoBehaviour
    { 
        [Header("Inspect")]

        [SerializeField, AnimatorParameterKey(AnimatorControllerParameterType.Bool, true, false), Tooltip("The name of the bool parameter to set on the weapon's animator while inspecting.")]
        private string m_InspectKey = "Inspect";
        [SerializeField, Tooltip("How long after releasing the inspect key, should the weapon be able to function again.")]
        private float m_ReleaseDelay = 0.25f;

        [Header ("Poses")]

        [SerializeField, Min(1), Tooltip("How many inspect poses does the weapon have.")]
        private int m_NumPoses = 1;
        [SerializeField, AnimatorParameterKey(AnimatorControllerParameterType.Int, true, false), Tooltip("The name of the integer parameter to set on the weapon's animator that controls the animation pose.")]
        private string m_PoseKey = "PoseIndex";

        private IWieldable m_Wieldable = null;
        private IWieldableAnimationHandler m_AnimationHandler = null;
        private int m_InspectHash = 0;
        private int m_PoseHash = 0;
        private int m_PoseIndex = 0;
        private bool m_Inspecting = true;
        private Coroutine m_InspectCoroutine = null;

        public bool inspecting
        {
            get { return m_Inspecting; }
            set
            {
                if (m_Inspecting != value)
                {
                    m_Inspecting = value;

                    // Reset pose parameter
                    if (m_PoseHash != 0)
                    {
                        m_PoseIndex = 0;
                        m_AnimationHandler.SetInteger(m_PoseHash, m_PoseIndex);
                    }

                    // Set inspect parameter
                    if (m_InspectHash != 0)
                        m_AnimationHandler.SetBool(m_InspectHash, m_Inspecting);

                    if (m_Inspecting)
                    {
                        // Block wieldable (can't shoot / reload, etc while inspecting)
                        if (m_Wieldable != null && m_InspectCoroutine == null)
                            m_InspectCoroutine = StartCoroutine(InspectCoroutine());

                        OnStartInspecting();
                    }
                    else
                    {
                        OnStopInspecting();
                    }
                }
            }
        }

        public virtual bool toggle
        {
            get { return false; }
        }

        public int pose
        {
            get { return m_PoseIndex; }
            set
            {
                // Clamp index
                int clamped = Mathf.Clamp(value, 0, m_NumPoses - 1);
                if (m_PoseIndex != clamped)
                {
                    // Set pose parameter
                    m_PoseIndex = clamped;
                    m_AnimationHandler.SetInteger(m_PoseHash, m_PoseIndex);
                }
            }
        }

        public int numPoses
        {
            get { return m_NumPoses; }
        }

        protected virtual void Start()
        {
            m_Wieldable = GetComponentInParent<IWieldable>();
            m_AnimationHandler = m_Wieldable.animationHandler;
            if (!string.IsNullOrWhiteSpace(m_InspectKey))
                m_InspectHash = Animator.StringToHash(m_InspectKey);
            if (m_NumPoses > 1 && !string.IsNullOrWhiteSpace(m_PoseKey))
                m_PoseHash = Animator.StringToHash(m_PoseKey);
            inspecting = false;
        }
        
        public void CyclePose()
        {
            if (m_PoseHash != 0)
            {
                // Cycle index
                if (++m_PoseIndex == m_NumPoses)
                    m_PoseIndex = 0;

                // Set pose parameter
                m_AnimationHandler.SetInteger(m_PoseHash, m_PoseIndex);
            }
        }

        private void OnDisable()
        {
            if (m_InspectCoroutine != null)
            {
                StopCoroutine(m_InspectCoroutine);
                m_InspectCoroutine = null;
                m_Wieldable.RemoveBlocker(this);
            }
        }

        protected virtual void OnStartInspecting() { }
        protected virtual void OnStopInspecting() { }

        IEnumerator InspectCoroutine()
        {
            m_Wieldable.AddBlocker(this);

            while (inspecting)
                yield return null;

            float delay = m_ReleaseDelay;
            while (delay > 0f)
            {
                yield return null;
                delay -= Time.deltaTime;
            }

            m_Wieldable.RemoveBlocker(this);

            m_InspectCoroutine = null;
        }
    }
}