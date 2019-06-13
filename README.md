# MSGraphOneDriveIssue
In order to get 504 exception you need to call function end-point many times. It can be done by running the script ```function-caller.js``` using follow  command:

```node ./function-caller.js```

During running the script exception will thrown and you see log ```'The exception was thrown'```. You can see more details of the exception in the azure function console. The our exception has follow log:
```
Error: Status Code: GatewayTimeout
Microsoft.Graph.ServiceException: Code: UnknownError

Inner error

   at Microsoft.Graph.HttpProvider.SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
   at Microsoft.Graph.BaseRequest.SendRequestAsync(Object serializableObject, CancellationToken cancellationToken, HttpCompletionOption completionOption)
   at Microsoft.Graph.BaseRequest.SendAsync[T](Object serializableObject, CancellationToken cancellationToken, HttpCompletionOption completionOption)
   at MSGraphOneDriveIssue.UploadAndEditFileFunction.Run(HttpRequest req, ILogger log) in C:\Users\vitalii.kushch\Desktop\MSGraphOneDriveIssueExample\MSGraphOneDriveIssue\MSGraphOneDriveIssue\UploadAndEditFile.cs:line 72
   ```
   
Sometimes to get the exception you need to run ```function-caller.js``` about 10 times, also possible to increase count of requests to the azure function it will give you bigger probability to reproduce the exception.
