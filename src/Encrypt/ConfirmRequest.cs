public class ConfirmRequest
{
    public int BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string Command { get { return "Confirm"; } }
    public int SessionNumber { get; set; }
    public string SessionNumberDatetime { get; set; } = string.Empty;
}