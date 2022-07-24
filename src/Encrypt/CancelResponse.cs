public class CancelResponse
{
    public int BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string? Command { get; set; }
    public int SessionNumber { get; set; }
    public bool OKNG { get; set; }
}