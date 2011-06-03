using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;

namespace Farley.Controls
{
	public class TextEditor : TextBox
	{
		public TextEditor()
		{
			Mode = TextEditorMode.Simple;
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			string tinyMceIncludeKey = "TinyMCEInclude";
			string tinyMceIncludeScript = "<script type=\"text/javascript\" src=\"/Scripts/tinymce/jscripts/tiny_mce/tiny_mce.js\"></script>";

			if (!Page.ClientScript.IsStartupScriptRegistered(tinyMceIncludeKey))
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), tinyMceIncludeKey, tinyMceIncludeScript);
			}

			if (!Page.ClientScript.IsStartupScriptRegistered(GetInitKey()))
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), GetInitKey(), GetInitScript());
			}

			if (!CssClass.Contains(GetEditorClass())) 
			{
				if (CssClass.Length > 0)
				{
					CssClass += " ";
				}
				CssClass += GetEditorClass();
			}

			HttpContext.Current.Response.Cookies[HttpContext.Current.Session.SessionID].Value = "True";
			HttpContext.Current.Response.Cookies[HttpContext.Current.Session.SessionID].Expires = DateTime.Now.AddHours(3);

			base.OnPreRender(e);
		}

		private string GetInitKey()
		{
			string simpleKey = "TinyMCESimple";
			string fullKey = "TinyMCEFull";

			switch (Mode)
			{
				case TextEditorMode.Simple:
					return simpleKey;
				case TextEditorMode.Full:
					return fullKey;
				default:
					goto case TextEditorMode.Simple;
			}
		}

		private string GetEditorClass()
		{
			return GetEditorClass(Mode);
		}

		private string GetEditorClass(TextEditorMode mode)
		{
			string simpleClass = "SimpleTextEditor";
			string fullClass = "FullTextEditor";

			switch (mode)
			{
				case TextEditorMode.Simple:
					return simpleClass;
				case TextEditorMode.Full:
					return fullClass;
				default:
					goto case TextEditorMode.Simple;
			}
		}

		private string GetInitScript()
		{
			switch (Mode)
			{
				case TextEditorMode.Simple:
					return string.Format(GetSimpleScript(), GetEditorClass(TextEditorMode.Simple));
				case TextEditorMode.Full:
					return string.Format(GetFullScript(), GetEditorClass(TextEditorMode.Full));
				default:
					goto case TextEditorMode.Simple;
			}
		}

		private string GetSimpleScript()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=\"javascript\" type=\"text/javascript\">tinyMCE.init({{");
			
			sb.Append("mode : \"textareas\",");
			sb.Append("theme : \"simple\",");
			sb.Append("plugins : \"inlinepopups\",");
			sb.Append("dialog_type : \"modal\",");
			sb.Append("content_css: \"/css/editor.css\",");

			// allow embed code
			sb.Append("extended_valid_elements: \"object[width|height|classid|codebase],param[name|value],embed[src|type|width|height|flashvars|wmode],iframe[src|height|width|frameborder|allowfullscreen]\",");

			sb.Append("editor_selector : \"{0}\"");

			sb.Append("}});</script>");
			return sb.ToString();
		}

		private string GetFullScript()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=\"javascript\" type=\"text/javascript\">tinyMCE.init({{");

			sb.Append("mode : \"textareas\",");
			sb.Append("theme : \"advanced\",");
			sb.Append("plugins : \"inlinepopups\",");
			sb.Append("dialog_type : \"modal\",");
			sb.Append("theme_advanced_toolbar_location: \"top\",");
			sb.Append("theme_advanced_toolbar_align: \"left\",");
			sb.Append("theme_advanced_buttons3: \"\",");
			sb.Append("content_css: \"/css/editor.css\",");

			sb.Append("relative_urls: false,");
            sb.Append("remove_script_host: true,");
            sb.Append("document_base_url: \"\",");
			sb.Append("file_browser_callback : \"filebrowser\",");

			sb.Append("editor_selector : \"{0}\"");
			sb.Append("}}); ");

			sb.Append("function filebrowser(field_name, url, type, win) {{ ");
			sb.Append("fileBrowserURL = \"/Script/tinymce/jscripts/FileManager/Default.aspx?sessionid=" + HttpContext.Current.Session.SessionID.ToString() + "\"; ");
			sb.Append("tinyMCE.activeEditor.windowManager.open({{ ");
			sb.Append("title: \"Ajax File Manager\",");
			sb.Append("url: fileBrowserURL,");
			sb.Append("width: 950,");
			sb.Append("height: 650,");
			sb.Append("inline: 0,");
			sb.Append("maximizable: 1,");
			sb.Append("close_previous: 0");
			sb.Append(" }}, {{ ");
			sb.Append("window: win,");
			sb.Append("input: field_name,");
			sb.Append("sessionid: '" + HttpContext.Current.Session.SessionID.ToString() + "'");
			sb.Append(" }}");
			sb.Append("); ");
			sb.Append(" }} ");
			
			sb.Append("</script>");
			return sb.ToString();
		}

		public override TextBoxMode TextMode
		{
			get
			{
				return TextBoxMode.MultiLine;
			}
		}

		public TextEditorMode Mode
		{
			get
			{
				object obj = ViewState["Mode"];
				if (obj == null)
				{
					return TextEditorMode.Simple;
				}
				return (TextEditorMode)obj;
			}
			set
			{
				ViewState["Mode"] = value;
			}
		}

		public enum TextEditorMode
		{
			Simple,
			Full
		}
	}
}