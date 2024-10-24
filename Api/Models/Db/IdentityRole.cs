using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Api.Models.Db;

[CollectionName("Roles")]
public class IdentityRole : MongoIdentityRole<Guid>
{
    public const string Admin = "ADMIN";
    public const string User = "USER";
    
    public IdentityRole() { }
    public IdentityRole(string roleName) : base (roleName) { }
}
