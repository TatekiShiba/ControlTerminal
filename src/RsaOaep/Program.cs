using System.Security.Cryptography;
using System.Text;
using NullFX.CRC;

// publicKeyとprivateKeyの生成
var keys = GenerateKeys(3072);

// // 3バイト
var CTRLNumber = Encoding.ASCII.GetBytes("012");
// 6バイト
var BeaconId = Encoding.ASCII.GetBytes("345678");
// 12バイト
var AslId = Encoding.ASCII.GetBytes("0123456789ab");

Console.WriteLine($"Source   :{Encoding.ASCII.GetString(CTRLNumber.Concat(BeaconId).Concat(AslId).ToArray())}");
var crc = Crc16.ComputeChecksum(
    Crc16Algorithm.Ccitt,
    CTRLNumber.Concat(BeaconId).Concat(AslId).ToArray());
Console.WriteLine($"SourceCrc:{crc:X4}");

var data = CTRLNumber
    .Concat(BeaconId)
    .Concat(AslId)
    .Concat(BitConverter.GetBytes(crc))  // uint16のエンディアン
    .ToArray();

var encryptedData = Encrypt(data, keys.publicKey);
Console.WriteLine($"Encripted:{encryptedData}");

var decrypted = Decrypt(encryptedData, keys.privateKey);
var decryptedData = decrypted.Take(decrypted.Length - 2).ToArray();
var decryptedCrc = BitConverter.ToUInt16(decrypted, decrypted.Length - 2);
Console.WriteLine($"Decripted   :{Encoding.ASCII.GetString(decryptedData)}");
Console.WriteLine($"DecriptedCrc:{decryptedCrc:X4}");
Console.WriteLine($"CRC Check: {Crc16.ComputeChecksum(Crc16Algorithm.Ccitt, decrypted)}");

Console.ReadLine();

//static (RSAParameters publicKey, RSAParameters privateKey) GenerateKeys(int keyLength)
static (string publicKey, string privateKey) GenerateKeys(int keyLength)
{
    using (var rsa = RSA.Create())
    {
        rsa.KeySize = keyLength;

        // return (
        //     publicKey: rsa.ExportParameters(includePrivateParameters: false),
        //     privateKey: rsa.ExportParameters(includePrivateParameters: true)
        // );
        return (
            publicKey: rsa.ToXmlString(false),
            privateKey: rsa.ToXmlString(true)
        );
    }
}

// static string Encrypt(byte[] data, RSAParameters publicKey)
static string Encrypt(byte[] data, string publicKey)
{
    using (var rsa = RSA.Create())
    {
        // rsa.ImportParameters(publicKey);
        rsa.FromXmlString(publicKey);

        var result = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(result);
    }
}

// static byte[] Decrypt(string encryptedDataBase64String, RSAParameters privateKey)
static byte[] Decrypt(string encryptedDataBase64String, string privateKey)
{
    var data = Convert.FromBase64String(encryptedDataBase64String);
    using (var rsa = RSA.Create())
    {
        // rsa.ImportParameters(privateKey);
        rsa.FromXmlString(privateKey);
        return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
    }
}


