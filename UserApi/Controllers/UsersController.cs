
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    private readonly UserService _service;

    public UsersController(UserService service) => _service = service;

    [HttpPost()]
    public IActionResult Create([FromQuery] string adminLogin, [FromBody] UserCreate newUser)
    {
        var admin = _service.GetByLogin(adminLogin);
        if (admin == null || !admin.Admin || admin.RevokedOn != null)
            return Unauthorized("Создавать пользователей могут только админы");

        if (_service.LoginExists(newUser.Login))
            return BadRequest("Логин уже есть");

        var user = new User
        {
            Guid = Guid.NewGuid(),
            Login = newUser.Login,
            Password = newUser.Password,
            Name = newUser.Name,
            Gender = newUser.Gender,
            Birthday = newUser.Birthday,
            Admin = newUser.Admin,
            CreatedBy = adminLogin,
            CreatedOn = DateTime.UtcNow,
        };
        _service.Add(user);
        return Ok(user);
    }

    [HttpPut("update-user")]
    public IActionResult UpdateUser([FromQuery] string adminLogin, [FromQuery] string targetLogin, [FromBody] UserUpdate request)
    {
        var success = _service.UpdateUser(targetLogin, request.Name!, request.Gender, request.Birthday, adminLogin);
        if (!success)
            return BadRequest("Ошибка обновления пользователя");
        return Ok($"{targetLogin} - обновлен успешно");
    }

    [HttpPut("update-password")]
    public IActionResult UpdatePassword([FromQuery] string login, [FromQuery] string newPassword)
    {
        var success = _service.UpdatePassword(login, newPassword);
        if (!success)
            return BadRequest("Ошибка при изменении пароля");
        return Ok("Пароль успешно изменен");
    }

    [HttpPut("update-login")]
    public IActionResult UpdateLogin([FromQuery] string login, [FromQuery] string newLogin)
    {
        var success = _service.UpdateLogin(login, newLogin);
        if (!success)
            return BadRequest("Ошибка при изменении логина");
        return Ok("Логин успешно изменен");
    }


    [HttpGet("get-all-active")]
    public IActionResult GetAllActive([FromQuery] string adminLogin)
    {
        var admin = _service.GetByLogin(adminLogin);
        if (admin == null || !admin.Admin || admin.RevokedOn != null)
            return Unauthorized("Просматривать активных пользователей могут только админы");

        return Ok(_service.GetAllActive());
    }

    [HttpGet("get-user-login")]
    public IActionResult GetUserLogin([FromQuery] string adminLogin, [FromQuery] string login)
    {
        var admin = _service.GetByLogin(adminLogin);
        if (admin == null || !admin.Admin || admin.RevokedOn != null)
            return Unauthorized("Просматривать пользователей могут только админы");

        var user = _service.GetByLogin(login);
        if (user == null)
            return NotFound("Пользователь не найден");

        var result = new
        {
            user.Name,
            user.Gender,
            user.Birthday,
            IsActive = user.RevokedOn == null
        };

        return Ok(result);
    }

    [HttpGet("get-one-user")]
    public IActionResult GetUser([FromQuery] string login, [FromQuery] string password)
    {
        var user = _service.GetUser(login, password);
        if (user == null || user.RevokedOn != null)
            return NotFound("Данного пользователя нет в базе или не активен");


        return Ok(user);
    }
    [HttpGet("get-old-users")]
    public IActionResult GetOldUsers([FromQuery] string adminLogin, [FromQuery] int age)
    {
        var admin = _service.GetByLogin(adminLogin);
        if (admin == null || !admin.Admin || admin.RevokedOn != null)
            return Unauthorized("Просматривать пользователей могут только админы");

        List<User> oldUsers = _service.GetOlderThan(age);
        if (!oldUsers.Any())
            return NotFound($"Нет пользователей старше {age} лет");

        return Ok(oldUsers);
    }

    [HttpDelete()]
    public IActionResult Delete([FromQuery] string adminLogin, [FromQuery] string login, [FromQuery] bool hard = false)
    {
        var admin = _service.GetByLogin(adminLogin);
        if (admin == null || !admin.Admin || admin.RevokedOn != null)
            return Unauthorized("Выполнить действие могут только админы");

        var success = _service.Delete(login, hard, adminLogin);
        if (!success)
            return NotFound($"Не удалось удалить пользователя");

        return Ok(hard ? "Пользователь успешно удален полностью" : "Пользователь теперь не активен");
    }
    [HttpPatch("restore")]
    public IActionResult Restore([FromQuery] string adminLogin, [FromQuery] string login)
    {
        var admin = _service.GetByLogin(adminLogin);
        if (admin == null || !admin.Admin || admin.RevokedOn != null)
            return Unauthorized("Выполнить действие могут только админы");

        var success = _service.Restore(login);
        if (!success)
            return NotFound($"Не удалось восстановить пользователя");

        return Ok("Пользователь успешно восстановлен");
    }

    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] LoginRequest request)
    {
        var user = _service.Authenticate(request.Login, request.Password);
        if (user == null)
            return Unauthorized("Неверный логин или пароль");

        return Ok("Аутентификация прошла успешно");
    }
}

