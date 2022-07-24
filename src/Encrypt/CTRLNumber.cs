public class CTRLNumber
{
    public UInt16 SerialNumber { get; set; }
    private byte _portAndReserve;

    public Byte Port // 0~7まで(上位3bit)
    {
        get { return (byte)(_portAndReserve >> 5); }
        set { _portAndReserve = (byte)((value << 5) + (_portAndReserve & 0b_0001_1111)); }
    }

    public byte Reserve // 下位5bit
    {
        get { return (byte)(_portAndReserve & 0b_0001_1111); }
        set { _portAndReserve = (byte)((_portAndReserve & 0b_1110_0000) + value); }
    }

    public Byte[] GetBytes()
    {
        var s = BitConverter.GetBytes(SerialNumber);
        if (BitConverter.IsLittleEndian)
        {
            s = s.Reverse().ToArray();
        }
        return s.Append(_portAndReserve).ToArray();
    }
}