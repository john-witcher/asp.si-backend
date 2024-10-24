namespace Api.Models.Db;

public class ChangeLog
{
    public Guid DocumentId { get; set; } // Reference to the original document (assumed as Guid for unique identifier)
    public string ModifiedBy { get; set; } // User who made the change
    public DateTime ModifiedAt { get; set; } // Timestamp of the change
    
    private List<Change> _changes = new();
    public IReadOnlyList<Change> Changes => _changes.AsReadOnly();
    

    public ChangeLog(Guid documentId, string modifiedBy)
    {
        DocumentId = documentId;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void AddChange(string propertyName, object oldValue, object newValue)
    {
        var changeDetail = new Change(propertyName, oldValue, newValue);
        _changes.Add(changeDetail);
    }
    
    public void AddChanges(List<Change> changes)
    {
        _changes.AddRange(changes);
    }
}

public class Change
{
    public string PropertyName { get; set; }
    public object OldValue { get; set; }
    public object NewValue { get; set; }

    public Change(string propertyName, object oldValue, object newValue)
    {
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}