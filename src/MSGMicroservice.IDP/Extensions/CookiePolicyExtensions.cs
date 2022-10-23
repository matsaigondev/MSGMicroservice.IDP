namespace MSGMicroservice.IDP.Extensions;

public static class CookiePolicyExtensions
{
    //Trong truong hop identity ko login duoc chinh trang cua no
    public static void ConfigureCookiePolicy(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            //.net core > 3.0 should change value to SameSiteMode.Unspecified
            options.MinimumSameSitePolicy = (SameSiteMode)(-1);
            options.OnAppendCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });
    }

    static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
        if (options.SameSite != SameSiteMode.None && options.SameSite != SameSiteMode.Unspecified) return;
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        if (DisallowSameSiteNone(userAgent))
            options.SameSite = (SameSiteMode)(-1);
    }

    static bool DisallowSameSiteNone(string userAgent)
    {
        if (userAgent.Contains("CPU iPhone OS 12")
            || userAgent.Contains("iPad; CPU OS 12"))
            return true;

        if (userAgent.Contains("Safari")
            && userAgent.Contains("Macintosh; Intel Mac OS X 10_14")
            && userAgent.Contains("Version/"))
            return true;
        if (userAgent.Contains("Chrome")) return true;

        return false;
    }
}