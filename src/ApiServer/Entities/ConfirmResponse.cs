namespace ApiServer.Entities;

public class ConfirmResponse
{
    public UInt16 BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string Response { get { return "Confirm-Res"; } }
    public Byte SessionNumber { get; set; }
    public bool OKNG { get; set; }
}
