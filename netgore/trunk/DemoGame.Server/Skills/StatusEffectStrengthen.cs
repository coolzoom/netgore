﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoGame.Server
{
    public class StatusEffectStrengthen : StatusEffectBase
    {
        StatusEffectStrengthen() : base(StatusEffectType.Strengthen, DefaultStatusEffectMergeType.Buff)
        {
        }

        public override int GetEffectTime(ushort power)
        {
            return CalculateEffectTime(2);
        }

        protected override int? InternalTryGetStatModifier(StatType statType, ushort power)
        {
            switch (statType)
            {
                case StatType.Str:
                    return 1 + (power / 5);

                default:
                    return null;
            }
        }
    }
}
