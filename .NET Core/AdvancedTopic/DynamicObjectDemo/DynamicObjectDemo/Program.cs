using DynamicObjectDemo;

dynamic rFile = new DynamicObjectFromFile(@"..\..\..\txtFile.txt");
foreach (string line in rFile.Customer)
{
    Console.WriteLine(line);
}
Console.WriteLine("----------------------------");
foreach (string line in rFile.Supplier(StringSearchOption.Contains, true))
{
    Console.WriteLine(line);
}