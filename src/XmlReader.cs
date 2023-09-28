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
        List <Component> computers = new List<Component>();
        int activeThread = 0;
        List <Component> partitions = new List<Component>();
        List <Component> applications = new List<Component>();
        List <Component> threads = new List<Component>();
        Dictionary<string, List<Port>> connections = new Dictionary<string, List<Port>>();
        string applicationName;
        int ramSize = 0;
        int initStack = 0;

        try
        {            
            StreamReader topologyReader = new StreamReader(path + "/topology/topology.xml");
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
                        connections = ReadApplication(applicationName, threads, activeThread, path);
                        applications.Add(new Application(applicationName, ramSize, initStack));
                        applications[applications.Count-1].SetChildren(threads);
                    }
                }else if (line == "</Computer>"){
                    computers[computers.Count-1].SetChildren(partitions);
                }else if (line == "</Partition>"){
                    partitions[partitions.Count-1].SetChildren(applications);
                }

                line = topologyReader.ReadLine();
            }
            topologyReader.Close();
        }
        catch(Exception e)
        {
           // Console.WriteLine("Exception: " + e.Message);
        }
        ComponentsAndConnections returnValue = new ComponentsAndConnections(computers, connections);
        return returnValue;
    }

    Dictionary<string, List<Port>> ReadApplication(string applicationName, List<Component> threads, int activeThread, string path) {
        Dictionary<string, List<Port>> connections = new Dictionary<string, List<Port>>();
        List<Component> ports = new List<Component> ();
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
                        index ++;
                    } else if (line.Split('\"')[0] =="<Port name=" ) {
                        name = (line.Split('\"')[1]);
                        interf = (line.Split('\"')[3]);
                        role = (line.Split('\"')[5]);
                        ports.Add(new Port (name, interf, role));
                        if (!connections.ContainsKey(interf)) {
                            connections.Add(interf, new List<Port>());    
                        }
                        connections[interf].Add((Port)ports[ports.Count]);
                    }
                }else if (line == "</Thread>"){
                    threads[activeThread].SetChildren(ports);
                    ports.Clear();
                    index = 0;
                    activeThread++;
                }
                line = applicationReader.ReadLine();
            }
            applicationReader.Close();
        }
        catch(Exception e)
        {
         //   Console.WriteLine("Exception: " + e.Message);
        }
        return connections;
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
        catch(Exception e)
        {
          //  Console.WriteLine("Exception: " + e.Message);
        }
    }

    public struct ComponentsAndConnections {
        public List<Component> components = new List<Component>();
        public Dictionary<string, List<Port>> connections = new Dictionary<string, List<Port>>();
        
        public ComponentsAndConnections(List<Component> components, Dictionary<string, List<Port>> connections)
        {
            this.components = components;
            this.connections = connections;
        }
    };

    /*public struct Connection{
        List<Port> ports = new List<Port>();
        
        public Connection(Port senderPort, Port reciverPort)
        {
            this.senderPort = senderPort;
            this.reciverPort = reciverPort;
        }
    };
    /*
    public List<Connection> ReadConnections(string path)
    {
        List <Connection> connections = new List<Connection>();
        try
        {
            string line;
            string SenderPort = "test";
            StreamReader ConnectionReader = new StreamReader(path + "/connections/connections.xml");
            line = ConnectionReader.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if(line.Contains('\"')){
                    if (line.Split('\"')[0] == "<SenderPort name=" ) {
                        SenderPort = (line.Split('\"')[1]);
                    } else if (line.Split('\"')[0] == "<RecieverPort name=" ) {
                        connections.Add(new Connection(SenderPort, line.Split('\"')[1]));
                    }
                }
                line = ConnectionReader.ReadLine();
            }
            ConnectionReader.Close();
        }
        catch(Exception e)
        {
           // Console.WriteLine("Exception: " + e.Message);
        }
        return connections;
    }*/
}