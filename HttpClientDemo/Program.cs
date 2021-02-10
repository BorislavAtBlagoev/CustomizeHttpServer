namespace HttpClientDemo
{
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;
    using System.Collections.Generic;

    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var listOfTweets = new List<string>();
            const string NewLine = "\r\n";
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 80);
            tcpListener.Start();

            while (true)
            {
                var client = tcpListener.AcceptTcpClient();
                var sb = new StringBuilder();

                using (var stream = client.GetStream())
                {
                    var buffer = new byte[1000000];
                    var request = stream.Read(buffer, 0, buffer.Length);
                    var requestString = Encoding.UTF8.GetString(buffer, 0, request);
                    string response = string.Empty;
                    sb.Append(requestString);

                    if (requestString[0] == 'G')
                    {
                        string html = $"<h1>Hello from Boro's Server</h1>" +
                                      $"<h2>Tweet</h2>" +
                                      $"<form action=/tweet method=post><input name=tweet />" +
                                      $"<p></p>" +
                                      $"<input type=submit value=Tweet /></form>";

                         response = "HTTP/1.1 200 OK" + NewLine +
                            "Server: BoroServer 2020" + NewLine +
                            "Content-Type: text/html; charset=utf-8" + NewLine +
                            "Content-Lenght: " + html.Length + NewLine +
                            NewLine +
                            html + NewLine;
                    }
                    else if (requestString[0] == 'P')
                    {
                        var position = 0;

                        for (int i = 0; i < requestString.Length; i++)
                        {
                            if (requestString[i] == '=')
                            {
                                position = i;
                            }
                        }

                        var tweet = requestString.Substring(position + 1);
                        tweet = tweet.Replace('+', ' ');
                        listOfTweets.Add($"<h2>{tweet} <span>{DateTime.Now}</span></h2>");

                        string html = $"<h1>Tweet</h1>" +
                                      $"<form action=/tweet method=post><input name=tweet />" +
                                      $"<p></p>" +
                                      $"<input type=submit value=Tweet /></form>" +
                                      $"<p></p>" +
                                      $"{string.Join(NewLine, listOfTweets)}";

                        response = "HTTP/1.1 200 OK" + NewLine +
                           "Server: BoroServer 2020" + NewLine +
                           "Content-Type: text/html; charset=utf-8" + NewLine +
                           "Content-Lenght: " + html.Length + NewLine +
                           NewLine +
                           html + NewLine;
                    }

                    var responseByte = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseByte, 0, responseByte.Length);
                }

                Console.WriteLine(sb.ToString());
                Console.WriteLine(new string('=', 70));
            }
        }
    }
}
