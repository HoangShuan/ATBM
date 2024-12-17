using System.IO;
using System.Security.Cryptography;
using System.Text;

public class ClassAES
{
    public string MaHoa(string plainText, string key)
    {
            byte[] encryptedBytes = EncryptAES(Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(key));
            string encryptedText = Convert.ToBase64String(encryptedBytes);

            return encryptedText;
    }

    public string GiaiMa(string plainText, string key)
    {
        byte[] encryptedBytes = Convert.FromBase64String(plainText);
        byte[] decryptedBytes = DecryptAES(encryptedBytes, Encoding.UTF8.GetBytes(key));
        string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

        return decryptedText;
    }

    public byte[] EncryptAES(byte[] plainBytes, byte[] keyBytes)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.Mode = CipherMode.CBC; 
            aesAlg.GenerateIV();

            using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                   
                    byte[] encrypted = new byte[aesAlg.IV.Length + msEncrypt.ToArray().Length];
                    Array.Copy(aesAlg.IV, encrypted, aesAlg.IV.Length);
                    Array.Copy(msEncrypt.ToArray(), 0, encrypted, aesAlg.IV.Length, msEncrypt.ToArray().Length);

                    return encrypted;
                }
            }
        }
    }

    public byte[] DecryptAES(byte[] cipherBytes, byte[] keyBytes)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.Mode = CipherMode.CBC;

            byte[] iv = new byte[aesAlg.BlockSize / 8];
            Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
            aesAlg.IV = iv;

            byte[] cipherText = new byte[cipherBytes.Length - iv.Length];
            Array.Copy(cipherBytes, iv.Length, cipherText, 0, cipherText.Length);

            using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            {
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream msPlainText = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlainText);
                            return msPlainText.ToArray();
                        }
                    }
                }
            }
        }
    }
}
