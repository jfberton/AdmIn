namespace AdmIn.Mock.Services.UtilityServices
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
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            return userAgent.Contains("Mobi");
        }
    }
}
