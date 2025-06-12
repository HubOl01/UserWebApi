// namespace UserApi.Services;


using System.Text.RegularExpressions;

public class UserService
{
    private static readonly List<User> users = new List<User>();

    public void CreateInitialAdmin()
    {
        if (!users.Any())
        {
            users.Add(new User
            {
                Login = "admin",
                Password = "pass_admin",
                Name = "Admin",
                Gender = 1,
                Admin = true,
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow,
            });
        }
    }
    public bool Add(User user)
    {
        if (LoginExists(user.Login)) return false;
        if (!IsValidLogin(user.Login)) return false;
        if (!IsValidName(user.Name)) return false;
        if (!IsValidPassword(user.Password)) return false;
        users.Add(user);
        return true;
    }

    private bool IsValidPassword(string password)
    {
        return Regex.IsMatch(password, @"^[a-zA-Z0-9]+$");
    }
    private bool IsValidLogin(string login)
    {
        return Regex.IsMatch(login, @"^[a-zA-Z0-9]+$");
    }

    private bool IsValidName(string name)
    {
        return Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯёЁ]+$");
    }


    public bool UpdateUser(string targetLogin, string? name, int? gender, DateTime? birthday, string adminLogin)
    {
        var target = GetByLogin(targetLogin);
        var actor = GetByLogin(adminLogin);
        if (target == null || actor == null) return false;
        if (target.RevokedOn != null) return false;
        if (!actor.Admin) return false;
        if (!IsValidName(name!)) return false;

        target.Name = name!;
        if (gender.HasValue) target.Gender = gender.Value;
        target.Birthday = birthday;
        target.ModifiedOn = DateTime.UtcNow;
        target.ModifiedBy = adminLogin;
        return true;
    }

    public bool UpdatePassword(string login, string newPassword)
    {
        var target = GetByLogin(login);
        if (target!.RevokedOn != null) return false;
        if (!IsValidPassword(newPassword)) return false;

        target.Password = newPassword;
        target.ModifiedOn = DateTime.UtcNow;
        target.ModifiedBy = login;
        return true;
    }

    public bool UpdateLogin(string targetLogin, string newLogin)
    {
        var target = GetByLogin(targetLogin);
        if (target == null) return false;
        if (target.RevokedOn != null) return false;
        if (!target.Admin) return false;
        if (!IsValidLogin(newLogin)) return false;
        if (LoginExists(newLogin)) return false;

        target.Login = newLogin;
        target.ModifiedOn = DateTime.UtcNow;
        target.ModifiedBy = targetLogin;
        return true;
    }

    public List<User> GetAllActive() =>
        users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn).ToList();

    public User? GetByLogin(string login) =>
        users.FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
    public User? GetUser(string login, string password) =>
        users.FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase) && u.Password.Equals(password, StringComparison.OrdinalIgnoreCase));

    public bool LoginExists(string login) =>
        users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
    public List<User> GetOlderThan(int age) => users.Where(u => u.Birthday != null && DateTime.UtcNow.Year - u.Birthday.Value.Year >= age).ToList();



    public bool Delete(string login, bool hard, string deletedBy)
    {
        var user = GetByLogin(login);
        if (user == null) return false;

        if (hard)
            return users.Remove(user);

        user.RevokedOn = DateTime.UtcNow;
        user.RevokedBy = deletedBy;
        return true;
    }

    public bool Restore(string login)
    {
        var user = GetByLogin(login);
        if (user == null) return false;

        user.RevokedOn = null;
        user.RevokedBy = null;
        return true;
    }


    public User? Authenticate(string login, string password)
    {
        return users.FirstOrDefault(u =>
            u.Login == login &&
            u.Password == password &&
            u.RevokedOn == null);
    }
}
