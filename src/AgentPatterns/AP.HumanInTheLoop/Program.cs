using AP.HumanInTheLoop;
using Microsoft.Agents.AI.Workflows;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Human in the loop example[/]");
var workflow = WorkflowFactory.BuildWorkflow();

// Execute the workflow
await using StreamingRun handle = await InProcessExecution.RunStreamingAsync(workflow, NumberSignal.Init);
await foreach (WorkflowEvent evt in handle.WatchStreamAsync())
{
    switch (evt)
    {
        case RequestInfoEvent requestInputEvt:
            // Handle `RequestInfoEvent` from the workflow
            ExternalResponse response = HandleExternalRequest(requestInputEvt.Request);
            await handle.SendResponseAsync(response);
            break;

        case WorkflowOutputEvent outputEvt:
            // The workflow has yielded output
            Console.WriteLine($"Workflow completed with result: {outputEvt.Data}");
            return;
    }
}

static ExternalResponse HandleExternalRequest(ExternalRequest request)
{
    if (request.TryGetDataAs<NumberSignal>(out var signal))
    {
        switch (signal)
        {
            case NumberSignal.Init:
                int initialGuess = ReadIntegerFromConsole("Please provide your initial guess: ");
                return request.CreateResponse(initialGuess);
            case NumberSignal.Above:
                int lowerGuess =
                    ReadIntegerFromConsole("You previously guessed too large. Please provide a new guess: ");
                return request.CreateResponse(lowerGuess);
            case NumberSignal.Below:
                int higherGuess =
                    ReadIntegerFromConsole("You previously guessed too small. Please provide a new guess: ");
                return request.CreateResponse(higherGuess);
        }
    }

    throw new NotSupportedException($"Request {request.PortInfo.RequestType} is not supported");
}

static int ReadIntegerFromConsole(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int value))
        {
            return value;
        }

        Console.WriteLine("Invalid input. Please enter a valid integer.");
    }
}