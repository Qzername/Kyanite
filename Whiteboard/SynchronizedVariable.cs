using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Whiteboard.Models;

namespace Whiteboard
{
    internal class SynchronizedVariable<T>
    {
        const string ip = "http://localhost:5000/api/Data";
        static HttpClient client = new();

        DataItem variableData;
        bool synchronizeOnExit;

        T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;

                if (!synchronizeOnExit)
                    _ = SetVariable();
            }
        }

        public SynchronizedVariable(string variableName, bool synchronizeOnExit = false)
        {
            variableData = new DataItem()
            {
                Name = variableName,
                Type = "string",
                Value = [string.Empty]
            };

            this.synchronizeOnExit = synchronizeOnExit;

            if (synchronizeOnExit && Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime view)
                view.MainWindow.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _ = SetVariable();
        }

        public async Task InitializeVariable()
        {
            variableData.Value = [string.Empty];

            var jsonItem = JsonConvert.SerializeObject(variableData);

            await client.PostAsync(ip, new StringContent(jsonItem, Encoding.UTF8, "application/json"));

            await GetVariable();
        }

        async Task GetVariable()
        {
            var response = await client.GetStringAsync(ip + $"?name=" + variableData.Name);
            var item = JsonConvert.DeserializeObject<DataItem>(response);
            _value = (T)Convert.ChangeType(item.Value[0], typeof(T));
        }

        async Task SetVariable()
        {
            variableData.Value = [Value.ToString()];
            var jsonItem = JsonConvert.SerializeObject(variableData);
            await client.PutAsync(ip, new StringContent(jsonItem, Encoding.UTF8, "application/json"));
        }
    }
}
