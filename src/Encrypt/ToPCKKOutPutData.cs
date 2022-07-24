using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using NullFX.CRC;

public class ToPCKKOutPutData
{
    public UInt16 BusinessNumber { get; set; }
    public CTRLNumber? CTRLNumber { get; set; }
    public BeaconId? BeaconId { get; set; }
    public string? AslId { get; set; }
    public string? PublicKey { get; set; }

    private readonly ILogger<ToPCKKOutPutData> _logger;

    public ToPCKKOutPutData(ILogger<ToPCKKOutPutData> logger)
    {
        _logger = logger;
    }

    public byte[] GetBytes()
    {
        var data = CTRLNumber!.GetBytes()
            .Concat(BeaconId!.GetBytes())
            .Concat(Encoding.ASCII.GetBytes(AslId!))
            .ToArray();

        _logger.LogTrace($"Source    : {string.Join(" ", data.Select(d => $"{d:X2}"))}");

        var crc = Crc16.ComputeChecksum(Crc16Algorithm.Ccitt, data);

        _logger.LogTrace($"Crc       : {crc}({crc:X4})");

        var crcBytes = BitConverter.GetBytes(crc).ToArray();
        if (BitConverter.IsLittleEndian)
        {
            crcBytes = crcBytes.Reverse().ToArray();
        }

        var allBytes = data.Concat(crcBytes).ToArray();

        return allBytes;
    }

    public string Encrypt()
    {
        var allBytes = this.GetBytes();

        _logger.LogTrace($"allbytes  : {string.Join(" ", allBytes.Select(d => $"{d:X2}"))}");

        var encrypted = Encrypt(allBytes, this.PublicKey!);

        _logger.LogTrace($"Encripted : {string.Join(" ", encrypted.Select(e => $"{e:X2}"))}");

        var base64string = Convert.ToBase64String(encrypted);

        _logger.LogTrace($"Encripted : {base64string}");

        return base64string;
    }

    public InquiryAndRecordRequest ToInquiryAndRecordRequest(int sessionNumber, DateTimeOffset sessionStartTime)
    {
        return new InquiryAndRecordRequest
        {
            BusinessNumber = this.BusinessNumber,
            CtrlTerminalNumber = new CtrlTerminalNumber
            {
                SerialNumber = CTRLNumber!.SerialNumber,
                Port = CTRLNumber!.Port,
                Reserve = CTRLNumber!.Reserve,
            },
            SessionNumber = sessionNumber,
            SessionNumberDatetime = sessionStartTime.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
            TargetDatetime = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
            ToPCKKOutPutData = this.Encrypt()
        };
    }


    public ConfirmRequest ToConfirmRequest(int businessNumber, int sessionNumber, DateTimeOffset sessionStartTime)
    {
        return new ConfirmRequest
        {
            BusinessNumber = businessNumber,
            CtrlTerminalNumber = new CtrlTerminalNumber
            {
                SerialNumber = CTRLNumber!.SerialNumber,
                Port = CTRLNumber!.Port,
                Reserve = CTRLNumber!.Reserve,
            },
            SessionNumber = sessionNumber,
            SessionNumberDatetime = sessionStartTime.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
        };
    }

    public CancelRequest ToCancelRequest(int businessNumber, int sessionNumber, DateTimeOffset sessionStartTime)
    {
        return new CancelRequest
        {
            BusinessNumber = businessNumber,
            CtrlTerminalNumber = new CtrlTerminalNumber
            {
                SerialNumber = CTRLNumber!.SerialNumber,
                Port = CTRLNumber!.Port,
                Reserve = CTRLNumber!.Reserve,
            },
            SessionNumber = sessionNumber,
            SessionNumberDatetime = sessionStartTime.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
        };
    }

    private byte[] Encrypt(byte[] data, string publicKey)
    {
        using (var rsa = RSA.Create())
        {
            rsa.ImportFromPem(publicKey);

            // var result = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            // return Convert.ToBase64String(result);

            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }
    }
}