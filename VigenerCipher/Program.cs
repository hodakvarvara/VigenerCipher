using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

string pathAlph = "C:\\Документы\\8Sem\\Kriptografia\\VigenerCipher\\inputAlp.txt";
string pathText = "C:\\Документы\\8Sem\\Kriptografia\\VigenerCipher\\input.txt";
string pathKriptogramma = "C:\\Документы\\8Sem\\Kriptografia\\VigenerCipher\\Kriptogramma.txt";
string pathKey = "C:\\Документы\\8Sem\\Kriptografia\\VigenerCipher\\Key.txt";
string pathDecryptText = "C:\\Документы\\8Sem\\Kriptografia\\VigenerCipher\\DecryptText.txt";

// пронумеровали алфавит 
Dictionary<char, int> alphavit = inputAlphavitAsync(pathAlph).Result;
alphavit['\r'] = alphavit.Count();
alphavit['\n'] = alphavit.Count();

string plainText = inputText(pathText);
Console.WriteLine($"--------------");
Console.WriteLine($"Открытый текст:\n{plainText}");

plainText = plainText.ToLower();
for(int i = 0; i < plainText.Length; i++)
{
    var symbol = plainText[i];
    if (!alphavit.ContainsKey(symbol))
    {
        plainText = plainText.Remove(i, 1);
        i--;
    }
}

interfaceUser();

void interfaceUser()
{
    Console.WriteLine($"\n--------------");
    Console.WriteLine("Введите 1 - если хотите зашифровать текст");
    Console.WriteLine("2 - если хотите расшифровать текст");
    Console.WriteLine("0 - выход");
    Console.WriteLine($"--------------");
    int number = Convert.ToInt32(Console.ReadLine());

    if (number == 1)
    {
        List<char> keys = alphavit.Keys.ToList();
        Random random = new Random();
        char randomKey = keys.ElementAt(random.Next(keys.Count));
        Console.WriteLine($"--------------");
        Console.Write($"Рандомный ключ:\n{randomKey}\n");
        outputText(pathKey, randomKey.ToString());

        string encryptedText = encryptTextVigener(plainText, alphavit, randomKey);
        Console.WriteLine($"\n--------------");
        Console.Write($"Зашифрованный текст:\n{encryptedText}");
        outputText(pathKriptogramma, encryptedText);
        interfaceUser();
    }
    else if (number == 2)
    {
        string encryptedText = inputText(pathKriptogramma);
        string randomKey = inputText(pathKey);
        char randomKeyChar = randomKey[0];
        string decryptText = decryptTextVigener(encryptedText, alphavit, randomKeyChar);
        Console.WriteLine($"\n--------------");
        Console.Write($"Расшифрованный текст:\n{decryptText}");
        Console.WriteLine($"\n--------------");
        outputText(pathDecryptText, decryptText);
        interfaceUser();
    }
    else if (number == 0)
    {
        Console.WriteLine("Exit");
    }
    else
    {
        Console.WriteLine("Error");
    }

}
string inputText (string path)
{
    string textFromFile = File.ReadAllText(path);
    return textFromFile;
}
void outputText(string path, string Text)
{
    File.WriteAllText(path, Text);
}
async Task<Dictionary<char, int>> inputAlphavitAsync(string path)
{
    Dictionary<char, int> alphaMap = new Dictionary<char, int>();
    using (FileStream fstream = File.OpenRead(path))
    {
        byte[] buffer = new byte[fstream.Length];
        await fstream.ReadAsync(buffer, 0, buffer.Length);
        string textFromFile = Encoding.Default.GetString(buffer);
        int i = 0;
        foreach(var item in textFromFile)
        {
            char curentLetter = item;
            alphaMap[curentLetter] = i;
            i++;
        }
    }
    return alphaMap;
}



string encryptTextVigener(string text, Dictionary<char, int> alphavit, char randomKey )
{
   
    string key = randomKey + text;
    List<int> textNumber = convertTextToNumber(text, alphavit);
    List<int> keyNumber = convertTextToNumber(key, alphavit);
    List<int> encryptedTextNumber = new List<int>(); 
    string encryptedText = "";
    
    for (int i = 0; i < textNumber.Count(); i++)
    {
        var value = (textNumber[i] + keyNumber[i]) % alphavit.Count;
        encryptedTextNumber.Add(value);
        encryptedText += alphavit.FirstOrDefault(x => x.Value == value).Key;
    }
    Console.WriteLine($"--------------");
    Console.Write($"Код зашифрованного текста:\n");
    encryptedTextNumber.ForEach(number => Console.Write($" {number}"));
    
    return encryptedText;
}


List<int> convertTextToNumber(string text, Dictionary<char, int> alphavit)
{
    List<int> textNumber = new List<int>();
    foreach (var item in text)
    {
        textNumber.Add(alphavit[item]);
    }
    return textNumber;
}

string decryptTextVigener(string encryptedText, Dictionary<char, int> alphavit, char key)
{
    List<int> decryptedTextNumber = new List<int>(); 
    string decryptedTextChar = "";
    List<int> encryptedTextNumber = convertTextToNumber(encryptedText, alphavit); // зашифрованный текст в цифрах
    List<int> keyNumber = new List<int>();
    keyNumber.Add(alphavit[key]);

    for (int i = 0; i < encryptedTextNumber.Count(); i++)
    {
        int value;
        if (encryptedTextNumber[i] - keyNumber[i] < 0)
        {
            value = alphavit.Count - (keyNumber[i] - encryptedTextNumber[i]);
        }
        else
        {
            value = encryptedTextNumber[i] - keyNumber[i]; 
        }
        decryptedTextNumber.Add(value);
        keyNumber.Add(value);
        decryptedTextChar += alphavit.FirstOrDefault(x => x.Value == value).Key;
    }

    return decryptedTextChar;
}