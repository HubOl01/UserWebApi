public class UserUpdate
{
    public string? Name { get; set; }

    public int? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string ModifiedBy { get; set; } = "";
    public DateTime? RevokedOn { get; set; }
    public string? RevokedBy { get; set; }

}