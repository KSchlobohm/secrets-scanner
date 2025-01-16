import random
import csv

def create_test_data_with_passwords():
    # Top 50 most common passwords
    common_passwords = [
        "123456", "password", "123456789", "12345678", "12345", "1234567", "qwerty", "abc123", "password1", "123123",
        "admin", "letmein", "welcome", "iloveyou", "1234", "1q2w3e4r", "sunshine", "1234567890", "000000", "password123",
        "654321", "superman", "123qwe", "trustno1", "passw0rd", "monkey", "football", "batman", "charlie", "696969",
        "ninja", "master", "zaq12wsx", "ashley", "baseball", "michael", "football1", "shadow", "jessica", "password!",
        "superuser", "hunter2", "flower", "tigger", "qwertyuiop", "admin123", "test123", "dragon", "puppy", "peanut"
    ]

    # Generate test data
    rows = []
    for i in range(100):
        password = random.choice(common_passwords)
        variable_name = random.choice(["password", "pass", "secret", "key", "credentials"])
        constant_or_variable = random.choice(["const", "var"])
        data_type = random.choice(["string", "String"])
        snippet = f'public {constant_or_variable} {data_type} {variable_name}{i} = "{password}";'
        label = 1  # 1 means this snippet contains a hardcoded secret
        rows.append({"text": snippet, "label": label})

    return rows

def create_test_data_without_passwords():
    # Examples of non-secret C# code snippets
    non_secret_code_snippets = [
        'public int Add(int a, int b) { return a + b; }',
        'Console.WriteLine("Hello, World!");',
        'public string GetGreeting(string name) { return $"Hello, {name}!"; }',
        'for (int i = 0; i < 10; i++) { Console.WriteLine(i); }',
        'public class Person { public string Name { get; set; } public int Age { get; set; } }',
        'List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };',
        'DateTime now = DateTime.Now;',
        'try { File.ReadAllText("example.txt"); } catch (Exception ex) { Console.WriteLine(ex.Message); }',
        'string[] fruits = { "apple", "banana", "cherry" };',
        'public bool IsEven(int number) { return number % 2 == 0; }',
        'Dictionary<string, int> scores = new Dictionary<string, int> { { "Alice", 10 }, { "Bob", 20 } };',
        'int[] numbers = Enumerable.Range(1, 100).ToArray();',
        'double CalculateArea(double radius) { return Math.PI * radius * radius; }',
        'public interface IVehicle { void Drive(); }',
        'public enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }',
        'using (StreamReader sr = new StreamReader("file.txt")) { Console.WriteLine(sr.ReadToEnd()); }',
        'Thread.Sleep(1000);',
        'int sum = numbers.Sum();',
        'public delegate int MathOperation(int x, int y);',
        'Task.Delay(1000).Wait();',
        'File.WriteAllText("output.txt", "Hello, File!");',
        'public abstract class Shape { public abstract double GetArea(); }',
        'Console.ReadLine();',
        'public static T Max<T>(T a, T b) where T : IComparable<T> { return a.CompareTo(b) > 0 ? a : b; }',
        'public void PrintList<T>(IEnumerable<T> items) { foreach (var item in items) Console.WriteLine(item); }'
    ]

    # Generate 100 rows of random non-secret C# code
    rows = []
    for i in range(100):
        snippet = random.choice(non_secret_code_snippets)
        label = 0  # 0 means this snippet does not contain a hardcoded secret
        rows.append({"text": snippet, "label": label})

    return rows

def save_to_csv(rows, output_file):
    with open(output_file, mode="w", newline="", encoding="utf-8") as file:
        writer = csv.DictWriter(file, fieldnames=["text", "label"])
        writer.writeheader()
        writer.writerows(rows)

if __name__ == "__main__":
    # CSV file names
    output_train_file = "csharp_train_data.csv"
    output_test_file = "csharp_test_data.csv"

    sans_password_data = create_test_data_without_passwords()
    print(f"Generated 100 rows of non-secret C# test data.")

    password_data = create_test_data_with_passwords()
    print(f"Generated 100 rows of C# test data.")

    combined_data = password_data + sans_password_data
    random.shuffle(combined_data)
    print(f"Combined and randomized rows")

    # slice 20% of the data for testing
    test_data = combined_data[:40]
    save_to_csv(test_data, output_test_file)
    print(f'Saved test data to "{output_test_file}"')
    
    # slice 80% of the data for training
    train_data = combined_data[160:]
    save_to_csv(train_data, output_train_file)
    print(f'Saved training data to "{output_train_file}"')
