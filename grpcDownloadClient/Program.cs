using Grpc.Core;
using Grpc.Net.Client;
using grpcFileTransportClient;

var channel = GrpcChannel.ForAddress("http://localhost:5156");
var client = new FileService.FileServiceClient(channel);

string downloadPath = @"D:\Codes\Visual Studio 2022\gRPCFileApp\grpcDownloadClient\downloadFiles";

var fileInfo = new grpcFileTransportClient.FileInfo
{
    FileName = "yazlab2",
    FileExtension = ".rar",
};

FileStream fileStream = null;

var request = client.FileDowload(fileInfo);

int count = 0;
decimal chunkSize = 0;
while (await request.ResponseStream.MoveNext())
{
    if (count++ == 0)
    {
        fileStream = new FileStream
        ($"{downloadPath}/{request.ResponseStream.Current.Info.FileName}{request.ResponseStream.Current.Info.FileExtension}"
        , FileMode.CreateNew);

        fileStream.SetLength(request.ResponseStream.Current.FileSize);
    }

    var buffer = request.ResponseStream.Current.Buffer.ToByteArray();
    await fileStream.WriteAsync(buffer, 0, request.ResponseStream.Current.ReadedByte);
    Console.WriteLine
    ($"{Math.Round(((chunkSize += request.ResponseStream.Current.ReadedByte) * 100) / request.ResponseStream.Current.FileSize)}%");
}

System.Console.WriteLine("Yüklendi");
await fileStream.DisposeAsync();
fileStream.Close();
