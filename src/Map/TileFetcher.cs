//https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/TileFetcher.cs

using System;
using System.Threading;
using Amib.Threading;
using System.Net;
using Mapbox.Utils;
using System.Runtime.Serialization;

namespace Mapbox.Map {
	public class TileFetcher : IDisposable {
		internal class NoopCache : ITileCache<byte[]> {
			public static readonly NoopCache Instance = new NoopCache();

			public void Add(CanonicalTileId index, byte[] image) {
			}

			public void Remove(CanonicalTileId index) {
			}

			public byte[] Get(CanonicalTileId index) {
				return null;
			}
		}

		private IFileSource _FileSource;
		private MemoryCache<byte[]> _volatileCache;
		private ITileCache<byte[]> _permaCache;
		private SmartThreadPool _threadPool;

		private System.Collections.Concurrent.ConcurrentDictionary<CanonicalTileId, int> _activeTileRequests =
			new System.Collections.Concurrent.ConcurrentDictionary<CanonicalTileId, int>();
		private System.Collections.Concurrent.ConcurrentDictionary<CanonicalTileId, int> _openTileRequests =
			new System.Collections.Concurrent.ConcurrentDictionary<CanonicalTileId, int>();

		/// <summary>
		/// Creates an instance of this class
		/// </summary>
		/// <param name="provider">The tile provider</param>
		/// <param name="minTiles">min. number of tiles in memory cache</param>
		/// <param name="maxTiles">max. number of tiles in memory cache</param>
		/// <param name="permaCache">The perma cache</param>
		internal TileFetcher(IFileSource fileSource, Tile tile, int minTiles, int maxTiles, ITileCache<byte[]> permaCache)
			: this(fileSource, minTiles, maxTiles, permaCache, 4) {
		}

		/// <summary>
		/// Creates an instance of this class
		/// </summary>
		/// <param name="provider">The tile provider</param>
		/// <param name="minTiles">min. number of tiles in memory cache</param>
		/// <param name="maxTiles">max. number of tiles in memory cache</param>
		/// <param name="permaCache">The perma cache</param>
		/// <param name="maxNumberOfThreads">The maximum number of threads used to get the tiles</param>
		internal TileFetcher(
			IFileSource fileSource
			, int minTiles
			, int maxTiles
			, ITileCache<byte[]> permaCache,
			int maxNumberOfThreads
		) {
			_FileSource = fileSource;
			_volatileCache = new MemoryCache<byte[]>(minTiles, maxTiles);
			_permaCache = permaCache ?? NoopCache.Instance;
			_threadPool = new SmartThreadPool(
				10000 //idletimeout in ms
				, maxNumberOfThreads
			);
			AsyncMode = maxNumberOfThreads > 1;
		}

		/// <summary>
		/// Method to get the tile
		/// </summary>
		/// <param name="tileInfo">The tile info</param>
		/// <param name="are">A manual reset event object</param>
		/// <returns>An array of bytes</returns>
		internal byte[] GetTile(string tileUrl, CanonicalTileId tileId, AutoResetEvent are) {
			var res = _volatileCache.Get(tileId);
			if(res != null)
				return res;

			res = _permaCache.Get(tileId);
			if(res != null) {
				_volatileCache.Add(tileId, res);
				return res;
			}

			if(!Contains(tileId)) {
				Add(tileId);
				_threadPool.QueueWorkItem(
					new WorkItemInfo() /*{ UseCallerCallContext = true }*/
					, GetTileOnThread
					, AsyncMode
					? new object[] { tileUrl, tileId }
					: new object[] { tileUrl, tileId, are ?? new AutoResetEvent(false) }
				);
			}

			return null;
		}

		/// <summary>
		/// Method to check if a tile has already been requested
		/// </summary>
		/// <param name="tileIndex">The tile index object</param>
		/// <returns><c>true</c> if the index object is already in the queue</returns>
		private bool Contains(CanonicalTileId tileIndex) {
			var res = _activeTileRequests.ContainsKey(tileIndex) || _openTileRequests.ContainsKey(tileIndex);
			return res;
		}

