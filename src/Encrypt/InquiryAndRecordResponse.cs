public class InquiryAndRecordResponse
{
    public int BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string Response { get; set; } = string.Empty;
    public int SessionNumber { get; set; }
    public bool OKNG { get; set; }
}