namespace Mapbox.Platform
{
	public interface IAsyncRequestHandler : IFileSource
	{
		IAsyncRequestHandler NextHandler { set; }
	}
}
