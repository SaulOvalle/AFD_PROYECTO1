using System;
using System.Collections.Generic;
using System.IO;

class NFA
{
    private int numberOfStates;
    private int initialState;
    private HashSet<int> finalStates;
    private Dictionary<(int, string), List<int>> transitions;
    private List<string> acceptedPaths;

    public NFA()
    {
        transitions = new Dictionary<(int, string), List<int>>();
        finalStates = new HashSet<int>();
    }

    public void LoadFromFile(string filePath)
    {
        Console.Clear(); // Limpiar la consola antes de cargar el archivo
        ParseFile(filePath);
    }

    private void ParseFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        numberOfStates = int.Parse(lines[0].Trim());
        initialState = int.Parse(lines[1].Trim());
        foreach (var state in lines[2].Split(','))
            finalStates.Add(int.Parse(state.Trim()));

        for (int i = 3; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            int fromState = int.Parse(parts[0].Trim());
            string symbol = parts[1].Trim() == "" ? "epsilon" : parts[1].Trim();
            int toState = int.Parse(parts[2].Trim());

            var key = (fromState, symbol);
            if (!transitions.ContainsKey(key))
                transitions[key] = new List<int>();

            transitions[key].Add(toState);
        }
    }

    public List<string> Accepts(string input)
    {
        acceptedPaths = new List<string>();
        CheckAccepts(initialState, input, "");
        return acceptedPaths;
    }

    private void CheckAccepts(int currentState, string input, string currentPath)
    {
        string newPath = currentPath + (currentPath == "" ? "" : " -> ") + currentState;

        if (input == "")
        {
            if (finalStates.Contains(currentState))
                acceptedPaths.Add(newPath + " -> Accept");
        }
        else
        {
            string remainingInput = input.Substring(1);
            string symbol = input.Substring(0, 1);

            if (transitions.TryGetValue((currentState, symbol), out var states))
            {
                foreach (var state in states)
                    CheckAccepts(state, remainingInput, newPath + " ->(" + symbol + ")");
            }
        }

        // Check epsilon transitions at any point
        if (transitions.TryGetValue((currentState, "epsilon"), out var epsilonStates))
        {
            foreach (var nextState in epsilonStates)
                CheckAccepts(nextState, input, newPath + " ->(epsilon)");
        }
    }

    public void PrintTransitions()
    {
        Console.WriteLine("Possible transitions within the NFA:");
        foreach (var transition in transitions)
        {
            var (fromState, symbol) = transition.Key;
            var toStates = transition.Value;
            foreach (var toState in toStates)
            {
                Console.WriteLine($"{fromState} ->({symbol})-> {toState}");
            }
        }
    }

    static void Main(string[] args)
    {
        NFA nfa = new NFA();

        Console.WriteLine(@"
     ___       _______ .__   __. 
    /   \     |   ____||  \ |  | 
   /  ^  \    |  |__   |   \|  | 
  /  /_\  \   |   __|  |  . `  | 
 /  _____  \  |  |     |  |\   | 
/__/     \__\ |__|     |__| \__| 
                                                            
");

        while (true)
        {
            Console.WriteLine("MENU:");
            Console.WriteLine("1. Ingresar archivo");
            Console.WriteLine("2. Probar Cadena");
            Console.WriteLine("3. Salir");

            Console.Write("Selecciona una opción: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.Clear();
                    Console.Write("Ingrese la ruta del archivo de definición del NFA: ");
                    string filePath = Console.ReadLine();
                    nfa.LoadFromFile(filePath);
                    Console.WriteLine("Archivo cargado correctamente.");
                    Console.WriteLine("Toca cualquier tecla para continuar");
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "2":
                    Console.Clear();
                    if (nfa.transitions.Count == 0)
                    {
                        Console.WriteLine("¡Error! Debe cargar un archivo primero.");
                        break;
                    }

                    Console.Write("Ingrese la cadena a validar: ");
                    string input = Console.ReadLine();

                    var acceptedPaths = nfa.Accepts(input);
                    Console.WriteLine($"Número de caminos aceptados: {acceptedPaths.Count}");

                    foreach (var path in acceptedPaths)
                    {
                        Console.WriteLine(path);
                    }

                    Console.WriteLine("Presiona cualquier tecla para limpiar la consola...");
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine(@"
     ___       _______ .__   __. 
    /   \     |   ____||  \ |  | 
   /  ^  \    |  |__   |   \|  | 
  /  /_\  \   |   __|  |  . `  | 
 /  _____  \  |  |     |  |\   | 
/__/     \__\ |__|     |__| \__|       
");
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine("Saliendo del programa.");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Por favor, seleccione una opción válida.");
                    break;
            }
        }
    }
}