# ApiAutoFast
### Fast and automagical, pristine API

Build a REST api around your entities with simple generator marker attributes.
Define type conversion from request to entity with validation on a per property basis, aswell as entity to response.
Access & add to the partial classes and modify & extend generated endpoints by inheritance.


### Required packages


```console
dotnet add package ApiAutoFast --version 0.6.1-beta
dotnet add package ApiAutoFast.SourceGenerator --version 0.6.1-beta
```

Microsoft.EntityFrameworkCore.Design is also required to generate the tables.
https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/

### The setup


```C#
[AutoFastEntity]
public sealed class AuthorEntity
{
    public Name Name { get; set; } = default!;
    public BlogsRelation Blogs { get; set; } = default!;
}
```

Will generate an EF dbset named Author with an x-to-many relationship to Blog.

```C#
public sealed class Name : DomainValue<string, Name>
{ }
```
Configures property "Name" to have a request, database (entity), and response type as string.

```C#
public sealed class BlogsRelation : DomainValue<ICollection<Blog>, BlogsRelation>
{ }
```
Configures property "Blogs" to be an x-to-many relationship.

```C#
[AutoFastContext]
public partial class DemoAafContext
{ }
```
Configures EF context, for clarity could explicitly inherit DbContext.
When marked will trigger registered entities to generate crud endpoints.

```C#
[AutoFastEndpoint]
public class MyCustom : CreateBlogEndpoint
{
    public MyCustom(DemoAafContext dbContext) : base(dbContext)
    { }

    public override Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }
}
```
Optionally modify generated endpoint.
Attribute will change endpoint from partial to abstract and place handle operation in method HandleRequest.


### Startup


```C#
builder.Services.AddAutoFast(o =>
{
    o.AssemblyType = typeof(Program);
});
```
Simple startup registration.

Refer to /sample project for more info regarding config & features.

There also exists some build functions in .csproj that is useful when dealing with generated files, for marking them to source control etc.


jolly sorcery 
