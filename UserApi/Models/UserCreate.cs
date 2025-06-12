
public class UserCreate
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool Admin { get; set; } = false;
    public required DateTime CreatedOn { get; set; }
    public required string CreatedBy { get; set; }
}



