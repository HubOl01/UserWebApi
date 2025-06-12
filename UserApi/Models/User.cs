public class User
{
    public Guid Guid { get; set; } = Guid.NewGuid(); // Уникальный идентификатор пользователя
    public required string Login { get; set; } // Уникальный Логин (запрещены все символы кроме латинских букв и цифр),
    public required string Password { get; set; } // Пароль(запрещены все символы кроме латинских букв и цифр),
    public required string Name { get; set; } // (запрещены все символы кроме латинских и русских букв)
    public int Gender { get; set; } // Пол 0 - женщина, 1 - мужчина, 2 - неизвестно
    public DateTime? Birthday { get; set; } //  поле даты рождения может быть Null
    public bool Admin { get; set; } = false; // Указание - является ли пользователь админом
    public required DateTime CreatedOn { get; set; } // Дата создания пользователя
    public required string CreatedBy { get; set; } // Логин Пользователя, от имени которого этот пользователь создан
    public DateTime? ModifiedOn { get; set; } // Дата изменения пользователя
    public string? ModifiedBy { get; set; } // Логин Пользователя, от имени которого этот пользователь изменён
    public DateTime? RevokedOn { get; set; } // Дата удаления пользователя
    public string? RevokedBy { get; set; } // Логин Пользователя, от имени которого этот пользователь удалён
}