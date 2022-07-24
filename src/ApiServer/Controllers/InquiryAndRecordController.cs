using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ApiServer.Entities;

namespace ApiServer.Controllers;


[ApiController]
[Route("[controller]")]
public class InquiryAndRecordController : ControllerBase
{
    private readonly ILogger<InquiryAndRecordController> _logger;

    public InquiryAndRecordController(ILogger<InquiryAndRecordController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return new JsonResult(new InquiryAndRecordResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var contentText = string.Empty;
        using (var memstream = new MemoryStream())
        {
            await Request.BodyReader.CopyToAsync(memstream);
            contentText = System.Text.Encoding.UTF8.GetString(memstream.ToArray());
        }
        var requestData = JsonSerializer.Deserialize<InquiryAndRecordRequest>(contentText);

        return new JsonResult(new InquiryAndRecordResponse
        {
            BusinessNumber = requestData!.BusinessNumber,
            CtrlTerminalNumber = new CtrlTerminalNumber
            {
                SerialNumber = requestData!.CtrlTerminalNumber.SerialNumber,
                Port = requestData!.CtrlTerminalNumber.Port,
                Reserve = requestData!.CtrlTerminalNumber.Reserve,
            },
            SessionNumber = (byte)requestData.SessionNumber,
            OKNG = false
    });
    }
}



