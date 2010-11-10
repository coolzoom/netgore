﻿using System;

namespace NetGore.Db
{
    /// <summary>
    /// Interface for an object that contains the statistics for a single query.
    /// </summary>
    public interface IQueryStats
    {
        /// <summary>
        /// Gets the query that these stats are for.
        /// </summary>
        object Query { get; }

        /// <summary>
        /// Gets the number of times that the query has been executed.
        /// </summary>
        int TimesExecuted { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> that this query was last executed.
        /// </summary>
        DateTime TimeLastExecuted { get; }
    }
}