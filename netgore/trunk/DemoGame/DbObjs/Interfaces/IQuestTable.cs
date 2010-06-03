/********************************************************************
                   DO NOT MANUALLY EDIT THIS FILE!

This file was automatically generated using the DbClassCreator
program. The only time you should ever alter this file is if you are
using an automated code formatter. The DbClassCreator will overwrite
this file every time it is run, so all manual changes will be lost.
If there is something in this file that you wish to change, you should
be able to do it through the DbClassCreator arguments.

Make sure that you re-run the DbClassCreator every time you alter your
game's database.

For more information on the DbClassCreator, please see:
    http://www.netgore.com/wiki/dbclasscreator.html

This file was generated on (UTC): 6/2/2010 10:29:24 PM
********************************************************************/

using System;
using System.Linq;
using NetGore.Features.Quests;

namespace DemoGame.DbObjs
{
    /// <summary>
    /// Interface for a class that can be used to serialize values to the database table `quest`.
    /// </summary>
    public interface IQuestTable
    {
        /// <summary>
        /// Gets the value of the database column `id`.
        /// </summary>
        QuestID ID { get; }

        /// <summary>
        /// Gets the value of the database column `repeatable`.
        /// </summary>
        Boolean Repeatable { get; }

        /// <summary>
        /// Gets the value of the database column `reward_cash`.
        /// </summary>
        Int32 RewardCash { get; }

        /// <summary>
        /// Gets the value of the database column `reward_exp`.
        /// </summary>
        Int32 RewardExp { get; }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        IQuestTable DeepCopy();
    }
}