

public class TestCase
{
    public string CaseNo { get; set; } = string.Empty;
    public int TestNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Wcn { get; set; } = string.Empty;
    public bool IsNewSession { get; set; }
    public int SessionNumber { get; set; }
    public DateTimeOffset SessionTime { get; set; }
    public int DelaySec { get; set; }
    public UInt16 BusinessNumber { get; set; }
    public BeaconId? BeaconId { get; set; }
    public CTRLNumber? CTRLNumber { get; set; }
    public string EncryptKey { get; set; } = string.Empty;
    public enum RequestTypes
    {
        InquiryAndRecord,
        Confirm,
        Cancel,
    }
    public RequestTypes RequestType { get; set; }
}