using System.ComponentModel.DataAnnotations;

public record SignInDto
(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password
);