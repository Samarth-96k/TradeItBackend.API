using TradeIt.API.DTOs;

namespace TradeIt.API.Services;

public interface IAuthService
{
    Task<object> RegisterAsync(RegisterDto dto);

    Task<object?> LoginAsync(LoginDto dto);
}