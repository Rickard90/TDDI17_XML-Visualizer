using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;

static class XmlReader {

    public static List<Component> ReadComponents(string path) {
        String line;
        List <Component> computers = new();
        List <Component> partitions = new();
        List <Component> applications = new();
        List <Component> threads = new();
        Dictionary<string, List<Port>> connections = new();
        string applicationName;
        int ramSize = 0;
        int initStack = 0;

        try
        {
            using StreamReader topologyReader = new(path + "/topology/topology.xml");
            line = topologyReader.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if(line.Contains('\"')){
                    if (line.Split('\"')[0] == "<Computer name=" ) {
                        partitions.Clear();
                        computers.Add(new Computer (line.Split('\"')[1]));
                    } else if (line.Split('\"')[0] == "<Partition name=" ) {
                        applications.Clear();
                        partitions.Add(new Partition (line.Split('\"')[1]));
                    }else if (line.Split('\"')[0] == "<Application name=" ) {
                        threads.Clear();
                        applicationName = (line.Split('\"')[1]);
                        ReadResourses(applicationName, threads, ref ramSize, ref initStack, path);
                        ReadApplication(applicationName, threads, path, connections);
                        applications.Add(new Application(applicationName, ramSize, initStack));
                        applications[^1].SetChildren(threads);
                    }
                }else if (line == "</Computer>"){
                    ((Computer)computers[^1]).SetChildren(partitions);
                }else if (line == "</Partition>"){
                    partitions[^1].SetChildren(applications);
                }

                line = topologyReader.ReadLine();
            }
            topologyReader.Close();
        }
        catch(Exception e)
        {
            Log.Print("Exception: " + e.Message);
        }

        //Sets connections off all ports:
        foreach (var connection in connections) {
            foreach (Port connected in connection.Value) {
                connected.AddConnections(connection.Value);
            }
        }
        foreach (Computer comp in computers) {
            comp.UpdateConnections();
        }

        return computers;
    }

    private static void ReadApplication(string applicationName, List<Component> threads, string path, Dictionary<string, List<Port>> connections) {
        List<Component> ports = new();
        String line;
        int index = 0;
        string name;
        string interf;
        string role;
        int frequency = 0;
        try
        {
            using StreamReader applicationReader = new(path + "/applications/"+applicationName.ToLower()+"/application.xml");
            line = applicationReader.ReadLine();
            
            while (line != null)
            {
                line = line.Trim();
                if(line.Contains('\"')){ 
                    if (line.Split('\"')[0] == "<Thread name=" ) {
                        frequency = Int32.Parse(line.Split('\"')[3].Remove(line.Split('\"')[3].Length-2));
                        ((Thread)threads[index]).SetFrequency(frequency);
                    } else if (line.Split('\"')[0] =="<Port name=" ) {
                        name = (line.Split('\"')[1]);
                        interf = (line.Split('\"')[3]);
                        role = (line.Split('\"')[5]);
                        ports.Add(new Port (name, interf, role));
                        if (!connections.ContainsKey(interf)) {
                            connections.Add(interf, new List<Port>());    
                        }
                        connections[interf].Add((Port)ports[^1]);
                    }
                }else if (line == "</Thread>"){
                    threads[index].SetChildren(ports);
                    ports.Clear();
                    index ++;
                }
                line = applicationReader.ReadLine();
            }
            applicationReader.Close();
        }
        catch (Exception e)
        {
            Log.Print("Exception: " + e.Message);
        }
    }

    private static void ReadResourses(string applicationName, List<Component> threads, ref int ramSize, ref int initStack, string path) {
        string line;
        try {
            using StreamReader ResourcesReader = new(path + "/applications/"+applicationName.ToLower()+"/resources.xml");
            line = ResourcesReader.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if (line.Length > 10) {
                    if (line.Split(' ')[0] == "<ApplicationResourceRequirements" ) {
                        initStack = Int32.Parse(line.Split('\"')[3]);
                        ramSize = Int32.Parse(line.Split('\"')[5]);
                    } else if (line.Split(' ')[0] == "<ThreadResourceRequirements" ) {
                        threads.Add(new Thread (line.Split('\"')[1], Int32.Parse(line.Split('\"')[5]), Int32.Parse(line.Split('\"')[3])));
                    }
                }
                line = ResourcesReader.ReadLine();
            }
        }
        catch(Exception e)
        {
            Log.Print("Exception: " + e.Message);
        }
    }
}