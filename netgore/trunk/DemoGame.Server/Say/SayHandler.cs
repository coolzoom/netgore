﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using DemoGame.Extensions;
using log4net;
using NetGore.Network;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace DemoGame.Server
{
    /// <summary>
    /// Handles processing what Users say.
    /// </summary>
    public class SayHandler
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        readonly SayCommandManager _commandManager;
        readonly Server _server;

        /// <summary>
        /// Gets the Server that the commands are coming from.
        /// </summary>
        public Server Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Gets the World to use.
        /// </summary>
        public World World
        {
            get { return Server.World; }
        }

        /// <summary>
        /// SayHandler constructor.
        /// </summary>
        /// <param name="server">Server that the commands are coming from.</param>
        public SayHandler(Server server)
        {
            _server = server;
            _commandManager = new SayCommandManager(this);
        }

        [SayCommand("Shout", true)]
        void CmdShout(string text, User user)
        {
            using (PacketWriter pw = ServerPacket.SendMessage(GameMessage.CommandShout, user.Name, text))
            {
                World.Send(pw);
            }
        }

        [SayCommand("CreateTestDamageTrap", false)]
        void CmdCreateTestDamageTrap(string text, User user)
        {
            // NOTE: This is a temporary command
            var trap = new DamageTrapEntity();
            trap.Resize(new Microsoft.Xna.Framework.Vector2(64, 64));
            trap.Teleport(user.Position);
            user.Map.AddEntity(trap);
        }

        [SayCommand("Tell", true)]
        [SayCommand("Whisper", true)]
        void CmdTell(string text, User user)
        {
            // Check for a message to tell
            if (string.IsNullOrEmpty(text))
            {
                // Invalid message
                using (PacketWriter pw = ServerPacket.SendMessage(GameMessage.CommandTellNoName))
                {
                    user.Send(pw);
                }
                return;
            }

            // Find the user to tell
            int spaceIndex = text.IndexOf(' ');
            if (spaceIndex < 0)
            {
                // No or invalid message
                using (PacketWriter pw = ServerPacket.SendMessage(GameMessage.CommandTellNoMessage))
                {
                    user.Send(pw);
                }
                return;
            }

            string targetName = text.Substring(0, spaceIndex);
            User target = World.FindUser(targetName);

            // Check if the target user is available or not
            if (target != null)
            {
                string message = text.Substring(spaceIndex + 1);

                // Message to sender ("You tell...")
                using (PacketWriter pw = ServerPacket.SendMessage(GameMessage.CommandTellSender, target.Name, message))
                {
                    user.Send(pw);
                }

                // Message to receivd ("X tells you...")
                using (PacketWriter pw = ServerPacket.SendMessage(GameMessage.CommandTellReceiver, user.Name, message))
                {
                    target.Send(pw);
                }
            }
            else
            {
                // User not found
                using (PacketWriter pw = ServerPacket.SendMessage(GameMessage.CommandTellInvalidUser, targetName))
                {
                    user.Send(pw);
                }
            }
        }

        /// <summary>
        /// Checks if a Say string is formatted to be a command.
        /// </summary>
        /// <param name="text">String to check.</param>
        /// <returns>True if a command, else false.</returns>
        static bool IsCommand(string text)
        {
            // Must be at least 2 characters long to be a command
            if (text.Length < 2)
                return false;

            // Check for a command character on the first character
            switch (text[0])
            {
                case '/':
                case '\\':
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if a string is a valid Say string.
        /// </summary>
        /// <param name="text">String to check.</param>
        /// <returns>True if a valid Say string, else false.</returns>
        static bool IsValidSayString(string text)
        {
            // Check if empty
            if (string.IsNullOrEmpty(text))
                return false;

            // Check each character in the string to make sure they are valid
            foreach (char letter in text)
            {
                int i = Convert.ToInt32(Convert.ToChar(letter));
                if (i > 126 || i < 32)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Processes a string of text.
        /// </summary>
        /// <param name="text">Text to process.</param>
        /// <param name="user">User that the text came from.</param>
        public void Process(string text, User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (log.IsInfoEnabled)
                log.InfoFormat("Processing Say string from User `{0}`: {1}", user, text);

            // Check for an invalid string
            if (!IsValidSayString(text))
            {
                const string errmsg = "Invalid Say string from User `{0}`: {1}";
                Debug.Fail(string.Format(errmsg, user, text));
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, user, text);
                return;
            }

            // Check if a command
            if (IsCommand(text))
            {
                ProcessCommand(text, user);
                return;
            }

            // Not a command, so just do a normal, to-area chat
            using (PacketWriter pw = ServerPacket.ChatSay(user.Name, user.MapCharIndex, text))
            {
                user.Map.SendToArea(user.Center, pw);
            }
        }

        /// <summary>
        /// Processes a string of text that is a command.
        /// </summary>
        /// <param name="text">Text to process.</param>
        /// <param name="user">User that the text came from.</param>
        void ProcessCommand(string text, User user)
        {
            // Split the text
            string commandName;
            string remainder;
            SplitCommandFromText(text, out commandName, out remainder);

            // Get the SayCommand for the given command name
            SayCommand command = _commandManager.GetCommand(commandName);
            if (command == null)
            {
                using (PacketWriter pw = ServerPacket.SendMessage(GameMessage.InvalidCommand))
                {
                    user.Send(pw);
                }
                return;
            }

            // Trim the remainder text
            if (remainder != null)
                remainder = remainder.Trim();

            // Call the handler
            if (!command.IsThreadSafe)
            {
                // Invoke synchronously
                command.Callback(remainder, user);
            }
            else
            {
                // Invoke asynchronously
                ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 command.Callback(remainder, user);
                                             });
            }
        }

        /// <summary>
        /// Splits a string of text into a command and remainder text.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="command">Output of the name of the command.</param>
        /// <param name="remainder">Remainder text (command's parameters), if any. Can be null or empty.</param>
        public static void SplitCommandFromText(string text, out string command, out string remainder)
        {
            // Check that the text is valid
            if (string.IsNullOrEmpty(text))
            {
                command = null;
                remainder = text;
                return;
            }

            int index = text.IndexOf(' ');

            if (index < 0)
            {
                // No space found, so just return the command
                command = text.Substring(1);
                remainder = null;
                return;
            }
            else
            {
                // Split the command and remainder apart
                command = text.Substring(1, index - 1);
                remainder = text.Substring(index + 1);
                return;
            }
        }
    }
}