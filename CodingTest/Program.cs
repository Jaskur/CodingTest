using System.Net.Http.Headers;
using System.Text;

namespace CodingTest
{
    public class JeopardyQuestion
    {
        public string Id { get; set; }
        public string Answer { get; set; }
        public string Question { get; set; }
        public int Value { get; set; }
        public int Category_ID { get; set; }
    }

    class Program
    {
        private const string URL = "https://jservice.io/";
        
        static void Main(string[] args)
        {
            //Setup basic connection to client
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            
            //Telling the server to send over JSON data
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            //Prompt user for number of questions and difficulty
            // TODO: prompt the user for difficulty
            Console.WriteLine("How many questions would you like: ");
            string numQuestions = Console.ReadLine();

            //access the API to get requested number of questions
            HttpResponseMessage response = client.GetAsync(URL + "api/random?count=" + numQuestions).Result;
            if (response.IsSuccessStatusCode)
            {
                var jeopardyQuestions = response.Content.ReadAsAsync<IEnumerable<JeopardyQuestion>>().Result;
                
                //Create template for csv
                String file = @"C:\Users\jasku\Documents\Questions.csv";
                String separator = ",";
                StringBuilder output = new StringBuilder();
                String[] headings = { "Difficulty", "Category", "Question", "Answer" };
                output.AppendLine(string.Join(separator, headings));
                
                foreach (var q in jeopardyQuestions)
                {
                    //Add all data to output string
                    String[] newLine = { q.Value.ToString(), q.Category_ID.ToString(), q.Question, q.Answer };
                    output.AppendLine(string.Join(separator, newLine));
                    
                    // TODO: Lookup the title of the category based off of the value
                    // ****Made an attempt at doing another api call to get the category, I was able to retrieve the object
                    // ****but I could not figure out how to get just the title out of the object so I commented it out
                    // HttpResponseMessage response2 = client.GetAsync(URL + "api/category?id=" + q.Category_ID).Result;
                    // if (response2.IsSuccessStatusCode)
                    // {
                    //     var questionCategory = response2.Content.ReadAsAsync<Object>().Result;
                    //     Console.WriteLine("Category:{0}, Question: {1}, Answer: {2}", questionCategory, q.Question, q.Answer);
                    // }
                    // else
                    // {
                    //     Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    // }
                }
                try
                {
                    //Attempt to write string to csv file
                    File.WriteAllText(file, output.ToString());
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Data could not be written to the CSV file.");
                    return;
                }
                Console.WriteLine("The CSV has been successfully written");
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}