		/// <summary>
		/// Method to add a tile to the active tile requests queue
		/// </summary>
		/// <param name="tileIndex">The tile index object</param>
		private void Add(CanonicalTileId tileId) {
			if(!Contains(tileId)) {
				_activeTileRequests.TryAdd(tileId, 1);
			} else {
				//Debug.WriteLine(
				//    "Add: Ignoring TileIndex({0}, {1}, {2}) because it has already been added"
				//    , tileId.Z
				//    , tileId.X
				//    , tileId.Y
				//);
			}
		}


		/// <summary>
		/// Method to actually get the tile from the <see cref="_provider"/>.
		/// </summary>
		/// <param name="parameter">The parameter, usually a <see cref="TileInfo"/> and a <see cref="AutoResetEvent"/></param>
		private object GetTileOnThread(object parameter) {
			var @params = (object[])parameter;
			string tileUrl = (string)@params[0];
			var tileId = (CanonicalTileId)@params[1];

			byte[] result = null;
			string errorMessage = string.Empty;

			if(!Thread.CurrentThread.IsAlive)
				return result;
			bool fetched = false;
			//Try get the tile
			try {

				_openTileRequests.TryAdd(tileId, 1);

				_FileSource.Request(tileUrl, (Response response) => {
					if(!string.IsNullOrEmpty(response.Error)) {
						//TODO: evaluate headers sent by server, or do this in IFileSource
						//if (null != response.Headers)
						//{
						//    string hdrs = "";
						//    foreach (var hdr in response.Headers)
						//    {
						//        hdrs += string.Format("{0}: {1}\n", hdr.Key, hdr.Value);
						//    }
						//    UnityEngine.Debug.LogErrorFormat("+++++ TileFetcher.GetTileOnThread(), _FileSource response.Error: \n[{0}]\n[{1}]\nheaders:\n{2}", tileUrl, response.Error, hdrs);
						//}
					}
					result = response.Data;
					if(null == result) {
						errorMessage = "+++++ TileFetcher.GetTileOnThread(), no data receiced, " + response.Error;
					} else {
						try {
							result = Compression.Decompress(result);
						}
						catch(Exception exDecompress) {
							string msg = string.Format("+++++ TileFetcher.GetTileOnThread(), exception: [{0}], {1}", exDecompress, response.Error);
							errorMessage = msg;
#if UNITY_EDITOR
								UnityEngine.Debug.LogError(msg);
#else
							System.Diagnostics.Debug.WriteLine(msg, "ERROR");
#endif
						}
					}
					fetched = true;
				});
			}
			catch(Exception ex) {
				PreserveStackTrace(ex);
				string msg = string.Format("+++++ TileFetcher.GetTileOnThread(), exception: [{0}]", ex);
				errorMessage = msg;
#if UNITY_EDITOR
				UnityEngine.Debug.LogError(msg);
#else
				System.Diagnostics.Debug.WriteLine(msg, "ERROR");
#endif
				fetched = true;
			}

			//HACK: wait till request has finish
			while(!fetched) {
				Thread.Sleep(5);
			}

			//Try at least once again
			if(result == null) {
				try {
					//result = _provider.GetTile(tileId);
					using(WebClient wc = new WebClient()) {
						result = wc.DownloadData(tileUrl);
					}
				}
				catch {
					if(!AsyncMode) {
						var are = (AutoResetEvent)@params[2];
						are.Set();
					}
				}
			}

			//Remove the tile info request
			int one;
			if(!_activeTileRequests.TryRemove(tileId, out one)) {
				//try again
				_activeTileRequests.TryRemove(tileId, out one);
			}
			if(!_openTileRequests.TryRemove(tileId, out one)) {
				//try again
				_openTileRequests.TryRemove(tileId, out one);
			}


