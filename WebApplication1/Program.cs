using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();

app.MapPost("/api/contact", async (ContactForm form, IEmailSender sender) =>
{
    if (string.IsNullOrWhiteSpace(form.Name) ||
        string.IsNullOrWhiteSpace(form.Email) ||
        string.IsNullOrWhiteSpace(form.Message))
        return Results.BadRequest("all fields are needed");

    await sender.SendAsync(
        subject: $"New Message {form.Name}",
        body:   $"Email: {form.Email}\n\n{form.Message}"
    );

    return Results.Ok(new { success = true });
});

app.Run();

record ContactForm(string Name, string Email, string Message);

public class EmailSettings
{
    public string Host { get; set; }
    public int    Port { get; set; }
    public string User { get; set; }
    public string Pass { get; set; }
    public string To   { get; set; }
}

public interface IEmailSender
{
    Task SendAsync(string subject, string body);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _s;
    public SmtpEmailSender(IOptions<EmailSettings> opts) => _s = opts.Value;

    public async Task SendAsync(string subject, string body)
    {
        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(_s.User));
        msg.To.Add(MailboxAddress.Parse(_s.To));
        msg.Subject = subject;
        msg.Body    = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_s.Host, _s.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_s.User, _s.Pass);
        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }
}
