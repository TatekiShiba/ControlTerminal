using System.Security.Cryptography;
using System.Text;
using NullFX.CRC;

Console.Write("Enter encrypted data > ");
var encryptedData = Console.ReadLine();
var privateKey = File.ReadAllText(Path.Join("Keys", "private-key.pem"));

var decrypted = Decrypt(encryptedData!, privateKey);
var decryptedData = decrypted.Take(decrypted.Length - 2).ToArray();
Console.WriteLine($"Decripted   :{string.Join(" ", decryptedData.Select(d => $"{d:X2}"))}");

var crc = decrypted.Skip(decrypted.Length - 2).ToArray();
Console.WriteLine($"DecriptedCrcBytes:{BitConverter.ToUInt16(crc)}({string.Join("", crc.Select(b => $"{b:X2}"))})");

Console.WriteLine($"CRC Check: {Crc16.ComputeChecksum(Crc16Algorithm.Ccitt, decrypted)}");

static byte[] Decrypt(string encryptedDataBase64String, string privateKey)
{
    var data = Convert.FromBase64String(encryptedDataBase64String);
    using (var rsa = RSA.Create())
    {
        // rsa.ImportParameters(privateKey);
        rsa.ImportFromPem(privateKey);
        return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
    }
}
