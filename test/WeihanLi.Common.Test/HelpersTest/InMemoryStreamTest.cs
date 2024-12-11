// using WeihanLi.Common.Helpers;
// using Xunit;

// namespace WeihanLi.Common.Test.HelpersTest
// {
//     public class InMemoryStreamTest
//     {
//         private readonly InMemoryStream _inMemoryStream;

//         public InMemoryStreamTest()
//         {
//             _inMemoryStream = new InMemoryStream();
//         }

//         [Fact]
//         public async Task AddMessageAsync_ShouldAddMessageToStream()
//         {
//             var streamName = "testStream";
//             var message = new StreamMessage
//             {
//                 Id = Guid.NewGuid().ToString(),
//                 Data = "TestData",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message);

//             // var messages = await _inMemoryStream.ReadMessagesAsync(streamName, 1);
//             // Assert.Single(messages);
//             // Assert.Equal(message.Id, messages[0].Id);
//             // Assert.Equal(message.Data, messages[0].Data);
//             // Assert.Equal(message.Timestamp, messages[0].Timestamp);
//             // Assert.Equal(message.Properties["traceId"], messages[0].Properties["traceId"]);
//             // Assert.Equal(message.Properties["correlationId"], messages[0].Properties["correlationId"]);
//         }

//         [Fact]
//         public async Task ReadMessagesAsync_ShouldReadMessagesFromStream()
//         {
//             var streamName = "testStream";
//             var message1 = new StreamMessage
//             {
//                 Id = Guid.NewGuid().ToString(),
//                 Data = "TestData1",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };
//             var message2 = new StreamMessage
//             {
//                 Id = Guid.NewGuid().ToString(),
//                 Data = "TestData2",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "54321" },
//                     { "correlationId", "09876" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message1);
//             await _inMemoryStream.AddMessageAsync(streamName, message2);

//             var messages = await _inMemoryStream.ReadMessagesAsync(streamName, 2).ToArrayAsync();
//             Assert.Equal(2, messages.Length);
//             Assert.Contains(messages, m => m.Id == message1.Id);
//             Assert.Contains(messages, m => m.Id == message2.Id);
//         }

//         [Fact]
//         public async Task AcknowledgeMessageAsync_ShouldRemoveMessageFromStream()
//         {
//             var streamName = "testStream";
//             var message = new StreamMessage
//             {
//                 Id = Guid.NewGuid().ToString(),
//                 Data = "TestData",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message);
//             await _inMemoryStream.AcknowledgeMessageAsync(streamName, message.Id);

//             var messages = await _inMemoryStream.ReadMessagesAsync(streamName, 1);
//             Assert.Empty(messages);
//         }

//         [Fact]
//         public async Task ReadMessagesWithStartEndAsync_ShouldReadMessagesWithinRange()
//         {
//             var streamName = "testStream";
//             var message1 = new StreamMessage
//             {
//                 Id = "1",
//                 Data = "TestData1",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };
//             var message2 = new StreamMessage
//             {
//                 Id = "2",
//                 Data = "TestData2",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "54321" },
//                     { "correlationId", "09876" }
//                 }
//             };
//             var message3 = new StreamMessage
//             {
//                 Id = "3",
//                 Data = "TestData3",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "67890" },
//                     { "correlationId", "12345" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message1);
//             await _inMemoryStream.AddMessageAsync(streamName, message2);
//             await _inMemoryStream.AddMessageAsync(streamName, message3);

//             var messages = await _inMemoryStream.ReadMessagesAsync(streamName, 2, "1", "2");
//             Assert.Equal(2, messages.Count);
//             Assert.Contains(messages, m => m.Id == message1.Id);
//             Assert.Contains(messages, m => m.Id == message2.Id);
//         }

//         [Fact]
//         public async Task CountAsync_ShouldReturnCorrectCount()
//         {
//             var streamName = "testStream";
//             var message1 = new StreamMessage
//             {
//                 Id = "1",
//                 Data = "TestData1",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };
//             var message2 = new StreamMessage
//             {
//                 Id = "2",
//                 Data = "TestData2",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "54321" },
//                     { "correlationId", "09876" }
//                 }
//             };
//             var message3 = new StreamMessage
//             {
//                 Id = "3",
//                 Data = "TestData3",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "67890" },
//                     { "correlationId", "12345" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message1);
//             await _inMemoryStream.AddMessageAsync(streamName, message2);
//             await _inMemoryStream.AddMessageAsync(streamName, message3);

