﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using DemoGame.Extensions;
using NetGore.Db;

namespace DemoGame.Server
{
    public abstract class UserQueryBase : DbQueryNonReader<UserQueryValues>
    {
        public const string UsersTableName = "users";
        static readonly IEnumerable<string> _otherFields = new string[] { "@body", "@guid", "@map", "@name", "@x", "@y" };

        static readonly IEnumerable<string> _statFields =
            UserStats.DatabaseStats.Select(statType => "@" + statType.GetDatabaseField());

        static string _queryFieldsStr;

        /// <summary>
        /// Gets an IEnumerable of the name of each field, except for the guid, without the parameter prefix.
        /// </summary>
        protected static IEnumerable<string> FieldNames
        {
            get { return GetAllFields().Where(x => x != "@guid").Select(x => GetParameterNameWithoutPrefix(x)); }
        }

        /// <summary>
        /// Gets the static cache of the GetValuesQuery(FieldNames).
        /// </summary>
        protected static string QueryFieldsStr
        {
            get
            {
                if (_queryFieldsStr == null)
                    _queryFieldsStr = GetValuesQuery(FieldNames);

                return _queryFieldsStr;
            }
        }

        protected UserQueryBase(DbConnectionPool connectionPool, string commandText) : base(connectionPool, commandText)
        {
        }

        static IEnumerable<string> GetAllFields()
        {
            return _statFields.Concat(_otherFields);
        }

        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters(GetAllFields());
        }

        protected override void SetParameters(DbParameterValues p, UserQueryValues user)
        {
            p["@guid"] = user.Guid;
            p["@map"] = user.MapIndex;
            p["@x"] = user.X;
            p["@y"] = user.Y;
            p["@body"] = user.BodyIndex;
            p["@name"] = user.Name;

            foreach (IStat stat in user.Stats)
            {
                string fieldName = stat.StatType.GetDatabaseField();
                p["@" + fieldName] = stat.Value;
            }
        }
    }
}