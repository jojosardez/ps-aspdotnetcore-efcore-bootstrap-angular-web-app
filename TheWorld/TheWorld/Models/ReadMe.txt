Migrations will be done outside the IDE using the command prompt.
The project file must be modified first to add the following entries:


    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="1.1.3" />

Should be added before Microsoft.EntityFrameworkCore.SqlServer and:

  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.1" />
  </ItemGroup>

Should be added as a separate group. Once added the following command should work in command prompt, running at the projects location:

	dotnet ef --help

This command will then create the initial migration based on the DbContext found in the project:

	dotnet ef migrations add InitialDatabase

This will execute the migrations and update the database:

	dotnet ef database update

