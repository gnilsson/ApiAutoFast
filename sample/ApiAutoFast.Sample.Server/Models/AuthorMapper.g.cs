using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;
using Mapster.Utils;

namespace ApiAutoFast.Sample.Server.Database
{
    public static partial class AuthorMapper
    {
        public static AuthorResponse AdaptToResponse(this Author p1)
        {
            return p1 == null ? null : new AuthorResponse()
            {
                Id = p1.Id.ToString(),
                CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
                ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                FirstName = p1.FirstName,
                LastName = p1.LastName,
                Profession = p1.Profession == null ? null : Enum<ProfessionCategory>.ToString((ProfessionCategory)p1.Profession)
            };
        }
        public static AuthorResponse AdaptTo(this Author p2, AuthorResponse p3)
        {
            if (p2 == null)
            {
                return null;
            }
            AuthorResponse result = p3 ?? new AuthorResponse();
            
            result.Id = p2.Id.ToString();
            result.CreatedDateTime = ((IEntity)p2).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm");
            result.ModifiedDateTime = ((IEntity)p2).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.FirstName = p2.FirstName;
            result.LastName = p2.LastName;
            result.Profession = p2.Profession == null ? null : Enum<ProfessionCategory>.ToString((ProfessionCategory)p2.Profession);
            return result;
            
        }
    }
}