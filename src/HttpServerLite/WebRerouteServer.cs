using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerLite {
	/// <summary>
	/// A simple webserver that redirects all requests to a single URL
	/// </summary>
	public class WebRerouteServer {

		/// <summary>
		/// 
		/// </summary>
		public enum RedirectionTypes: int {
			MOVED_PERMANENTLY = 301,
			FOUND = 302,
			SEE_OTHER = 303,
			NOT_MODIFIED = 304,
			TEMPORARY_REDIRECT = 307,
			PERMANENT_REDIRECT = 308
		}
		
		protected Webserver _Server;
		public string RedirectUrl { get; set; }
		public bool RedirectUrlPath { get; set; } = true;
		public RedirectionTypes RedirectionType { get; set; } = RedirectionTypes.PERMANENT_REDIRECT;

		public bool IsRunning => _Server.IsListening;

		/// <summary>
		/// Access to the server's debug settings
		/// </summary>
		public WebserverSettings.DebugSettings DebugSettings => _Server.Settings.Debug;

		/// <summary>
		/// Access to the server's event settings
		/// </summary>
		public Action<string> EventsLogger {
			get => _Server.Events.Logger;
			set => _Server.Events.Logger = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hostname">The listening hostname</param>
		/// <param name="port">the listening port</param>
		/// <param name="redirectBaseUrl">the base URL that will be redirected</param>
		/// <param name="sslCertificate">Can add a ssl layer</param>
		public WebRerouteServer(string hostname, int port, string redirectBaseUrl, X509Certificate2 sslCertificate = null) {
			_Server = new Webserver(hostname, port, Reroute, sslCertificate);
			RedirectUrl = redirectBaseUrl;
		}



		private async Task Reroute(HttpContext ctx) {
			ctx.Response.Headers["Location"] = RedirectUrl + (RedirectUrlPath ? ctx.Request.Url.Full : "");
			ctx.Response.StatusCode = (int) RedirectionType;
			await ctx.Response.SendAsync(0);
		}

		/// <summary>
		/// Start the redirection
		/// </summary>
		public void Start() {
			_Server.Start(false);
		}

		/// <summary>
		/// Stop the redirection
		/// </summary>
		public void Stop() {
			_Server.Stop();
		}
		
	}
}
