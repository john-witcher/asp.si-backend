using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Api.Models.Db;

[CollectionName("Users")]
public sealed class IdentityUser : MongoIdentityUser<Guid>
{
    public GlobalSettings? GlobalSettings { get; set; }
}

public class GlobalSettings
{
    public List<string> MenuButtons { get; set; } = new ();
}