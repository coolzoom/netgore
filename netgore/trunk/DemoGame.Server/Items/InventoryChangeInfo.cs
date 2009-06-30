using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace DemoGame.Server
{
    struct InventoryChangeInfo
    {
        readonly ItemEntity _item;
        readonly ItemValueTracker _oldValues;
        readonly byte _slot;

        /// <summary>
        /// Gets the current item that has changed. If null, this means that the item has changed
        /// to null (ie been removed from the inventory).
        /// </summary>
        public ItemEntity Item
        {
            get { return _item; }
        }

        /// <summary>
        /// Gets the previous values of the item. If null, this means the item the item used to be null.
        /// </summary>
        public ItemValueTracker OldValues
        {
            get { return _oldValues; }
        }

        public byte Slot
        {
            get { return _slot; }
        }

        public InventoryChangeInfo(ItemEntity item, ItemValueTracker oldValues, byte slot)
        {
            _slot = slot;
            _item = item;

            if (oldValues == null || oldValues.IsNull)
                _oldValues = null;
            else
                _oldValues = oldValues;

            Debug.Assert(_item != null || _oldValues != null,
                         "item and oldValues can not both be null. " + "This would imply that the item changed from null to null.");
        }
    }
}