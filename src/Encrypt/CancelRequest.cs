public class CancelRequest
{
    public int BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string Command { get { return "Cancel"; } }
    public int SessionNumber { get; set; }
    public string? SessionNumberDatetime { get; set; }
}