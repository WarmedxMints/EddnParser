using ICSharpCode.SharpZipLib.BZip2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EddnParser.EDDN
{
    internal sealed class EddnHistoryParser
    {
        public static async ValueTask<List<Commanders>> ParseHistory(string systemOfInterest)
        {
            var Commanders = new List<Commanders>();

            await Task.Run(() =>
            {
                JsonSerializer serializer = new()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };



                var ret = new List<string>();

                var dir = Directory.GetCurrentDirectory();

                var fsdJumpFiles = Directory.GetFiles(dir).Where(f => f.Contains("Journal.FSDJump") && f.EndsWith(".bz2")).ToArray();

                foreach (var file in fsdJumpFiles)
                {
                    using FileStream ss = new(file, FileMode.Open);
                    using MemoryStream ms = new();
                    using (BZip2InputStream bzip2Stream = new(ss))
                    {
                        bzip2Stream.CopyTo(ms);
                    }

                    ms.Seek(0, SeekOrigin.Begin);  // so I put the stream to the initial position

                    using StreamReader sr = new(ms, Encoding.UTF8);
                    using JsonTextReader reader = new(sr) { CloseInput = false, SupportMultipleContent = true };

                    while (reader.Read())
                    {
                        // deserialize only when there's "{" character in the stream
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            var jumpEventLog = serializer.Deserialize<dynamic>(reader);

                            if (jumpEventLog is null)
                            {
                                continue;
                            }
                            var uploader = jumpEventLog.header.uploaderID.ToString();

                            var system = jumpEventLog.message.StarSystem.ToString();

                            var target = string.Equals(jumpEventLog.message.StarSystem.ToString(), systemOfInterest, StringComparison.InvariantCultureIgnoreCase);

                            if (target && ret.Contains(uploader) == false)
                            {
                                ret.Add(uploader);
                                Commanders cmdr = new(uploader);

                                if (DateTime.TryParse(jumpEventLog.message.timestamp.ToString(), out DateTime result))
                                {
                                    CommanderMessages message = new()
                                    {
                                        TimeStamp = result.ToUniversalTime(),
                                        EventType = CommanderMessages.EventTpe.Jumped,
                                        Message = system,
                                        TargetSystem = target,
                                    };

                                    cmdr.Messages.Add(message);
                                }

                                Commanders.Add(cmdr);
                                continue;
                            }

                            if (ret.Contains(uploader) == true)
                            {
                                var cmdr = Commanders.Find(f => f.Id == uploader);

                                if (cmdr is null)
                                {
                                    continue;
                                }

                                if (DateTime.TryParse(jumpEventLog.message.timestamp.ToString(), out DateTime result))
                                {
                                    if (ContainsEvent(result, cmdr))
                                    {
                                        continue;
                                    }

                                    CommanderMessages message = new()
                                    {
                                        TimeStamp = result.ToUniversalTime(),
                                        EventType = CommanderMessages.EventTpe.Jumped,
                                        Message = system,
                                        TargetSystem = target,
                                    };

                                    cmdr.Messages.Add(message);
                                }
                            }
                        }
                    }

                    reader.Close();
                    sr.Dispose();
                    ms.Dispose();
                    ss.Dispose();
                }

                foreach (var file in fsdJumpFiles)
                {
                    using FileStream ss = new(file, FileMode.Open);
                    using MemoryStream ms = new();
                    using (BZip2InputStream bzip2Stream = new(ss))
                    {
                        bzip2Stream.CopyTo(ms);
                    }

                    ms.Seek(0, SeekOrigin.Begin);  // so I put the stream to the initial position

                    using StreamReader sr = new(ms, Encoding.UTF8);
                    using JsonTextReader reader = new(sr) { CloseInput = false, SupportMultipleContent = true };

                    while (reader.Read())
                    {
                        // deserialize only when there's "{" character in the stream
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            var jumpEventLog = serializer.Deserialize<dynamic>(reader);

                            if (jumpEventLog is null)
                            {
                                continue;
                            }
                            var uploader = jumpEventLog.header.uploaderID.ToString();

                            var system = jumpEventLog.message.StarSystem.ToString();

                            var target = string.Equals(jumpEventLog.message.StarSystem.ToString(), systemOfInterest, StringComparison.InvariantCultureIgnoreCase);

                            if (ret.Contains(uploader) == true)
                            {
                                var cmdr = Commanders.Find(f => f.Id == uploader);

                                if (cmdr is null)
                                {
                                    continue;
                                }

                                if (DateTime.TryParse(jumpEventLog.message.timestamp.ToString(), out DateTime result))
                                {
                                    if (ContainsEvent(result, cmdr))
                                    {
                                        continue;
                                    }

                                    CommanderMessages message = new()
                                    {
                                        TimeStamp = result.ToUniversalTime(),
                                        EventType = CommanderMessages.EventTpe.Jumped,
                                        Message = system,
                                        TargetSystem = target,
                                    };

                                    cmdr.Messages.Add(message);
                                }
                            }
                        }
                    }

                    reader.Close();
                    sr.Dispose();
                    ms.Dispose();
                    ss.Dispose();
                }

                var dockedFiles = Directory.GetFiles(dir).Where(f => f.Contains("Journal.Docked") && f.EndsWith(".bz2")).ToArray();

                foreach (var file in dockedFiles)
                {
                    using FileStream ss = new(file, FileMode.Open);
                    using MemoryStream ms = new();
                    using (BZip2InputStream bzip2Stream = new(ss))
                    {
                        bzip2Stream.CopyTo(ms);
                    }

                    ms.Seek(0, SeekOrigin.Begin);  // so I put the stream to the initial position

                    using StreamReader sr = new(ms, Encoding.UTF8);
                    using JsonTextReader reader = new(sr) { CloseInput = false, SupportMultipleContent = true };

                    while (reader.Read())
                    {
                        // deserialize only when there's "{" character in the stream
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            var dockedEventLog = serializer.Deserialize<dynamic>(reader);

                            if (dockedEventLog is null)
                            {
                                continue;
                            }
                            var uploader = dockedEventLog.header.uploaderID.ToString();

                            if (ret.Contains(uploader) == true)
                            {
                                var cmdr = Commanders.Find(f => f.Id == uploader);

                                if (cmdr is null)
                                {
                                    continue;
                                }

                                if (DateTime.TryParse(dockedEventLog.message.timestamp.ToString(), out DateTime result))
                                {
                                    if (ContainsEvent(result, cmdr))
                                    {
                                        continue;
                                    }

                                    CommanderMessages message = new()
                                    {
                                        TimeStamp = result.ToUniversalTime(),
                                        EventType = CommanderMessages.EventTpe.Docked,
                                        Message = dockedEventLog.message.StationName.ToString()
                                    };

                                    cmdr.Messages.Add(message);
                                }
                            }
                        }
                    }

                    reader.Close();
                    sr.Dispose();
                    ms.Dispose();
                    ss.Dispose();
                }

                var approachFiles = Directory.GetFiles(dir).Where(f => f.Contains("Journal.Approach") && f.EndsWith(".bz2")).ToArray();

                foreach (var file in approachFiles)
                {
                    using FileStream ss = new(file, FileMode.Open);
                    using MemoryStream ms = new();
                    using (BZip2InputStream bzip2Stream = new(ss))
                    {
                        bzip2Stream.CopyTo(ms);
                    }

                    ms.Seek(0, SeekOrigin.Begin);  // so I put the stream to the initial position

                    using StreamReader sr = new(ms, Encoding.UTF8);
                    using JsonTextReader reader = new(sr) { CloseInput = false, SupportMultipleContent = true };

                    while (reader.Read())
                    {
                        // deserialize only when there's "{" character in the stream
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            var approachEvent = serializer.Deserialize<dynamic>(reader);

                            if (approachEvent is null)
                            {
                                continue;
                            }
                            var uploader = approachEvent.header.uploaderID.ToString();

                            if (ret.Contains(uploader) == true)
                            {
                                var cmdr = Commanders.Find(f => f.Id == uploader);

                                if (cmdr is null)
                                {
                                    continue;
                                }

                                if (DateTime.TryParse(approachEvent.message.timestamp.ToString(), out DateTime result))
                                {
                                    if (ContainsEvent(result, cmdr))
                                    {
                                        continue;
                                    }

                                    CommanderMessages message = new()
                                    {
                                        TimeStamp = result.ToUniversalTime(),
                                        EventType = CommanderMessages.EventTpe.Approached,
                                        Message = approachEvent.message.Name.ToString()
                                    };

                                    cmdr.Messages.Add(message);
                                }
                            }
                        }
                    }

                    reader.Close();
                    sr.Dispose();
                    ms.Dispose();
                    ss.Dispose();
                }

                ret.Clear();

                foreach (var cmdr in Commanders)
                {
                    var messages = new List<CommanderMessages>(cmdr.Messages);

                    messages.Sort((x, y) => x.TimeStamp.CompareTo(y.TimeStamp));

                    cmdr.Messages = new System.Collections.ObjectModel.ObservableCollection<CommanderMessages>(messages);
                }

                foreach (var cmdr in Commanders)
                {
                    var msgs = cmdr.Messages.Where(x => x.EventType == CommanderMessages.EventTpe.Jumped).ToArray();

                    if (msgs.Length < 2)
                    {
                        break;
                    }

                    for (var i = 1; i < msgs.Length; i++)
                    {
                        msgs[i - 1].TimeInSystem = msgs[i].TimeStamp.Subtract(msgs[i - 1].TimeStamp);
                    }

                    var stops = cmdr.Messages.Where(x => x.TargetSystem == true);

                    var total = new TimeSpan();
                    var longest = new TimeSpan();   

                    foreach (var stop in stops) 
                    {
                        total += stop.TimeInSystem;

                        if(stop.TimeInSystem > longest)
                        {
                            longest = stop.TimeInSystem;
                        }
                    }

                    cmdr.TotalTimeInSystem = total;
                    cmdr.LongestStopInSystem = longest;

                    var start = cmdr.Messages[0].TimeStamp;
                    var end = cmdr.Messages.Last().TimeStamp;

                    cmdr.SessionTime = end - start;

                    cmdr.JumpCount = cmdr.Messages.Where(x => x.EventType == CommanderMessages.EventTpe.Jumped).Count();
                }
            });
            return Commanders;
        }

        private static bool ContainsEvent(DateTime a, Commanders msg)
        {
            var b = msg.Messages.FirstOrDefault(x => x.TimeStamp == a);

            return b is not null;
        }
    }


}
