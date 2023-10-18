how to check nullable variable null or not: 
- use .HasValue or
- "is operator with type pattern" - "a is int valueOfA" where valueOfA is the declaration pattern, which only when is condition matches it will declare a new variable
```
int? a = null;
Console.WriteLine("a = {0}", a.HasValue?1:0);
if (a is int valueOfA)
{
    Console.WriteLine($"a is {valueOfA}");
}
else
{
    Console.WriteLine("a does not have a value");
}
// Output:
// a is 42
```
