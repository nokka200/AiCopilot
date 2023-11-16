namespace AiCopilot;
using LLama.Common;
using LLama;
using System.Reflection.Metadata;
using System.Threading.Tasks;

class Program
{
    const string modelName = "llama-2-7b-guanaco-qlora.Q4_K_M.gguf";
    static void Main(string[] args)
    {
        var re = StartChatAsync();
    }

    static async Task StartChatAsync()
    {
        string modelPath = (Directory.GetCurrentDirectory() + $"/model/{modelName}");
        string prompt = "Start";

        ModelParams parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024,
            Seed = 1337,
            GpuLayerCount = 5
        };

        using LLamaWeights model = LLamaWeights.LoadFromFile(parameters);

        // Initialize a chat session
        using var context = model.CreateContext(parameters);
        var ex = new InteractiveExecutor(context);
        ChatSession session = new ChatSession(ex);

        // show the prompt
        Console.WriteLine();
        Console.Write(prompt);

        // run the inference in a loop to chat with LLM
        while (prompt != "stop")
        {
            await foreach (var text in session.ChatAsync(prompt, new InferenceParams() { Temperature = 0.1f, AntiPrompts = new List<string> { "User:" } }))
            {
                Console.Write(text);
            }
            prompt = Console.ReadLine()!;
        }
    }
}
