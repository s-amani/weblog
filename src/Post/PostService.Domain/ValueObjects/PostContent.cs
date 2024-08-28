namespace PostService.Domain.ValueObjects;

public record PostContent
{
    public string Text { get; init; }

    public List<string> Images { get; init; }

    public bool IsValidContent()
    {
        // Implement validation logic here, e.g., check for prohibited words
        return true;
    }

    public PostContent Merge(PostContent otherContent)
    {
        if (otherContent == null) throw new ArgumentNullException(nameof(otherContent));

        var mergedText = $"{Text} {otherContent.Text}";
        var mergedImages = Images.Concat(otherContent.Images).ToList();

        return new PostContent { Text = mergedText, Images = mergedImages };
    }
}