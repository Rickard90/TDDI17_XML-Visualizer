using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;

//namespace XML_Visualizer;
class XmlReader {

    public XmlReader()
    {
    }

    public ComponentsAndConnections ReadComponents(string path) {
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
            StreamReader topologyReader = new(path + "/topology/topology.xml");
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
                    computers[^1].SetChildren(partitions);
                }else if (line == "</Partition>"){
                    partitions[^1].SetChildren(applications);
                }

                line = topologyReader.ReadLine();
            }
            topologyReader.Close();
        }
        catch(Exception)
        {
            //Console.WriteLine("Exception: " + e.Message);
        }
        ComponentsAndConnections returnValue = new(computers, connections);
        return returnValue;
    }

    void ReadApplication(string applicationName, List<Component> threads, string path, Dictionary<string, List<Port>> connections) {
        List<Component> ports = new();
        String line;
        int index = 0;
        string name;
        string interf;
        string role;
        int frequency = 0;
        try
        {
            StreamReader applicationReader = new StreamReader(path + "/applications/"+applicationName+"/application.xml");
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
                        connections[interf].Add((Port)ports[ports.Count-1]);
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
           // Console.WriteLine("Exception: " + e.Message);
        }
        Console.WriteLine(connections.Count);
    }

   private void ReadResourses(string applicationName, List<Component> threads, ref int ramSize, ref int initStack, string path) {
        string line;
        try {
            StreamReader ResourcesReader = new StreamReader(path + "/applications/"+applicationName+"/resources.xml");
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
        catch(Exception)
        {
            //Console.WriteLine("Exception: " + e.Message);
        }
    }

    public struct ComponentsAndConnections {
        public List<Component> components = new();
        public Dictionary<string, List<Port>> connections = new();
        
        public ComponentsAndConnections(List<Component> components, Dictionary<string, List<Port>> connections)
        {
            this.components = components;
            this.connections = connections;
        }
    };
}