﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS.ModularFirearms
{
    [RequireComponent(typeof (InteractiveObject))]
    public class ModularFirearmAttachmentPickup : MonoBehaviour
    {
        [SerializeField, Tooltip("")]
        private ModularFirearmAttachment m_Attachment = null;

        [SerializeField, Tooltip("")]
        private string m_Socket = "Socket Name";

        [SerializeField, Tooltip("")]
        private ConsumeResult m_ConsumeResult = ConsumeResult.Destroy;

        private InteractiveObject m_InteractiveObject = null;

        enum ConsumeResult
        {
            Destroy,
            Disable,
            Nothing
        }

        protected void Start()
        {
            if (m_InteractiveObject == null)
                m_InteractiveObject = GetComponent<InteractiveObject>();

            if (m_InteractiveObject is InteractivePickup pickup)
                pickup.onPickedUp += OnPickedUp;
            else
                m_InteractiveObject.onUsed += OnUsed;
        }

        private void OnPickedUp(IInventory inventory, IInventoryItem item)
        {
            var currentWeapon = (inventory as IQuickSlots).selected;
            if (currentWeapon != null)
            {
                var attachmentSystem = currentWeapon.GetComponent<ModularFirearmAttachmentSystem>();
                if (attachmentSystem != null)
                    attachmentSystem.TryAttach(m_Socket, m_Attachment);
            }
        }

        private void OnUsed(ICharacter character)
        {
            var currentWeapon = character.quickSlots.selected;
            if (currentWeapon != null)
            {
                var attachmentSystem = currentWeapon.GetComponent<ModularFirearmAttachmentSystem>();
                if (attachmentSystem != null && attachmentSystem.TryAttach(m_Socket, m_Attachment))
                {
                    switch (m_ConsumeResult)
                    {
                        case ConsumeResult.Destroy:
                            Destroy(gameObject);
                            break;
                        case ConsumeResult.Disable:
                            m_InteractiveObject.interactable = false;
                            enabled = false;
                            break;
                    }
                }
            }
        }
    }
}