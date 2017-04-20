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

			_callback = callback;
			_timeOut = timeOut;

			_hwr = WebRequest.Create(url) as HttpWebRequest;
			_hwr.Method = "GET";
#if !UNITY && !NETFX_CORE
			_hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
#endif
#if !NETFX_CORE
			_hwr.UserAgent = "mapbox-sdk-cs";
			//_hwr.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			_hwr.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
#endif
			//_hwr.Timeout = timeOut * 1000; doesn't work in async calls, see below

			getResponseAsync(_hwr, EvaluateResponse);
		}

#region oldversion

		//private void GetResponseAsync(HttpWebRequest request, Action<HttpWebResponse, Exception> gotResponse) {
		//	//private void GetResponseAsync(HttpWebRequest request) {

		//	// create an additional action wrapper, because of:
		//	// https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.begingetresponse.aspx
		//	// The BeginGetResponse method requires some synchronous setup tasks to complete (DNS resolution,
		//	//proxy detection, and TCP socket connection, for example) before this method becomes asynchronous.
		//	// As a result, this method should never be called on a user interface (UI) thread because it might
		//	// take considerable time(up to several minutes depending on network settings) to complete the
		//	// initial synchronous setup tasks before an exception for an error is thrown or the method succeeds.

		//	Action actionWrapper = () => {
		//		request.BeginGetResponse((r) => {
		//			try { // there's a try/catch here because execution path is different from invokation one, exception here may cause a crash
		//				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(r);
		//				System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest before 'gotResponse', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
		//				gotResponse(response, null);
		//			}
		//			// EndGetResponse() throws on on some status codes, try to get response anyway (and status codes)
		//			catch (WebException wex) {
		//				HttpWebResponse hwr = wex.Response as HttpWebResponse;
		//				if (null == hwr) {
		//					throw;
		//				}
		//				gotResponse(hwr, wex);
		//			}
		//			catch (Exception ex) {
		//				gotResponse(null, ex);
		//			}
		//		}
		//	, request);
		//	};

		//	//http://www.cshandler.com/2011/07/delegate-and-async-programming-part-ii.html
		//	//http://www.yoda.arachsys.com/csharp/threads/printable.shtml
		//	//https://msdn.microsoft.com/en-us/library/2e08f6yc(v=vs.110).aspx
		//	//http://www.c-sharpcorner.com/UploadFile/vendettamit/delegate-and-async-programming-C-Sharp-asynccallback-and-object-state/
		//	//http://xcalibursystems.com/2011/10/c-how-to-get-a-return-value-from-an-action/
		//	//https://stackoverflow.com/questions/8099631/how-to-return-value-from-action
		//	//http://blog.aggregatedintelligence.com/2010/06/c-asynchronous-programming-using.html
		//	//http://csharpindepth.com/Articles/Chapter2/Events.aspx?printable=true


		//	// !!!!BeginInvoke runs on a thread of the thread pool (!= main thread)!!!!
		//	// TODO: how to influence threadpool: nr of threads etc.
		//	System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest before 'BeginInvoke', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
		//	actionWrapper.BeginInvoke(new AsyncCallback((iASyncResult) => {
		//		System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest within 'BeginInvoke', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
		//		var action = (Action)iASyncResult.AsyncState;
		//		action.EndInvoke(iASyncResult);
		//		System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest after 'EndInvoke', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
		//	})
		//	, actionWrapper
		//	);

		//	System.Diagnostics.Debug.WriteLine(string.Format("HTTPRequest past 'BeginInvoke', thread id:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
		//}


#endregion

		private void getResponseAsync(HttpWebRequest request, Action<HttpWebResponse, Exception> gotResponse) {

			// create an additional action wrapper, because of:
			// https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.begingetresponse.aspx
			// The BeginGetResponse method requires some synchronous setup tasks to complete (DNS resolution,
			//proxy detection, and TCP socket connection, for example) before this method becomes asynchronous.
			// As a result, this method should never be called on a user interface (UI) thread because it might
			// take considerable time(up to several minutes depending on network settings) to complete the
			// initial synchronous setup tasks before an exception for an error is thrown or the method succeeds.

			Action actionWrapper = () => {
				// BeginInvoke runs on a thread of the thread pool (!= main/UI thread)
				// that's why we need SynchronizationContext when 
				// TODO: how to influence threadpool: nr of threads etc.
				request.BeginGetResponse((asycnResult) => {
					try { // there's a try/catch here because execution path is different from invokation one, exception here may cause a crash
						HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asycnResult);
						gotResponse(response, null);
					}
					// EndGetResponse() throws on on some status codes, try to get response anyway (and status codes)
					catch (WebException wex) {
						HttpWebResponse hwr = wex.Response as HttpWebResponse;
						if (null == hwr) {
							throw;
						}
						gotResponse(hwr, wex);
					}
					catch (Exception ex) {
						gotResponse(null, ex);
					}
				}
				, null);
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
#if !NETFX_CORE
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
#endif
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
				}
			}

			// post (async) callback back to the main/UI thread
			// Unity: SynchronizationContext doesn't do anything
			//        use the Dispatcher
#if !UNITY
			_sync.Post(delegate { _callback(response); }, null);
#else
			UnityMainThreadDispatcher.Instance().Enqueue(() => _callback(response));
#endif
			IsCompleted = true;
		}


		public void Cancel() {

			_callback = null;
			if (null != _hwr) {
				_hwr.Abort();
			}
			IsCompleted = true;
		}


	}
}