			if(result != null) {
				//Add to the volatile cache
				_volatileCache.Add(tileId, result);
				//Add to the perma cache
				_permaCache.Add(tileId, result);

				if(AsyncMode) {
					//Raise the event
					OnTileReceived(new TileFetcherTileReceivedEventArgs(tileId, result));
				} else {
					var are = (AutoResetEvent)@params[1];
					are.Set();
				}
			}

			//Tile couldn't be fetched - fire event with error
			//TODO: bubble proper message
			if(null == result) {
				OnTileReceived(new TileFetcherTileReceivedEventArgs(tileId, result, errorMessage));
			}
			return result;
		}


		//TODO: for debuggin during development. remove here, or move to utils
		private static void PreserveStackTrace(Exception e) {
			var ctx = new StreamingContext(StreamingContextStates.CrossAppDomain);
			var mgr = new ObjectManager(null, ctx);
			var si = new SerializationInfo(e.GetType(), new FormatterConverter());

			e.GetObjectData(si, ctx);
			mgr.RegisterObject(e, 1, si); // prepare for SetObjectData
			mgr.DoFixups(); // ObjectManager calls SetObjectData

		}

		/// <summary>
		/// Gets or sets a value indicating whether the tile fetcher should work in async mode or not.
		/// </summary>
		public bool AsyncMode { get; private set; }

		public bool Ready() {
			return (_activeTileRequests.Count == 0 && _openTileRequests.Count == 0);
		}

		/// <summary>
		/// Event raised when tile fetcher is in <see cref="AsyncMode"/> and a tile has been received.
		/// </summary>
		public event EventHandler<TileFetcherTileReceivedEventArgs> TileReceived;

		/// <summary>
		/// Event invoker for the <see cref="TileReceived"/> event
		/// </summary>
		/// <param name="tileReceivedEventArgs">The event arguments</param>
		private void OnTileReceived(TileFetcherTileReceivedEventArgs tileReceivedEventArgs) {
			// Don't raise events if we are not in async mode!
			if(!AsyncMode)
				return;

			if(TileReceived != null) {
				TileReceived(this, tileReceivedEventArgs);
			}

			var i = tileReceivedEventArgs.TileId;

			if(_activeTileRequests.Count == 0 && _openTileRequests.Count == 0) {
				OnQueueEmpty(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Event raised when <see cref="AsyncMode"/> is <c>true</c> and the tile request queue is empty
		/// </summary>
		public event EventHandler QueueEmpty;

		/// <summary>
		/// Event invoker for the <see cref="TileReceived"/> event
		/// </summary>
		/// <param name="eventArgs">The event arguments</param>
		private void OnQueueEmpty(EventArgs eventArgs) {
			// Don't raise events if we are not in async mode!
			if(!AsyncMode) {
				return;
			}

			if(QueueEmpty != null) {
				QueueEmpty(this, eventArgs);
			}
		}


		void IDisposable.Dispose() {

			if(_volatileCache == null) { return; }

			_volatileCache.Clear();
			_volatileCache = null;
			_permaCache = null;

			_threadPool.Dispose();
			_threadPool = null;

			_activeTileRequests.Clear();
			_activeTileRequests = null;

			_openTileRequests.Clear();
			_openTileRequests = null;
		}


		/// <summary>
		/// Clears the memory cache
		/// </summary>
		public void ClearMemoryCache() {
			if(null == _volatileCache) { return; }
			_volatileCache.Clear();
		}


		/// <summary>
		/// Method to cancel the working queue, see http://dotspatial.codeplex.com/discussions/473428
		/// </summary>
		public void Clear() {
			_threadPool.Cancel(false);
			foreach(var request in _activeTileRequests.ToArray()) {
				int one;
				if(!_openTileRequests.ContainsKey(request.Key)) {
					if(!_activeTileRequests.TryRemove(request.Key, out one))
						_activeTileRequests.TryRemove(request.Key, out one);
				}
			}
			_openTileRequests.Clear();
		}



	}
}