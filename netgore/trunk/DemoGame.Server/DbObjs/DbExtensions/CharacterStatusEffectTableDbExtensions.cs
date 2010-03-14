using System;
using System.Linq;
using NetGore;
using NetGore.IO;
using System.Collections.Generic;
using System.Collections;
using NetGore.Db;
using DemoGame.DbObjs;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Contains extension methods for class CharacterStatusEffectTable that assist in performing
/// reads and writes to and from a database.
/// </summary>
public static  class CharacterStatusEffectTableDbExtensions
{
/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void CopyValues(this ICharacterStatusEffectTable source, NetGore.Db.DbParameterValues paramValues)
{
paramValues["@character_id"] = (System.Int32)source.CharacterID;
paramValues["@id"] = (System.Int32)source.ID;
paramValues["@power"] = (System.UInt16)source.Power;
paramValues["@status_effect_id"] = (System.Byte)source.StatusEffect;
paramValues["@time_left_secs"] = (System.UInt16)source.TimeLeftSecs;
}

/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. The database column's name is used to as the key, so the value
/// will not be found if any aliases are used or not all columns were selected.
/// </summary>
/// <param name="source">The object to add the extension method to.</param>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public static void ReadValues(this CharacterStatusEffectTable source, System.Data.IDataReader dataReader)
{
System.Int32 i;

i = dataReader.GetOrdinal("character_id");

source.CharacterID = (DemoGame.CharacterID)(DemoGame.CharacterID)dataReader.GetInt32(i);

i = dataReader.GetOrdinal("id");

source.ID = (DemoGame.ActiveStatusEffectID)(DemoGame.ActiveStatusEffectID)dataReader.GetInt32(i);

i = dataReader.GetOrdinal("power");

source.Power = (System.UInt16)(System.UInt16)dataReader.GetUInt16(i);

i = dataReader.GetOrdinal("status_effect_id");

source.StatusEffect = (DemoGame.StatusEffectType)(DemoGame.StatusEffectType)dataReader.GetByte(i);

i = dataReader.GetOrdinal("time_left_secs");

source.TimeLeftSecs = (System.UInt16)(System.UInt16)dataReader.GetUInt16(i);
}

/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. Unlike ReadValues(), this method not only doesn't require
/// all values to be in the IDataReader, but also does not require the values in
/// the IDataReader to be a defined field for the table this class represents.
/// Because of this, you need to be careful when using this method because values
/// can easily be skipped without any indication.
/// </summary>
/// <param name="source">The object to add the extension method to.</param>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public static void TryReadValues(this CharacterStatusEffectTable source, System.Data.IDataReader dataReader)
{
for (int i = 0; i < dataReader.FieldCount; i++)
{
switch (dataReader.GetName(i))
{
case "character_id":
source.CharacterID = (DemoGame.CharacterID)(DemoGame.CharacterID)dataReader.GetInt32(i);
break;


case "id":
source.ID = (DemoGame.ActiveStatusEffectID)(DemoGame.ActiveStatusEffectID)dataReader.GetInt32(i);
break;


case "power":
source.Power = (System.UInt16)(System.UInt16)dataReader.GetUInt16(i);
break;


case "status_effect_id":
source.StatusEffect = (DemoGame.StatusEffectType)(DemoGame.StatusEffectType)dataReader.GetByte(i);
break;


case "time_left_secs":
source.TimeLeftSecs = (System.UInt16)(System.UInt16)dataReader.GetUInt16(i);
break;


}

}
}

/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The key must already exist in the DbParameterValues
/// for the value to be copied over. If any of the keys in the DbParameterValues do not
/// match one of the column names, or if there is no field for a key, then it will be
/// ignored. Because of this, it is important to be careful when using this method
/// since columns or keys can be skipped without any indication.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void TryCopyValues(this ICharacterStatusEffectTable source, NetGore.Db.DbParameterValues paramValues)
{
for (int i = 0; i < paramValues.Count; i++)
{
switch (paramValues.GetParameterName(i))
{
case "@character_id":
paramValues[i] = (System.Int32)source.CharacterID;
break;


case "@id":
paramValues[i] = (System.Int32)source.ID;
break;


case "@power":
paramValues[i] = (System.UInt16)source.Power;
break;


case "@status_effect_id":
paramValues[i] = (System.Byte)source.StatusEffect;
break;


case "@time_left_secs":
paramValues[i] = (System.UInt16)source.TimeLeftSecs;
break;


}

}
}

/// <summary>
/// Checks if this <see cref="ICharacterStatusEffectTable"/> contains the same values as an<paramref name="other"/> <see cref="ICharacterStatusEffectTable"/>.
/// </summary>
/// <param name="other">The <see cref="ICharacterStatusEffectTable"/> to compare the values to.</param>
/// <returns>
/// True if this <see cref="ICharacterStatusEffectTable"/> contains the same values as the <paramref name="<paramref name="other"/>"/>; <paramref name="other"/>wise false.
/// </returns>
public static System.Boolean HasSameValues(this ICharacterStatusEffectTable source, ICharacterStatusEffectTable other)
{
return Equals(source.CharacterID, other.CharacterID) && 
Equals(source.ID, other.ID) && 
Equals(source.Power, other.Power) && 
Equals(source.StatusEffect, other.StatusEffect) && 
Equals(source.TimeLeftSecs, other.TimeLeftSecs);
}

}

}
