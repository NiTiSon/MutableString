# MutableString [![Build](https://github.com/NiTiSon/MutableString/actions/workflows/tests.yml/badge.svg)](https://github.com/NiTiSon/MutableString/actions/workflows/tests.yml)

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


## Installing

### Via PackageReference
Append `PackageReference` node to your C# project file
```csproj
<Project Sdk="Microsoft.NET.Sdk">
...
	<ItemGroup>
		...
		<!-- Append next line into your project file -->
		<PackageReference Include="MutableString" />
	</ItemGroup>
...
</Project>
```
### Via .NET CLI
```sh
dotnet add package MutableString
```

## Contributing
Contributions are welcome!
Please read [CONTRIBUTING.md](https://github.com/NiTiSon/MutableString/blob/stable/CONTRIBUTING.md) before making any contribution to repository.

## License
This project licensed under [MIT license](https://raw.githubusercontent.com/NiTiSon/MutableString/refs/heads/stable/LICENSE)
