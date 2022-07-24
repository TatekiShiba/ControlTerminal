namespace ApiServer.Entities;

public class InquiryAndRecordResponse
{
    public UInt16 BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string Response { get { return "InquiryAndRecord-Res"; } }
    public Byte SessionNumber { get; set; }
    public bool OKNG { get; set; }
}
