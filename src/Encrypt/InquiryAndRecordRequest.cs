public class CtrlTerminalNumber
{
    public int SerialNumber { get; set; }
    public int Port { get; set; }
    public int Reserve { get; set; }
}

public class InquiryAndRecordRequest
{
    public int BusinessNumber { get; set; }
    public CtrlTerminalNumber CtrlTerminalNumber { get; set; } = new CtrlTerminalNumber();
    public string Command { get { return "InquiryAndRecord"; } }
    public int SessionNumber { get; set; }
    public string SessionNumberDatetime { get; set; } = string.Empty;
    public string TargetDatetime { get; set; } = string.Empty;
    public string ToPCKKOutPutData { get; set; } = string.Empty;
}