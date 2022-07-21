
using FastEndpoints;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorMappingProfile : Mapper<AuthorCreateCommand, AuthorResponse, Author>
{
    public override AuthorResponse FromEntity(Author e)
    {
        return e.MapToResponse();
    }

    public Author UpdateDomainEntity(Author entity, AuthorModifyCommand command, Action<string, string> addValidationError)
    {
        entity.FirstName = FirstName.UpdateFromRequest(entity.FirstName, command.FirstName, addValidationError);
        entity.LastName = LastName.UpdateFromRequest(entity.LastName, command.LastName, addValidationError);
        return entity;
    }

    public Author ToDomainEntity(AuthorCreateCommand command, Action<string, string> addValidationError)
    {
        return new Author
        {
            FirstName = FirstName.ConvertFromRequest(command.FirstName, addValidationError),
            LastName = LastName.ConvertFromRequest(command.LastName, addValidationError),
        };
    }
}
