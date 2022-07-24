// public class BeaconIDp
// {
//     public UInt16 ManufactureId { get; set; }
//     public UInt32 IndividualId { get; set; }

//     // public byte[] GetBytes()
//     // {
//     //     var mId = Convert.ToString(ManufactureId, 2).PadLeft(16, '0');
//     //     var iId = Convert.ToString(IndividualId, 2).PadLeft(32, '0');
//     //     var all = "0" + mId + iId.Substring(1);

//     //     List<byte> result = new List<byte>();
//     //     for (var i = 0; i < 6; i++)
//     //     {
//     //         result.Add(Convert.ToByte(all.Substring(i * 8, 8), 2));
//     //     }

//     //     return result.ToArray();
//     // }

//     public byte[] GetBytes()
//     {
//         var mId = BitConverter.GetBytes(this.ManufactureId);
//         var iId = BitConverter.GetBytes((UInt32)(this.IndividualId << 5));
//         if (BitConverter.IsLittleEndian)
//         {
//             mId = mId.Reverse().ToArray();
//             iId = iId.Reverse().ToArray();
//         }

//         var bytes = mId.Concat(iId).ToArray();

//         return bytes;
//     }
// }