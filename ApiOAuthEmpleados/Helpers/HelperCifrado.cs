using System.Security.Cryptography;
using System.Text;

namespace ApiOAuthEmpleados.Helpers
{
    public static class HelperCifrado
    {
        public static string EncryptString(string keyString, string plainText)
        {
            // Convertimos tu string de 32 caracteres a un array de 32 bytes
            byte[] key = Encoding.UTF8.GetBytes(keyString);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV(); // Genera un IV aleatorio y seguro para cada cifrado

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Escribimos el IV al principio del stream para no perderlo
                    memoryStream.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                    }

                    // Retornamos todo (IV + Texto cifrado) en formato Base64
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string DecryptString(string keyString, string cipherText)
        {
            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Extraemos los primeros 16 bytes, que corresponden al IV aleatorio que guardamos
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Leemos el resto del stream (saltándonos los 16 bytes del IV)
                using (MemoryStream memoryStream = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
