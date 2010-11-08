#region Copyright & License

//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System;
using System.Linq;
using log4net.Core;

namespace log4net.Filter
{
    /// <summary>
    /// This is a simple filter based on <see cref="Level"/> matching.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The filter admits three options <see cref="LevelMin"/> and <see cref="LevelMax"/>
    /// that determine the range of priorities that are matched, and
    /// <see cref="AcceptOnMatch"/>. If there is a match between the range
    /// of priorities and the <see cref="Level"/> of the <see cref="LoggingEvent"/>, then the 
    /// <see cref="Decide"/> method returns <see cref="FilterDecision.Accept"/> in case the <see cref="AcceptOnMatch"/> 
    /// option value is set to <c>true</c>, if it is <c>false</c>
    /// then <see cref="FilterDecision.Deny"/> is returned. If there is no match, <see cref="FilterDecision.Deny"/> is returned.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public class LevelRangeFilter : FilterSkeleton
    {
        #region Member Variables

        /// <summary>
        /// Flag to indicate the behavior when matching a <see cref="Level"/>
        /// </summary>
        bool m_acceptOnMatch = true;

        /// <summary>
        /// the maximum <see cref="Level"/> value to match
        /// </summary>
        Level m_levelMax;

        /// <summary>
        /// the minimum <see cref="Level"/> value to match
        /// </summary>
        Level m_levelMin;

        #endregion

        #region Constructors

        #endregion

        /// <summary>
        /// <see cref="FilterDecision.Accept"/> when matching <see cref="LevelMin"/> and <see cref="LevelMax"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="AcceptOnMatch"/> property is a flag that determines
        /// the behavior when a matching <see cref="Level"/> is found. If the
        /// flag is set to true then the filter will <see cref="FilterDecision.Accept"/> the 
        /// logging event, otherwise it will <see cref="FilterDecision.Neutral"/> the event.
        /// </para>
        /// <para>
        /// The default is <c>true</c> i.e. to <see cref="FilterDecision.Accept"/> the event.
        /// </para>
        /// </remarks>
        public bool AcceptOnMatch
        {
            get { return m_acceptOnMatch; }
            set { m_acceptOnMatch = value; }
        }

        /// <summary>
        /// Sets the maximum matched <see cref="Level"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The maximum level that this filter will attempt to match against the 
        /// <see cref="LoggingEvent"/> level. If a match is found then
        /// the result depends on the value of <see cref="AcceptOnMatch"/>.
        /// </para>
        /// </remarks>
        public Level LevelMax
        {
            get { return m_levelMax; }
            set { m_levelMax = value; }
        }

        /// <summary>
        /// Set the minimum matched <see cref="Level"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The minimum level that this filter will attempt to match against the 
        /// <see cref="LoggingEvent"/> level. If a match is found then
        /// the result depends on the value of <see cref="AcceptOnMatch"/>.
        /// </para>
        /// </remarks>
        public Level LevelMin
        {
            get { return m_levelMin; }
            set { m_levelMin = value; }
        }

        #region Override implementation of FilterSkeleton

        /// <summary>
        /// Check if the event should be logged.
        /// </summary>
        /// <param name="loggingEvent">the logging event to check</param>
        /// <returns>see remarks</returns>
        /// <remarks>
        /// <para>
        /// If the <see cref="Level"/> of the logging event is outside the range
        /// matched by this filter then <see cref="FilterDecision.Deny"/>
        /// is returned. If the <see cref="Level"/> is matched then the value of
        /// <see cref="AcceptOnMatch"/> is checked. If it is true then
        /// <see cref="FilterDecision.Accept"/> is returned, otherwise
        /// <see cref="FilterDecision.Neutral"/> is returned.
        /// </para>
        /// </remarks>
        public override FilterDecision Decide(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
                throw new ArgumentNullException("loggingEvent");

            if (m_levelMin != null)
            {
                if (loggingEvent.Level < m_levelMin)
                {
                    // level of event is less than minimum
                    return FilterDecision.Deny;
                }
            }

            if (m_levelMax != null)
            {
                if (loggingEvent.Level > m_levelMax)
                {
                    // level of event is greater than maximum
                    return FilterDecision.Deny;
                }
            }

            if (m_acceptOnMatch)
            {
                // this filter set up to bypass later filters and always return
                // accept if level in range
                return FilterDecision.Accept;
            }
            else
            {
                // event is ok for this filter; allow later filters to have a look..
                return FilterDecision.Neutral;
            }
        }

        #endregion
    }
}