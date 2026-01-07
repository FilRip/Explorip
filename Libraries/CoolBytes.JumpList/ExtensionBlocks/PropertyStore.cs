using System;
using System.Collections.Generic;
using System.Text;

namespace CoolBytes.JumpList.ExtensionBlocks
{
    public class PropertyStore
    {
        public PropertyStore()
        {
            Sheets = [];
        }

        public PropertyStore(byte[] rawBytes)
        {
            Sheets = [];

            //shellPropertySheetList now contains what we need to parse for the rest of this process

            int shellPropertyIndex = 0;

            Dictionary<int, byte[]> sheetLists = [];
            int sheetListslot = 0;

            while (shellPropertyIndex < rawBytes.Length)
            {
                //cut up shellPropertySheetList into byte arrays based on length, then process each one
                int serializedSize = BitConverter.ToInt32(rawBytes, shellPropertyIndex);

                if (serializedSize == 0 || (uint)serializedSize >= rawBytes.Length)
                {
                    break; // we are out of lists
                }

                byte[] sheetListBytes = new byte[serializedSize];
                Array.Copy(rawBytes, shellPropertyIndex, sheetListBytes, 0, serializedSize);

                sheetLists.Add(sheetListslot, sheetListBytes);
                sheetListslot += 1;

                shellPropertyIndex += serializedSize;
            } //end of while in shellPropertySheetList

            foreach (KeyValuePair<int, byte[]> sheetList in sheetLists)
            {
                PropertySheet sheet = new(sheetList.Value);

                Sheets.Add(sheet);
            }
        }

        public List<PropertySheet> Sheets { get; }

        public override string ToString()
        {
            StringBuilder sb = new();

            int sheetNumber = 0;

            foreach (PropertySheet propertySheet in Sheets)
            {
                sb.AppendLine($"Sheet #{sheetNumber} => {propertySheet}");

                sheetNumber += 1;

            }

            return sb.ToString();
        }
    }
}
