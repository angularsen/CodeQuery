# CodeQuery
Experimenting with Roslyn to analyze and query code, until ReSharper or Rider/Visual Studio gets native support for something like this.


### Examples
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
00:00:01.6212122 001 CodeQuery.Program.NormalDateTime(System.DateTime)
00:00:01.6508902 002 CodeQuery.Program.NullableDateTime1(System.DateTime?)
00:00:01.6602779 003 CodeQuery.Program.NullableDateTime2(System.DateTime?)
```
