using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS.ModularFirearms
{
    [DisallowMultipleComponent]
    [HelpURL("https://docs.neofps.com/manual/weaponsref-mb-modularfirearmattachment.html")]
    public class StandardModularFirearmAttachment : ModularFirearmAttachment
    {
        [SerializeField, Tooltip("(Optional) If this is set then the character must have an instance of this item (or an item with matching inventory ID) in their inventory before attaching to their weapon.")]
        private FpsInventoryItemBase m_RequiredInventoryItem = null;

        public override bool CheckRequirements(ModularFirearmAttachmentSystem attachmentSystem)
        {
            // Passes if there's no inventory requirement
            if (m_RequiredInventoryItem == null)
                return true;

            // Get the wielder's inventory (passes if wielder doesn't have one)
            var inventory = attachmentSystem.firearm.wielder?.GetComponent<IInventory>();
            if (inventory == null)
                return true;

            // Check if inventory contains the item
            var item = inventory.GetItem(m_RequiredInventoryItem.itemIdentifier);
            return item != null && item.quantity > 0;
        }
    }
}