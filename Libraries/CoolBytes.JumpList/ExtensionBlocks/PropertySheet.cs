using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using CoolBytes.JumpList.Helpers;

namespace CoolBytes.JumpList.ExtensionBlocks;

public class PropertySheet
{
    public PropertySheet(byte[] contents)
    {
        PropertyNames = [];

        int sheetindex = 0;

        int serializedSize = BitConverter.ToInt32(contents, sheetindex);
        sheetindex = 4; //skip size

        Size = serializedSize;

        string serializedVersion = BitConverter.ToString(contents, sheetindex, 4);

        sheetindex += 4;

        if (serializedVersion != "31-53-50-53")
        {
            throw new ExtensionsJumpListException($"Version mismatch! {serializedVersion} != 31-53-50-53");
        }

        Version = serializedVersion;

        byte[] rawguidshellProperty = new byte[16];

        Array.Copy(contents, sheetindex, rawguidshellProperty, 0, 16);

        string formatClassIdguid = Utils.ExtractGuidFromShellItem(rawguidshellProperty);

        sheetindex += 16;

        GUID = formatClassIdguid;

        if (formatClassIdguid == "d5cdd505-2e9c-101b-9397-08002b2cf9ae")
        {
            //all serialized property values are named properties
            PropertySheetType = PropertySheetTypeEnum.Named;

            int valueSize = 0;
            string propertyName = "";

            Dictionary<int, byte[]> propertyValues = [];
            int propertySlotNumber = 0;

            while (sheetindex < contents.Length)
            {
                //cut up shellPropertySheetList into byte arrays based on length, then process each one
                valueSize = BitConverter.ToInt32(contents, sheetindex);

                if (valueSize == 0)
                {
                    break; // we are out of lists
                }

                byte[] sheetListBytes = new byte[valueSize];
                Array.Copy(contents, sheetindex, sheetListBytes, 0, valueSize);

                propertyValues.Add(propertySlotNumber, sheetListBytes);
                propertySlotNumber += 1;

                sheetindex += valueSize;
            } //end of while in shellPropertySheetList

            foreach (KeyValuePair<int, byte[]> propertyValue in propertyValues)
            {
                int propertyIndex = 0;

                valueSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                propertyIndex += 4;

                int nameSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                propertyIndex += 4;

                propertyIndex += 1; //reserved

                propertyName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex, nameSize - 2);

                propertyIndex += (nameSize);

                ushort namedType = BitConverter.ToUInt16(propertyValue.Value, propertyIndex);

                propertyIndex += 2; //skip type
                propertyIndex += 2; //skip padding?

                //TODO Combine these with what is below. Make a function to take the type, process and return a string?
                switch (namedType)
                {
                    case 0x000b:
                        //VT_BOOL (0x000B)
                        int boolInt = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 8;

                        bool boolval = boolInt > 0;

                        PropertyNames.Add(propertyName,
                            boolval.ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0:
                    case 0x1:
                        PropertyNames.Add(propertyName, "");
                        break;

                    case 0x0002:
                        PropertyNames.Add(propertyName,
                            BitConverter.ToInt16(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));
                        break;

                    case 0x0003:
                        PropertyNames.Add(propertyName,
                            BitConverter.ToInt32(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));
                        break;

                    case 0x0004:
                        PropertyNames.Add(propertyName,
                            BitConverter.ToSingle(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));
                        break;

                    case 0x0005:
                        PropertyNames.Add(propertyName,
                            BitConverter.ToDouble(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));
                        break;

                    case 0x0008:

                        int uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        string unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                            uniLength - 2);
                        propertyIndex += (uniLength);

                        PropertyNames.Add(propertyName, unicodeName);

                        //  PropertyNames.Add(propertyName, BitConverter.ToDouble(propertyValue.Value, propertyIndex).ToString(CultureInfo.InvariantCulture));
                        break;


                    case 0x000a:
                        PropertyNames.Add(propertyName,
                            BitConverter.ToUInt32(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));
                        break;

                    case 0x0014:
                        //VT_I8 (0x0014)  MUST be an 8-byte signed integer.

                        PropertyNames.Add(propertyName,
                            BitConverter.ToInt64(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0015:
                        //VT_I8 (0x0014)  MUST be an 8-byte unsigned integer.

                        PropertyNames.Add(propertyName,
                            BitConverter.ToUInt64(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0016:
                        //VT_I8 (0x0014)  MUST be an 4-byte signed integer.

                        PropertyNames.Add(propertyName,
                            BitConverter.ToInt32(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0013:
                    case 0x0017:
                        //VT_I8 (0x0014)  MUST be an 4-byte unsigned integer.

                        PropertyNames.Add(propertyName,
                            BitConverter.ToUInt32(propertyValue.Value, propertyIndex)
                                .ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x001f: //unicode string

                        uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        if (uniLength <= 0)
                        {
                            PropertyNames.Add(propertyName, string.Empty);
                            break;
                        }

                        unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                            (uniLength * 2) - 2);
                        propertyIndex += (uniLength * 2);

                        PropertyNames.Add(propertyName, unicodeName);

                        break;

                    case 0x0040:
                        // VT_FILETIME 0x0040 Type is FILETIME, and the minimum property set version is 0.

                        long hexNumber = BitConverter.ToInt64(propertyValue.Value, propertyIndex);
                        // "01CDF407";

                        propertyIndex += 8;

                        DateTime dd = DateTime.FromFileTimeUtc(hexNumber);

                        PropertyNames.Add(propertyName,
                            dd.ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0041:
                        //VT_BLOB 0x0041 Type is binary large object (BLOB), and the minimum property set version is 0

                        //TODO FINISH THIS

                        int blobSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        byte[] bytes = [.. propertyValue.Value.Skip(0x69)];

                        PropertyStore props = new(bytes);

                        PropertyNames.Add(propertyName,
                            $"BLOB data: {BitConverter.ToString(propertyValue.Value, propertyIndex)}");

                        foreach (PropertySheet prop in props.Sheets)
                        {
                            foreach (KeyValuePair<string, string> name in prop.PropertyNames)
                            {
                                PropertyNames.Add($"{name.Key}", name.Value); // (From BLOB data)
                            }
                        }

                        propertyIndex += blobSize;

                        break;

                    case 0x0042:
                        //TODO FINISH THIS

                        //Type is Stream, and the minimum property set version is 0. VT_STREAM is not allowed in a simple property set.
                        PropertyNames.Add(propertyName,
                            "VT_STREAM not implemented (yet) See extension block section for contents for now");

                        break;

                    default:
                        PropertyNames.Add(propertyName,
                            $"Unknown named property type: {namedType.ToString("X")}, Hex data (after property type): {BitConverter.ToString(propertyValue.Value, propertyIndex)}. Send file to saericzimmerman@gmail.com to get support added");
                        break;
                        //throw new ExtensionsJumpListException($"Unknown named property type: {namedType.ToString("X")}, Hex data (after property type): {BitConverter.ToString(propertyValue.Value, propertyIndex)}");
                }
            }

            int terminator = BitConverter.ToInt32(contents, sheetindex);

            if (terminator != 0)
            {
                throw new ExtensionsJumpListException($"Expected terminator of 0, but got {terminator}");
            }
        }
        else
        {
            //treat as numeric property values

            PropertySheetType = PropertySheetTypeEnum.Numeric;

            int valueSize = 0;
            int propertyId = 0;

            Dictionary<int, byte[]> propertyValues = [];
            int propertySlotNumber = 0;

            while (sheetindex < contents.Length)
            {
                //cut up shellPropertySheetList into byte arrays based on length, then process each one
                int sheetSize = BitConverter.ToInt32(contents, sheetindex);

                if (sheetSize == 0 || (uint)sheetSize >= contents.Length)
                {
                    break; // we are out of lists
                }

                byte[] sheetListBytes = new byte[sheetSize];
                Array.Copy(contents, sheetindex, sheetListBytes, 0, sheetSize);

                propertyValues.Add(propertySlotNumber, sheetListBytes);
                propertySlotNumber += 1;

                sheetindex += sheetSize;
            } //end of while in shellPropertySheetList

            foreach (KeyValuePair<int, byte[]> propertyValue in propertyValues)
            {
                int propertyIndex = 0;

                valueSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                propertyIndex += 4;

                propertyId = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                propertyIndex += 4;

                propertyIndex += 1; //skip reserved

                ushort numericType = BitConverter.ToUInt16(propertyValue.Value, propertyIndex);

                propertyIndex += 2; //skip type
                propertyIndex += 2; //skip padding?

                //TODO Combine these with what is below. Make a function to take the type, process and return a string?
                switch (numericType)
                {
                    case 0x1048:
                        //MUST be a VectorHeader followed by a sequence of GUID (Packet Version) packets.

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            "VT_VECTOR data not implemented (yet)");

                        break;
                    case 0x01e:
                        int uniLength1e = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        string unicodeName1e =
                            Encoding.Unicode.GetString(propertyValue.Value, propertyIndex, uniLength1e)
                                .Split('\0')
                                .First();

                        // Debug.WriteLine($"Find me: {BitConverter.ToString(propertyValue.Value)}, propertyIndex: {propertyIndex} unicodeName1e: {unicodeName1e}");

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), unicodeName1e);

                        break;
                    case 0x001f: //unicode string

                        int uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        if (uniLength <= 0)
                        {
                            PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), string.Empty);
                            break;
                        }

                        string unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                            (uniLength * 2) - 2);
                        propertyIndex += (uniLength * 2);

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), unicodeName);

                        break;

                    case 0x000b:
                        //VT_BOOL (0x000B) MUST be a VARIANT_BOOL as specified in [MS-OAUT] section 2.2.27, followed by zero padding to 4 bytes.

                        int boolInt = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 8;

                        bool boolval = boolInt > 0;

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            boolval.ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0003:
                        //VT_I4 (0x0003) MUST be a 32-bit signed integer.

                        int signedInt = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            signedInt.ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0015:
                        //VT_UI8 (0x0015) MUST be an 8-byte unsigned integer

                        ulong unsigned8int = BitConverter.ToUInt64(propertyValue.Value, propertyIndex);
                        propertyIndex += 8;

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            unsigned8int.ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0042:
                        //VT_STREAM (0x0042) MUST be an IndirectPropertyName. The storage representing the
                        //(non-simple) property set MUST have a stream element with this name

                        //defer for now

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            "VT_STREAM not implemented");

                        break;

                    case 0x0013:
                        //VT_UI4 (0x0013) MUST be a 4-byte unsigned integer

                        uint unsigned4int = BitConverter.ToUInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            unsigned4int.ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0001:
                        //VT_NULL (0x0001) MUST be zero bytes in length.

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), "Null");

                        break;

                    case 0x0002:
                        //VT_I2 (0x0002) Either the specified type, or the type of the element or contained field MUST be a 2-byte signed int

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), BitConverter.ToUInt16(propertyValue.Value, propertyIndex).ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x101f:
                        //VT_VECTOR | VT_LPWSTR 0x101F Type is Vector of UnicodeString, and the minimum property set version is 0

                        propertyIndex += 4;

                        unicodeName = string.Empty;

                        if (propertyValue.Value.Length > propertyIndex)
                        {
                            uniLength = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                            propertyIndex += 4;

                            unicodeName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                                (uniLength * 2) - 2);
                            propertyIndex += (uniLength * 2);
                        }

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), unicodeName);

                        break;

                    case 0x0048:
                        //VT_CLSID 0x0048 Type is CLSID, and the minimum property set version is 0.

                        byte[] rawguid1 = new byte[16];

                        Array.Copy(propertyValue.Value, propertyIndex, rawguid1, 0, 16);

                        propertyIndex += 16;

                        string rawguid = Utils.ExtractGuidFromShellItem(rawguid1);

                        string foldername = Utils.GetFolderNameFromGuid(rawguid);

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), foldername);

                        break;

                    case 0x1011:
                        //VT_VECTOR | VT_UI1 0x1011 Type is Vector of 1-byte unsigned integers, and the minimum property  set version is 0.

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            "VT_VECTOR data not implemented (yet) See extension block section for contents for now");

                        //TODO i see indicators from 0x00, case 0x23febbee: ProcessPropertyViewGUID(rawBytes) in the bits for this
                        // can we pull out the property sheet and add them to the property names here?

                        break;

                    case 0x0040:
                        //VT_FILETIME 0x0040 Type is FILETIME, and the minimum property set version is 0.

                        long hexNumber = BitConverter.ToInt64(propertyValue.Value, propertyIndex);
                        // "01CDF407";

                        propertyIndex += 8;

                        DateTime dd = DateTime.FromFileTimeUtc(hexNumber);

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            dd.ToString(CultureInfo.InvariantCulture));

                        break;

                    case 0x0008:

                        int codePageSize = BitConverter.ToInt32(propertyValue.Value, propertyIndex);
                        propertyIndex += 4;

                        string codePageName = Encoding.Unicode.GetString(propertyValue.Value, propertyIndex,
                            codePageSize - 2);
                        propertyIndex += (codePageSize);

                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture), codePageName);

                        break;

                    default:
                        PropertyNames.Add(propertyId.ToString(CultureInfo.InvariantCulture),
                            $"Unknown numeric property type: {numericType.ToString("X")}, Hex data (after property type): {BitConverter.ToString(propertyValue.Value, propertyIndex)}. Send file to saericzimmerman@gmail.com to get support added");
                        break;
                        //  throw new ExtensionsJumpListException($"Unknown numeric property type: {numericType.ToString("X")}, Hex data (after property type): {BitConverter.ToString(propertyValue.Value, propertyIndex)}");
                }
            }

            int terminator = BitConverter.ToInt32(contents, sheetindex);

            if (terminator != 0)
            {
                throw new ExtensionsJumpListException($"Expected terminator of 0, but got {terminator}");
            }
        }
    }

    public int Size { get; private set; }

    public string Version { get; private set; }

    public string GUID { get; private set; }

    public byte[]? HexValue { get; set; }

    public Dictionary<string, string> PropertyNames { get; }

    public PropertySheetTypeEnum PropertySheetType { get; private set; }

    public override string ToString()
    {
        StringBuilder sb = new();

        if (PropertySheetType == PropertySheetTypeEnum.Numeric)
        {
            string s = string.Join("; ", PropertyNames.Select(x => $"Guid: {GUID}, Key: {x.Key} ==> {Utils.GetDescriptionFromGuidAndKey(GUID, int.Parse(x.Key))}, Value: {x.Value}"));

            sb.Append(s);
        }
        else
        {
            string s = string.Join("; ", PropertyNames.Select(x => $"Guid: {GUID}, Key: {x.Key} ==> {x.Key}, Value: {x.Value}"));

            sb.Append(s);
        }

        return sb.ToString();
    }
}
