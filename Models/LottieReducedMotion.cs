namespace BlazorTwUI.Models;

/// <summary>How a <c>TwLottie</c> behaves when the viewer prefers reduced motion.</summary>
public enum LottieReducedMotion
{
    /// <summary>Do not play; hold the file's "poster" marker frame (frame 0 when it has none). The default.</summary>
    Poster,
    /// <summary>Do not show it at all (for pure decoration such as confetti); the words beside it carry the message.</summary>
    Hide,
    /// <summary>Play anyway. Only for an animation the person started and that carries meaning nothing else shows.</summary>
    Play
}

/// <summary>The celebrations <c>TwCelebration</c> knows, each mapped to a bundled sample in wwwroot/animations.</summary>
public enum CelebrationKind
{
    Success,
    LevelUp,
    Confetti,
    Badge,
    Streak
}
