namespace BBB_ApplicationDashboard.Infrastructure.Configuration;

public record EmailOptions
{
    public string SMTP_HOST { get; set; } = null!;
    public int SMTP_PORT { get; set; }
    public string SMTP_USERNAME { get; set; } = null!;
    public string SMTP_PASSWORD { get; set; } = null!;
    public string FROM_EMAIL { get; set; } = null!;
    public string FROM_NAME { get; set; } = null!;
}
