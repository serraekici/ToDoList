using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace ToDoApp.Models;

public partial class TodoItem : ObservableObject
{
    private static readonly string[] Motifs =
    [
        "🌼", "🦋", "🐝", "🌸", "🐞",
        "🌷", "🌿", "🍄", "🪻", "🍋"
    ];

    private static readonly string[] CardTints =
    [
        "#FFF8F4", "#F7FBF6", "#F8F4FB", "#FFF9F0", "#FDF6F8"
    ];

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private bool isCompleted;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? DueDate { get; set; }

    [Ignore]
    public string DueDateLabel
    {
        get
        {
            if (DueDate is null) return string.Empty;
            var day = DueDate.Value.Date;
            var today = DateTime.Today;
            var text = "bitiş " + day.ToString("d MMM", new CultureInfo("tr-TR"));
            if (IsCompleted) return text;
            if (day < today) return text + " · geçti";
            if (day == today) return text + " · bugün";
            return text;
        }
    }

    [Ignore]
    public Color DueDateColor
    {
        get
        {
            if (DueDate is null || IsCompleted) return Color.FromArgb("#7A7268");
            if (DueDate.Value.Date < DateTime.Today) return Color.FromArgb("#E36888");
            if (DueDate.Value.Date == DateTime.Today) return Color.FromArgb("#F08C21");
            return Color.FromArgb("#7A7268");
        }
    }

    [Ignore]
    public bool HasDueDate => DueDate is not null;

    [Ignore]
    public string Motif
    {
        get
        {
            var seed = Id != 0 ? Id : Math.Abs(Title.GetHashCode());
            return Motifs[Math.Abs(seed) % Motifs.Length];
        }
    }

    [Ignore]
    public Color CardTint
    {
        get
        {
            var seed = Id != 0 ? Id : Math.Abs(Title.GetHashCode());
            return Color.FromArgb(CardTints[Math.Abs(seed) % CardTints.Length]);
        }
    }

    [Ignore]
    public string CheckMark => IsCompleted ? "✿" : "○";

    [Ignore]
    public double TitleOpacity => IsCompleted ? 0.55 : 1;

    [Ignore]
    public TextDecorations TitleDecorations =>
        IsCompleted ? TextDecorations.Strikethrough : TextDecorations.None;

    partial void OnIsCompletedChanged(bool value)
    {
        OnPropertyChanged(nameof(CheckMark));
        OnPropertyChanged(nameof(TitleOpacity));
        OnPropertyChanged(nameof(TitleDecorations));
        OnPropertyChanged(nameof(DueDateLabel));
        OnPropertyChanged(nameof(DueDateColor));
    }
}
