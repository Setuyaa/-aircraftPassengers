using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Net;


namespace AircraftPassengers
{
    class Program
    {
        public static double GenerateInterval(int lambda) //  обновлять ежечастно
        {
            Random rnd2 = new Random();
            double Probability = rnd2.NextDouble();
            double value = -Math.Log(1 - Probability) / lambda;
            return value * 100000;
        }
        // перевести на логи
        static async void PrintResponse(HttpResponseMessage response)
{
    Console.WriteLine($"Status: {response.StatusCode}\n");
    Console.WriteLine("Headers");
    foreach (var header in response.Headers)
    {
        Console.Write($"{header.Key}:");
        foreach (var headerValue in header.Value)
        {
            Console.WriteLine(headerValue);
        }
    }
    Console.WriteLine("\nContent");
    string content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(content);
}
        static void WriteLogsRequestBoxOffice(Passenger pas, string address)
        {
            StreamWriter writer = new StreamWriter("logs.txt", true);
            writer.WriteLine($"\nA POST-request has been sent to the address {address} ");
            writer.Write($"\"fio\": \"{pas.nameSurname}\" ");
            writer.Write($"\"passport\": \"{pas.password}\" ");
            writer.Write($"\"date_of_birth\": \"{pas.dateOfBirth}\" ");
            writer.Write($"\"bag\": \"{pas.luggage}\" ");
            writer.Write($"\"food\": \"{pas.food}\" ");
            writer.Write($"\"where\": \"{pas.cityTo}\" ");
            writer.Close();
        }
        public async static void WriteLogsResponse(string address, HttpResponseMessage response)
        {
            StreamWriter writer = new StreamWriter("logs.txt", true);
            writer.WriteLine($"\nA POST-request has been recieved from the address {address} ");
            writer.WriteLine($"Status: {response.StatusCode}\n");
            writer.WriteLine("Headers");
            foreach (var header in response.Headers)
            {
                writer.Write($"{header.Key}:");
                foreach (var headerValue in header.Value)
                {
                    writer.WriteLine(headerValue);
                }
            }
            writer.WriteLine("\nContent");
            string content = await response.Content.ReadAsStringAsync();
            writer.WriteLine(content);
            writer.Close();
        }
        public static async Task SendRefound(Passenger pas, int id_transaction)
        {
          
            string url = "http://adel_ip_here:8080/refund_ticket"; //Adel
            string json = "{\"fio\": \"" + pas.nameSurname + "\", \"passport\": \"" + pas.password +
                             "\", \"date_of_birth\": \"" + pas.dateOfBirth + "\", \"id_transaction\": \"" + id_transaction + "\"}";
   
            HttpClient client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                PrintResponse(response);
                WriteLogsResponse(url, response);
            }
        public static async Task SendRequestToRegistrationAsync(HttpResponseMessage response, Passenger pas)
        {
            int id_transaction;
            HttpContent contentPrev = response.Content;
            string contentString = await contentPrev.ReadAsStringAsync();
            string[] splitArray = contentString.Split('_');
            int.TryParse(splitArray[1], out id_transaction);
            HttpClient client = new HttpClient();
            string url = "http://tanya_ip_here:5247/check-in";
            string json = "{\"fio\": \"" + pas.nameSurname + "\", \"passport\": \"" + pas.password +
                               "\", \"id_transaction\": \"" + id_transaction + "\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage responseReg = await client.PostAsync(url, content);
                PrintResponse(responseReg);
                string content2 = await responseReg.Content.ReadAsStringAsync();
                if (content2 == "Flight cancelled") { // 
                    await SendRefound(pas, id_transaction);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при выполнении запроса: " + ex.Message);
            }
        }

        private static async Task MainCircle()
        {
            var pas = new Passenger();
            string url = "http://adel_ip_here:8080/buy_ticket"; //Adel
            WriteLogsRequestBoxOffice(pas, url);
            HttpClient client = new HttpClient();
            string json = "{\"fio\": \"" + pas.nameSurname + "\", \"passport\": \"" + pas.password +
                            "\", \"date_of_birth\": \"" + pas.dateOfBirth + "\", \"bag\": \"" + pas.luggage +
                            "\", \"food\": \"" + pas.food + "\", \"where\": \"" + pas.cityTo + "\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                PrintResponse(response);
                WriteLogsResponse(url, response);
                if (response.IsSuccessStatusCode)
                    await SendRequestToRegistrationAsync(response, pas);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при выполнении запроса: " + ex.Message);
            }
        }
        static async Task Main()
        {
            while (true)
            {
                await MainCircle();
                double interval = GenerateInterval(30);
                Thread.Sleep((int)interval);
            }
        }
    }
}

//HttpClient client = new HttpClient();
//string url = "http://192.168.0.147:5247/check-in";
//string json = "{\"fio\": \"" + "Some Name" + "\", \"passport\": \"" + "1234567890" +
//                   "\", \"id_transaction\": \"" + 10 + "\"}";
//var content = new StringContent(json, Encoding.UTF8, "application/json");
//try
//{
//    HttpResponseMessage responseReg = await client.PostAsync(url, content);
//    //PrintResponse(response);
//}
//catch (Exception ex)
//{
//    Console.WriteLine("Произошла ошибка при выполнении запроса: " + ex.Message);
//}

//using System.Text;

//HttpClient client = new HttpClient();
//string url = "http://192.168.0.16:8080/refund_ticket"; //Adel
//string json = "{\"fio\": \"" + "Daniel Rodriguez" + "\", \"passport\": \"" + 8319630374 +
//                 "\", \"date_of_birth\": \"" + "08/08/2000" + "\", \"id_transaction\": \"" + 45 + "\"}";
//var content = new StringContent(json, Encoding.UTF8, "application/json");
//HttpResponseMessage responseRefund = await client.PostAsync(url, content);
//PrintResponse(responseRefund);