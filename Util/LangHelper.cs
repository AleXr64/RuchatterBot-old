using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BotWithAPI.Util
{
	public static class LangHelper
	{
		public static string ToHTMLTagCode(this string text)
		{
			var code = new TagBuilder("code");
			code.InnerHtml.Append(text);
			var writer = new StringWriter();
			code.WriteTo(writer, HtmlEncoder.Default);
			return writer.ToString();
		}
	}
}
