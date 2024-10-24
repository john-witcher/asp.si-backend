using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Api.Models.Db;

[CollectionName("Cars")]
public class Car
{
    [BsonId]
    public Guid Id { get; set; }
    public required string Model { get; set; }
    public required string Color { get; set; }
    public required string VinNumber { get; set; }
    public List<string> AdditionalEquipment { get; set; } = new(); 
    
    public required float Price { get; set; }
    public Discount? Discount { get; set; }
    public required Status Status { get; set; }
    public string? Buyer { get; set; }
    
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public int ChangesAmount { get; set; }
    
    private List<ChangeLog> _changeLog = new();
    public IReadOnlyList<ChangeLog> ChangeLog => _changeLog.AsReadOnly();

    public Car(string createdBy, string lastModifiedBy)
    {
        CreatedBy = createdBy;
        LastModifiedBy = lastModifiedBy;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void NewChangeLog(string modifiedBy, List<Change> changes)
    {
        var changeLog = new ChangeLog(Id, modifiedBy);
        changeLog.AddChanges(changes);
        _changeLog.Add(changeLog);
    }
}

public enum Status
{
    NotAvailable = 0,  // Default value (not ordered, not in stock)
    Ordered = 1,       // Item is ordered but not yet in stock
    InStock = 2,       // Item is in stock
    BackOrdered = 3    // Item is ordered but out of stock (backordered)
}

public class Discount
{
    public required float Total { get; set; }
    public required int Percentage { get; set; }
    public required float Value { get; set; }
}
