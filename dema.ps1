$name=$args[0]
dotnet ef migrations add $name -p Web/Web.csproj -o ../DAL/Migrations