﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace NetGore.Graphics.GUI
{
    /// <summary>
    /// Delegate for handling events from the <see cref="GameControl"/>.
    /// </summary>
    /// <param name="gameControl">The <see cref="GameControl"/> the event came from.</param>
    public delegate void GameControlEventHandler(GameControl gameControl);

    /// <summary>
    /// Handles checking for the state needed by a game control.
    /// </summary>
    public class GameControl
    {
        int _lastInvokeTime;
        int _delay;
        bool _isEnabled = true;
        IEnumerable<Keys> _keysDown;
        IEnumerable<Keys> _keysUp;
        string _name;
        IEnumerable<Keys> _newKeysDown;
        IEnumerable<Keys> _newKeysUp;

        /// <summary>
        /// Notifies listeners that this <see cref="GameControl"/>'s key state requirements match the current key
        /// state and that it is ready to be handled.
        /// </summary>
        public event GameControlEventHandler OnInvoke;

        /// <summary>
        /// Gets or sets a Func containing any additional requirements for this <see cref="GameControl"/> to be invoked.
        /// If value must return true for <see cref="OnInvoke"/> to be raised. If this value is false,
        /// <see cref="OnInvoke"/> will not be raised and the internal delay counter wil not be altered. If this value
        /// is null, it will be treated the same as if it always returned true.
        /// </summary>
        public Func<bool> AdditionalRequirements { get; set; }

        /// <summary>
        /// Gets or sets the minimum delay in milliseconds between invokes of this <see cref="GameControl"/>. If the
        /// delay is greater than 0, then <see cref="OnInvoke"/> will never be raised until the specified amount of
        /// time has elapsed, no matter the key state. This value must be greater than or equal to zero.
        /// </summary>
        public int Delay
        {
            get { return _delay; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                _delay = value;
            }
        }

        /// <summary>
        /// Gets or sets if the <see cref="GameControl"/> is enabled. If false, <see cref="OnInvoke"/> will never
        /// be raised.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        /// <summary>
        /// Gets or sets an IEnumerable of the <see cref="Keys"/> required to be down for this <see cref="GameControl"/>
        /// to be invoked.
        /// </summary>
        public IEnumerable<Keys> KeysDown
        {
            get { return _keysDown; }
            set { _keysDown = value ?? _emptyKeys; }
        }

        /// <summary>
        /// Gets or sets an IEnumerable of the <see cref="Keys"/> required to be up for this <see cref="GameControl"/>
        /// to be invoked.
        /// </summary>
        public IEnumerable<Keys> KeysUp
        {
            get { return _keysUp; }
            set { _keysUp = value ?? _emptyKeys; }
        }

        /// <summary>
        /// Gets or sets the optional name of this <see cref="GameControl"/>. This name is intended primarily for
        /// debugging purposes. Cannot be null.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets an IEnumerable of the <see cref="Keys"/> required to be pressed for this <see cref="GameControl"/>
        /// to be invoked. These keys must have been up the last frame, but pressed this frame. If they were already
        /// pressed, they do not count.
        /// </summary>
        public IEnumerable<Keys> NewKeysDown
        {
            get { return _newKeysDown; }
            set { _newKeysDown = value ?? _emptyKeys; }
        }

        /// <summary>
        /// Gets or sets an IEnumerable of the <see cref="Keys"/> required to be raised for this <see cref="GameControl"/>
        /// to be invoked. These keys must have been pressed the last frame, but up this frame. If they were already
        /// up, they do not count.
        /// </summary>
        public IEnumerable<Keys> NewKeysUp
        {
            get { return _newKeysUp; }
            set { _newKeysUp = value ?? _emptyKeys; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keysDown">The keys required to be down.</param>
        public GameControl(string name, IEnumerable<Keys> keysDown) : this(name, keysDown, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keysDown">The keys required to be down.</param>
        /// <param name="keysUp">The keys required to be up.</param>
        public GameControl(string name, IEnumerable<Keys> keysDown, IEnumerable<Keys> keysUp) : this(name, keysDown, keysUp, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keysDown">The keys required to be down.</param>
        /// <param name="keysUp">The keys required to be up.</param>
        /// <param name="newKeysDown">The keys required to be down this frame, and up last frame.</param>
        public GameControl(string name, IEnumerable<Keys> keysDown, IEnumerable<Keys> keysUp, IEnumerable<Keys> newKeysDown)
            : this(name, keysDown, keysUp, newKeysDown, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keysDown">The keys required to be down.</param>
        /// <param name="keysUp">The keys required to be up.</param>
        /// <param name="newKeysDown">The keys required to be down this frame, and up last frame.</param>
        /// <param name="newKeysUp">The keys required to be up this frame, and down last frame.</param>
        public GameControl(string name, IEnumerable<Keys> keysDown, IEnumerable<Keys> keysUp, IEnumerable<Keys> newKeysDown,
                           IEnumerable<Keys> newKeysUp)
        {
            Name = name;
            NewKeysDown = newKeysDown;
            NewKeysUp = newKeysUp;
            KeysUp = keysUp;
            KeysDown = keysDown;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keyDown">The key required to be down.</param>
        public GameControl(string name, Keys? keyDown)
            : this(name, keyDown, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keyDown">The key required to be down.</param>
        /// <param name="keyUp">The key required to be up.</param>
        public GameControl(string name, Keys? keyDown, Keys? keyUp)
            : this(name, keyDown, keyUp, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keyDown">The key required to be down.</param>
        /// <param name="keyUp">The key required to be up.</param>
        /// <param name="newKeyDown">The key required to be down this frame, and up last frame.</param>
        public GameControl(string name, Keys? keyDown, Keys? keyUp, Keys? newKeyDown) : this(name, keyDown, keyUp, newKeyDown, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        /// <param name="name">The optional name of the control. Cannot be null.</param>
        /// <param name="keyDown">The key required to be down.</param>
        /// <param name="keyUp">The key required to be up.</param>
        /// <param name="newKeyDown">The key required to be down this frame, and up last frame.</param>
        /// <param name="newKeyUp">The key required to be up this frame, and down last frame.</param>
        public GameControl(string name, Keys? keyDown, Keys? keyUp, Keys? newKeyDown,
                           Keys? newKeyUp)
        {
            Name = name;
            KeysDown = keyDown.HasValue ? new Keys[] { keyDown.Value } : _emptyKeys;
            KeysUp = keyUp.HasValue ? new Keys[] { keyUp.Value } : _emptyKeys;
            NewKeysDown = newKeyDown.HasValue ? new Keys[] { newKeyDown.Value } : _emptyKeys;
            NewKeysUp = newKeyUp.HasValue ? new Keys[] { newKeyUp.Value } : _emptyKeys;
        }

        /// <summary>
        /// An empty IEnumerable of <see cref="Keys"/>.
        /// </summary>
        static readonly IEnumerable<Keys> _emptyKeys = Enumerable.Empty<Keys>();

        /// <summary>
        /// Updates the <see cref="GameControl"/>.
        /// </summary>
        /// <param name="gui">The <see cref="GUIManagerBase"/> to get the key states from.</param>
        /// <param name="currentTime">The current time in milliseconds.</param>
        public void Update(GUIManagerBase gui, int currentTime)
        {
            // Ensure the object is enabled and there is an invoke handler before checking the key states
            if (!IsEnabled || OnInvoke == null)
                return;
            
            // Check that enough time has elapsed since the last invoke
            if (currentTime - _lastInvokeTime < Delay)
                return;

            // Check the key states
            KeyboardState ks = gui.KeyboardState;

            foreach (Keys key in KeysUp)
            {
                if (!ks.IsKeyUp(key))
                    return;
            }

            foreach (Keys key in KeysDown)
            {
                if (!ks.IsKeyDown(key))
                    return;
            }

            foreach (Keys key in NewKeysDown)
            {
                if (!gui.NewKeysDown.Contains(key))
                    return;
            }

            foreach (Keys key in NewKeysUp)
            {
                if (!gui.NewKeysUp.Contains(key))
                    return;
            }
            
            // Check additional requirements
            if (AdditionalRequirements == null || !AdditionalRequirements())
                return;

            // All keys are in the needed state, so invoke the handler
            OnInvoke(this);
            _lastInvokeTime = currentTime;
        }
    }
}