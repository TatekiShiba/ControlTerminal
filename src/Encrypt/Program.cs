using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
var configration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", true, true)
    .Build();

var services = new ServiceCollection();
services.AddLogging(logging =>
{
    logging.AddConfiguration(configration.GetSection("Logging"));
    logging.AddSystemdConsole(options =>
    {
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
    });
});
services.Configure<CentralServiceOptions>(configration.GetSection("CentralService"))
.AddHttpClient("HttpClientWithSSLUntrusted")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
    });

var serviceProvider = services.BuildServiceProvider();


var BusinessNumber = 65535;

// 6バイト
var EntranceBeaconId = new BeaconId()
{
    ManufactureId = 1,
    IndividualId = 1,
};

// 3バイト
var EntranceCTRLNumber = new CTRLNumber
{
    SerialNumber = 1,
    Port = 1,
    Reserve = 0,
};

// 暗号化鍵
var EntranceEncryptKey = File.ReadAllText(Path.Join("Keys", "entrance_CTRLTerminal_1_1_0.pem"));


var ExitBeaconId = new BeaconId()
{
    ManufactureId = 65535,
    IndividualId = 134217727,
};


var ExitCTRLNumber = new CTRLNumber
{
    SerialNumber = 65535,
    Port = 7,
    Reserve = 0,
};

var ExitEncryptKey = File.ReadAllText(Path.Join("Keys", "exit_CTRLTerminal_65535_7_0.pem"));


