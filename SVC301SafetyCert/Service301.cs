using NotesMail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SVC301SafetyCert
{
    public partial class Service301 : ServiceBase
    {
        public Service301()
        {
            InitializeComponent();
        }

        int defaulDelay = 1; //minute
        private Task serviceTask;
        private bool isRunning;

        protected override void OnStart(string[] args)
        {
            isRunning = true;

            AppendRowToFileAsync("Start");
            serviceTask = Task.Run(RunTasksAsync);
        }

        protected override void OnStop()
        {
            isRunning = false;
            AppendRowToFileAsync("Stop");
            try
            {
                serviceTask.Wait();
            }
            catch (Exception ex)
            {
                AppendRowToFileAsync("Task - An error occurred: " + ex.Message);
            }
        }

        private void AppendRowToFileAsync(string msg)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Today.ToString("yyyy"), DateTime.Today.ToString("MM"));
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, DateTime.Today.ToString("dd") + ".txt");

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"{DateTime.Now:hh:mm:ss tt}: {msg}");
            }
        }

        private async Task RunTasksAsync()
        {
            var lsTasks = new List<Task>();

            Task task1 = Task.Run(async () =>
            {
                while (isRunning)
                {
                    try
                    {
                        AppendRowToFileAsync("Recal: NotifyDeptChange");
                        await NotesMail.Mail301SafetyCert.Notify301DeptChange();
                    }
                    catch (Exception ex)
                    {
                        AppendRowToFileAsync("Task NotifyDeptChange - An error occurred: " + ex.Message);
                    }
                    await Task.Delay(TimeSpan.FromMinutes(defaulDelay));
                }
            });
            lsTasks.Add(task1);

            //Task task2 = Task.Run(async () =>
            //{
            //    while (isRunning)
            //    {
            //        try
            //        {
            //            AppendRowToFileAsync("Recal: NotifyDocProcessing");
            //            await Notify207DocProcessing();
            //        }
            //        catch (Exception ex)
            //        {
            //            AppendRowToFileAsync("Task NotifyDocProcessing - An error occurred: " + ex.Message);
            //        }
            //        await Task.Delay(TimeSpan.FromMinutes(defaulDelay));
            //    }
            //});
            //lsTasks.Add(task2);

            await Task.WhenAll(lsTasks);
        }
    }
}
