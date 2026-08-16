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
    }
}
