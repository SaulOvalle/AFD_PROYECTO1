using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class FiniteAutomata
{
    private int statesQuantity;
    private int initialState;
    private List<int> finalStates;
    private Dictionary<(int, string), int> transitions;

    public FiniteAutomata(int statesQuantity, int initialState, List<int> finalStates)
    {
        this.statesQuantity = statesQuantity;
        this.initialState = initialState;
        this.finalStates = finalStates;
        this.transitions = new Dictionary<(int, string), int>();
    }
    public void AddTransition(int initialState, string stringRead, int finalState)
    {
        var key = (initialState, stringRead);
        transitions[key] = finalState;
    }

    public bool AcceptsString(string inputStr, out List<string> usedTransitions)
    {
        int currentState = initialState;
        usedTransitions = new List<string>();

        foreach (char symbol in inputStr)
        {
            string stringRead = symbol.ToString();
            var key = (currentState, stringRead);

            if (transitions.ContainsKey(key))
            {
                int nextState = transitions[key];
                usedTransitions.Add($"{currentState} --({stringRead})--> {nextState}"); // Guarda la transición utilizada
                currentState = nextState;
            }
            else
            {
                return false;
            }
        }

        return finalStates.Contains(currentState);
    }

    public void PrintTransitions()
    {
        foreach (var transition in transitions)
        {
            Console.WriteLine($"{transition.Key.Item1} --({transition.Key.Item2})--> {transition.Value}");
        }
    }

    public void PrintUsedTransitions(List<string> usedTransitions)
    {
        Console.WriteLine("Transiciones utilizadas:");
        foreach (var transition in usedTransitions)
        {
            Console.WriteLine(transition);
        }
    }
}



class DocumentReader
{
    private FiniteAutomata automata;


    public void PrintTransitionsFromFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length < 3)
            {
                Console.WriteLine("Formato de archivo incorrecto. Líneas insuficientes para configurar el autómata.");
                return;
            }

            Console.WriteLine("Transiciones del autómata:");

            for (int i = 3; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                if (values.Length == 3)
                {
                    int transitionInitialState = int.Parse(values[0]);
                    string stringRead = values[1].Trim();
                    int transitionFinalState = int.Parse(values[2].Trim());

                    // Imprimir la transición
                    Console.WriteLine($"{transitionInitialState} --({stringRead})--> {transitionFinalState}");
                }
                else
                {
                    Console.WriteLine($"Formato de línea inválido en la línea {i + 1}: {lines[i]}");
                }
            }

            Console.WriteLine($"El autómata tiene un total de {lines.Length - 3} transiciones.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }
    }


    public FiniteAutomata ReadAutomatonFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length < 3)
            {
                Console.WriteLine("Formato de archivo incorrecto. Líneas insuficientes para configurar el autómata.");
                return null;
            }

            int totalStates = int.Parse(lines[0]);
            int initialState = int.Parse(lines[1]);
            List<int> finalStates = new List<int>(Array.ConvertAll(lines[2].Split(','), int.Parse));

            automata = new FiniteAutomata(totalStates, initialState, finalStates);

            for (int i = 3; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                if (values.Length == 3)
                {
                    int transitionInitialState = int.Parse(values[0]);
                    string stringRead = values[1].Trim();
                    int transitionFinalState = int.Parse(values[2].Trim()); // Corrección aplicada aquí

                    automata.AddTransition(transitionInitialState, stringRead, transitionFinalState);
                }
                else
                {
                    Console.WriteLine($"Formato de línea inválido en la línea {i + 1}: {lines[i]}");
                }
            }

            Console.WriteLine($"El autómata tiene {finalStates.Count} estados finales");
            Console.WriteLine($"El autómata tiene un total de {lines.Length - 3} transiciones.");

            return automata;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex}");
            return null;
        }
    }
}



class Program
{
    static void Main(string[] args)
    {
        FiniteAutomata automata = null;
        DocumentReader documentReader = new DocumentReader();

        while (true)
        {

            try
            {
                Console.WriteLine("Ingrese la opción deseada: \n 1: Ruta del archivo \n 2: Cadena a validar ");
                int decision = int.Parse(Console.ReadLine());

                if (decision == 1)
                {
                    Console.Clear();
                    Console.WriteLine("Ingrese la ruta del archivo del AFD: ");
                    string filePath = Console.ReadLine();
                    automata = documentReader.ReadAutomatonFile(filePath);
                    Console.ReadLine();
                    Console.Clear();
                }
                else if (decision == 2)
                {
                    if (automata != null)
                    {
                        Console.Clear();
                        Console.WriteLine("Ingrese una cadena para verificar si el Autómata la acepta: ");
                        string userInput = Console.ReadLine();

                        if (!string.IsNullOrEmpty(userInput))
                        {
                            List<string> usedTransitions;
                            bool accepted = automata.AcceptsString(userInput, out usedTransitions);

                            automata.PrintUsedTransitions(usedTransitions);

                            Console.ForegroundColor = accepted ? ConsoleColor.Green : ConsoleColor.Red;
                            Console.WriteLine(accepted ? "El autómata ha aceptado la cadena." : "El autómata no ha aceptado la cadena.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine("No ha ingresado ninguna cadena. Por favor, ingrese algo válido.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se ha podido configurar el autómata.");
                    }
                    Console.ReadLine();
                }
                else
                {
                    break;
                }
                Console.Clear();
            }
            catch
            {
                Console.Write("Ingrese el formato valido");
                Console.ReadLine();
                Console.Clear();

            }
        }
    }



}