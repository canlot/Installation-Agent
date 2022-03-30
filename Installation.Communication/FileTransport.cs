using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Newtonsoft.Json;

namespace Installation.Communication
{
    public class FileTransport : ITransport
    {
        public string SendDirectoryPath { get; set; }
        public string ReceiveDirectoryPath { get; set; }

        public string SendFileName = "Client-";
        public string ReceiveFileName = "Service-";

        private int fetchInvervalInmSec = 500;

        public FileTransport()
        {

            SendDirectoryPath = System.IO.Directory.GetCurrentDirectory() + "\\";
            ReceiveDirectoryPath = System.IO.Directory.GetCurrentDirectory() + "\\";
        }

        public FileTransport(string SendDirectoryPath, string ReceiveDirectoryPath) : this()
        {
            this.SendDirectoryPath = SendDirectoryPath;
            this.ReceiveDirectoryPath = ReceiveDirectoryPath;
        }

        public delegate void JobDelegate(Job job);
        public event JobDelegate OnNewJob;

        public void Listen(CancellationToken cancelationToken)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (cancelationToken.IsCancellationRequested)
                        break;
                    await Task.Delay(fetchInvervalInmSec);
                    var jobs = checkAndGetNewJob();
                    foreach (var job in jobs)
                    {
                        OnNewJob(job);
                    }

                }

            }, cancelationToken);
        }

        public void Send(Job job)
        {
            string filePath = SendDirectoryPath + SendFileName + job.JobID.ToString() + ".json";
            try
            {
                serializeFile(filePath, job);
            }
            catch
            {

            }
        }

        private List<Job> checkAndGetNewJob()
        {
            List<Job> jobs = new List<Job>();
            try
            {
                string[] files = Directory.GetFiles(ReceiveDirectoryPath, ReceiveFileName + "*.json");

                foreach (var file in files)
                {
                    try
                    {
                        jobs.Add(deserializeFile(file));
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return jobs;
        }

        private Job deserializeFile(string path)
        {
            Job job;
            JsonSerializer serializer = new JsonSerializer();
            
            using(StreamReader rd = new StreamReader(path))
            using(JsonReader reader = new JsonTextReader(rd))
            {
                job = serializer.Deserialize<Job>(reader);
            }
            return job;
        }
        private void serializeFile(string path, Job job)
        {
            string text = JsonConvert.SerializeObject(job);
            File.WriteAllText(path, text);
        }
    }
}
