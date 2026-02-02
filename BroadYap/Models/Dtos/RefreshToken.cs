public record RefreshTokenDto
(
    string Token,
    DateTime ExpiresAt
);