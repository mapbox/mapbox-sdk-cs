# Contributing code

If you want to contribute code:

1. Ensure that existing [pull requests](https://github.com/mapbox/mapbox-sdk-cs/pulls) and [issues](https://github.com/mapbox/mapbox-sdk-cs/issues) don’t already cover your contribution or question.

2. Pull requests are gladly accepted. We require code reviews before merging PRs. When your tests pass, tag a project contributor (for example, @tmpsantos or @BergWerkGIS) and request a review.

# Requirements and installation

#### Mac OS

Use [Mono](http://www.mono-project.com/) to compile the SDK and to run executables. [Xamarin Studio](https://www.xamarin.com/download) is the recommended IDE.

#### Linux

Coming soon.

####  Windows

Coming soon.

# Running tests

Tests are located in [test/UnitTest](https://github.com/mapbox/mapbox-sdk-cs/tree/master/test/UnitTest). On Linux or a mac run them with Mono, `mono packages/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe bin/Debug/test/Mapbox.UnitTest.dll` or from within your IDE of choice. Tests require a valid MAPBOX_ACCESS_TOKEN env variable in order to pass. Get a MAPBOX_ACCESS_TOKEN by logging into [Mapbox Studio](https://www.mapbox.com/studio/).

# Generating documentation

Documentation for the the Mapbox SDK C# is automatically generated from XML headers in code. *Instructions for generating documentation are coming soon*.

# Code of conduct

Everyone is invited to participate in Mapbox’s open source projects and public discussions: we want to create a welcoming and friendly environment. Harassment of participants or other unethical and unprofessional behavior will not be tolerated in our spaces. The [Contributor Covenant](http://contributor-covenant.org) applies to all projects under the Mapbox organization and we ask that you please read [the full text](http://contributor-covenant.org/version/1/2/0/).

You can learn more about our open source philosophy on [mapbox.com](https://www.mapbox.com/about/open/).
