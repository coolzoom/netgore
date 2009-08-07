using System;
using System.Linq;
using NetGore.Db;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Interface for a class that can be used to serialize values to the database table `map_spawn`.
/// </summary>
public interface IMapSpawnTable
{
/// <summary>
/// Gets the value for the database column `amount`.
/// </summary>
System.Byte Amount
{
get;
}
/// <summary>
/// Gets the value for the database column `character_id`.
/// </summary>
DemoGame.Server.CharacterID CharacterId
{
get;
}
/// <summary>
/// Gets the value for the database column `height`.
/// </summary>
System.UInt16 Height
{
get;
}
/// <summary>
/// Gets the value for the database column `id`.
/// </summary>
DemoGame.Server.MapSpawnValuesID Id
{
get;
}
/// <summary>
/// Gets the value for the database column `map_id`.
/// </summary>
NetGore.MapIndex MapId
{
get;
}
/// <summary>
/// Gets the value for the database column `width`.
/// </summary>
System.UInt16 Width
{
get;
}
/// <summary>
/// Gets the value for the database column `x`.
/// </summary>
System.UInt16 X
{
get;
}
/// <summary>
/// Gets the value for the database column `y`.
/// </summary>
System.UInt16 Y
{
get;
}
}

/// <summary>
/// Provides a strongly-typed structure for the database table `map_spawn`.
/// </summary>
public class MapSpawnTable : IMapSpawnTable
{
/// <summary>
/// Array of the database column names.
/// </summary>
 static  readonly System.String[] _dbColumns = new string[] {"amount", "character_id", "height", "id", "map_id", "width", "x", "y" };
/// <summary>
/// Gets an IEnumerable of strings containing the names of the database columns for the table that this class represents.
/// </summary>
public System.Collections.Generic.IEnumerable<System.String> DbColumns
{
get
{
return (System.Collections.Generic.IEnumerable<System.String>)_dbColumns;
}
}
/// <summary>
/// The name of the database table that this class represents.
/// </summary>
public const System.String TableName = "map_spawn";
/// <summary>
/// The number of columns in the database table that this class represents.
/// </summary>
public const System.Int32 ColumnCount = 8;
/// <summary>
/// The field that maps onto the database column `amount`.
/// </summary>
System.Byte _amount;
/// <summary>
/// The field that maps onto the database column `character_id`.
/// </summary>
System.UInt16 _characterId;
/// <summary>
/// The field that maps onto the database column `height`.
/// </summary>
System.UInt16 _height;
/// <summary>
/// The field that maps onto the database column `id`.
/// </summary>
System.Int32 _id;
/// <summary>
/// The field that maps onto the database column `map_id`.
/// </summary>
System.UInt16 _mapId;
/// <summary>
/// The field that maps onto the database column `width`.
/// </summary>
System.UInt16 _width;
/// <summary>
/// The field that maps onto the database column `x`.
/// </summary>
System.UInt16 _x;
/// <summary>
/// The field that maps onto the database column `y`.
/// </summary>
System.UInt16 _y;
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `amount`.
/// The underlying database type is `tinyint(3) unsigned`.
/// </summary>
public System.Byte Amount
{
get
{
return (System.Byte)_amount;
}
set
{
this._amount = (System.Byte)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `character_id`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public DemoGame.Server.CharacterID CharacterId
{
get
{
return (DemoGame.Server.CharacterID)_characterId;
}
set
{
this._characterId = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `height`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public System.UInt16 Height
{
get
{
return (System.UInt16)_height;
}
set
{
this._height = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `id`.
/// The underlying database type is `int(11)`.
/// </summary>
public DemoGame.Server.MapSpawnValuesID Id
{
get
{
return (DemoGame.Server.MapSpawnValuesID)_id;
}
set
{
this._id = (System.Int32)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `map_id`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public NetGore.MapIndex MapId
{
get
{
return (NetGore.MapIndex)_mapId;
}
set
{
this._mapId = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `width`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public System.UInt16 Width
{
get
{
return (System.UInt16)_width;
}
set
{
this._width = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `x`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public System.UInt16 X
{
get
{
return (System.UInt16)_x;
}
set
{
this._x = (System.UInt16)value;
}
}
/// <summary>
/// Gets or sets the value for the field that maps onto the database column `y`.
/// The underlying database type is `smallint(5) unsigned`.
/// </summary>
public System.UInt16 Y
{
get
{
return (System.UInt16)_y;
}
set
{
this._y = (System.UInt16)value;
}
}

/// <summary>
/// MapSpawnTable constructor.
/// </summary>
public MapSpawnTable()
{
}
/// <summary>
/// MapSpawnTable constructor.
/// </summary>
/// <param name="amount">The initial value for the corresponding property.</param>
/// <param name="characterId">The initial value for the corresponding property.</param>
/// <param name="height">The initial value for the corresponding property.</param>
/// <param name="id">The initial value for the corresponding property.</param>
/// <param name="mapId">The initial value for the corresponding property.</param>
/// <param name="width">The initial value for the corresponding property.</param>
/// <param name="x">The initial value for the corresponding property.</param>
/// <param name="y">The initial value for the corresponding property.</param>
public MapSpawnTable(System.Byte @amount, DemoGame.Server.CharacterID @characterId, System.UInt16 @height, DemoGame.Server.MapSpawnValuesID @id, NetGore.MapIndex @mapId, System.UInt16 @width, System.UInt16 @x, System.UInt16 @y)
{
Amount = (System.Byte)@amount;
CharacterId = (DemoGame.Server.CharacterID)@characterId;
Height = (System.UInt16)@height;
Id = (DemoGame.Server.MapSpawnValuesID)@id;
MapId = (NetGore.MapIndex)@mapId;
Width = (System.UInt16)@width;
X = (System.UInt16)@x;
Y = (System.UInt16)@y;
}
/// <summary>
/// MapSpawnTable constructor.
/// </summary>
/// <param name="dataReader">The IDataReader to read the values from. See method ReadValues() for details.</param>
public MapSpawnTable(System.Data.IDataReader dataReader)
{
ReadValues(dataReader);
}
public MapSpawnTable(IMapSpawnTable source)
{
CopyValuesFrom(source);
}
/// <summary>
/// Reads the values from an IDataReader and assigns the read values to this
/// object's properties. The database column's name is used to as the key, so the value
/// will not be found if any aliases are used or not all columns were selected.
/// </summary>
/// <param name="dataReader">The IDataReader to read the values from. Must already be ready to be read from.</param>
public void ReadValues(System.Data.IDataReader dataReader)
{
Amount = (System.Byte)(System.Byte)dataReader.GetByte(dataReader.GetOrdinal("amount"));
CharacterId = (DemoGame.Server.CharacterID)(DemoGame.Server.CharacterID)dataReader.GetUInt16(dataReader.GetOrdinal("character_id"));
Height = (System.UInt16)(System.UInt16)dataReader.GetUInt16(dataReader.GetOrdinal("height"));
Id = (DemoGame.Server.MapSpawnValuesID)(DemoGame.Server.MapSpawnValuesID)dataReader.GetInt32(dataReader.GetOrdinal("id"));
MapId = (NetGore.MapIndex)(NetGore.MapIndex)dataReader.GetUInt16(dataReader.GetOrdinal("map_id"));
Width = (System.UInt16)(System.UInt16)dataReader.GetUInt16(dataReader.GetOrdinal("width"));
X = (System.UInt16)(System.UInt16)dataReader.GetUInt16(dataReader.GetOrdinal("x"));
Y = (System.UInt16)(System.UInt16)dataReader.GetUInt16(dataReader.GetOrdinal("y"));
}

/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="dic">The Dictionary to copy the values into.</param>
public void CopyValues(System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
CopyValues(this, dic);
}
/// <summary>
/// Copies the column values into the given Dictionary using the database column name
/// with a prefixed @ as the key. The keys must already exist in the Dictionary;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="dic">The Dictionary to copy the values into.</param>
public static void CopyValues(IMapSpawnTable source, System.Collections.Generic.IDictionary<System.String,System.Object> dic)
{
dic["@amount"] = (System.Byte)source.Amount;
dic["@character_id"] = (DemoGame.Server.CharacterID)source.CharacterId;
dic["@height"] = (System.UInt16)source.Height;
dic["@id"] = (DemoGame.Server.MapSpawnValuesID)source.Id;
dic["@map_id"] = (NetGore.MapIndex)source.MapId;
dic["@width"] = (System.UInt16)source.Width;
dic["@x"] = (System.UInt16)source.X;
dic["@y"] = (System.UInt16)source.Y;
}

/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public void CopyValues(NetGore.Db.DbParameterValues paramValues)
{
CopyValues(this, paramValues);
}
/// <summary>
/// Copies the column values into the given DbParameterValues using the database column name
/// with a prefixed @ as the key. The keys must already exist in the DbParameterValues;
///  this method will not create them if they are missing.
/// </summary>
/// <param name="source">The object to copy the values from.</param>
/// <param name="paramValues">The DbParameterValues to copy the values into.</param>
public static void CopyValues(IMapSpawnTable source, NetGore.Db.DbParameterValues paramValues)
{
paramValues["@amount"] = (System.Byte)source.Amount;
paramValues["@character_id"] = (DemoGame.Server.CharacterID)source.CharacterId;
paramValues["@height"] = (System.UInt16)source.Height;
paramValues["@id"] = (DemoGame.Server.MapSpawnValuesID)source.Id;
paramValues["@map_id"] = (NetGore.MapIndex)source.MapId;
paramValues["@width"] = (System.UInt16)source.Width;
paramValues["@x"] = (System.UInt16)source.X;
paramValues["@y"] = (System.UInt16)source.Y;
}

public void CopyValuesFrom(IMapSpawnTable source)
{
Amount = (System.Byte)source.Amount;
CharacterId = (DemoGame.Server.CharacterID)source.CharacterId;
Height = (System.UInt16)source.Height;
Id = (DemoGame.Server.MapSpawnValuesID)source.Id;
MapId = (NetGore.MapIndex)source.MapId;
Width = (System.UInt16)source.Width;
X = (System.UInt16)source.X;
Y = (System.UInt16)source.Y;
}

public System.Object GetValue(System.String columnName)
{
switch (columnName)
{
case "amount":
return Amount;
case "character_id":
return CharacterId;
case "height":
return Height;
case "id":
return Id;
case "map_id":
return MapId;
case "width":
return Width;
case "x":
return X;
case "y":
return Y;
default:
throw new ArgumentException("Field not found.","columnName");
}
}

public void SetValue(System.String columnName, System.Object value)
{
switch (columnName)
{
case "amount":
Amount = (System.Byte)value;
break;
case "character_id":
CharacterId = (DemoGame.Server.CharacterID)value;
break;
case "height":
Height = (System.UInt16)value;
break;
case "id":
Id = (DemoGame.Server.MapSpawnValuesID)value;
break;
case "map_id":
MapId = (NetGore.MapIndex)value;
break;
case "width":
Width = (System.UInt16)value;
break;
case "x":
X = (System.UInt16)value;
break;
case "y":
Y = (System.UInt16)value;
break;
default:
throw new ArgumentException("Field not found.","columnName");
}
}

public static ColumnMetadata GetColumnData(System.String fieldName)
{
switch (fieldName)
{
case "amount":
return new ColumnMetadata("amount", "", "tinyint(3) unsigned", null, typeof(System.Byte), false, false, false);
case "character_id":
return new ColumnMetadata("character_id", "", "smallint(5) unsigned", null, typeof(System.UInt16), false, false, true);
case "height":
return new ColumnMetadata("height", "", "smallint(5) unsigned", null, typeof(System.UInt16), true, false, false);
case "id":
return new ColumnMetadata("id", "", "int(11)", null, typeof(System.Int32), false, true, false);
case "map_id":
return new ColumnMetadata("map_id", "", "smallint(5) unsigned", null, typeof(System.UInt16), false, false, false);
case "width":
return new ColumnMetadata("width", "", "smallint(5) unsigned", null, typeof(System.UInt16), true, false, false);
case "x":
return new ColumnMetadata("x", "", "smallint(5) unsigned", null, typeof(System.UInt16), true, false, false);
case "y":
return new ColumnMetadata("y", "", "smallint(5) unsigned", null, typeof(System.UInt16), true, false, false);
default:
throw new ArgumentException("Field not found.","fieldName");
}
}

}

}
