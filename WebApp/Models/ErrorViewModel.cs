namespace WebApp.Models;

internal class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}
