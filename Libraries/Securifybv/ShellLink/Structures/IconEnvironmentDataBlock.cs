﻿using System;
using System.Text;

namespace Securify.ShellLink.Structures;

/// <summary>
/// The IconEnvironmentDataBlock structure specifies the path to an icon. The path is encoded using 
/// environment variables, which makes it possible to find the icon across machines where the 
/// locations vary but are expressed using environment variables.
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="Target">The path that is constructed with environment variables</param>
public class IconEnvironmentDataBlock(string Target) : ExtraDataBlock()
{
    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public IconEnvironmentDataBlock() : this("") { }
    #endregion // Constructor

    /// <summary>
    /// BlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the 
    /// IconEnvironmentDataBlock structure. This value MUST be 0x00000314.
    /// </summary>
    public override uint BlockSize => 0x314;

    /// <summary>
    /// BlockSignature (4 bytes): A 32-bit, unsigned integer that specifies the signature of the 
    /// IconEnvironmentDataBlock extra data section. This value MUST be 0xA0000007.
    /// </summary>
    public override BlockSignature BlockSignature => BlockSignature.ICON_ENVIRONMENT_PROPS;

    /// <summary>
    /// TargetAnsi (260 bytes): A NULL-terminated string, defined by the system default code page, 
    /// which specifies a path that is constructed with environment variables.
    /// </summary>
    public string TargetAnsi { get; set; } = Target;

    /// <summary>
    /// TargetUnicode (520 bytes): An optional, NULL-terminated, Unicode string that specifies a 
    /// path that is constructed with environment variables.
    /// </summary>
    public string TargetUnicode { get; set; } = Target;

    #region GetBytes
    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        byte[] IconEnvironmentDataBlock = new byte[BlockSize];
        Buffer.BlockCopy(BitConverter.GetBytes(BlockSize), 0, IconEnvironmentDataBlock, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes((uint)BlockSignature), 0, IconEnvironmentDataBlock, 4, 4);
        Buffer.BlockCopy(Encoding.Default.GetBytes(TargetAnsi), 0, IconEnvironmentDataBlock, 8, TargetAnsi.Length < 259 ? TargetAnsi.Length : 259);
        Buffer.BlockCopy(Encoding.Unicode.GetBytes(TargetUnicode), 0, IconEnvironmentDataBlock, 268, TargetUnicode.Length < 259 ? TargetUnicode.Length * 2 : 518);
        return IconEnvironmentDataBlock;
    }
    #endregion // GetBytes

    #region ToString
    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendFormat("TargetAnsi: {0}", TargetAnsi);
        builder.AppendLine();
        builder.AppendFormat("TargetUnicode: {0}", TargetUnicode);
        builder.AppendLine();
        return builder.ToString();
    }
    #endregion // ToString

    #region FromByteArray
    /// <summary>
    /// Create an IconEnvironmentDataBlock from a given byte array
    /// </summary>
    /// <param name="ba">The byte array</param>
    /// <returns>An IconEnvironmentDataBlock object</returns>
    public static IconEnvironmentDataBlock FromByteArray(byte[] ba)
    {
        IconEnvironmentDataBlock IconEnvironmentDataBlock = new();
        if (ba.Length < 0x314)
        {
            throw new ArgumentException(string.Format("Size of the IconEnvironmentDataBlock Structure is less than 788 ({0})", ba.Length));
        }

        uint blockSize = BitConverter.ToUInt32(ba, 0);
        if (blockSize > ba.Length)
        {
            throw new ArgumentException(string.Format("BlockSize is {0} is incorrect (expected {1})", blockSize, IconEnvironmentDataBlock.BlockSize));
        }

        BlockSignature blockSignature = (BlockSignature)BitConverter.ToUInt32(ba, 4);
        if (blockSignature != IconEnvironmentDataBlock.BlockSignature)
        {
            throw new ArgumentException(string.Format("BlockSignature is {0} is incorrect (expected {1})", blockSignature, IconEnvironmentDataBlock.BlockSignature));
        }

        byte[] targetAnsi = new byte[260];
        Buffer.BlockCopy(ba, 8, targetAnsi, 0, 260);
        IconEnvironmentDataBlock.TargetAnsi = Encoding.Default.GetString(targetAnsi).TrimEnd((char)0);

        byte[] targetUnicode = new byte[520];
        Buffer.BlockCopy(ba, 268, targetUnicode, 0, 520);
        IconEnvironmentDataBlock.TargetUnicode = Encoding.Unicode.GetString(targetUnicode).TrimEnd((char)0);

        return IconEnvironmentDataBlock;
    }
    #endregion // FromByteArray
}
