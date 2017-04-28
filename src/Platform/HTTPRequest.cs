#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_WP_8_1 || UNITY_WSA || UNITY_WEBGL || UNITY_IOS || UNITY_PS4 || UNITY_SAMSUNGTV || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS
#define UNITY
#endif
//-----------------------------------------------------------------------
// <copyright file="HTTPRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
//     Based on http://stackoverflow.com/a/12606963 and http://wiki.unity3d.com/index.php/WebAsync
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Platform {


	using System;
	using System.Net;
#if !UNITY && !NETFX_CORE
	using System.Net.Cache;
#endif
	using System.IO;
	using System.Collections.Generic;
	using System.Threading;
	using System.ComponentModel;
#if NETFX_CORE
	using System.Net.Http;
	using System.Linq;
#endif

	//using System.Windows.Threading;

	internal sealed class HTTPRequest : IAsyncRequest {


		public bool IsCompleted = false;


		private Action<Response> _callback;
		private HttpWebRequest _hwr;
		private int _timeOut;
#if !UNITY
		private SynchronizationContext _sync = AsyncOperationManager.SynchronizationContext;
#endif


		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="callback"></param>
		/// <param name="timeOut">seconds</param>
		public HTTPRequest(string url, Action<Response> callback, int timeOut = 10) {
			//UnityEngine.Debug.Log(url);
			_callback = callback;
			_timeOut = timeOut;

			//The answer is changing HttpWebRequest / HttpWebResponse to WebRequest/ WebResponse only.That fixed the problem.

			_hwr = WebRequest.Create(url) as HttpWebRequest;
			_hwr.Credentials = CredentialCache.DefaultCredentials;
			_hwr.KeepAlive = true;

			//_hwr.ProtocolVersion = HttpVersion.Version11; //that's it!!!

			//that' it !!!!
			// https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest(v=vs.90).aspx#Remarks
			// use a value that is 12 times the number of CPUs on the local computer

			_hwr.ServicePoint.ConnectionLimit  = Environment.ProcessorCount * 6;

			_hwr.ServicePoint.UseNagleAlgorithm = true;
			_hwr.ServicePoint.Expect100Continue = false;
			_hwr.ServicePoint.MaxIdleTime = 2000;
			//System.Net.ServicePointManager.SetTcpKeepAlive(false, 0, 0);


			//UnityEngine.Debug.Log("CurrentConnections: " + _hwr.ServicePoint.CurrentConnections);
			//UnityEngine.Debug.Log("ConnectionLimit: " + _hwr.ServicePoint.ConnectionLimit);
			//UnityEngine.Debug.Log("ConnectionName: " + _hwr.ServicePoint.ConnectionName);
			_hwr.Method = "GET";
			_hwr.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
			_hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
#if !UNITY && !NETFX_CORE
			//_hwr.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable);
			_hwr.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
#endif
#if !NETFX_CORE
			_hwr.UserAgent = "mapbox-sdk-cs";
			//_hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
			//_hwr.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
#endif
			//_hwr.Timeout = timeOut * 1000; doesn't work in async calls, see below

			getResponseAsync(_hwr, EvaluateResponse);
		}


#if NETFX_CORE
		private async void getResponseAsync(HttpWebRequest request, Action<HttpResponseMessage, Exception> gotResponse) {

			//http://rextester.com/discussion/XPKY90132/async-example-with-HttpClient
			using (var client = new HttpClient()) {
				var response = await client.GetAsync(_hwr.RequestUri);
				gotResponse(response, null);
			}
		}


		private async void EvaluateResponse(HttpResponseMessage apiResponse, Exception apiEx) {

			var response = new Response();

			if (null != apiEx) {
				response.AddException(apiEx);
			}

			// timeout: API response is null
			if (null == apiResponse) {
				response.AddException(new Exception("No Reponse."));
			} else {
				// TODO: evaluate headers and add custom exception, eg if rate limit is exceeded
				// https://www.mapbox.com/api-documentation/#rate-limits
				// X-Rate-Limit-Interval
				// X-Rate-Limit-Limit
				// X-Rate-Limit-Reset
				if (null != apiResponse.Headers) {
					response.Headers = new Dictionary<string, string>();
					foreach (var hdr in apiResponse.Headers) {
						string key = hdr.Key;
						string val = hdr.Value.FirstOrDefault();
						response.Headers.Add(key, val);
						if (key.Equals("X-Rate-Limit-Interval", StringComparison.OrdinalIgnoreCase)) {
							int limitInterval;
							if (int.TryParse(val, out limitInterval)) { response.XRateLimitInterval = limitInterval; }
						} else if (key.Equals("X-Rate-Limit-Limit", StringComparison.OrdinalIgnoreCase)) {
							long limitLimit;
							if (long.TryParse(val, out limitLimit)) { response.XRateLimitLimit = limitLimit; }
						} else if (key.Equals("X-Rate-Limit-Reset", StringComparison.OrdinalIgnoreCase)) {
							double unixTimestamp;
							if (double.TryParse(val, out unixTimestamp)) {
								DateTime beginningOfTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
								response.XRateLimitReset = beginningOfTime.AddSeconds(unixTimestamp).ToLocalTime();
							}
						} else if (key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)) {
							response.ContentType = val;
						}
					}
				}

				if (apiResponse.StatusCode != HttpStatusCode.OK) {
					response.AddException(new Exception(string.Format("{0}: {1}", apiResponse.StatusCode, apiResponse.ReasonPhrase)));
				}
				int statusCode = (int)apiResponse.StatusCode;
				response.StatusCode = statusCode;
				if (429 == statusCode) {
					response.AddException(new Exception("Rate limit hit"));
				}

				if (null != apiResponse) {
					response.Data = await apiResponse.Content.ReadAsByteArrayAsync();
				}
			}

			// post (async) callback back to the main/UI thread
			// Unity: SynchronizationContext doesn't do anything
			//        use the Dispatcher
#if !UNITY
			_sync.Post(delegate {
				_callback(response);
				IsCompleted = true;
				_callback = null;
			}, null);
#else
			UnityMainThreadDispatcher.Instance().Enqueue(() => {
				_callback(response);
				IsCompleted = true;
				_callback = null;
			});
#endif
		}

#endif


#if !NETFX_CORE
		private void getResponseAsync(HttpWebRequest request, Action<HttpWebResponse, Exception> gotResponse) {

			// create an additional action wrapper, because of:
			// https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.begingetresponse.aspx
			// The BeginGetResponse method requires some synchronous setup tasks to complete (DNS resolution,
			//proxy detection, and TCP socket connection, for example) before this method becomes asynchronous.
			// As a result, this method should never be called on a user interface (UI) thread because it might
			// take considerable time(up to several minutes depending on network settings) to complete the
			// initial synchronous setup tasks before an exception for an error is thrown or the method succeeds.

			Action actionWrapper = () => {
				try {
					// BeginInvoke runs on a thread of the thread pool (!= main/UI thread)
					// that's why we need SynchronizationContext when 
					// TODO: how to influence threadpool: nr of threads etc.
					long startTicks = DateTime.Now.Ticks;
					request.BeginGetResponse((asycnResult) => {
						try { // there's a try/catch here because execution path is different from invokation one, exception here may cause a crash
							long beforeEndGet = DateTime.Now.Ticks;
							HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asycnResult);
							//long finished = DateTime.Now.Ticks;
							//long duration = finished - startTicks;
							//TimeSpan ts = TimeSpan.FromTicks(duration);
							//TimeSpan tsEndGet = TimeSpan.FromTicks(finished - beforeEndGet);
							//TimeSpan tsBeginGet = TimeSpan.FromTicks(beforeEndGet - startTicks);
							//UnityEngine.Debug.Log("received response - " + ts.Milliseconds + "ms" + " BeginGet: " + tsBeginGet.Milliseconds + " EndGet: " + tsEndGet.Milliseconds + " CompletedSynchronously: " + asycnResult.CompletedSynchronously);
							gotResponse(response, null);
						}
						// EndGetResponse() throws on on some status codes, try to get response anyway (and status codes)
						catch (WebException wex) {
							//another place to watchout for HttpWebRequest.Abort to occur
							if (wex.Status == WebExceptionStatus.RequestCanceled) {
								gotResponse(null, wex);
							} else {
								HttpWebResponse hwr = wex.Response as HttpWebResponse;
								if (null == hwr) {
									throw;
								}
								gotResponse(hwr, wex);
							}
						}
						catch (Exception ex) {
							gotResponse(null, ex);
						}
					}
					, null);
				}
				catch (Exception ex) {
					//catch exception from HttpWebRequest.Abort
					gotResponse(null, ex);
				}
			};

			try {
				actionWrapper.BeginInvoke(new AsyncCallback((iAsyncResult) => {
					var action = (Action)iAsyncResult.AsyncState;
					action.EndInvoke(iAsyncResult);
				})
				, actionWrapper);
			}
			catch (Exception ex) {
				gotResponse(null, ex);
			}
		}



		private void EvaluateResponse(HttpWebResponse apiResponse, Exception apiEx) {

			var response = new Response();
			response.Request = this;

			if (null != apiEx) {
				response.AddException(apiEx);
			}

			// timeout: API response is null
			if (null == apiResponse) {
				response.AddException(new Exception("No Reponse."));
			} else {
				// TODO: evaluate headers and add custom exception, eg if rate limit is exceeded
				// https://www.mapbox.com/api-documentation/#rate-limits
				// X-Rate-Limit-Interval
				// X-Rate-Limit-Limit
				// X-Rate-Limit-Reset
				if (null != apiResponse.Headers) {
					response.Headers = new Dictionary<string, string>();
					for (int i = 0; i < apiResponse.Headers.Count; i++) {
						// TODO: implement .Net Core / UWP implementation
						string key = apiResponse.Headers.Keys[i];
						string val = apiResponse.Headers[i];
						response.Headers.Add(key, val);
						if (key.Equals("X-Rate-Limit-Interval", StringComparison.InvariantCultureIgnoreCase)) {
							int limitInterval;
							if (int.TryParse(val, out limitInterval)) { response.XRateLimitInterval = limitInterval; }
						} else if (key.Equals("X-Rate-Limit-Limit", StringComparison.InvariantCultureIgnoreCase)) {
							long limitLimit;
							if (long.TryParse(val, out limitLimit)) { response.XRateLimitLimit = limitLimit; }
						} else if (key.Equals("X-Rate-Limit-Reset", StringComparison.InvariantCultureIgnoreCase)) {
							double unixTimestamp;
							if (double.TryParse(val, out unixTimestamp)) {
								DateTime beginningOfTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
								response.XRateLimitReset = beginningOfTime.AddSeconds(unixTimestamp).ToLocalTime();
							}
						} else if (key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase)) {
							response.ContentType = val;
						}
					}
				}

				if (apiResponse.StatusCode != HttpStatusCode.OK) {
					response.AddException(new Exception(string.Format("{0}: {1}", apiResponse.StatusCode, apiResponse.StatusDescription)));
				}
				int statusCode = (int)apiResponse.StatusCode;
				response.StatusCode = statusCode;
				if (429 == statusCode) {
					response.AddException(new Exception("Rate limit hit"));
				}

				if (null != apiResponse) {
					using (Stream responseStream = apiResponse.GetResponseStream()) {
						byte[] buffer = new byte[0x1000];
						int bytesRead;
						using (MemoryStream ms = new MemoryStream()) {
							while (0 != (bytesRead = responseStream.Read(buffer, 0, buffer.Length))) {
								ms.Write(buffer, 0, bytesRead);
							}
							response.Data = ms.ToArray();
						}
					}
					apiResponse.Close();
				}
			}

			// post (async) callback back to the main/UI thread
			// Unity: SynchronizationContext doesn't do anything
			//        use the Dispatcher
#if !UNITY
			_sync.Post(delegate {
				_callback(response);
				IsCompleted = true;
				_callback = null;
			}, null);
#else
			//UnityMainThreadDispatcher.Enqueue(() => {
			//	_callback(response);
			//	IsCompleted = true;
			//	_callback = null;
			//});
			UnityToolbag.Dispatcher.InvokeAsync(() => {
				_callback(response);
				IsCompleted = true;
				_callback = null;
			});
#endif
		}
#endif



		public void Cancel() {

			if (null != _hwr) {
				_hwr.Abort();
			}
		}


	}
}