//             var count = await _inMemoryStream.CountAsync(streamName, "1", "2");
//             Assert.Equal(2, count);
//         }

//         [Fact]
//         public async Task TrimAsync_ShouldRemoveMessagesExceedingMaxSize()
//         {
//             var streamName = "testStream";
//             var message1 = new StreamMessage
//             {
//                 Id = "1",
//                 Data = "TestData1",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };
//             var message2 = new StreamMessage
//             {
//                 Id = "2",
//                 Data = "TestData2",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "54321" },
//                     { "correlationId", "09876" }
//                 }
//             };
//             var message3 = new StreamMessage
//             {
//                 Id = "3",
//                 Data = "TestData3",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "67890" },
//                     { "correlationId", "12345" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message1);
//             await _inMemoryStream.AddMessageAsync(streamName, message2);
//             await _inMemoryStream.AddMessageAsync(streamName, message3);

//             await _inMemoryStream.TrimAsync(streamName, 2);

//             var messages = await _inMemoryStream.ReadMessagesAsync(streamName, 3);
//             Assert.Equal(2, messages.Count);
//             Assert.DoesNotContain(messages, m => m.Id == message1.Id);
//         }

//         [Fact]
//         public async Task TrimAsync_ShouldRemoveMessagesExceedingMaxAge()
//         {
//             var streamName = "testStream";
//             var message1 = new StreamMessage
//             {
//                 Id = "1",
//                 Data = "TestData1",
//                 Timestamp = DateTimeOffset.Now.AddMinutes(-10),
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };
//             var message2 = new StreamMessage
//             {
//                 Id = "2",
//                 Data = "TestData2",
//                 Timestamp = DateTimeOffset.Now.AddMinutes(-5),
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "54321" },
//                     { "correlationId", "09876" }
//                 }
//             };
//             var message3 = new StreamMessage
//             {
//                 Id = "3",
//                 Data = "TestData3",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "67890" },
//                     { "correlationId", "12345" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message1);
//             await _inMemoryStream.AddMessageAsync(streamName, message2);
//             await _inMemoryStream.AddMessageAsync(streamName, message3);

//             await _inMemoryStream.TrimAsync(streamName, TimeSpan.FromMinutes(6));

//             var messages = await _inMemoryStream.ReadMessagesAsync(streamName, 3);
//             Assert.Equal(2, messages.Count);
//             Assert.DoesNotContain(messages, m => m.Id == message1.Id);
//         }

//         [Fact]
//         public async Task ReadMessagesAsync_WithCancellationToken_ShouldReadMessagesFromStream()
//         {
//             var streamName = "testStream";
//             var message1 = new StreamMessage
//             {
//                 Id = Guid.NewGuid().ToString(),
//                 Data = "TestData1",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "12345" },
//                     { "correlationId", "67890" }
//                 }
//             };
//             var message2 = new StreamMessage
//             {
//                 Id = Guid.NewGuid().ToString(),
//                 Data = "TestData2",
//                 Timestamp = DateTimeOffset.Now,
//                 Properties = new Dictionary<string, object?>
//                 {
//                     { "traceId", "54321" },
//                     { "correlationId", "09876" }
//                 }
//             };

//             await _inMemoryStream.AddMessageAsync(streamName, message1);
//             await _inMemoryStream.AddMessageAsync(streamName, message2);

//             var cancellationTokenSource = new CancellationTokenSource();
//             var messages = new List<StreamMessage>();

//             await foreach (var message in _inMemoryStream.ReadMessagesAsync(streamName, cancellationTokenSource.Token))
//             {
//                 messages.Add(message);
//                 if (messages.Count == 2)
//                 {
//                     cancellationTokenSource.Cancel();
//                 }
//             }

//             Assert.Equal(2, messages.Count);
//             Assert.Contains(messages, m => m.Id == message1.Id);
//             Assert.Contains(messages, m => m.Id == message2.Id);
//         }
//     }
// }
