namespace ApiServer.Entities;

public class ConfirmRequest
{
    public UInt16 BusinessNumber { get; set; }
    public CtrlTerminalNumber CtrlTerminalNumber { get; set; } = new CtrlTerminalNumber();
    public string Command { get; set; } = string.Empty;
    public int SessionNumber { get; set; }
    public string SessionNumberDatetime { get; set; } = string.Empty;
}