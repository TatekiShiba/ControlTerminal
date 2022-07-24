public class ConfirmResponse
{
    public int BusinessNumber { get; set; }
    public CtrlTerminalNumber? CtrlTerminalNumber { get; set; }
    public string? Response { get; set; }
    public int SessionNumber { get; set; }
    public bool OKNG { get; set; }
}