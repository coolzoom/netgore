﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoGame.Server
{
    public class SkillStrengthen : SkillBase
    {
        SkillStrengthen() : base(SkillType.Strengthen)
        {
        }

        /// <summary>
        /// When overridden in the derived class, makes the <paramref name="user"/> Character use this skill.
        /// </summary>
        /// <param name="user">The Character that used this skill. Will never be null.</param>
        /// <param name="target">The optional Character that the skill was used on. Can be null if there was
        /// no targeted Character.</param>
        /// <returns>True if the skill was successfully used; otherwise false.</returns>
        protected override bool HandleUse(Character user, Character target)
        {
            if (target == null)
                target = user;

            int power = user.ModStats[StatType.Int] + user.ModStats[StatType.Bra] / 4;
            bool successful = target.StatusEffects.TryAdd(StatusEffectType.Strengthen, (ushort)power);

            return successful;
        }

        /// <summary>
        /// When overridden in the derived class, gets if this Skill requires a target to be specified for the skill
        /// to be used. If this is false, the skill will never even attempt to be used unless there is a target.
        /// </summary>
        public override bool RequiresTarget
        {
            get { return false; }
        }
    }
}
