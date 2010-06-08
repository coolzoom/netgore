﻿using System;
using System.Linq;
using System.Text;
using NetGore.Audio;
using NetGore.IO;

namespace NetGore.Features.DisplayAction
{
    /// <summary>
    /// Describes how to display an action.
    /// </summary>
    public class ActionDisplay : IPersistable
    {
        const string _keyID = "ID";
        const string _keyHasSound = "HasSound";
        const string _keySoundID = "SoundID";
        const string _keyScript = "Script";
        const string _keyParticleEffect = "ParticleEffect";
        const string _keyGrhIndex = "GrhIndex";

        ActionDisplayID _id;
        SoundID? _sound;
        string _script;
        string _particleEffect;
        GrhIndex _grhIndex;

        /// <summary>
        /// Gets or sets the ID of the sound to play when showing this action.
        /// </summary>
        public SoundID? Sound { get { return _sound; } set { _sound = value; } }

        /// <summary>
        /// Gets the ID of this <see cref="ActionDisplay"/>.
        /// </summary>
        public ActionDisplayID ID { get { return _id; } }

        /// <summary>
        /// Gets or sets the <see cref="GrhIndex"/> to use. If set to <see cref="NetGore.GrhIndex.Invalid"/>, no sprite
        /// will be displayed.
        /// </summary>
        public GrhIndex GrhIndex { get { return _grhIndex; } set { _grhIndex = value; } }

        /// <summary>
        /// Gets or sets the name of the particle effect to display.
        /// </summary>
        public string ParticleEffect { get { return _particleEffect; } set { _particleEffect = value; } }

        /// <summary>
        /// Gets or sets the name of the script to use to handle how this <see cref="ActionDisplay"/>'s appearance.
        /// Scripts are identified using the <see cref="ActionDisplayScriptAttribute"/>. The script is what
        /// determines how the actual usage of the parameters is handled. Without a script, nothing can be displayed.
        /// </summary>
        public string Script { get { return _script; } set { _script = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDisplay"/> class.
        /// </summary>
        /// <param name="id">The <see cref="ActionDisplayID"/>.</param>
        public ActionDisplay(ActionDisplayID id)
        {
            _id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDisplay"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="IValueReader"/> to read the values from.</param>
        public ActionDisplay(IValueReader reader)
        {
            ((IPersistable)this).ReadState(reader);
        }

        /// <summary>
        /// Reads the state of the object from an <see cref="IValueReader"/>. Values should be read in the exact
        /// same order as they were written.
        /// </summary>
        /// <param name="reader">The <see cref="IValueReader"/> to read the values from.</param>
        void IPersistable.ReadState(IValueReader reader)
        {
            _id = reader.ReadActionDisplayID(_keyID);

            bool hasSound = reader.ReadBool(_keyHasSound);
            var soundID = reader.ReadSoundID(_keySoundID);
            _sound = (hasSound ? soundID : (SoundID?)null);

            _script = reader.ReadString(_keyScript);

            _grhIndex = reader.ReadGrhIndex(_keyGrhIndex);

            _particleEffect = reader.ReadString(_keyParticleEffect);
        }

        /// <summary>
        /// Writes the state of the object to an <see cref="IValueWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="IValueWriter"/> to write the values to.</param>
        void IPersistable.WriteState(IValueWriter writer)
        {
            writer.Write(_keyID, ID);

            writer.Write(_keyHasSound, Sound.HasValue);
            writer.Write(_keySoundID, (Sound.HasValue ? Sound.Value : new SoundID(0)));

            writer.Write(_keyScript, Script);

            writer.Write(_keyGrhIndex, GrhIndex);

            writer.Write(_keyParticleEffect, ParticleEffect);
        }

        /// <summary>
        /// Executes the <see cref="ActionDisplay"/>.
        /// </summary>
        /// <param name="map">The map that the entities are on.</param>
        /// <param name="source">The <see cref="Entity"/> this action came from. Cannot be null;</param>
        /// <param name="target">The <see cref="Entity"/> this action is targeting. Can be any value, including null or equal to the
        /// <paramref name="source"/>.</param>
        public virtual void Execute(IMap map, Entity source, Entity target)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            if (source == null)
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(Script))
                return;

            var scriptCaller = ActionDisplayScriptManager.GetHandler(Script);
            if (scriptCaller == null)
                return;

            scriptCaller(this, map, source, target);
        }
    }
}
