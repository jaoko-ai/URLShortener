namespace URLShortener.utilities;

public class Utilities
{

    const string ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";


    public static string Encode(int num)
    {
        string output = "";
        if (num == 0)
        {
            return "0";
        }
        while (num > 0)
        {
            output = $"{ALPHABET[num % 62]}{output}";
            num /= 62;
        }
        return output;
    }



    public static int Decode(string str)
    {
        int result = 0;
        foreach (char character in str)
        {
            var value = ALPHABET.IndexOf(character);
            if (value == -1) throw new ArgumentException($"Invalid Base62 character: {character}");
            result = result * 62 + value;
        }
        return result;
    }
}