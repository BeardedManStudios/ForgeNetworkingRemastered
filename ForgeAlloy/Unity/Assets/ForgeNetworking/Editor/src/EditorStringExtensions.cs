namespace Forge.Editor
{
	public static class EditorStringExtensions
	{
		public static string ToUnityUiHtml(this string input)
		{
			if (input.StartsWith("| "))
				input = "│ " + input.Remove(0, 2);

			return input.Replace("\r", "")
				.Replace("&quot;", "\"")
				.Replace("&bull;", "•")
				.Replace("&trade;", "™")
				.Replace("&copy;", "Ⓒ")
				.Replace("&sum;", "∑")
				.Replace("&prod;", "∏")
				.Replace("&ni;", "∋")
				.Replace("&notin;", "∉")
				.Replace("&isin;", "∈")
				.Replace("&nabla;", "∇")
				.Replace("&empty;", "∅")
				.Replace("&exist;", "∃")
				.Replace("&part;", "∂")
				.Replace("&forall;", "∀")
				.Replace("&forall;", "Δ")
				.Replace("<dash />", " — ")
				.Replace("<code>", "┌—————————")
				.Replace("</code>", "└—————————")

				// <h1> </h1> etc...
				.Replace("<h1>", "<size=18>")
				.Replace("<h2>", "<size=16>")
				.Replace("<h3>", "<size=14>")
				.Replace("</h1>", "</size>")
				.Replace("</h2>", "</size>")
				.Replace("</h3>", "</size>")

				.Replace("<p>", "")
				.Replace("</p>\n", "\n")
				.Replace("</p>", "\n")

				.Replace("<em>", "<i>")
				.Replace("</em>", "</i>")

				// <strong> </strong>
				.Replace("<strong>", "<b>")
				.Replace("</strong>", "</b>");
		}
	}
}
