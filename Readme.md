# Yuki.Blog.API

Provides a small API that allows blog post create, get and delete.

## Prerequisites
- Visual Studio 2022 or higher
- .NET 9 SDK or higher
- A Docker-compatible container host (eg. Docker Desktop + WSL)

## Installation
1) From Visual Studio:

Simply build and run as any other solution. 

There are 2 launch profiles defined: `http`, which will run under Kestrel locally, and `Container (Dockerfile)`, which will build the Docker image and start a new Docker container.

This configuration supports debug.

2) From the .NET CLI:

* Open a new CMD/PS console in the solution folder (or use the Developer Powershell window in Visual Studio)
* `dotnet build -c Release`
* `dotnet run`
* Open a new browser tab at `http://localhost:5136/scalar`

This configuration does not support debug.

3) Using Docker CLI:

* Open a new CMD/PS console in the solution folder (or use the Developer Powershell window in Visual Studio)
* `docker build -t yuki.blog.api -f Dockerfile ..`
* `docker run --name yuki.blog.api -p 32705:8080 -it --rm yuki.blog.api`
* Open a new browser tab at `http://localhost:32705/scalar`

This configuration does not support debug.

## Testing
- Open a new CMD/PS console in the solution folder 
- `- dotnet tool install --global dotnet-reportgenerator-globaltool`
- `- dotnet test -- --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml`
- `- ReportGenerator -reports:"../Yuki.Blog.Tests/bin/Debug/net9.0/TestResults/coverage.cobertura.xml;../Yuki.Blog.Tests.Integration/bin/Debug/net9.0/TestResults/coverage.cobertura.xml" -targetdir:../TestReport`

## Usage
First, the API is versioned. You can select the version from the top-left selector. In the current implementation, both APIs have identical surface, so this feature is here just to demo API versioning.

<img width="298" height="325" alt="version_selector" src="https://github.com/user-attachments/assets/85e4f81d-87f7-4df6-b0c7-d9d0aa0be1cf" />

API v1 uses Controllers, v2 uses Minimal API.

To test an endpoint, you can use the Scalar Test Request feature:

<img width="1620" height="843" alt="test_request" src="https://github.com/user-attachments/assets/78bd04b5-9650-418e-9c5d-e79a4fd1646f" />

The API version can be specified within the request through either the X-API-Version header or the api-version query string:

<img width="698" height="780" alt="request_version" src="https://github.com/user-attachments/assets/43a4d56a-8467-4087-9ae8-5529d24c419a" />

Finally, to use content negotiation you can change the accept header to either `application/json` or `application/xml`. Only v1 supports this.

<img width="1389" height="540" alt="request_xml" src="https://github.com/user-attachments/assets/ea40f046-0d9f-4fd5-9e5e-bc200352024a" />
