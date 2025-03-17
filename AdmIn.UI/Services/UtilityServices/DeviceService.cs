namespace AdmIn.UI.Services.UtilityServices
{
    public class DeviceService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeviceService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsMobileDevice()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                Console.WriteLine("HttpContext is null. Ensure that the IHttpContextAccessor is properly configured.");
                return false;
            }

            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            return userAgent.Contains("Mobi");
        }
    }
}
