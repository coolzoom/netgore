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
/// Contains extension methods for class MapTable that assist in performing
/// reads and writes to and from a database.
/// </summary>
public static  class MapTableDbExtensions
{
/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void CopyValues(this IMapTable source, NetGore.Db.DbParameterValues paramValues)
{
paramValues["@id"] = (System.UInt16)source.ID;
paramValues["@name"] = (System.String)source.Name;
}

/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. The database column's name is used to as the key, so the value
/// will not be found if any aliases are used or not all columns were selected.
/// </summary>
/// <param name="source">The object to add the extension method to.</param>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public static void ReadValues(this MapTable source, System.Data.IDataReader dataReader)
{
System.Int32 i;

i = dataReader.GetOrdinal("id");

source.ID = (NetGore.MapIndex)(NetGore.MapIndex)dataReader.GetUInt16(i);

i = dataReader.GetOrdinal("name");

source.Name = (System.String)(System.String)dataReader.GetString(i);
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
public static void TryReadValues(this MapTable source, System.Data.IDataReader dataReader)
{
for (int i = 0; i < dataReader.FieldCount; i++)
{
switch (dataReader.GetName(i))
{
case "id":
source.ID = (NetGore.MapIndex)(NetGore.MapIndex)dataReader.GetUInt16(i);
break;


case "name":
source.Name = (System.String)(System.String)dataReader.GetString(i);
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
public static void TryCopyValues(this IMapTable source, NetGore.Db.DbParameterValues paramValues)
{
for (int i = 0; i < paramValues.Count; i++)
{
switch (paramValues.GetParameterName(i))
{
case "@id":
paramValues[i] = (System.UInt16)source.ID;
break;


case "@name":
paramValues[i] = (System.String)source.Name;
break;


}

}
}

/// <summary>
/// Checks if this <see cref="IMapTable"/> contains the same values as an<paramref name="other"/> <see cref="IMapTable"/>.
/// </summary>
/// <param name="other">The <see cref="IMapTable"/> to compare the values to.</param>
/// <returns>
/// True if this <see cref="IMapTable"/> contains the same values as the <paramref name="<paramref name="other"/>"/>; <paramref name="other"/>wise false.
/// </returns>
public static System.Boolean HasSameValues(this IMapTable source, IMapTable other)
{
return Equals(source.ID, other.ID) && 
Equals(source.Name, other.Name);
}

}

}
