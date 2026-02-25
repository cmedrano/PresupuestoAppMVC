using System.Security.Cryptography;
using System.Text;
public class GeneratePassword
{
    public static string GenerateRandomPassword(int length = 12)
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string symbols = "!@#$%^&*()_-+=";

        string allChars = upper + lower + numbers + symbols;

        var password = new StringBuilder();
        using var rng = RandomNumberGenerator.Create();

        byte[] buffer = new byte[sizeof(uint)];

        for (int i = 0; i < length; i++)
        {
            rng.GetBytes(buffer);
            uint num = BitConverter.ToUInt32(buffer, 0);
            password.Append(allChars[(int)(num % (uint)allChars.Length)]);
        }

        return password.ToString();
    }
}
