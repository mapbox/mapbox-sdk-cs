Thank you note here.

Support email:

To view the documentation, open "index.html."

Demos
* ForwardGeocoder:
	A forward geocoding request will fetch GeoJSON from a place name query.
	See: https://www.mapbox.com/api-documentation/#geocoding for more information.

* ReverseGeocoder:
	A reverse geocoding request will fetch GeoJSON from a latitude, longitude query.
	See: https://www.mapbox.com/api-documentation/#geocoding for more information.

* VectorTile:
	Uses a forward geocoder request to fetch GeoJSON from a Map object.
	In this example, the result is GeoJSON with a feature collection.
	See: https://www.mapbox.com/api-documentation/#retrieve-features-from-vector-tiles

* RasterTile:
	Uses a forward geocoder request to fetch a static tile from a Map object.
	"Static maps are standalone images that can be displayed on web and mobile devices without the aid of a mapping library or API.
	They look like an embedded map without interactivity or controls."
	See: https://www.mapbox.com/api-documentation/#static

* Directions:
	Enter a start location query (eg. "San Francisco, CA"), this is a forward geocode request.
	Enter a destination query (eg. "Los Angeles, CA"), this is also a forward geocode request.
	When the requests have been completed, a directions request is executed.
	Direction results will be logged to the UI when they are available (in the form of JSON).