// 12バイト
var testCases = new TestCase[] {
    new TestCase {
        CaseNo = "1-1",
        TestNo = 1,
        Title = "1-1 登録されていない車両の入場 登録・照会処理実行（入場）",
        Wcn = "003064647045",
        IsNewSession = true,
        SessionNumber = 1,
        SessionTime = new DateTimeOffset(2022, 6, 24, 16, 0, 0, 0, new TimeSpan(9,0,0)),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "1-2",
        TestNo = 2,
        Title = "1-2 登録されていない車両の入場 登録・照会処理実行（出場）",
        Wcn = "003064647045",
        IsNewSession = true,
        SessionNumber = 2,
        SessionTime = new DateTime(2022, 6, 24, 18, 0, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "2-1",
        TestNo = 3,
        Title = "2-1 登録されている車両の通常入場 登録・照会処理実行（入場）",
        Wcn = "016086767441",
        IsNewSession = true,
        SessionNumber = 2,
        SessionTime = new DateTime(2022, 6, 24, 16, 1, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "2-1",
        TestNo = 4,
        Title = "2-1 登録されている車両の通常入場 確定処理実行（入場）",
        Wcn = "016086767441",
        IsNewSession = false,
        SessionNumber = 2,
        SessionTime = new DateTime(2022, 6, 24, 16, 1, 0, DateTimeKind.Local),
        DelaySec = 2,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.Confirm,
    },
    new TestCase
    {
        CaseNo = "2-2",
        TestNo = 5,
        Title = "2-2 登録されている車両の通常出場 登録・照会処理実行（出場）",
        Wcn = "016086767441",
        IsNewSession = true,
        SessionNumber = 3,
        SessionTime = new DateTime(2022, 6, 24, 18, 1, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "2-2",
        TestNo = 6,
        Title = "2-2 登録されている車両の通常出場 確定処理実行（出場）",
        Wcn = "016086767441",
        IsNewSession = false,
        SessionNumber = 3,
        SessionTime = new DateTime(2022, 6, 24, 18, 1, 0, DateTimeKind.Local),
        DelaySec = 2,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.Confirm,
    },
    new TestCase
    {
        CaseNo = "3-1",
        TestNo = 7,
        Wcn = "021015619719",
        Title = "3-1 発券処理による入場 登録・照会処理実行（入場）",
        IsNewSession = true,
        SessionNumber = 3,
        SessionTime = new DateTime(2022, 6, 24, 16, 2, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "3-1",
        TestNo = 8,
        Title = "3-1 キャンセル処理実行（入場）",
        Wcn = "021015619719",
        IsNewSession = false,
        SessionNumber = 3,
        SessionTime = new DateTime(2022, 6, 24, 16, 2, 0, DateTimeKind.Local),
        DelaySec = 5,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.Cancel,
    },
    new TestCase
    {
        CaseNo = "3-1",
        TestNo = 9,
        Title = "3-1 確定処理実行（入場）",
        Wcn = "021015619719",
        IsNewSession = false,
        SessionNumber = 3,
        SessionTime = new DateTime(2022, 6, 24, 16, 2, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.Confirm,
    },
    new TestCase
    {
        CaseNo = "3-2",
        TestNo = 10,
        Wcn = "021015619719",
        Title = "3-2 発券処理による入場 登録・照会処理実行（出場）",
        IsNewSession = true,
        SessionNumber = 4,
        SessionTime = new DateTime(2022, 6, 24, 18, 2, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "3-2",
        TestNo = 11,
        Wcn = "021015619719",
        Title = "3-2 発券処理による入場 キャンセル処理実行（出場）",
        IsNewSession = false,
        SessionNumber = 4,
        SessionTime = new DateTime(2022, 6, 24, 18, 2, 0, DateTimeKind.Local),
        DelaySec = 5,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.Cancel,
    },
    new TestCase
    {
        CaseNo = "3-2",
        TestNo = 12,
        Wcn = "021015619719",
        Title = "3-2 発券処理による入場 確定処理実行（出場）",
        IsNewSession = false,
        SessionNumber = 4,
        SessionTime = new DateTime(2022, 6, 24, 18, 2, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.Confirm,
    },
    new TestCase
    {
        TestNo = 13,
        CaseNo = "4-1",
        Wcn = "003064678342",
        Title = "4-1 一度バックしてからの再入場  登録・照会処理実行（入場）",
        IsNewSession = true,
        SessionNumber = 4,
        SessionTime = new DateTime(2022, 6, 24, 16, 3, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "4-1",
        TestNo = 14,
        Wcn = "003064678342",
        Title = "4-1 一度バックしてからの再入場  キャンセル処理実行（入場）",
        IsNewSession = false,
        SessionNumber = 4,
        SessionTime = new DateTime(2022, 6, 24, 16, 3, 0, DateTimeKind.Local),
        DelaySec = 5,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.Cancel,
    },
    new TestCase
    {
        CaseNo = "4-1",
        TestNo = 15,
        Wcn = "003064678342",
        Title = "4-1 一度バックしてからの再入場  登録・照会処理実行（入場）",
        IsNewSession = true,
        SessionNumber = 5,
        SessionTime = new DateTime(2022, 6, 24, 16, 4, 0, DateTimeKind.Local),
        DelaySec = 5,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "4-1",
        TestNo = 16,
        Wcn = "003064678342",
        Title = "4-1 一度バックしてからの再入場  確定処理実行（入場）",
        IsNewSession = false,
        SessionNumber = 5,
        SessionTime = new DateTime(2022, 6, 24, 16, 4, 0, DateTimeKind.Local),
        DelaySec = 2,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = EntranceBeaconId,
        CTRLNumber = EntranceCTRLNumber,
        EncryptKey = EntranceEncryptKey,
        RequestType = TestCase.RequestTypes.Confirm,
    },
    new TestCase
    {
        CaseNo = "4-2",
        TestNo = 17,
        Wcn = "003064678342",
        Title = "4-2 一度バックしてからの再出場  登録・照会処理実行（出場）",
        IsNewSession = true,
        SessionNumber = 5,
        SessionTime = new DateTime(2022, 6, 24, 18, 3, 0, DateTimeKind.Local),
        DelaySec = 0,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "4-2",
        TestNo = 18,
        Wcn = "003064678342",
        Title = "4-2 一度バックしてからの再出場  キャンセル処理実行（出場）",
        IsNewSession = false,
        SessionNumber = 5,
        SessionTime = new DateTime(2022, 6, 24, 18, 3, 0, DateTimeKind.Local),
        DelaySec = 10,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.Cancel,
    },
    new TestCase
    {
        CaseNo = "4-2",
        TestNo = 19,
        Wcn = "003064678342",
        Title = "4-2 一度バックしてからの再出場  登録・照会処理実行（入場）",
        IsNewSession = true,
        SessionNumber = 6,
        SessionTime = new DateTime(2022, 6, 24, 18, 4, 0, DateTimeKind.Local),
        DelaySec = 5,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.InquiryAndRecord,
    },
    new TestCase
    {
        CaseNo = "4-2",
        TestNo = 20,
        Wcn = "003064678342",
        Title = "4-2 一度バックしてからの再出場  確定処理実行（出場）",
        IsNewSession = false,
        SessionNumber = 5,
        SessionTime = new DateTime(2022, 6, 24, 18, 4, 0, DateTimeKind.Local),
        DelaySec = 2,
        BusinessNumber = (UInt16)BusinessNumber,
        BeaconId = ExitBeaconId,
        CTRLNumber = ExitCTRLNumber,
        EncryptKey = ExitEncryptKey,
        RequestType = TestCase.RequestTypes.Confirm,
    },
};


var logger = serviceProvider.GetRequiredService<ILogger<CentralServiceApi>>();
var encryptLogger = serviceProvider.GetRequiredService<ILogger<ToPCKKOutPutData>>();
var options = serviceProvider.GetRequiredService<IOptions<CentralServiceOptions>>();
var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
using (var httpClient = httpClientFactory.CreateClient("HttpClientWithSSLUntrusted"))
{
    var centralServiceApi = new CentralServiceApi(logger, options, httpClient);
    var testCase = testCases.Where(t => t.CaseNo == args[0]);
    int sessionNumber = 0;
    DateTimeOffset sessionTime = DateTimeOffset.Now;
    foreach (var test in testCase)
    {
        if (test.IsNewSession)
        {
            sessionNumber = test.SessionNumber;
            sessionTime = DateTimeOffset.Now;
            await Task.Delay(500);
        }
        test.SessionNumber = sessionNumber;
        test.SessionTime = sessionTime;

        if (test.DelaySec > 0)
        {
            await Task.Delay(test.DelaySec * 1000);
        }

        var data = new ToPCKKOutPutData(encryptLogger)
        {
            BusinessNumber = test.BusinessNumber,
            BeaconId = test.BeaconId,
            CTRLNumber = test.CTRLNumber,
            AslId = test.Wcn,
            PublicKey = test.EncryptKey,
        };

        if (test.RequestType == TestCase.RequestTypes.InquiryAndRecord)
        {
            var rawBytes = data.GetBytes();
            using (var file = new FileStream(Path.Join("Data", $"NO_{test.TestNo,2:00}.bin"), FileMode.OpenOrCreate))
            {
                file.Write(rawBytes, 0, rawBytes.Length);
            }
            var requestData = data.ToInquiryAndRecordRequest(
                test.SessionNumber,
                test.SessionTime);
            await centralServiceApi.InquiryAndRecordAsync(requestData, test.TestNo);
        }
        else if (test.RequestType == TestCase.RequestTypes.Confirm)
        {
            var requestData = data.ToConfirmRequest(
                BusinessNumber,
                test.SessionNumber,
                test.SessionTime);
            await centralServiceApi.Confirm(requestData, test.TestNo);
        }
        else if (test.RequestType == TestCase.RequestTypes.Cancel)
        {
            var requestData = data.ToCancelRequest(
                BusinessNumber,
                test.SessionNumber,
                test.SessionTime);
            await centralServiceApi.Cancel(requestData, test.TestNo);
        }
    }
}
