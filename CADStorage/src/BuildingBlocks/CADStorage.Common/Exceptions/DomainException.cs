// Базовое исключение для всех ошибок доменной области
// Используется для обработки бизнес-логики ошибок
namespace CADStorage.Common.Exceptions;

using System;
using System.Runtime.Serialization;

/// <summary>
/// Базовый класс для всех исключений предметной области
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Код ошибки для клиентского приложения
    /// </summary>
    public string ErrorCode { get; } = "DOMAIN_ERROR";

    /// <summary>
    /// Конструктор по умолчанию
    /// </summary>
    public DomainException() : base()
    {
    }

    /// <summary>
    /// Конструктор с сообщением об ошибке
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Конструктор с сообщением и внутренним исключением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Конструктор с кодом ошибки и сообщением
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="message">Сообщение об ошибке</param>
    public DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Исключение возникающее когда сущность не найдена
/// </summary>
public class NotFoundException : DomainException
{
    public override string ErrorCode => "NOT_FOUND";

    public NotFoundException(string entityName, object key)
        : base($"Сущность '{entityName}' с ключом ({key}) не найдена.")
    {
    }
}

/// <summary>
/// Исключение возникающее при нарушении правил бизнес-логики
/// </summary>
public class BusinessRuleValidationException : DomainException
{
    public override string ErrorCode => "BUSINESS_RULE_VIOLATION";

    public BusinessRuleValidationException(string message) : base(message)
    {
    }
}

/// <summary>
/// Исключение возникающее при конфликте данных
/// </summary>
public class ConflictException : DomainException
{
    public override string ErrorCode => "CONFLICT";

    public ConflictException(string message) : base(message)
    {
    }
}
