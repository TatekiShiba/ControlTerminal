public class BeaconId
{
    public UInt16 ManufactureId { get; set; }
    public UInt32 IndividualId { get; set; }

    public byte[] GetBytes()
    {
        var mId = BitConverter.GetBytes(this.ManufactureId);
        var iId = BitConverter.GetBytes(this.IndividualId);
        if (BitConverter.IsLittleEndian)
        {
            mId = mId.Reverse().ToArray();
            iId = iId.Reverse().ToArray();
        }

        var bytes = mId.Concat(iId).ToArray();

        return bytes;
    }
}