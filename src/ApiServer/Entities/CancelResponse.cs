namespace ApiServer.Entities;

public class CancelResponse
{
    public UInt16 BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string Response { get { return "Cancel-Res"; } }
    public Byte SessionNumber { get; set; }
    public bool OKNG { get; set; }
}
