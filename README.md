# CodeQuery
Experimenting with Roslyn to analyze and query code, until ReSharper or Rider/Visual Studio gets native support for something like this.


### Examples
```
$ dotnet run
Supported commands:
methods solution-or-project-or-source-file [--projects regex] [--namespaces regex] [--method-param-type regex]

Examples:
---

Query all methods with a parameter of type System.DateTime -or- System.DateTime?
CodeQuery.exe methods "C:/dev/CodeQuery/CodeQuery.sln" --projects "^(CodeQuery)$" --method-param-type "System.DateTime\??"
```

#### Find all methods with a parameter of type `System.DateTime` or the nullable `System.DateTime?`
Note that `Nullable<T>` and `T?` is handled as if the same.

```cmd
dotnet run -- methods ../CodeQuery.sln --method-param-type "System.DateTime\??"
```

Output
```
File: C:/dev/CodeQuery/CodeQuery.sln
Projects: (all)
Namespaces: (all)
Method param type: System.DateTime\??
00:00:01.1675201 001 CodeQuery.SampleClass.NormalDateTime(System.DateTime)
00:00:01.2013137 002 CodeQuery.SampleClass.NullableDateTime1(System.DateTime?)
00:00:01.2120629 003 CodeQuery.SampleClass.NullableDateTime2(System.DateTime?)
```
