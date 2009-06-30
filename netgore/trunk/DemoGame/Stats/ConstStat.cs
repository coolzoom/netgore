using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using NetGore.IO;

namespace DemoGame
{
    public class ConstStat<T> : IStat where T : IStatValueType, new()
    {
        const string _readonlyErrorMessage = "Can not set the Value for a ConstStat.";

        readonly StatType _statType;
        readonly T _value;

        public ConstStat(StatType statType, T value)
        {
            _statType = statType;
            _value = value;
        }

        #region IStat Members

        public int Value
        {
            get { return _value.GetValue(); }
            set { throw new MethodAccessException(_readonlyErrorMessage); }
        }

        public void Read(BitStream bitStream)
        {
            throw new MethodAccessException(_readonlyErrorMessage);
        }

        public void Read(IDataRecord dataRecord, int ordinal)
        {
            throw new MethodAccessException(_readonlyErrorMessage);
        }

        public void Read(IDataReader dataReader, string name)
        {
            throw new MethodAccessException(_readonlyErrorMessage);
        }

        public event StatChangeHandler OnChange
        {
            // FUTURE: Put a Debug.Fail() on this OnChange to see who, if anyone, is listening to it
            add { }
            remove { }
        }

        public void Write(BitStream bitStream)
        {
            _value.Write(bitStream);
        }

        public IStatValueType DeepCopyValueType()
        {
            return _value.DeepCopy();
        }

        public IStat DeepCopy()
        {
            return new ConstStat<T>(_statType, _value);
        }

        public bool CanWrite
        {
            get { return false; }
        }

        public StatType StatType
        {
            get { return _statType; }
        }

        #endregion
    }
}