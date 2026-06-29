using System;
using System.IO;

class Program
{
    static string path = @"C:\-\-\-\VS\VS\bank.txt";

    static void Main()
    {
        Console.WriteLine("BANK SYSTEM");
        Console.WriteLine("1 - Login");
        Console.WriteLine("2 - Register");

        string? choice = Console.ReadLine();

        if (choice == "1") Login();
        else if (choice == "2") Register();
        else Console.WriteLine("Invalid choice.");
    }

    // ---------------- REGISTER ----------------
    static void Register()
    {
        Console.WriteLine("REGISTER");

        Console.Write("Username: ");
        string? username = Console.ReadLine();

        Console.Write("Password: ");
        string password = ReadPassword();

        int balance = new Random().Next(1000, 10000);
        string iban = "LP" + new Random().Next(10000000, 99999999);

        File.AppendAllText(path, $"{username},{password},{balance},{iban}\n");

        Console.WriteLine($"Registered!");
        Console.WriteLine($"IBAN: {iban}");
        Console.WriteLine($"Balance: {balance}");
    }

    // ---------------- LOGIN ----------------
    static void Login()
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("No users.");
            return;
        }

        string[] users = File.ReadAllLines(path);

        Console.WriteLine("LOGIN");

        string? username;
        string? password;

        int userIndex = -1;
        int balance = 0;

        while (true)
        {
            Console.Write("Username: ");
            username = Console.ReadLine();

            Console.Write("Password: ");
            password = ReadPassword();

            bool found = false;

            for (int i = 0; i < users.Length; i++)
            {
                string[] data = users[i].Split(',');

                if (data.Length == 4 &&
                    data[0] == username &&
                    data[1] == password)
                {
                    found = true;
                    userIndex = i;
                    balance = int.Parse(data[2]);
                    break;
                }
            }

            if (found)
            {
                Console.WriteLine("Login successful");
                break;
            }

            Console.WriteLine("Wrong credentials\n");
        }

        Menu(users, userIndex, ref balance);
    }

    // ---------------- MENU ----------------
    static void Menu(string[] users, int userIndex, ref int balance)
    {
        while (true)
        {
            Console.WriteLine($"\nBalance: {balance}");
            Console.WriteLine("1 - Withdraw");
            Console.WriteLine("2 - Send Money (IBAN)");
            Console.WriteLine("3 - Exit");

            string? input = Console.ReadLine();

            if (input == "1")
                Withdraw(users, userIndex, ref balance);

            else if (input == "2")
                SendMoney(users, userIndex, ref balance);

            else if (input == "3")
            {
                File.WriteAllLines(path, users);
                Console.WriteLine("Logged out");
                return;
            }

            else
                Console.WriteLine("Invalid option");
        }
    }

    // ---------------- WITHDRAW ----------------
    static void Withdraw(string[] users, int userIndex, ref int balance)
    {
        Console.Write("Amount: ");

        if (!int.TryParse(Console.ReadLine(), out int amount))
            return;

        if (amount > balance)
        {
            Console.WriteLine("Not enough money");
            return;
        }

        balance -= amount;

        string[] data = users[userIndex].Split(',');
        data[2] = balance.ToString();
        users[userIndex] = string.Join(",", data);

        File.WriteAllLines(path, users);

        Console.WriteLine("Withdraw OK");
    }

    // ---------------- SEND MONEY ----------------
    static void SendMoney(string[] users, int userIndex, ref int balance)
    {
        Console.Write("Receiver IBAN: ");
        string? iban = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(iban))
        {
            Console.WriteLine("Invalid IBAN");
            return;
        }

        Console.Write("Amount: ");

        if (!int.TryParse(Console.ReadLine(), out int amount))
            return;
        if (balance > amount) {
            Console.WriteLine("Wrong Amount Try again");
        }
        int receiverIndex = FindByIBAN(users, iban);

        if (receiverIndex == -1)
        {
            Console.WriteLine("IBAN not found");
            return;
        }

        if (amount > balance)
        {
            Console.WriteLine("Not enough money");
            return;
        }

        // sender
        balance -= amount;
        string[] sender = users[userIndex].Split(',');
        sender[2] = balance.ToString();
        users[userIndex] = string.Join(",", sender);

        // receiver
        string[] receiver = users[receiverIndex].Split(',');
        int rBalance = int.Parse(receiver[2]);
        rBalance += amount;
        receiver[2] = rBalance.ToString();
        users[receiverIndex] = string.Join(",", receiver);

        File.WriteAllLines(path, users);

        Console.WriteLine("Transfer success");
    }

    // ---------------- IBAN SEARCH ----------------
    static int FindByIBAN(string[] users, string iban)
    {
        for (int i = 0; i < users.Length; i++)
        {
            string[] data = users[i].Split(',');

            if (data.Length == 4 && data[3] == iban)
                return i;
        }
        return -1;
    }

    // ---------------- PASSWORD INPUT ----------------
    static string ReadPassword()
    {
        string pass = "";
        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
                break;

            if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
            {
                pass = pass[..^1];
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Backspace)
            {
                pass += key.KeyChar;
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return pass;
    }
}