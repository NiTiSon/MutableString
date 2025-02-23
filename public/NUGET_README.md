# MutableString

`MutableString` is a lightweight string manipulation library designed to enhance performance
and reduce memory allocations in scenarios where string manipulations are frequent.
Unlike traditional immutable strings in .NET, `MutableString` allows you to modify string content
without incurring the overhead of creating new string instances for each operation.

This package provides new types:
+ NiTiS.MutableString

## Using

```cs
using System;
using NiTiS;

class Example
{
    void Main()
    {
        MutableString str = "Hello World";

        str.Insert(0, "> ");

        Console.WriteLine(str); // Output: "> Hello World"
    }
}
```

## Contributing
Contributions are welcome!
Please read [CONTRIBUTING.md](https://github.com/NiTiSon/MutableString/blob/stable/CONTRIBUTING.md) before making any contribution to repository.