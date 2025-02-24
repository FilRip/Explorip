using System;
using System.Linq;
using System.Text;

using Securify.PropertyStore.Flags;

namespace Securify.PropertyStore.Structures;

/// <summary>
/// The TypedPropertyValue structure represents the typed value of a property in a property set.
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
public class TypedPropertyValue(PropertyType Type, byte[] Value) : Structure()
{
    private readonly PropertyType _Type = Type;
    private readonly byte[] _Value = Value;

    #region Constructor
    #endregion // Constructor

    /// <summary>
    /// Type (2 bytes): MUST be a value from the PropertyType enumeration, indicating the type 
    /// of property represented.
    /// </summary>
    public PropertyType Type => _Type;

    /// <summary>
    /// Padding (2 bytes): MUST be set to zero, and any nonzero value SHOULD be rejected.
    /// </summary>
    public static ushort Padding => 0x0000;

    #region Value
    /// <summary>
    /// Value (variable): MUST be the value of the property represented and serialized 
    /// according to the value of Type 
    /// </summary>
    public object Value
    {
        get
        {
            if (_Value == null)
            {
                return null;
            }
            return Type switch
            {
                PropertyType.VT_I1 => (sbyte)_Value[0],
                PropertyType.VT_UI1 => _Value[0],
                PropertyType.VT_I2 => BitConverter.ToInt16(_Value, 0),
                PropertyType.VT_UI2 => BitConverter.ToUInt16(_Value, 0),
                PropertyType.VT_INT or PropertyType.VT_I4 => BitConverter.ToInt32(_Value, 0),
                PropertyType.VT_ERROR or PropertyType.VT_UINT or PropertyType.VT_UI4 => BitConverter.ToUInt32(_Value, 0),
                PropertyType.VT_I8 => BitConverter.ToInt64(_Value, 0),
                PropertyType.VT_CY or PropertyType.VT_UI8 => BitConverter.ToUInt64(_Value, 0),
                PropertyType.VT_R4 => BitConverter.ToSingle(_Value, 0),
                PropertyType.VT_R8 => BitConverter.ToDouble(_Value, 0),
                PropertyType.VT_DATE or PropertyType.VT_FILETIME => DateTime.FromFileTimeUtc(BitConverter.ToInt64(_Value, 0)),
                PropertyType.VT_BOOL => (_Value[0] != 0x00 || _Value[1] != 0x00),
                PropertyType.VT_LPSTR => Encoding.Default.GetString([.. _Value.Skip(4)]).TrimEnd((char)0),
                PropertyType.VT_LPWSTR => Encoding.Unicode.GetString([.. _Value.Skip(4)]).TrimEnd((char)0),
                PropertyType.VT_CLSID => new Guid(_Value),
                // Not supported
                _ => _Value,
            };
        }
    }
    #endregion // Value

    #region GetBytes
    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        byte[] value = new byte[_Value.Length + 4];
        Buffer.BlockCopy(BitConverter.GetBytes((ushort)Type), 0, value, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(Padding), 0, value, 2, 2);
        Buffer.BlockCopy(_Value, 0, value, 4, _Value.Length);
        return value;
    }
    #endregion // GetBytes

    #region ToString
    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendFormat("Type: {0}", Type);
        builder.AppendLine();
        builder.AppendFormat("Value: {0}", Value.ToString());
        builder.AppendLine();
        return builder.ToString();
    }
    #endregion // ToString
}
