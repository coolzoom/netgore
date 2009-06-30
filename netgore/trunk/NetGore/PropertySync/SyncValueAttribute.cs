using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NetGore
{
    /// <summary>
    /// Attribute for a Property that will have its value synchronized by a PropertySyncBase.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SyncValueAttribute : Attribute
    {
        readonly string _customName;

        /// <summary>
        /// Gets the custom name to be used for when synchronizing. Can be null, and is null by default.
        /// When null, the name of the Property will be used instead.
        /// </summary>
        public string CustomName
        {
            get { return _customName; }
        }

        /// <summary>
        /// Gets or sets if this SyncValueAttribute will be skipped when synchronizing values from
        /// the Server to the Client. Default is true.
        /// </summary>
        public bool SkipNetworkSync { get; set; }

        /// <summary>
        /// SyncValueAttribute constructor.
        /// </summary>
        public SyncValueAttribute()
        {
        }

        /// <summary>
        /// SyncValueAttribute constructor.
        /// </summary>
        /// <param name="customName">The custom name to be used for when synchronizing.</param>
        public SyncValueAttribute(string customName) : this()
        {
            _customName = customName;
        }
    }
}