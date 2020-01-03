# CodeQuery
Experimenting with Roslyn to analyze and query code, until ReSharper or Rider/Visual Studio gets native support for something like this.


### Examples
#### Find all methods with a parameter of type `System.DateTime` or the nullable `System.DateTime?`
Note that `Nullable<T>` and `T?` is handled as if the same.

```cmd
dotnet run -- methods ../CodeQuery.sln --method-param-type "System.DateTime\??"
```
